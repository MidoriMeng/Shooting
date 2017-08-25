﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public float Speed = 3f;
    public float attackDistance = 1f;
    public float alarmDistance = 7f;
    public float alarmHPPercent;
    /*public Transform goodWaitPos;
    public Transform badWaitPos;
    public Transform carPos;*/
    public float curHP = 50f;
    public float maxHP = 50f;
    float attack;
    float playerDistance = 0;

    bool attackComplete = false;//bad

    EnemyState curState;
    //Mood curMood;
    Personality personality;
    //public Task curTask = Task.NormalWaitForCar;
    //TaskCompletion completion;

    NavMeshAgent agent;
    Animator anim;

    void Awake() {
        personality = Personality.EatMelon;
        // Debug.Log(personality);
        alarmHPPercent = Random.Range(0.2f, 0.5f);
        //personality = Personality.Evil;
        //completion = TaskCompletion.NotStarted;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        curState = new IdleState(this, Singleton<Player>.Instance);
        curState.Enter();
    }

    void Update() {
        //决策是否搞事(curTask)

        //更新数值
        anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        playerDistance = Vector3.Distance(Singleton<Player>.Instance.transform.position, transform.position);

        //状态机更新
        RunStateMachine();
        MoodTransition();
    }

    void MoodTransition() {
        //curMood
    }

    public void CompleteAttack() { attackComplete = true; }

    /// <summary>
    /// 设定agent目标到车门附近，根据是否守规矩决定具体位置
    /// </summary>
    /// <param name="isGood"></param>
    /* void WaitForCar(bool isGood) {
         agent.enabled = true;
         agent.SetDestination(isGood ? goodWaitPos.position : badWaitPos.position);
     }

     void ComeUp() {
         agent.enabled = true;
         agent.SetDestination(carPos.position);
     }

     public void AssignTask(Task newTask) {
         curTask = newTask;
         completion = TaskCompletion.NotStarted;
     }*/

    public void Attack(Player player) {
        player.curHealth -= attack;
    }

    public float HPPercent {
        get {
            Debug.Log(curHP + "   " + maxHP);
            return curHP / maxHP; }
    }
    //-------------------------STATE----------------------------------
    void RunStateMachine() {
        EnemyState newState = curState.CheckTransition();//先check再update
        if (newState.type != curState.type) {
            curState.Exit();
            curState = newState;
            curState.Enter();
        }
        curState.Update();
    }

    public class IdleState : EnemyState {

        public IdleState(Enemy enemy, Player player) : base(enemy, player) { type = EnemyStateEnum.Idle; }
        public override void Enter() {
            Debug.Log("enter idle state");
            context.agent.enabled = false;
        }

        public override void Exit() {
            context.agent.enabled = true;
        }
        public override void Update() {
            //执行Task
            /*if (context.completion == TaskCompletion.NotStarted) {
                switch (context.curTask) {
                    case Task.ComeUp:
                        context.ComeUp();
                        break;
                    case Task.NormalWaitForCar:
                        context.WaitForCar(true);
                        break;
                    case Task.BadWaitForCar:
                        context.WaitForCar(false);
                        break;
                }
            }
            //检查Task进度
            switch (context.curTask) {
                case Task.Idle:
                    break;
                case Task.NormalWaitForCar:
                case Task.BadWaitForCar:
                    if (context.agent.pathPending)
                        context.completion = TaskCompletion.Doing;
                    else
                        context.completion = context.agent.remainingDistance <= context.agent.stoppingDistance ?
                            TaskCompletion.Finished : TaskCompletion.Doing;
                    break;
            }
            if (context.completion == TaskCompletion.Finished) {
                context.curTask = Task.Idle;
                context.completion = TaskCompletion.Finished;
                //stop & resume
                context.agent.enabled = false;
            }*/
        }

        public override EnemyState CheckTransition() {
            switch (context.personality) {
                case Personality.EatMelon:
                    //主角在攻击范围内：攻击
                    if (Vector3.Distance(context.transform.position, player.transform.position) < context.attackDistance)
                        return new AttackState(context, player);
                    //自身血量低于警戒值：逃跑
                    Debug.Log(context.HPPercent + "   " + context.alarmHPPercent);
                    if (context.HPPercent <= context.alarmHPPercent)
                        return new FleeState(context, player);
                    break;
                    //TODO: 参照表格
            }
            
            //默认：idle
            return this;
        }
    }

    public class FleeState : EnemyState {
        Vector3 destination;
        NavMeshAgent agent;

        public FleeState(Enemy enemy, Player player) : base(enemy, player) {  type = EnemyStateEnum.Flee; }

        public override void Enter() {
            Debug.Log("enter flee state");
            agent = context.agent;
            
            switch (context.personality) {
                case Personality.EatMelon://逃到警戒范围外
                    Vector3 dir = Vector3.Normalize(context.transform.position - player.transform.position);
                    destination = context.transform.position + dir * context.alarmDistance;
                    break;
                    //@TODO
            }
            agent.destination = destination;
        }
        public override void Exit() {
        }
        public override void Update() {
        }
        public override EnemyState CheckTransition() {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                return new IdleState(context, player);
            }
            return this;
        }
    }

    public class AttackState : EnemyState {
        public AttackState(Enemy enemy, Player player): base(enemy, player) {  type = EnemyStateEnum.Attack; }

        public override void Enter() {
            Debug.Log("enter attack state");
            context.anim.SetTrigger("Attack");
            context.attackComplete = false;

        }
        public override void Update() {
        }

        public override EnemyState CheckTransition() {
            if (context.attackComplete)
                return new IdleState(context, player);
            /*if (context.anim.GetFloat("Speed") == 0) {
                return new IdleState(context);
            }*/
            return this;
        }
    }

    public abstract class EnemyState {
        public EnemyStateEnum type;
        protected Enemy context;
        protected Player player;

        public EnemyState(Enemy enemy, Player player) { context = enemy; this.player = player; }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual EnemyState CheckTransition() { return this; }
    }

    //----------------ENUMS-------------------------------
    public enum EnemyStateEnum {
        Idle, Flee, Attack
    };

    public enum Mood {
        Normal, Angry, Frightened
    }

    public enum PersonaEnum {
        EatMelon, Coward, Evil
    }

    public enum Personality {
        EatMelon, Evil, Coward
    };

    /* public enum Task {
         NormalWaitForCar, BadWaitForCar, ComeUp, Idle
     }
     public enum TaskCompletion {
         NotStarted, Doing, Finished
     }*/

    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alarmDistance);
    }
}

