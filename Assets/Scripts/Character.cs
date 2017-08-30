using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    protected float curHP = 50f;
    protected float baseMaxHP = 50f;
    public float alarmHPPercent;
    protected float baseAtk = 10f;
    protected float baseDef = 3f;
    protected float selfHeal = 0.03f;
    protected bool dead = false;
    protected int curLevel = 1;

    protected static int idleState = Animator.StringToHash("Base Layer.Dead");

    protected Rigidbody rb;
    protected Animator anim;
    protected void AwakeBase() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    protected void UpdateBase() {

    }



    public virtual float MakeDamage() { return atk; }
    /// <summary>
    /// 可以返回负值
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public virtual float ReceiveDamage(float damage) { return damage - def; }

    protected virtual void TakeDamage(Vector3 hitPoint) { }
    /// <summary>
    /// 产生伤害，敌人受到伤害
    /// </summary>
    /// <param name="c"></param>
    public virtual void Attack(Character c, Vector3 point) {
        if (!c.dead) {
            float dmg = c.ReceiveDamage(MakeDamage());
            c.curHP -= Mathf.Clamp(dmg, 1, dmg);
            c.curHP = Mathf.Clamp(c.curHP, 0, c.maxHP);
            c.TakeDamage(point);
            if (c.curHP == 0) {
                c.Dead();
            }
            else
                c.anim.SetTrigger("Light Damage");
            //Debug.Log(name + " attack " + c.name + ", damage: " + dmg);
        }
    }

    public float HPPercent {
        get { return curHP / maxHP; }
    }

    public void Dead() {
        if (!dead) {//act once
            anim.SetTrigger("Dead");
            dead = true;
            DeadTemplate();
        }
    }

    protected virtual void DeadTemplate() { }

    public virtual void DeadComplete() {
        //Destroy(gameObject);
    }

    public bool isDead { get { return dead; } }

    public float maxHP { get { return baseMaxHP * (Mathf.Pow(1.1f, (float)(curLevel - 1))); } }
    public float atk { get { return baseAtk * (Mathf.Pow(1.1f, (float)(curLevel - 1))); } }
    public float def { get { return baseDef * (Mathf.Pow(1.1f, (float)(curLevel - 1))); } }
}
