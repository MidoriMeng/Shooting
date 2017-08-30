using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private static Player _instance;
    UnityChanControlScriptWithRgidBody control;
    public PlayerShoot shoot;
    //PlayerShoot shoot;
    float _curExp = 0;
    float maxExp = 50f;
    float _craziness = 0.5f;
    public float crazinessFallSpeed = 0.05f;
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
        shoot = GetComponentInChildren<PlayerShoot>();
    }

    void Start() {
    }

    void Update() {
        if (!dead) {
            _craziness = Mathf.Lerp(_craziness, 0f, Time.deltaTime * crazinessFallSpeed);
            //Debug.Log(_craziness);
            if (_craziness < 0.005f) {
                Dead();
            }
        }
    }

    public void gainExp(float gain) {
        _curExp += gain;
        if (_curExp >= maxExp) {
            //level up
            curLevel++;
            _curExp -= maxExp;
            maxExp *= 1.1f;
            maxExp = (int)maxExp;
            Debug.Log("level " + curLevel + "　　next level exp: " + maxExp);
            curHP = maxHP;
        }
    }

    public void gainScore(int gain) {
        GamePlayManager.Instance.AddScore(gain);
    }

    public void gainCraziness(float c) {
        _craziness += c;
        //Debug.Log(c);
        _craziness = Mathf.Clamp01(_craziness);
        if (_craziness >= 1f)
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
        if (!isCrazy) {
            anim.enabled = false;
            shoot.enabled = false;
            control.enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            GamePlayManager.Instance.PlayerDead();
        }
        //else TODO
    }

    public bool isCrazy { get { return _isCrazy; } }

    public int Level { get { return curLevel; } }
    public float craziness { get { return _craziness; } }
    public int curExp { get { return (int)_curExp; } }
    public static Player Instance { get { return _instance; } }
}
