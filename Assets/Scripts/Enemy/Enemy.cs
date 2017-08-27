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
    public float curHP = 50f;
    public float maxHP = 50f;
    public float attack;
    float playerDistance = 0;
    Player player;

    public Transform waitPos;


    EnemyState curState;
    //Mood curMood;
    Personality personality;
    public NavMeshAgent agent;
    public Animator anim;
    public Command command;
    private float guiltyPercentage = 0;

    void Awake() {
        personality = Personality.Evil;
        // Debug.Log(personality);
        alarmHPPercent = Random.Range(0.2f, 0.5f);
        //command = new Command();
        command = new GoAndWaitCmd(waitPos.position, true, Command.TypeEnum.BadWaitForCar);
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        player = Player.Instance;
        curState = new IdleState(this, player);
        curState.Enter();
    }

    void Update() {
        //更新数值
        anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        playerDistance = Vector3.Distance(Player.Instance.transform.position, transform.position);

        //状态机更新
        RunStateMachine();
    }
    
    public void Attack() {
        float atk = attack;
        switch (personality) { 
            case Personality.EatMelon:
                atk = Random.Range(0.8f, 1.2f);
                break;
            case Personality.Coward:
                atk = Random.Range(0.5f, 1.5f);
                break;
            case Personality.Evil:
                atk = Random.Range(0.9f, 1.5f);
                break;
        }
        player.curHP -= atk;        
    }

    public void CompleteAttack() {
        AttackState s = curState as AttackState;
        s.AttackComplete();
    }

    public void DoEvil(float delta = 0.2f) {
        guiltyPercentage += delta;
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


    public float HPPercent {
        get { return curHP / maxHP; }
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
        Command curCmd;
        public IdleState(Enemy enemy, Player player) : base(enemy, player) { type = EnemyStateEnum.Idle; }
        public override void Enter() {
            Debug.Log("enter idle state");
            context.agent.enabled = false;
            curCmd = context.command;
        }

        public override void Exit() {
            context.agent.enabled = true;
        }
        public override void Update() {
            //执行Command
            curCmd.Run(context);
        }

        public override EnemyState CheckTransition() {
            switch (context.personality) {
                case Personality.EatMelon:
                    //自身血量低于警戒值：逃跑
                    if (context.HPPercent <= context.alarmHPPercent && context.playerDistance < context.alarmDistance)
                        return new FleeState(context, player);
                    //主角在攻击范围内：攻击
                    if (context.playerDistance < context.attackDistance)
                        return new AttackState(context, player);
                    break;
                case Personality.Coward:
                    //自身血量非满且主角在警戒范围内：逃跑
                    if (context.HPPercent < 1f && context.playerDistance < context.alarmDistance)
                        return new FleeState(context, player);
                    //主角血量低于血量警戒值且在攻击范围内：攻击
                    if (player.HPPercent < player.alarmHPPercent && context.playerDistance < context.attackDistance)
                        return new AttackState(context, player);
                    break;
                case Personality.Evil:
                    if (context.playerDistance < context.alarmDistance)
                        return new AttackState(context, player);
                    break;
            }

            //默认：idle
            return this;
        }
    }

    public class FleeState : EnemyState {
        Vector3 destination;
        NavMeshAgent agent;

        public FleeState(Enemy enemy, Player player) : base(enemy, player) { type = EnemyStateEnum.Flee; }

        public override void Enter() {
            Debug.Log("enter flee state");
            agent = context.agent;

            switch (context.personality) {
                case Personality.EatMelon://逃到警戒范围外
                    RunAway(context.alarmDistance);
                    break;
                case Personality.Coward://逃到2倍警戒范围外
                    RunAway(context.alarmDistance * 2f);
                    break;
                //Evil: never flee
            }
            agent.destination = destination;
        }

        void RunAway(float distance) {
            Vector3 dir = Vector3.Normalize(context.transform.position - player.transform.position);
            Vector3 startPoint = context.transform.position;
            Vector3 delta = dir * distance;
            //Vector3 endPoint = context.transform.position + dir * context.alarmDistance;
            //从结束点到开始点依次采样，如果成功则移动到那里
            NavMeshHit hit;
            Vector3 pos = startPoint;
            for (float i = 1f; i > 0; i -= 0.1f) {
                pos = startPoint + delta * i;
                if (NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas)) {
                    destination = pos;
                    //Debug.Log("select: " + destination);
                    break;
                }
            }
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
        enum AttackProcess { finding, attacking, complete };
        AttackProcess state = AttackProcess.finding;
        public AttackState(Enemy enemy, Player player) : base(enemy, player) { type = EnemyStateEnum.Attack; }

        public void AttackComplete() {
            state = AttackProcess.complete;
        }

        public override void Enter() {
            Debug.Log("enter attack state");
            context.agent.destination = player.transform.position;
        }

        public override void Update() {
            if (state == AttackProcess.finding)
                if (context.agent.remainingDistance < context.attackDistance) {
                    //如果玩家已离开
                    if (context.playerDistance > context.attackDistance) {
                        state = AttackProcess.complete;
                        return;
                    }
                    context.anim.SetTrigger("Attack");
                    context.agent.destination = context.transform.position;
                    state = AttackProcess.attacking;
                }
        }

        public override EnemyState CheckTransition() {
            if (state == AttackProcess.complete)
                return new IdleState(context, player);
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

    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alarmDistance);
    }
}

