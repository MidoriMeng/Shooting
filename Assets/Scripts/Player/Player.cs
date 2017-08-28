using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private static Player _instance;
    UnityChanControlScriptWithRgidBody control;

    public GameObject bulletPrefab;
    public float timeBetweenBullets = 0.15f;
    public float shootRange = 100f;
    float timer = 0f;

    void Awake () {
        AwakeBase();
        _instance = this;
        atk = 15f;
        def = 3f;
        //rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
        Debug.Log(rb);
        control = GetComponent<UnityChanControlScriptWithRgidBody>();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
            Shoot();
        }
	}

    void Shoot() {
        timer = 0f;
        //GameObject bullet=Instantiate(bulletPrefab
        Ray shootRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(shootRay.origin, shootRay.direction * 15f, Color.red);
        RaycastHit shootHit;
        if (Physics.Raycast(shootRay, out shootHit, 15f, LayerMask.GetMask("Enemy"))) {
            Enemy enemy = shootHit.collider.GetComponent<Enemy>();
            if (enemy != null) {
                Attack(enemy/*, shootHit.point*/);
            }
        }

    }

    protected override void DeadTemplate() {
        control.enabled = false;
    }

    public override void DeadComplete() {
        Debug.Log("dead");
    }

    public static Player Instance { get { return _instance; } }
}
