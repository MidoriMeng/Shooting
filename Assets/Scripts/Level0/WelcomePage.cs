using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WelcomePage : MonoBehaviour {
    public GameObject highScore;
    public Text[] highScores;

    void Awake() {
        CreateDatas();
    }
    public void Exit() {
        Singleton<GamePlayManager>.Instance.QuitGame();
    }

    public void NewGame() {
        Singleton<GamePlayManager>.Instance.LoadLevel(1);
    }

    public void OpenHighScore() {
        highScore.SetActive(true);
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            float score = PlayerPrefs.GetFloat(GamePlayManager.HIGHSCORENUMBER + i);
            string name = PlayerPrefs.GetString(GamePlayManager.HIGHSCORENAME + i);
            highScores[i].text = (i + 1) + ".  " + name + "     " + score;
        }
    }

    public void CloseHighScore() {
        highScore.SetActive(false);
    }

    void CreateDatas() {
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            if (!PlayerPrefs.HasKey(GamePlayManager.HIGHSCORENUMBER + i)) {
                PlayerPrefs.SetFloat(GamePlayManager.HIGHSCORENUMBER + i, 0);
                PlayerPrefs.SetString(GamePlayManager.HIGHSCORENAME + i, string.Empty);
            }
        }
        PlayerPrefs.Save();
    }
}
