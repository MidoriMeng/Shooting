using System.Collections;
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
    public float curHealth;
    public float maxHealth;
    float attack;
    float playerDistance = 0;

    EnemyState curState;
    //Mood curMood;
    Personality personality;
    //public Task curTask = Task.NormalWaitForCar;
    //TaskCompletion completion;

    NavMeshAgent agent;
    Animator anim;

    void Awake() {
        personality = (Personality)(int)Random.Range(0, 2.999f);//性格在这里随机赋值？
        Debug.Log(personality);
        curState = new IdleState(this);

        personality = Personality.Evil;
        //completion = TaskCompletion.NotStarted;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
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

    
    //-------------------------STATE----------------------------------
    void RunStateMachine() {
        EnemyState newState = CheckTransition();//先check再update
        if (newState.type != curState.type) {
            curState.Exit();
            curState = newState;
            curState.Enter();
        }
        curState.Update();
    }

    protected virtual EnemyState CheckTransition() { return curState; }
    /*EnemyState CheckTransition() {
        if (anim.GetFloat("Speed") > 0) {
            return new MoveState(this);
        }
        if (Vector3.Distance(Singleton<PlayerMovementControl>.Instance.transform.position, transform.position) <= attackDistance) {
            return new AttackState(this);
        }
        if (this.anim.GetFloat("Speed") == 0) {
             return new IdleState(this);
         }
    }*/
    public class IdleState : EnemyState {

        public IdleState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Idle; }
        public override void Enter() {
            Debug.Log("enter idle state");

        }

        public override void Exit() {
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
    }

    public class MoveState : EnemyState {
        Vector3 target;
        NavMeshAgent agent;

        public MoveState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Move; }
        public override void Enter() {
            Debug.Log("enter move state");
            agent = context.agent;
            target = agent.destination;
        }
        public override void Exit() {
        }
        public override void Update() {
        }
        /*public override EnemyState CheckTransition() {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                return new IdleState(context);
            }
            if (context.playerDistance <= context.attackDistance) {
                return new AttackState(context);
            }
            return this;
        }*/
    }

    public class AttackState : EnemyState {
        public AttackState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Attack; }
        public override void Update() {
            base.Update();
        }

        /*public override EnemyState CheckTransition() {
            if (context.anim.GetFloat("Speed") == 0) {
                return new IdleState(context);
            }
            if (context.playerDistance <= context.attackDistance) {
                return new AttackState(context);
            }
            return this;
        }*/
    }

    public class EnemyState {
        public EnemyStateEnum type;
        protected Enemy context;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        //public virtual EnemyState CheckTransition() { return this; }
    }

    //----------------ENUMS-------------------------------
    public enum EnemyStateEnum {
        Idle, Move, Attack
    };

    public enum Mood {
        Normal, Angry, Frightened
    }

    public class Personality { public virtual EnemyStateEnum CheckTransition(Player player) { return EnemyStateEnum.Idle; } }

    class EatMelonPersona : Personality {
        public override EnemyStateEnum CheckTransition(Player player) { return EnemyStateEnum.Idle; }
    }
    class EvilPersona : Personality { }
    class CowardPersona : Personality { }

   /* public enum Task {
        NormalWaitForCar, BadWaitForCar, ComeUp, Idle
    }
    public enum TaskCompletion {
        NotStarted, Doing, Finished
    }*/
}

