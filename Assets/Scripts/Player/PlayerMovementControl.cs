using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControl : MonoBehaviour {
    public float speed = 5f;
    /// <summary>
    /// 倍速
    /// </summary>
    public float accelerate = 2f;

    Vector3 movement = Vector3.zero;
    Rigidbody rb;
    // Use this for initialization
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {
        Move();
    }
    void Move() {
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        movement.y = 0;

        movement = movement.normalized * speed * Time.deltaTime;
        if (Input.GetButton("Accelerate"))
            movement *= 2f;
        rb.MovePosition(transform.position + movement.z * transform.forward + movement.x * transform.right + movement.y * transform.up);
    }
}
