using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableController : Enemy {

    public List<GameObject> explosion;

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        hp--;

        if (hp <= 0)
        {
            foreach (GameObject obj in explosion)
            {
                GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(obj, transform.position, Quaternion.identity).gameObject;

                explosionObj.transform.rotation = transform.rotation;

                PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);
            }

            gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start () {
        hp = maxHp;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
