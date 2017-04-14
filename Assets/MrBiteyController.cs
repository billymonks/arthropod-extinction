using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrBiteyController : Enemy {
    public Animator anim;

    private List<Color> matColorList;
    private Renderer[] rendList;
    // Use this for initialization
    void Start () {
        hp = maxHp;

        rendList = GetComponentsInChildren<Renderer>();

        matColorList = new List<Color>();

        foreach (Renderer r in rendList)
        {
            matColorList.Add(r.material.color);
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < rendList.Length; i++)
        {
            rendList[i].material.color = (matColorList[i] + rendList[i].material.color) / 2f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            anim.SetBool("enraged", true);
        }
    }

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        hp -= dmg;

        foreach (Renderer r in rendList)
        {
            r.material.color = Color.red;
        }

        if (hp <= 0)
            GameObject.Destroy(this.gameObject);
        //throw new NotImplementedException();
    }
}
