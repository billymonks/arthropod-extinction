using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChangeTrigger : MonoBehaviour {

    public float tSpeed, mod;
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
            other.gameObject.GetComponent<ArthropodController>().ChangeTargetSpeed(tSpeed, mod);
            

        }

    }
}
