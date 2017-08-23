using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WelcomePage : MonoBehaviour {
    public GameObject highScore;
    public Text[] highScores;
    public void Exit() {
        Singleton<GameSettings>.Instance.QuitGame();
    }

    public void NewGame() {
        Singleton<GameSettings>.Instance.LoadLevel(1);
    }

    public void OpenHighScore() {
        highScore.SetActive(true);
        for (int i = 0; i < highScores.Length; i++) {
            float score = PlayerPrefs.GetFloat(GameSettings.HIGHSCORENUMBER + i);
            string name = PlayerPrefs.GetString(GameSettings.HIGHSCORENAME + i);
            highScores[i].text = (i + 1) + ".  " + name + "     " + score;
        }
    }

    public void CloseHighScore() {
        highScore.SetActive(false);
    }
}
