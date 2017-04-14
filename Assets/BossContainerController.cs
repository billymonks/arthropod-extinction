using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossContainerController : MonoBehaviour {

    Animator anim;

    public GameObject boss;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        if(c.CompareTag("Player"))
        {
            ArthropodController arthropod = c.gameObject.GetComponent<ArthropodController>();
            boss.SetActive(true);
            boss.GetComponent<Enemy>().player = c.gameObject;
            arthropod.ChangeTargetSpeed(0f, 1f);
            arthropod.cameraFollowBirdy = boss;
            anim.SetBool("started", true);
        }
    }
}
