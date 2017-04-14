using BansheeGz.BGSpline.Curve;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy {
    protected List<Enemy> children;

    public BGCurve railA, railB, railC;

    public GameObject minion;

    public float minionSpawn;

    private float spawnTime = 0f;

    private int railSelect = 0;

    private Color matColor;
    private List<Renderer> rend;

    // Use this for initialization
    void Start () {
        children = new List<Enemy>();
        hp = maxHp;
        spawnTime = minionSpawn;

        rend = new List<Renderer>();
        GetComponentsInChildren<Renderer>(rend);
        //rend.Add(GetComponent<Renderer>());

        matColor = rend[rend.Count - 1].material.color;
    }
	
	// Update is called once per frame
	void Update () {
        spawnTime -= Time.deltaTime;

        rend[rend.Count - 1].material.color = (matColor + rend[rend.Count - 1].material.color) / 2f;

        if (spawnTime <= 0f)
        {

            spawnTime += minionSpawn;
            GameObject m = PoolManager.Pools["Enemies"].Spawn(minion).gameObject;

            Enemy e = m.GetComponentInChildren<Enemy>();
            e.player = this.player;

            children.Add(e);

            switch((railSelect++)%3)
            {
                case 0:
                    m.GetComponent<BGCurveFollow>().curve = railA;
                    break;
                case 1:
                    m.GetComponent<BGCurveFollow>().curve = railB;
                    break;
                case 2:
                    m.GetComponent<BGCurveFollow>().curve = railC;
                    break;
            }
            

            
        }
	}

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        rend[rend.Count - 1].material.color = Color.red;

        hp -= dmg;

        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        player.GetComponent<ArthropodController>().ChangeTargetSpeed(180f, 0.2f);

        foreach (Enemy o in children)
        {
            if(o != null)
                o.Hit(99, o.transform.position, Vector3.up);
            //o.SetActive(false);
        }
    }
}
