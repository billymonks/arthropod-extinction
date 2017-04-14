using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLockController : Enemy {

    private Animator anim;

    private Color matColor;

    private List<Renderer> rend;

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        hp--;

        rend[rend.Count - 1].material.color = Color.red;

        if (hp <= 0)
        {
            anim.SetBool("open", true);
            gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        hp = maxHp;

        rend = new List<Renderer>();
        GetComponentsInChildren<Renderer>(rend);
        rend.Add(GetComponent<Renderer>());

        matColor = rend[rend.Count - 1].material.color;
    }

    // Update is called once per frame
    void Update()
    {
        rend[rend.Count - 1].material.color = (matColor + rend[rend.Count - 1].material.color) / 2f;
    }
}