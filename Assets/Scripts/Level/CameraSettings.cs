using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour {
    public GameObject firstCamera;
    public GameObject thirdCamera;

    private bool _isFirstPerson;

    public bool isFirstPerson {
        get { return _isFirstPerson; }
        set {
            _isFirstPerson = value;
            firstCamera.SetActive(_isFirstPerson);
            thirdCamera.SetActive(!_isFirstPerson);
        }
    }

    void Start() {

    }

    void Update() {

    }
}
