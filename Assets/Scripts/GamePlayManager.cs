using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GamePlayManager : MonoBehaviour {
    private int _curPlayerScore = 0;
    private static GamePlayManager _instance;
    public const int MAX_HIGHSCORE = 10;
    public static string HIGHSCORENUMBER = "HightScore";
    public static string HIGHSCORENAME = "HighScoreName";
    private int highestScore = 0;
    //private List<ScoreStruct> highScores;

    public MouseLook fpsCamera;


    void Awake() {
        //DontDestroyOnLoad(gameObject);
        _instance = this;
        //highScores = LoadHighScores();
        //Debug.Log("game settings awake");
    }

    

    void OnEnable() {
        Debug.Log("game settings enable");
    }

    public void AddScore(int score) {
        _curPlayerScore += score;
        Debug.Log(_curPlayerScore);
    }


    public void NewGame() {
        _curPlayerScore = 0;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void LoadLevel(int level) {
        SceneManager.LoadScene(level);
    }

    public void PlayerDead() {
        fpsCamera.enabled = false;
        EnemyManager.Instance.DestroyAllEnemies();
        //save score
        Debug.Log(_curPlayerScore);
        ScoreManagement.AddNewRecord("player", _curPlayerScore);


        SceneManager.LoadScene(2);
    }

    public int curPlayerScore { get { return _curPlayerScore; } }
    public static GamePlayManager Instance { get { return _instance; } }
}
