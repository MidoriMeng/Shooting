using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
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
            
        }

        public override void Exit() {
        }
        public override void Update() {
            //检查搞事进度
            switch (context.curTask) {
                case Task.NormalWaitForCar:
                case Task.BadWaitForCar:
                    if (context.agent.remainingDistance < context.agent.stoppingDistance)
                        context.completion = context.agent.remainingDistance < context.agent.stoppingDistance ?
                            TaskCompletion.Finished : TaskCompletion.Doing;
                    break;
            }
            //执行搞事
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
            
        }
        public override EnemyState CheckTransition() {
            if (context.anim.GetFloat("Speed") > 0.03f) {
                return new MoveState(context);
            }
            return this;
        }
    }

    public class MoveState : EnemyState {

        public MoveState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Move; }
        public override void Enter() {
        }
        public override void Exit() {
        }
        public override void Update() {
        }
        public override EnemyState CheckTransition() {
            if (context.anim.GetFloat("Speed") < 0.03f) {
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
        NormalWaitForCar, BadWaitForCar, ComeUp
    }
    public enum TaskCompletion {
        NotStarted, Doing, Finished
    }
}

