using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private static Player _instance;

    void Awake () {
        _instance = this;
        atk = 15f;
        def = 3f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public static Player Instance { get { return _instance; } }
}
