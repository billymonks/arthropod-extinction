using UnityEngine;
using System.Collections;
using BansheeGz.BGSpline.Curve;

public class BGCurveFollowEnemy : Enemy {
    public float speed;
    public BGCurve curve;
    public bool looping = false;
    public bool removeAtEnd = false;

    private float curveDistance;
    private float curveTotal;
    private BGCurveBaseMath cMath;

   
    // Use this for initialization
    void Start()
    {
        hp = 1;
        BGCurveBaseMath.Config cfg = new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent);
        cMath = new BGCurveBaseMath(curve, cfg);
        curveDistance = 1f;

        curveTotal = cMath.GetDistance();
    }
	
	// Update is called once per frame
	void Update ()
    { 
        if(dead)
        {
            deathTimer -= Time.deltaTime;
            if(deathTimer < 0)
            {
                GameObject.Destroy(transform.parent.gameObject);
            }

            return;
        }
        Vector3 nPos = cMath.CalcPositionByDistance(curveDistance);
        Vector3 nTan = cMath.CalcTangentByDistance(curveDistance);
        transform.position = nPos;
        transform.forward = nTan;
        
        curveDistance += speed * Time.deltaTime;
        if (curveDistance > curveTotal)
        {

            if(looping)
            {
                curveDistance = curveDistance%curveTotal;
            } else
            {
                curveDistance = curveTotal;
            }
            if(removeAtEnd)
            {
                GameObject.Destroy(transform.parent.gameObject);
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
        //print("Hit! HP=" + hp);

        if (hp <= 0)
            Die(hitPos, direction);
    }

    private void Die(Vector3 hitPos, Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        if (!dead)
            rb.AddForceAtPosition(Vector3.down * 100f, hitPos, ForceMode.Impulse);
        else
        {
            rb.AddForceAtPosition(direction * 100f, hitPos, ForceMode.Impulse);
        }

        dead = true;
        
        
    }
}
