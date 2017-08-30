using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour {
    //public Text showText;
    public Button newGameButton;
    public GameObject highScorePanel;
    InputField input;
    bool showNewgame = false;

    void Awake() {
        input = GetComponent<InputField>();
        //gameObject.SetActive(false);
    }

    void Update() {
        if (!showNewgame) {
            if (Input.GetMouseButton(0) && !highScorePanel.activeSelf) {
                highScorePanel.SetActive(true);
                HighScorePanel panel = highScorePanel.GetComponent<HighScorePanel>();
                panel.OpenHighScore();
                showNewgame = true;
            }
        }
        else
            newGameButton.gameObject.SetActive(true);
    }

    public void NewGame() {
        SceneManager.LoadScene(0);
    }
}
