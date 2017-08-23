using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {
    private int curPlayerScore = 0;

	void Awake () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NewGame() {
        curPlayerScore = 0;
    }

    public void AddScore(int add) {
        curPlayerScore += add;
    }


}
