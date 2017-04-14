using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

    public GameObject player;

    public int maxHp;
    protected int hp;

    protected bool dead = false;
    protected float deathTimer = 1f;
    private float startDeathTime = 1f;
    // Use this for initialization
    void Start () {
        hp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void Hit(int dmg, Vector3 hitPos, Vector3 direction);

    public virtual void Reset(GameObject player)
    {
        this.player = player;
        //hp = 1;
        dead = false;
        deathTimer = startDeathTime;
    }

    public bool isDead()
    {
        return dead;
    }
}
