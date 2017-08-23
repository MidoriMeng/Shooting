using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}

public class State {
    public virtual void Enter(){}
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual State CheckTransition() { return this; }
}

