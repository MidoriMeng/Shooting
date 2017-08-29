using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private static Player _instance;
    UnityChanControlScriptWithRgidBody control;
    //PlayerShoot shoot;
    float curExp = 0;
    float maxExp = 50f;
    float craziness = 0.5f;
    public float crazinessFallSpeed = 0.01f;

    void Awake () {
        AwakeBase();
        _instance = this;
        _atk = 15f;
        _def = 3f;
        control = GetComponent<UnityChanControlScriptWithRgidBody>();        
	}

    void Start() {
        //shoot = GetComponentInChildren<PlayerShoot>();
    }
	
    void Update () {
        if (!dead) {
            craziness = Mathf.Lerp(craziness, 0f, Time.deltaTime * crazinessFallSpeed);
            //Debug.Log(craziness);
            if (craziness < 0.005f) {
                Dead();
            }
        }
	}

    void gainExp(float gain) {
        curExp += gain;
        if (curExp >= maxExp) { 
            //level up
            curLevel++;
            curExp -= maxExp;
            maxExp *= 1.1f;
            maxExp = (int)maxExp;
            Debug.Log("level " + curLevel + "　　next level exp: " + maxExp);
            curHP = maxHP; 
        }
    }

    public override void Attack(Character c, Vector3 point) {
        base.Attack(c, point);
        if (c.isDead) {
            gainExp((c as Enemy).rewardExp);
        }
    }

    

    protected override void DeadTemplate() {
        control.enabled = false;
    }

    public override void DeadComplete() {
        Debug.Log("dead complete");
        GamePlayManager.Instance.PlayerDead();
    }

    public static Player Instance { get { return _instance; } }
}
