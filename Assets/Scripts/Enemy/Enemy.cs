using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public enum EnemyStateEnum {
        Attack, Idle
    };
    State curState;

    void Awake() {
        curState = new IdleState();
    }

    void Update() {
        State newState = curState.CheckTransition();
        //if(newState
	}

    
    public class IdleState : EnemyState {
        public override void Enter() { }
        public override void Exit() { }
        public override void Update() { }
        public override State CheckTransition() {
            return this;
        }
    }
}

public class EnemyState : State { 

}
