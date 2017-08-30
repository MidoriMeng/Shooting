using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AssignBlocks : MonoBehaviour {
	public GameObject[] Brush;
	public float blockLen;
	public TextAsset[] Layers;
	public Transform scene;

	void Awake() {
	}

	[ContextMenu("assgin blocks")]
	public void Assign() {
		DeleteChildren();
		for (int mapIndex = 0; mapIndex < Layers.Length; mapIndex++) {
			string map = Layers[mapIndex].text;
			//Debug.Log (Layers [mapIndex].name);
			AssignMap(map, float.Parse(Layers[mapIndex].name));
		}
	}

	void DeleteChildren() {
		Transform[] objs = scene.GetComponentsInChildren<Transform>();
		for (int i = objs.Length - 1; i > 0; i--) {
			DestroyImmediate(objs[i].gameObject);
		}
	}

	void AssignMap(string map, float height) {
		string[] rows = map.Split('\n');
		int rowLen = rows.Length;
		int colLen = rows[0].Split(' ').Length;
		//int[,] blockData = new int[rowLen, colLen];//2, 3
		//遍历这张map
		for (int i = 0; i < rowLen; i++) {
			string row = rows[i];
			string[] col = row.Split(' ');
			for (int j = 0; j < colLen; j++) {
				int objName = Int32.Parse(col[j]);
				//Debug.Log (objName);
				bool skip = false;
				if (objName == 0) {
					skip = true;
				}
				if (!skip) {
                    //Debug.Log(objName);
					GameObject obj = Brush[objName];
					GameObject.Instantiate(obj, new Vector3(i * blockLen, height, j * blockLen), obj.transform.rotation, scene);
				}
			}
		}
	}
}