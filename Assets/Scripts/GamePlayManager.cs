using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GamePlayManager : MonoBehaviour {
    private int curPlayerScore = 0;
    private static GamePlayManager _instance;
    public const int MAX_HIGHSCORE = 10;
    public static string HIGHSCORENUMBER = "HightScore";
    public static string HIGHSCORENAME = "HighScoreName";
    private int highestScore = 0;
    private List<ScoreStruct> highScores;

    public struct ScoreStruct {
        public string name;
        public int score;
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);        
        _instance = this;
        //load highScores
        
        Debug.Log("game settings awake");
    }

    List<ScoreStruct> LoadHighScores() {
        List<ScoreStruct> res = new List<ScoreStruct>();
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            ScoreStruct s;
            s.score = PlayerPrefs.GetInt(GamePlayManager.HIGHSCORENUMBER + i);
            s.name = PlayerPrefs.GetString(GamePlayManager.HIGHSCORENAME + i);
            res.Add(s);
            if (s.score > highestScore)
                highestScore = s.score;
        }
        return res;
    }

    void OnEnable() {
        Debug.Log("game settings enable");
    }

    public void AddScore(int score) {
        curPlayerScore += score;
        Debug.Log(curPlayerScore);
    }

    void Update() {

    }

    public void NewGame() {
        curPlayerScore = 0;
    }
    
    public void QuitGame() {
        Application.Quit();
    }

    public void LoadLevel(int level) {
        SceneManager.LoadScene(level);
    }

    public void PlayerDead() { 

    }

    public static GamePlayManager Instance { get { return _instance; } }
}
