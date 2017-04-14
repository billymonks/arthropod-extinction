using UnityEngine;
using System.Collections;

public class ActivationTriggerController : MonoBehaviour {
    public bool activator = true;
    public System.Collections.Generic.List<GameObject> toActivate;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            foreach(GameObject o in toActivate)
                o.SetActive(activator);
        }

    }
}
