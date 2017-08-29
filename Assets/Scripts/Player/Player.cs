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
    float madAtkScale = 50000f;
    float madSpeedScale = 2f;
    float madCrazinessFallSpeedScale = 2f;
    bool _isCrazy = false;

    void Awake() {
        AwakeBase();
        _instance = this;
        baseAtk = 15f;
        baseDef = 3f;
        control = GetComponent<UnityChanControlScriptWithRgidBody>();
    }

    void Start() {
        //shoot = GetComponentInChildren<PlayerShoot>();
    }

    void Update() {
        if (!dead) {
            craziness = Mathf.Lerp(craziness, 0f, Time.deltaTime * crazinessFallSpeed);
            //Debug.Log(craziness);
            if (craziness < 0.005f) {
                Dead();
            }
        }
    }

    public void gainExp(float gain) {
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

    public void gainCraziness(float c) {
        craziness += c;
        craziness = Mathf.Clamp01(craziness);
        if (craziness >= 1f)
            _isCrazy = true;
        if (isCrazy) {
            EnterCrazy();
        }
    }

    void EnterCrazy() {
        _isCrazy = true;
        control.forwardSpeed *= madSpeedScale;
        baseAtk *= madAtkScale;
        crazinessFallSpeed *= madCrazinessFallSpeedScale;
    }

    public void ExitCrazy() {
        _isCrazy = false;
        control.forwardSpeed /= madSpeedScale;
        baseAtk /= madAtkScale;
        crazinessFallSpeed /= madCrazinessFallSpeedScale;
    }


    protected override void DeadTemplate() {
        control.enabled = false;
    }

    public void Revive() {
        dead = false;
        control.enabled = true;
    }

    public override void DeadComplete() {
        Debug.Log("dead complete");
        if (!isCrazy)
            GamePlayManager.Instance.PlayerDead();
        //else TODO
    }

    public bool isCrazy { get { return _isCrazy; } }

    public static Player Instance { get { return _instance; } }
}
