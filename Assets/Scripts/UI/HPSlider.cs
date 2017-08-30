using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPSlider : MonoBehaviour {
    public Image hpImg;
    Slider slider;

    void Awake () {
        slider = GetComponent<Slider>();
	}
	
	void Update () {
        float value = slider.value;
        //0.3 0
        //1   0
        value *= 0.3f;
        hpImg.color = Color.HSVToRGB(value, 1f, 1f);
	}
}
