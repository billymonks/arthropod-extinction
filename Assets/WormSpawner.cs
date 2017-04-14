using BansheeGz.BGSpline.Curve;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawner : MonoBehaviour {

    private bool active = false;

    public GameObject wormHead, wormBody;

    public List<BGCurve> curves;

    public float spawnLength = 1f;
    private float spawnTimer;

    public int maxSpawnCount = 5;
    private int spawnCount = 0;

    private WormController wormHeadController, wormBodyController;

    private bool stopSpawning = false;
    // Use this for initialization
    void Start () {
        spawnTimer = spawnLength;
	}
	
	// Update is called once per frame
	void Update () {
        if(!active || spawnCount >= maxSpawnCount)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if(spawnTimer < 0f)
        {
            spawnTimer = spawnLength;

            if(spawnCount == 0)
            {
                GameObject wHead = PoolManager.Pools["Enemies"].Spawn(wormHead).gameObject;

                wormHeadController = wHead.GetComponent<WormController>();
                wormHeadController.Initialize(curves, this);
            } else
            {
                GameObject wBody = PoolManager.Pools["Enemies"].Spawn(wormBody).gameObject;

                wormBodyController = wBody.GetComponent<WormController>();
                wormBodyController.Initialize(curves, this);

                wormBodyController.wormHead = wormHeadController;

                foreach (WormController child in wormHeadController.children)
                    child.children.Add(wormBodyController);

                wormHeadController.children.Add(wormBodyController);

                
            }

            spawnCount++;
        }
    }

    public void StopSpawning()
    {
        stopSpawning = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            active = true;
        }

    }
}
