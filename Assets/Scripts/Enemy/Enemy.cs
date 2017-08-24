using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    float health;
    float attack;

    public enum EnemyStateEnum {
        Attack, Idle
    };
    EnemyState curState;

    void Awake() {
        curState = new IdleState(this);
        curState.Enter();
    }

    void Update() {
        EnemyState newState = curState.CheckTransition() as EnemyState;
        if(newState.type != curState.type) {
            curState.Exit();
            curState = newState;
            curState.Enter();
        }
	}

    
    public class IdleState : EnemyState {
        Enemy context;

        public IdleState(Enemy enemy) { context = enemy; type = EnemyStateEnum.Idle; }
        public override void Enter() { }
        public override void Exit() { }
        public override void Update() { }
        public override State CheckTransition() {
            return this;
        }
    }

    public class EnemyState : State {
        public EnemyStateEnum type;
    }
}

