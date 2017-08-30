using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScorePanel : MonoBehaviour {
    public Text[] highScores;
    
    public void OpenHighScore() {
        //highScore.SetActive(true);
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            int score = PlayerPrefs.GetInt(GamePlayManager.HIGHSCORENUMBER + i);
            string name = PlayerPrefs.GetString(GamePlayManager.HIGHSCORENAME + i);
            highScores[i].text = (i + 1) + ".  " + name + "     " + score;
        }
    }

    public void CloseHighScore() {
        gameObject.SetActive(false);
    }

}
