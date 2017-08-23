using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjects : MonoBehaviour {
    public GameObject[] prefabs;
    public float mapSize = 3f;

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void AddObjRandomly() {
        int index = (int)Random.Range(0, prefabs.Length - 0.001f);
        GameObject obj = Instantiate<GameObject>(prefabs[index]);
        obj.transform.parent = transform;

        float range = mapSize * 10f / 2f;
        Vector3 pos = new Vector3(
            Random.Range(-range, range),
            1.1f,
            Random.Range(-range, range)
            );
        obj.transform.localPosition = pos;
        if (index > 1) {
            obj.transform.localRotation = Quaternion.identity;
        }
        else {
            obj.transform.localRotation = Random.rotation;
        }

    }
}
