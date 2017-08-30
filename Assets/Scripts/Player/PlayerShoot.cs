using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {
    public Player player;
    public GameObject bulletPrefab;
    public float timeBetweenBullets = 0.15f;
    public float shootRange = 100f;
    float timer = 0f;
    public int maxBullet = 30;
    public int curBullet;

    //Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
    Light gunLight;                                 // Reference to the light component.
    //public Light faceLight;								// Duh
    float effectsDisplayTime = 0.2f;   

    void Awake() {
        curBullet = maxBullet;

        shootableMask = LayerMask.GetMask("Enemy");

        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
		
	}

    void Update() {
        timer += Time.deltaTime;
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
            Shoot();
        }
        if (timer >= timeBetweenBullets * effectsDisplayTime) {
            DisableEffects();
        }
        if (Input.GetButtonDown("Reload")) {
            Reload();
        }
	}

    void OnDisable() {
        DisableEffects();
    }
    public void DisableEffects() {
        gunLine.enabled = false;
        //faceLight.enabled = false;
        gunLight.enabled = false;
    }

    void Shoot() {
        if (curBullet > 0) {
            timer = 0f;
            gunAudio.Play();
            gunLight.enabled = true;
            //faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            //GameObject bullet=Instantiate(bulletPrefab
            Ray shootRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Debug.DrawRay(shootRay.origin, shootRay.direction * 15f, Color.red);
            RaycastHit shootHit;
            if (Physics.Raycast(shootRay, out shootHit, shootRange, shootableMask)) {
                Enemy enemy = shootHit.collider.GetComponent<Enemy>();
                if (enemy != null) {
                    player.Attack(enemy, shootHit.point);
                    gunLine.SetPosition(1, shootHit.point);
                }
            }
            else {
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * shootRange);
            }
            curBullet--;
        }
        else
            UIManager.Instance.Toast("Press R to reload bullet");
    }


    public void Reload() {
        curBullet = maxBullet;
    }
}
