using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {
    private static UIManager _instance;
    public Text scoreText;
    public Text lvText;
    public Text expText;
    public Slider crazinessScroll;
    BulletPanel bulletPanel;
    Player player;
    public Slider hpSlider;
    //public GameObject EndPanel;

    void Awake() {
        _instance = this;
    }
    void Start () {
        bulletPanel = GetComponentInChildren<BulletPanel>();
        player = Player.Instance;
        bulletPanel.Init(player.shoot.curBullet, player.shoot.maxBullet);
	}
	
	void Update () {
        bulletPanel.UpdateBullet(player.shoot.curBullet);
        scoreText.text = GamePlayManager.Instance.curPlayerScore.ToString();
        lvText.text = player.Level.ToString();
        expText.text = player.curExp.ToString();
        crazinessScroll.value = player.craziness;
        hpSlider.value = player.HPPercent;
	}

    public void Toast(string word) { 
        
    }
    /*
    public void ShowGameOverPanel(int score, bool isNewHighest) {
        EndPanel.SetActive(true);
    }*/

    public static UIManager Instance { get { return _instance; } }
}
