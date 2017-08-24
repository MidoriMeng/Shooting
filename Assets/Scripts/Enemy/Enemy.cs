using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float Speed;
    float speedPercentage;
    float health;
    float attack;

    public enum EnemyStateEnum {
        Idle, Move, Attack
    };

    public enum Mood {
        Normal, Angry, Frightened
    }

    public enum Personality {
        EatMelon, Evil, Coward
    }

    EnemyState curState;
    private Mood curMood;
    Personality personality;

    void Awake() {
        personality = (Personality)(int)Random.Range(0, 2.999f);//性格在这里随机赋值？
        Debug.Log(personality);
        curState = new IdleState(this);
        curState.Enter();

    }

    void Update() {
        RunStateMachine();
        MoodTransition();
    }

    void MoodTransition() {
        //curMood
    }


    //-------------------------STATE----------------------------------
    void RunStateMachine() {
        EnemyState newState = curState.CheckTransition();
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
        }
        public override EnemyState CheckTransition() {
            if (context.speedPercentage > 0.1f)
                return new MoveState(context);
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
            if (context.speedPercentage < 0.1f)
                return new IdleState(context);
            return this;
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
}

