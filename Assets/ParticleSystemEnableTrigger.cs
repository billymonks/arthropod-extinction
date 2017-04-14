using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEnableTrigger : MonoBehaviour
{
    public ParticleSystem ps;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            ps.Play();
            //ps.transform.SetParent(null);
        }
    }
}
