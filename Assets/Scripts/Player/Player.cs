using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float curHealth;
    private static Player _instance;

    void Awake () {
        _instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Player Instance { get { return _instance; } }
}
