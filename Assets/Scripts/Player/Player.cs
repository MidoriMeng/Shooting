using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float curHP = 50f;
    public float maxHP = 50f;
    public float alarmHPPercent = 0.5f;
    private static Player _instance;

    void Awake () {
        _instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float HPPercent {
        get { return curHP / maxHP; }
    }

    public static Player Instance { get { return _instance; } }
}
