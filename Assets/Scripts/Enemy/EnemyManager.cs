using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private static EnemyManager _instance;
    public int poolSize = 10;
    public GameObject enemyPrefab;
    public Material material;
    public Mesh[] enemyMeshes;
    public Transform[] spawnPoints;
    public Transform[] destinations;
    public float spawnTime = 3f;

    List<poolObject> pool;
    private List<Enemy> allEnemies;
    struct poolObject {
        public int hashID;
        public bool inUse { get { return obj.activeSelf; } }
        public GameObject obj;
    };

    void Awake() {
        _instance = this;
        allEnemies = new List<Enemy>();

        pool = new List<poolObject>();
        for (int i = 0; i < poolSize; i++) {
            poolObject pObj;
            pObj.obj = Instantiate(enemyPrefab);
            pObj.obj.transform.SetParent(transform);
            pObj.obj.SetActive(false);
            pObj.hashID = pObj.obj.GetHashCode();
            pool.Add(pObj);
        }
    }

    GameObject getPoolObject() {
        foreach (poolObject pObj in pool) {
            if (!pObj.inUse) {
                pObj.obj.SetActive(true);
                return pObj.obj;
            }
        }
        poolObject npObj;
        npObj.obj = Instantiate(enemyPrefab);
        npObj.obj.transform.SetParent(transform);
        npObj.obj.SetActive(true);
        npObj.hashID = npObj.obj.GetHashCode();
        pool.Add(npObj);
        return npObj.obj;
    }

    void Start() {
        InvokeRepeating("Spawn", spawnTime, spawnTime * Random.Range(0.5f, 1.5f));
    }

    void Update() {
        Gaze(Input.GetButton("Gaze"));
    }

    public void Spawn() {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        int enemyIndex = Random.Range(0, enemyMeshes.Length);

        GameObject obj = getPoolObject();
        obj.transform.position = spawnPoints[spawnPointIndex].position;
        obj.transform.rotation = spawnPoints[spawnPointIndex].rotation;
        ChangeEnemyAppearance(obj, enemyMeshes[enemyIndex]);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.command = new GotoAndDisappear(destinations[spawnPointIndex].position, Command.TypeEnum.ComeUp);
        //Debug.Log("assign mission");
    }

    void ChangeEnemyAppearance(GameObject obj, Mesh mesh) {
        SkinnedMeshRenderer renderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        renderer.sharedMesh = mesh;
        renderer.sharedMaterial = material;
    }

    public void Recycle(Enemy enemy) {
        enemy.gameObject.SetActive(false);
        if (pool.Count > poolSize) {//删掉多余
            int id = enemy.gameObject.GetHashCode();
            for (int i = 0; i < pool.Count; i++) {
                poolObject pObj = pool[i];
                if (id == pObj.hashID) {
                    pool.Remove(pObj);
                    break;
                }
            }
            Destroy(enemy.gameObject);
        }

    }

    public void Gaze(bool isOn) {
        foreach (poolObject obj in pool) {
            //if (obj.inUse) {
            Enemy enemy = obj.obj.GetComponent<Enemy>();
            enemy.gazed = isOn;
            //}
        }
    }

    public void EnemyDead(Enemy enemy) {
        GamePlayManager.Instance.AddScore(enemy.rewardScore);
        Player player = Player.Instance;
        player.gainExp(enemy.rewardExp);
        player.gainCraziness(enemy.rewardCraziness);
    }

    public static EnemyManager Instance { get { return _instance; } }
}
