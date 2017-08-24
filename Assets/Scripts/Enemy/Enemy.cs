using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public bool use = false;
    public float Speed = 3f;
    public Transform goodWaitPos;
    public Transform badWaitPos;
    public Transform carPos;
    float health;
    float attack;

    EnemyState curState;
    Mood curMood;
    Personality personality;
    public Task curTask = Task.NormalWaitForCar;
    TaskCompletion completion;

    NavMeshAgent agent;
    Animator anim;

    void Awake() {
        personality = (Personality)(int)Random.Range(0, 2.999f);//性格在这里随机赋值？
        Debug.Log(personality);
        curState = new IdleState(this);
        curState.Enter();
        personality = Personality.Evil;
        completion = TaskCompletion.NotStarted;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        //决策是否搞事(curTask)
        anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        RunStateMachine();
        MoodTransition();

        if (use)
            AssignTask(Task.ComeUp);
    }

    void MoodTransition() {
        //curMood
    }

    /// <summary>
    /// 设定agent目标到车门附近，根据是否守规矩决定具体位置
    /// </summary>
    /// <param name="isGood"></param>
    void WaitForCar(bool isGood) {
        agent.SetDestination(isGood ? goodWaitPos.position : badWaitPos.position);
    }

    void ComeUp() {
        agent.SetDestination(carPos.position);
    }

    public void AssignTask(Task newTask) {
        curTask = newTask;
        completion = TaskCompletion.NotStarted;
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

        public IdleState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Idle; }
        public override void Enter() {
            Debug.Log("enter idle state");
        }

        public override void Exit() {
        }
        public override void Update() {
            //执行Task
            if (context.completion == TaskCompletion.NotStarted) {
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
            }
        }
        public override EnemyState CheckTransition() {
            if (context.anim.GetFloat("Speed") > 0) {
                return new MoveState(context);
            }
            
            return this;
        }
    }

    public class MoveState : EnemyState {

        public MoveState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Move; }
        public override void Enter() {
            Debug.Log("enter move state");
        }
        public override void Exit() {
        }
        public override void Update() {
        }
        public override EnemyState CheckTransition() {
            if (context.anim.GetFloat("Speed") == 0) {
                return new IdleState(context);
            }
            return this;
        }
    }

    public class AttackState : EnemyState {
        public AttackState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Attack; }
        public override void Update() {
            base.Update();
            switch (context.curMood) {
                case Mood.Normal:
                    break;
                case Mood.Frightened: break;
                case Mood.Angry: break;
            }
        }

        public override EnemyState CheckTransition() {
            return base.CheckTransition();
        }
    }

    public class EnemyState {
        public EnemyStateEnum type;
        protected Enemy context;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual EnemyState CheckTransition() { return this; }
    }

    //----------------ENUMS-------------------------------
    public enum EnemyStateEnum {
        Idle, Move, Attack
    };

    public enum Mood {
        Normal, Angry, Frightened
    }

    public enum Personality {
        EatMelon, Evil, Coward
    }
    public enum Task {
        NormalWaitForCar, BadWaitForCar, ComeUp, Idle
    }
    public enum TaskCompletion {
        NotStarted, Doing, Finished
    }
}

