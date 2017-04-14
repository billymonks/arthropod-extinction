﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRestorePowerup : MonoBehaviour {
    public int hpAmt;
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
            other.gameObject.GetComponent<ArthropodController>().RestoreHp(hpAmt);
            gameObject.SetActive(false);
        }

    }
}
