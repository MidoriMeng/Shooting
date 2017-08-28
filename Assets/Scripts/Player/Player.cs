using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private static Player _instance;
    UnityChanControlScriptWithRgidBody control;
    //PlayerShoot shoot;


    void Awake () {
        AwakeBase();
        _instance = this;
        atk = 15f;
        def = 3f;
        control = GetComponent<UnityChanControlScriptWithRgidBody>();        
	}
    void Start() {
        //shoot = GetComponentInChildren<PlayerShoot>();
    }
	
    void Update () {
	}


    protected override void DeadTemplate() {
        control.enabled = false;
    }

    public override void DeadComplete() {
        Debug.Log("dead");
    }

    public static Player Instance { get { return _instance; } }
}
