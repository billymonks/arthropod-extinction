using UnityEngine;
using System.Collections;
using BansheeGz.BGSpline.Curve;

public class SpawnerController : MonoBehaviour {
    public float spawnTime;
    private float currentTime = 0f;
    public GameObject spawnObject;
    public BGCurve curveObject;

    private GameObject player;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if(currentTime > spawnTime)
        {
            Object o = GameObject.Instantiate(spawnObject);

            ((GameObject)o).GetComponentInChildren<BGCurveFollowEnemy>().curve = curveObject;
            currentTime %= spawnTime;
        }
	}
}
