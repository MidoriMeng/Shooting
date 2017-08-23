using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSettings : MonoBehaviour {
    private int curPlayerScore = 0;
    public const int MAX_HIGHSCORE = 10;
    public static string HIGHSCORENUMBER = "HightScore";
    public static string HIGHSCORENAME = "HighScoreName";
    void Awake() {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < MAX_HIGHSCORE; i++) {
            if (!PlayerPrefs.HasKey(HIGHSCORENUMBER + i)) {
                PlayerPrefs.SetFloat(HIGHSCORENUMBER + i, 0);
                PlayerPrefs.SetString(HIGHSCORENAME + i, string.Empty);
            }
        }
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update() {

    }

    public void NewGame() {
        curPlayerScore = 0;
    }

    public void AddScore(int add) {
        curPlayerScore += add;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void LoadLevel(int level) {
        SceneManager.LoadScene(level);
    }
}
