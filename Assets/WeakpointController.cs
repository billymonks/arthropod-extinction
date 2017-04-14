using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakpointController : Enemy {
    public Enemy parent;

    public int multiplier;
    private Color matColor;

    private Renderer rend;

    public GameObject weakpointHitEffect;
    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        matColor = rend.material.color;
    }
	
	// Update is called once per frame
	void Update () {
        rend.material.color = (matColor + rend.material.color) / 2f;
    }

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        rend.material.color = Color.red;

        parent.Hit(dmg* multiplier, hitPos, direction);

        GameObject hitEffect = PoolManager.Pools["Sfx"].Spawn(weakpointHitEffect, transform.position, transform.rotation).gameObject;
        PoolManager.Pools["Sfx"].Despawn(hitEffect.transform, 1f);
        //throw new NotImplementedException();
    }
}
