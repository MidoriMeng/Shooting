using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    BulletPanel bulletPanel;
    Player player;
    void Start () {
        bulletPanel = GetComponentInChildren<BulletPanel>();
        player = Player.Instance;
        bulletPanel.Init(player.shoot.curBullet, player.shoot.maxBullet);
	}
	
	void Update () {
        bulletPanel.UpdateBullet(player.shoot.curBullet);
	}
}
