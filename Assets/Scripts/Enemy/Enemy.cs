using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character {

    public float Speed = 3f;
    public float attackDistance = 1f;
    public float alarmDistance = 7f;
    public int rewardExp = 15;
    public int rewardScore = 10;
    public float rewardCraziness = 0.2f;
    float playerDistance = 0;
    Player player;

    EnemyState curState;
    public Personality personality = Personality.EatMelon;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Command command;
    private float guiltyPercentage = 0;
    Collider collider;
    int hashID;

    Color matColor;
    public bool gazed = false;

    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    Renderer rend;

    void Awake() {
        AwakeBase();
        command = new Command();
        agent = GetComponent<NavMeshAgent>();
        baseAtk = 3.5f;
        baseDef = 2f;
        collider = GetComponent<Collider>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        enemyAudio = GetComponent<AudioSource>();
        rend = GetComponentInChildren<Renderer>();
        matColor = rend.material.GetColor("_Color");
        //Debug.Log("awake");
    }

    void Start() {
        player = Player.Instance;
        //Debug.Log("Start");
    }

    void OnEnable() {
        alarmHPPercent = Random.Range(0.2f, 0.5f);
        dead = false;
        player = Player.Instance;
        curState = new IdleState(this, player);
        //Debug.Log("enabled");
        curState.Enter();
    }

    void Update() {
        if (!dead) {
            //更新数值
            //anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
            playerDistance = Vector3.Distance(Player.Instance.transform.position, transform.position);
            Vector3 clampedVelocity = agent.velocity / agent.speed;
            anim.SetFloat("Speed", curState.type == EnemyStateEnum.Attack ? 1 : 0.2f * clampedVelocity.magnitude);
            anim.SetFloat("Direction", transform.rotation.eulerAngles.y / 90f);
            //状态机更新
            RunStateMachine();

            float factor = gazed ? (Mathf.Sin(Time.time * 2f) + 1f) * guiltyPercentage / 2f : 1f;
            matColor = Color.Lerp(matColor, Color.white * factor, Time.deltaTime);
            matColor.r = 1f;
            rend.material.SetColor("_Color", matColor);
        }


    }

    public override float MakeDamage() {
        float dmg = 0;
        switch (personality) {
            case Personality.EatMelon:
                dmg = Random.Range(0.8f, 1.2f);
                break;
            case Personality.Coward:
                dmg = Random.Range(0.5f, 1.5f);
                break;
            case Personality.Evil:
                dmg = Random.Range(0.9f, 1.5f);
                break;
        }
        return dmg * atk;
    }

    public void EnemyAttack() {
        base.Attack(player, Vector3.zero);
    }

    protected override void TakeDamage(Vector3 hitPoint) {
        hitParticles.transform.position = hitPoint;
        hitParticles.Play();
    }
    public void CompleteAttack() {
        AttackState s = curState as AttackState;
        s.AttackComplete();
    }

    public void DoEvil(float delta = 0.2f) {
        guiltyPercentage += delta;
    }

    protected override void DeadTemplate() {
        collider.enabled = false;
        agent.enabled = false;
        EnemyManager.Instance.EnemyDead(this);
    }

    public override void DeadComplete() {
        EnemyManager.Instance.Recycle(this);
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
            //Debug.Log("enter idle state");
        }

        public override void Exit() {
        }
        public override void Update() {
            //执行Command
            context.command.Run(context);
            //Debug.Log(curCmd.commandType + "  " + curCmd.completion);
            //Lazy();
        }

        void Lazy() {
            if (Random.Range(0, 1) < 0.001f) {
                context.anim.SetTrigger("Rest");
                context.agent.enabled = false;
            }
        }

        public override EnemyState CheckTransition() {
            Vector3 enemyPos = context.transform.position;
            enemyPos.y = 1f;
            Vector3 dir = (player.transform.position - enemyPos);
            dir.y = 0;
            //lookAtPos.y = 1f;
            RaycastHit hit = new RaycastHit();
            bool seePlayer = false;
            if (Physics.Raycast(enemyPos, dir, out hit))
                seePlayer = hit.collider.tag == "Player";
            //Debug.DrawRay(enemyPos, dir);
            switch (context.personality) {
                case Personality.EatMelon:
                    //自身血量低于警戒值：逃跑
                    if (context.HPPercent <= context.alarmHPPercent && context.playerDistance < context.alarmDistance && seePlayer)
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
                    if (context.playerDistance < context.alarmDistance && seePlayer)
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
            //Debug.Log("enter flee state");
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
            //Debug.Log("enter attack state");
            context.agent.destination = player.transform.position;
        }

        public override void Update() {
            if (state == AttackProcess.finding)
                if (!context.agent.pathPending && context.agent.remainingDistance < context.attackDistance) {
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
    
    public enum Personality {
        EatMelon = 0, Evil = 1, Coward = 2
    };

    /* public void OnDrawGizmos() {
         Gizmos.color = Color.red;
         Gizmos.DrawWireSphere(transform.position, attackDistance);
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(transform.position, alarmDistance);
     }*/
}

