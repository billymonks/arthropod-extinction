using UnityEngine;
using System.Collections;
using PathologicalGames;

public class EnemyProjectileController : MonoBehaviour {
    private Vector3 destination;
    private float speed;

    //private float lifeTime = 5f;
    private float initSpawnTime = 0.35f;
    private float spawnTime = 0.35f;

    private bool initialized = false;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!initialized)
            return;

        spawnTime -= Time.deltaTime;

        this.transform.position += transform.forward * speed * Time.deltaTime;
        //lifeTime -= Time.deltaTime;
        //if(lifeTime <=0f)
        //{
            //GameObject.Destroy(this.gameObject);
        //}

    }

    void OnTriggerEnter(Collider other)
    {
        if(initialized && spawnTime <= 0 && !other.CompareTag("SafeToTouch"))
        { 
            if(other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().Hit(1, transform.position, transform.forward);
            }

            if(other.CompareTag("Player"))
            {
                other.GetComponent<ArthropodController>().Hit(1, transform.position, transform.forward);
            }

            //GameObject.Destroy(this.gameObject);
            PoolManager.Pools["Bullet"].Despawn(this.transform);
        }

        
    }

    public void Initialize(Vector3 destination, float speed)
    {
        this.destination = destination;
        this.speed = speed;

        transform.LookAt(destination);
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
        //print("despawned lol!");
    }
}
