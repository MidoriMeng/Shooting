using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanel : MonoBehaviour {
    public int curBullet;
    public int maxBullet;//30
    public float alpha = 0.2f;
    Image[] imgs;//count = 6
    Text tex;
    int bulletPerImage;//5

    void Awake() {
        imgs = GetComponentsInChildren<Image>();
        tex = GetComponentInChildren<Text>();
    }

    public void Init(int curBullet, int maxBullet) {
        this.curBullet = curBullet;
        this.maxBullet = maxBullet;

        int count = imgs.Length;
        bulletPerImage = maxBullet / count;

        UpdateBullet(curBullet);
    }

    public void UpdateBullet(int curBullet) {
        this.curBullet = curBullet;
        tex.text = getStr;
        int imgLightCount = curBullet / bulletPerImage;
        //Debug.Log(imgLightCount);
        imgLightCount = imgs.Length - imgLightCount;
        for (int i = imgs.Length - 1; i >= 0; i--) {
            Image img = imgs[i];
            if (i < imgLightCount)
                img.color = new Color(1f, 1f, 1f, alpha);
            else
                img.color = Color.white;
        }
    }


    string getStr { get { return curBullet + "/" + maxBullet; } }
}
