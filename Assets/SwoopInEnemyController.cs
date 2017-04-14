using UnityEngine;
using System.Collections;
using BansheeGz.BGSpline.Curve;
using PathologicalGames;
using System.Collections.Generic;

public class SwoopInEnemyController : Enemy
{
    public float speed;
    public BGCurve curve;
    public bool looping = false;
    public bool removeAtEnd = false;

    public GameObject projectile, splash, explosion, bloodEffect, smokeEffect;

    private float curveDistance;
    private float curveTotal;
    private BGCurveBaseMath cMath;

    public float attackTime = 2f;
    public float attackGap = 1f;
    public int attackCount = 2;

    private int startAttackCount = 3;

    private Color matColor;
    private List<Color> matColorList;
    private Renderer[] rendList;

    private GameObject smoke;

    public float baseAttackTime;





    // Use this for initialization
    void Start()
    {
        BGCurveBaseMath.Config cfg = new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent);
        cMath = new BGCurveBaseMath(curve, cfg);
        curveDistance = 0f;

        curveTotal = cMath.GetDistance();

        //maxHp = 2;
        //hp = maxHp;

        //rend = GetComponent<Renderer>();
        rendList = GetComponentsInChildren<Renderer>();

        float animTime = Random.Range(0, 1);

        foreach (Animator a in GetComponentsInChildren<Animator>())
        {
            a.Play(0, 0, animTime);
            //a.runtimeAnimatorController.animationClips[0].
        }

        //matColor = rend.material.color;
        matColorList = new List<Color>();

        foreach(Renderer r in rendList)
        {
            matColorList.Add(r.material.color);
        }

        baseAttackTime = attackTime;

    }

    // Update is called once per frame
    void Update()
    {
        //rend.material.color = (matColor + rend.material.color) / 2f;
        for(int i = 0; i < rendList.Length; i++)
        {
            rendList[i].material.color = (matColorList[i] + rendList[i].material.color) / 2f;
        }

        if (dead)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer < 0)
            {
                //GameObject.Destroy(transform.parent.gameObject);

                GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

                PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);

                smoke.transform.SetParent(null, false);

                PoolManager.Pools["Enemies"].Despawn(this.transform.parent);
            }

            return;
        }

        

        if (attackCount > 0 && attackTime > 0f)
        {

            attackTime -= Time.deltaTime;
            if (attackTime <= 0f)
            {
                //GameObject p = (GameObject)GameObject.Instantiate(projectile, player.transform.parent, false);
                Shoot();
            }
            
        }
        

        Vector3 nPos = cMath.CalcPositionByDistance(curveDistance);
        Vector3 nTan = cMath.CalcTangentByDistance(curveDistance);
        transform.position = nPos;
        transform.forward = nTan;

        curveDistance += speed * Time.deltaTime;
        if (curveDistance > curveTotal)
        {

            if (looping)
            {
                curveDistance = curveDistance % curveTotal;
            }
            else
            {
                curveDistance = curveTotal;
            }
            if (removeAtEnd)
            {
                //GameObject.Destroy(transform.parent.gameObject);
                PoolManager.Pools["Enemies"].Despawn(this.transform.parent);
            }
        }


    }

    public Vector3 GetNextPos()
    {
        return cMath.CalcPositionByDistance(curveDistance + speed * Time.deltaTime * 0f);
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

        GameObject bloodObj = PoolManager.Pools["Effects"].Spawn(bloodEffect, hitPos, Quaternion.identity).gameObject;

        PoolManager.Pools["Effects"].Despawn(bloodObj.transform, 2f);

        hp -= dmg;

        foreach(Renderer r in rendList)
        {
            r.material.color = Color.red;
        }
        
        //print("Hit! HP=" + hp);

        if (hp <= 0)
            Die(hitPos, direction);
    }

    private void Die(Vector3 hitPos, Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.None;

        if (!dead)
        {
            smoke = PoolManager.Pools["Effects"].Spawn(smokeEffect, hitPos, Quaternion.identity, transform).gameObject;
            smoke.transform.localScale = Vector3.one * 1.5f;
            PoolManager.Pools["Effects"].Despawn(smoke.transform, 5f);
            rb.AddForceAtPosition(Vector3.down * 100f, hitPos, ForceMode.Impulse);
        }
        
        
        rb.AddForceAtPosition(direction * 100f, hitPos, ForceMode.Impulse);
        
        dead = true;

        
    }

    public override void Reset(GameObject player)
    {
        base.Reset(player);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        hp = maxHp;
        curveDistance = 0f;

        //Vector3 nPos = cMath.CalcPositionByDistance(curveDistance);
        //Vector3 nTan = cMath.CalcTangentByDistance(curveDistance);
        //transform.position = nPos;
        //transform.forward = nTan;

        attackTime = Random.Range(baseAttackTime - 0.2f, baseAttackTime + 0.5f);

        attackCount = startAttackCount;
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.collider.gameObject.CompareTag("Environment"))
        {
            GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

            PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);

            smoke.transform.SetParent(null, false);

            PoolManager.Pools["Enemies"].Despawn(this.transform.parent);

        } else if (dead && c.collider.gameObject.CompareTag("Water"))
        {
            GameObject splashObj = PoolManager.Pools["Effects"].Spawn(splash, transform.position, Quaternion.identity).gameObject;

            PoolManager.Pools["Effects"].Despawn(splashObj.transform, 2f);

            smoke.transform.SetParent(null, false);

            PoolManager.Pools["Enemies"].Despawn(this.transform.parent);

        }
        else if(c.collider.gameObject.CompareTag("Enemy"))
        {

            if (c.collider.gameObject.GetComponent<Enemy>() is WormController)
                this.Hit(3, c.contacts[0].point, c.impulse);
                    //.Hit(1, transform.position, GetComponent<Rigidbody>().velocity);
            //GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

            //PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 2f);

            //PoolManager.Pools["Enemies"].Despawn(this.transform.parent);
        }
        else if (dead)
        {
            GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

            PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);

            smoke.transform.SetParent(null, false);

            PoolManager.Pools["Enemies"].Despawn(this.transform.parent);
        }

    }

    void OnSpawned()
    {
        //spawnTime = initSpawnTime;

        //print("spawned lol!");
    }

    void OnDespawned()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.freezeRotation = true;
        //initialized = false;
        //transform.localScale = initScale;
        //print("despawned lol!");
    }
}