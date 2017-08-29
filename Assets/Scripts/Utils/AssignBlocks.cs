using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AssignBlocks : MonoBehaviour {
    public GameObject[] prefabs;
    public float blockLen;
    public TextAsset[] maps;
    public Transform scene;

    void Awake() {
    }

    [ContextMenu("assgin blocks")]
    public void Assign() {
        for (int mapIndex = 0; mapIndex < maps.Length; mapIndex++) {
            string map = maps[mapIndex].text;
            AssignMap(map, float.Parse(maps[mapIndex].name));
        }
    }

    void AssignMap(string map, float height) {
        string[] rows = map.Split('\n');
        int rowLen = rows.Length;
        int colLen = rows[0].Split(' ').Length;
        int[,] blockData = new int[rowLen, colLen];//2, 3
        //遍历这张map
        for (int i = 0; i < rowLen; i++) {
            string row = rows[i];
            string[] col = row.Split(' ');
            for (int j = 0; j < colLen; j++) {
                int objName = Int32.Parse(col[j]);
                if (objName == 0)
                    return;
                GameObject obj = prefabs[objName];
                GameObject.Instantiate(obj, new Vector3(i * blockLen, height, j * blockLen), Quaternion.identity, scene);
            }
        }
    }
}
