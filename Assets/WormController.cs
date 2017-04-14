using BansheeGz.BGSpline.Curve;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormController : Enemy {
    public bool head;
    private List<BGCurve> curves;
    private int cIndex = 0;
    private BGCurve currentCurve;
    public float curveDistance;
    private float curveTotal;
    private BGCurveBaseMath cMath;
    private BGCurveBaseMath.Config cfg;

    public float speed, rSpeed;

    public float rotation = 0;

    private bool active = false;

    private float currentUnderwaterTime = 0f;
    private bool underwater = false;

    private Collider collider;
    private List<Renderer> rend;

    private bool readyToFire = true;

    public GameObject projectile;
    private float currentAttackTime = 0.5f;
    private float attackTime = 0.5f;

    public float underwaterTime = 3f;

    private Color matColor;

    public List<Transform> turrets;

    public WormController wormHead;
    public List<WormController> children;

    private WormSpawner _spawner;

    private bool waitingForChildren = false;

    private int turretIndex = 0;

    public GameObject splash;

    public GameObject enterSplash, exitSplash;


    // Use this for initialization
    void Start () {
        collider = GetComponent<Collider>();
        rend = new List<Renderer>();
        GetComponentsInChildren<Renderer>(rend);
        rend.Add(GetComponent<Renderer>());

        hp = maxHp;

        matColor = rend[rend.Count-1].material.color;

        children = new List<WormController>();

        currentAttackTime = attackTime;

        //
    }
	
	// Update is called once per frame
	void Update () {
        if (!active)
            return;

        if (head)
        {
            curveDistance += speed * Time.deltaTime;
            rotation += rSpeed * Time.deltaTime;

            for (int i = 0; i < children.Count + 0; i++)
            {
                float separationDistance = 35f;
                float rotationSeparation = 5f;
                children[i].curveDistance = curveDistance - separationDistance * (i+1);
                children[i].rotation = rotation - rotationSeparation * (i + 1);
            }
        }

        if (underwater)
        {
            collider.enabled = false;
            foreach(Renderer r in rend)
                r.enabled = false;

            if (head)
            {
                if (waitingForChildren && AllChildrenUnderwater())
                {
                    waitingForChildren = false;

                    //currentUnderwaterTime = underwaterTime;
                }

                if (!waitingForChildren)
                    currentUnderwaterTime -= Time.deltaTime;

                if (currentUnderwaterTime <= 0f)
                {
                    cIndex++;
                    SetCurve(cIndex);

                    GameObject splashObj = PoolManager.Pools["Effects"].Spawn(splash, cMath.CalcPositionByDistance(0), Quaternion.identity).gameObject;
                    PoolManager.Pools["Effects"].Despawn(splashObj.transform, 5f);
                    foreach (ParticleSystem p in splashObj.GetComponentsInChildren<ParticleSystem>())
                        p.Play();
                    exitSplash = splashObj;

                    foreach (WormController c in children)
                    {
                        c.SetCurve(cIndex);
                    }

                    underwater = false;
                    if (!dead)
                        collider.enabled = true;
                    foreach (Renderer r in rend)
                        r.enabled = true;

                }
                else
                {

                    return;
                }
            } else if (curveDistance < curveTotal && curveDistance > 0f)
            {

                if (children.Count <= 0)
                {
                    foreach (ParticleSystem p in wormHead.exitSplash.GetComponentsInChildren<ParticleSystem>())
                        p.Stop();
                }

                underwater = false;
                readyToFire = true;
                if (!dead)
                    collider.enabled = true;
                foreach (Renderer r in rend)
                    r.enabled = true;
            }
        }

        

        Vector3 nPos = cMath.CalcPositionByDistance(curveDistance);
        Vector3 nTan = cMath.CalcTangentByDistance(curveDistance);
        transform.position = nPos;
        transform.forward = nTan;

        transform.Rotate(0, 0, rotation);


        if (!dead)
            rend[rend.Count - 1].material.color = (matColor + rend[rend.Count - 1].material.color) / 2f;

        //if(!head && curveDistance > curveTotal/2f && readyToFire)
        if (!head && currentAttackTime<=0f && readyToFire)
        {
            currentAttackTime = attackTime;
            Shoot();
            
        } else
        {
            currentAttackTime -= Time.deltaTime;
        }

        if (!underwater && curveDistance > curveTotal)
        {
            if (head)
            {
                GameObject splashObj = PoolManager.Pools["Effects"].Spawn(splash, transform.position, Quaternion.identity).gameObject;
                PoolManager.Pools["Effects"].Despawn(splashObj.transform, 5f);
                foreach (ParticleSystem p in splashObj.GetComponentsInChildren<ParticleSystem>())
                    p.Play();
                enterSplash = splashObj;
                    
            } else if(children.Count <= 0)
            {
                    foreach (ParticleSystem p in wormHead.enterSplash.GetComponentsInChildren<ParticleSystem>())
                        p.Stop();
                    
            }

            turretIndex = 0;
            underwater = true;
            readyToFire = false;

            if (head)
            {
                waitingForChildren = true;

                currentUnderwaterTime = underwaterTime;
            }
        }


        

    }

    private bool AllChildrenUnderwater()
    {
        foreach(WormController w in children)
        {
            if (!w.isUnderwater() /*&& !w.isDead()*/)
                return false;
        }
        return true;
    }

    public override void Hit(int dmg, Vector3 pos, Vector3 dir)
    {
        hp--;

        if (hp <= 0 && !dead)
        {
            Die();
        } else
        {
            rend[rend.Count - 1].material.color = Color.red;
        }
    }

    public void Die()
    {
        //PoolManager.Pools["Enemies"].Despawn(transform);
        foreach (Transform t in turrets)
        {
            t.gameObject.SetActive(false);
        }

        turrets.Clear();

        collider.enabled = false;

        transform.localScale = Vector3.one * 9f;
        rend[rend.Count - 1].material.color = Color.gray;

        dead = true;

        //if(children.Count > 0)
        //{
            //children[0].Die();
        //}

        if(head)
        {
            _spawner.StopSpawning();

            foreach (WormController c in children)
            {
                c.Die();
            }

            PoolManager.Pools["Enemies"].Despawn(gameObject.transform, 0.25f);

            for (int i = 0; i < children.Count; i++)
            {
                PoolManager.Pools["Enemies"].Despawn(children[i].gameObject.transform, 0.5f + i * 0.25f);
            }
        }


    }

    public void Initialize(List<BGCurve> curves, WormSpawner spawner)
    {
        this.curves = curves;
        cfg = new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent);
        SetCurve(cIndex);
        active = true;
        dead = false;
        _spawner = spawner;
    }

    public void SetCurve(int index)
    {
        if (index >= curves.Count)
        {
            PoolManager.Pools["Enemies"].Despawn(transform);
            return;
        } else
        {
            cIndex = index;
        }

        currentCurve = curves[cIndex];
        cMath = new BGCurveBaseMath(currentCurve, cfg);
        curveDistance = 0f;
        curveTotal = cMath.GetDistance();
        readyToFire = true;
    }

    private void Shoot()
    {
        /*for (int i = 0; i < turrets.Count; i++)
        {

            GameObject p = PoolManager.Pools["Bullet"].Spawn(projectile, turrets[i].position, turrets[i].rotation).gameObject;
            p.GetComponent<BubbleController>().Initialize(100f);
            PoolManager.Pools["Bullet"].Despawn(p.transform, 9f);


        }*/

        turretIndex = (++turretIndex) % 2;

        GameObject p = PoolManager.Pools["Bullet"].Spawn(projectile, turrets[turretIndex].position, turrets[turretIndex].rotation).gameObject;
        p.GetComponent<BubbleController>().Initialize(100f);
        PoolManager.Pools["Bullet"].Despawn(p.transform, 9f);

        
    }

    public bool isUnderwater()
    {
        return underwater;
    }
}
