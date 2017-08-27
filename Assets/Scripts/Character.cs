using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    protected float curHP = 50f;
    protected float maxHP = 50f;
    public float alarmHPPercent;
    protected float atk = 10f;
    protected float def = 3f;
    protected float selfHeal = 0.03f;

    void Start() {

    }

    void Update() {

    }
    
    public virtual float MakeDamage() { return atk; }
    /// <summary>
    /// 可以返回负值
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public virtual float ReceiveDamage(float damage) { return damage - def; }

    public virtual void Attack(Character c) { 
        float dmg = c.ReceiveDamage(MakeDamage());
        c.curHP -= Mathf.Clamp(dmg, 1, dmg);
        c.curHP = Mathf.Clamp(c.curHP, 0, c.maxHP);
        Debug.Log(name + " attack " + c.name + ", damage: " + dmg);
    }

    public float HPPercent {
        get { return curHP / maxHP; }
    }
}
