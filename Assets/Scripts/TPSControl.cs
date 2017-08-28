using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSControl : MonoBehaviour {
    Vector3 camPosOffset;
    Transform player;

	void Start () {
        player = Player.Instance.transform;
        camPosOffset = transform.position - player.position;
        Debug.Log(camPosOffset);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = player.position + camPosOffset;
	}
}
