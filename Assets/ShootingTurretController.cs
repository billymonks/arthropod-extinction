using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTurretController : Enemy {

    public GameObject projectile, explosion;

    public float attackTime = 2f;
    public float attackGap = 0.4f;
    public int attackCount = 15;

    private Color matColor;

    private Renderer rend;

    public float attackRange = 200f;

    private float invisiTimer = 2f;

    private Rigidbody[] rbs;
    private List<Renderer> rbRends;
    private Collider _collider;

    private Animator animator;

    public bool ignoreWalls = false;

    // Use this for initialization
    void Start () {
        animator = this.transform.parent.GetComponentInChildren<Animator>();

        _collider = GetComponent<Collider>();

        rbs = this.transform.parent.GetComponentsInChildren<Rigidbody>();
        rbRends = new List<Renderer>();
        foreach (Rigidbody r in rbs)
        {
            rbRends.Add(r.gameObject.GetComponent<Renderer>());
        }

        rend = GetComponent<Renderer>();

        matColor = rend.material.color;

        hp = maxHp;
    }
	
	// Update is called once per frame
	void Update () {

        if (dead)
        {
            invisiTimer -= Time.deltaTime;
            if (invisiTimer <= 0f)
            {
                foreach (Renderer r in rbRends)
                {
                    Color c = r.material.color;
                    c.a -= Time.deltaTime * 1f;
                    r.material.color = c;
                }
            }

            return;
        }


        float distToPlayer = Vector3.Distance(player.transform.position, transform.position);
        
        if(distToPlayer < attackRange)
        {
            attackTime -= Time.deltaTime;

            if (attackTime <= 0 && attackCount > 0)
            {
                Ray r = new Ray(transform.position, player.transform.position - transform.position);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(r, out hit) || ignoreWalls)
                {
                    //if (hit.collider.gameObject.CompareTag("Player"))
                    if (ignoreWalls || hit.distance <= distToPlayer)
                    {
                        transform.LookAt(player.transform);
                        Shoot();
                    }
                }
                attackTime += attackGap;
            }
        }

        
	}

    void Shoot()
    {
        GameObject p = PoolManager.Pools["Bullet"].Spawn(projectile, player.transform.parent).gameObject;
        p.transform.position = transform.position + transform.forward * 0f;
        p.GetComponent<EnemyProjectileController>().Initialize(player.transform.position, 300f);
        //GameObject.Destroy(p, 4f);
        PoolManager.Pools["Bullet"].Despawn(p.transform, 4f);

        attackCount--;
        attackTime += attackGap;
    }

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {

        //GameObject bloodObj = PoolManager.Pools["Effects"].Spawn(bloodEffect, hitPos, Quaternion.identity).gameObject;

        //PoolManager.Pools["Effects"].Despawn(bloodObj.transform, 2f);

        hp -= dmg;

        rend.material.color = Color.red;
        //print("Hit! HP=" + hp);

        if (hp <= 0)
            Die(direction, hitPos);
            
            //gameObject.SetActive(false);
           // Die(hitPos, direction);
    }

    void Die(Vector3 direction, Vector3 hitPos)
    {
        //speed = 0;
        Transform.Destroy(gameObject, 5f);

        dead = true;
        GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

        PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);

        rend.enabled = false;

        _collider.enabled = false;
        //this.enabled = false;


        animator.enabled = false;

        foreach (Rigidbody r in rbs)
        {
            r.constraints = RigidbodyConstraints.None;

            r.AddForceAtPosition(direction * 100f, hitPos, ForceMode.Impulse);

        }

        dead = true;
    }
}
