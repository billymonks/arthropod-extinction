using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class SpawnTrigger : MonoBehaviour {
    //public bool activator = true;
    private GameObject player;
    //public List<GameObject> toSpawn;
    public int count;
    public GameObject toSpawn;
    public Vector3 posMin;
    public Vector3 posMax;
    public Vector3 rotationMin;
    public Vector3 rotationMax;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            for (int i = 0; i < count; i++)
            {
                //GameObject o = (GameObject)GameObject.Instantiate(toSpawn[i], player.transform.parent, false);
                GameObject o = PoolManager.Pools["Enemies"].Spawn(toSpawn, player.transform.parent).gameObject;

                
                //o.transform.rotation = Quaternion.identity;
                o.transform.localRotation = Quaternion.identity;
                o.transform.Rotate(Vector3.Lerp(rotationMin, rotationMax, ((float)i) / ((float)count)));

                o.transform.localPosition = Vector3.Lerp(posMin, posMax, ((float)i) / ((float)count));

                o.GetComponentInChildren<Enemy>().Reset(player);

                //o.SetActive(true);
                //o.transform.Rotate
            }

            GameObject.Destroy(transform.parent.gameObject);
        }

    }
}
