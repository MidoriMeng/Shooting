using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WelcomePage : MonoBehaviour {
    public GameObject highScore;

    void Awake() {
        ScoreManagement.CreateDatas();
    }
    public void Exit() {
        Singleton<GamePlayManager>.Instance.QuitGame();
    }

    public void NewGame() {
        Singleton<GamePlayManager>.Instance.LoadLevel(1);
    }

    public void OpenHighScore() {
        highScore.SetActive(true);
        HighScorePanel panel = highScore.GetComponent<HighScorePanel>();
        panel.OpenHighScore();
    }

    
}
