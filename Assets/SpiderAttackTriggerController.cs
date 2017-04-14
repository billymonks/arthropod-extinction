using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttackTriggerController : MonoBehaviour {
    public SpiderController spider;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spider.BeginAttack(other.gameObject.transform);
        }
    }
}
