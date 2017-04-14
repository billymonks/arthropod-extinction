using UnityEngine;
using System.Collections;
using BansheeGz.BGSpline.Curve;
using System;
using System.Collections.Generic;

public class SpiderController : Enemy
{
    enum SpiderState
    {
        Running = 0,
        Charging = 1,
        Attacking = 2,
        Uncharging = 3,
        Dead = 4
    }

    public float speed;
    public BGCurve curve;
    public bool looping = false;
    public bool removeAtEnd = false;

    public float curveDistance;
    private float curveTotal;
    private BGCurveBaseMath cMath;

    public GameObject core;

    private Animator animator;

    private Rigidbody[] rbs;
    private List<Renderer> rbRends;

    private float invisiTimer = 2f;

    private SpiderState state = SpiderState.Running;

    private float remainingChargeTime, remainingAttackTime;
    public float chargeTime = 0.5f, attackTime = 0.2f;

    public GameObject laser;

    Transform target;

    private Quaternion _lookRotation;
    private Vector3 _direction;

    // Use this for initialization
    void Start()
    {
        hp = maxHp;
        BGCurveBaseMath.Config cfg = new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent);
        cMath = new BGCurveBaseMath(curve, cfg);
        curveDistance = 1f;

        curveTotal = cMath.GetDistance();
        animator = GetComponentInChildren<Animator>();

        rbs = transform.GetComponentsInChildren<Rigidbody>();
        rbRends = new List<Renderer>();
        foreach (Rigidbody r in rbs)
        {
            rbRends.Add(r.gameObject.GetComponent<Renderer>());
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nPos = cMath.CalcPositionByDistance(curveDistance);
        Vector3 nTan = cMath.CalcTangentByDistance(curveDistance);
        transform.position = nPos;
        transform.forward = nTan;

        if (state == SpiderState.Running)
        {
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
                    GameObject.Destroy(this.gameObject);
                }
            }
        } else if (state == SpiderState.Charging)
        {
            LaserSlerp(2f);
            remainingChargeTime -= Time.deltaTime;
            if (remainingChargeTime <= 0f)
                state = SpiderState.Attacking;
        } else if (state == SpiderState.Attacking)
        {
            LaserSlerp(1f);
            laser.SetActive(true);
            remainingAttackTime -= Time.deltaTime;
            if (remainingAttackTime <= 0f)
            {
                state = SpiderState.Uncharging;
                laser.SetActive(false);
            }
        }

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
        }


    }

    public Vector3 GetNextPos()
    {
        return cMath.CalcPositionByDistance(curveDistance + speed * Time.deltaTime * 0f);
    }

    public override void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        hp -= dmg;

        print("hit! hp=" + hp);

        if (hp <= 0)
        {
            speed = 0;
            Transform.Destroy(gameObject, 5f);
            core.SetActive(false);
            

            animator.enabled = false;

            foreach (Rigidbody r in rbs)
            {
                r.constraints = RigidbodyConstraints.None;
                
                r.AddForceAtPosition(direction * 100f, hitPos, ForceMode.Impulse);
                
            }

            dead = true;
            state = SpiderState.Dead;



        }
        
    }

    public void BeginAttack(Transform target)
    {
        if (state == SpiderState.Running)
        {
            remainingChargeTime = chargeTime;
            remainingAttackTime = attackTime;
            state = SpiderState.Charging;
            animator.SetBool("attack", true);
            laser.GetComponent<LaserController>().SetTarget(target);
            //laser.transform.parent.parent.LookAt(target);
            this.target = target;
        }
    }

    private void LaserSlerp(float rotationSpeed)
    {
        _direction = (target.position - laser.transform.parent.parent.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        laser.transform.parent.parent.transform.rotation = Quaternion.Slerp(laser.transform.parent.parent.transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);

    }


}