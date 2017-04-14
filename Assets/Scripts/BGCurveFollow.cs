using UnityEngine;
using System.Collections;
using BansheeGz.BGSpline.Curve;
using UnityEngine.SceneManagement;

public class BGCurveFollow : MonoBehaviour {
    public float speed;
    public BGCurve curve;
    public bool looping = false;
    public bool removeAtEnd = false;

    public float curveDistance;
    private float curveTotal;
    private BGCurveBaseMath cMath;

    public bool loadSceneAtEnd = false;
    public string sceneName = "";

    
    // Use this for initialization
    void Start()
    {
        BGCurveBaseMath.Config cfg = new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent);
        cMath = new BGCurveBaseMath(curve, cfg);
        //curveDistance = 1f;

        curveTotal = cMath.GetDistance();

        
    }
	
	// Update is called once per frame
	void Update ()
    { 
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
                GameObject.Destroy(this.gameObject);
            }
            if(loadSceneAtEnd)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
            

    }

    public Vector3 GetNextPos()
    {
        //return cMath.CalcPositionByDistance(curveDistance + speed * Time.deltaTime * 0f);
        return cMath.CalcPositionByDistance(curveDistance);
    }

    public Vector3 GetAheadPos(float amt)
    {
        return cMath.CalcPositionByDistance(curveDistance + amt);
    }
}
