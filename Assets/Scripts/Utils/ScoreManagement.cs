using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagement  {

    public struct ScoreStruct {
        public string name;
        public int score;
    };

    public static void CreateDatas() {
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            if (!PlayerPrefs.HasKey(GamePlayManager.HIGHSCORENUMBER + i)) {
                PlayerPrefs.SetFloat(GamePlayManager.HIGHSCORENUMBER + i, i);
                PlayerPrefs.SetString(GamePlayManager.HIGHSCORENAME + i, string.Empty);
            }
        }
        PlayerPrefs.Save();
    }

    public static List<ScoreStruct> LoadHighScores(out float highestScore) {
        highestScore = 0;
        List<ScoreStruct> res = new List<ScoreStruct>();
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            ScoreStruct s;
            s.score = PlayerPrefs.GetInt(GamePlayManager.HIGHSCORENUMBER + i);
            s.name = PlayerPrefs.GetString(GamePlayManager.HIGHSCORENAME + i);
            res.Add(s);
            if (s.score > highestScore)
                highestScore = s.score;
        }

        SortHighScores(res);
        return res;
    }

    public static void SaveHighScores(List<ScoreStruct> list) {
        SortHighScores(list);
        for (int i = 0; i < GamePlayManager.MAX_HIGHSCORE; i++) {
            ScoreStruct s = list[i];
            PlayerPrefs.SetInt(GamePlayManager.HIGHSCORENUMBER + i, s.score);
            PlayerPrefs.SetString(GamePlayManager.HIGHSCORENAME + i, s.name);
        }
    }

    public static void SortHighScores(List<ScoreStruct> list) {
        list.Sort((a, b) => { return b.score - a.score; });
    }

    public static void AddNewRecord(string name, int score) {
        ScoreStruct s;
        s.name = name;
        s.score = score;
        float highestScore = 0;
        List<ScoreStruct> list = LoadHighScores(out highestScore);
        for (int i = 0; i < list.Count; i++) {
            if (s.score > list[i].score) {
                list.Insert(i, s);
                list.RemoveAt(list.Count - 1);
                SaveHighScores(list);
                break;
            }
        }
    }
}
