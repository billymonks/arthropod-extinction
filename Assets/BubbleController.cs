using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BubbleController : Enemy {
    private float speed;

    private float initSpawnTime = 0.35f;
    private float spawnTime = 0.35f;

    private bool initialized = false;

    private Vector3 initScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 finalScale;

    // Use this for initialization
    void Start () {
        finalScale = transform.localScale;
        transform.localScale = initScale;
    }
	
	// Update is called once per frame
	void Update () {

        if (!initialized)
            return;

        if (spawnTime > 0)
        {
            float t = Math.Min(1f, (initSpawnTime - spawnTime) / initSpawnTime);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            spawnTime -= Time.deltaTime;

            transform.localScale = Vector3.Lerp(initScale, finalScale, t);
        } else
        {
            transform.localScale = finalScale;
        }

        this.transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (initialized && spawnTime <= 0 && !other.CompareTag("SafeToTouch"))
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy en = other.GetComponent<Enemy>();
                if (en is BubbleController)
                    return;

                if(!(en is WormController))
                    other.GetComponent<Enemy>().Hit(1, transform.position, transform.forward);
            }

            if (other.CompareTag("Player"))
            {
                other.GetComponent<ArthropodController>().Hit(1, transform.position, transform.forward);
            }

            //GameObject.Destroy(this.gameObject);
            PoolManager.Pools["Bullet"].Despawn(this.transform);
        }


    }

    public void Initialize(float speed)
    {
        //this.destination = destination;
        this.speed = speed;

        //transform.LookAt(destination);
        //rb = GetComponent<Rigidbody>();

        initialized = true;

        //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    void OnSpawned()
    {
        spawnTime = initSpawnTime;

        //print("spawned lol!");
    }

    void OnDespawned()
    {
        initialized = false;
        transform.localScale = initScale;
        //print("despawned lol!");
    }

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        PoolManager.Pools["Bullet"].Despawn(this.transform);
    }


}
