using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {
    Renderer r;
    Color c1 = new Color(1f, 0.3f, 0.3f, 0.6f);
    Color c2 = new Color(1f, 0.6f, 0.6f, 0.9f);

    float flashTime = 0, totalFlashTime = 0.4f;


    private Quaternion _lookRotation;
    private Vector3 _direction;

    private Transform target;
    // Use this for initialization
    void Start () {
        r = GetComponent<Renderer>();

        

    }
	
	// Update is called once per frame
	void Update () {
        flashTime = (flashTime + Time.deltaTime) % totalFlashTime;
        
        r.material.color = Color.Lerp(c1, c2, (float)(Math.Sin(((flashTime / totalFlashTime) * Math.PI * 2.0)) + 1f)/2f);

        //transform.LookAt()
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ArthropodController>().Hit(1, new Vector3(transform.position.x, other.gameObject.transform.position.y, transform.position.z), Vector3.up);
        } else if (other.CompareTag("Enemy") && !other.gameObject.Equals(transform.parent.gameObject) && !other.gameObject.Equals(transform.parent.parent.gameObject))
        {
            other.gameObject.GetComponent<Enemy>().Hit(1, new Vector3(transform.position.x, other.gameObject.transform.position.y, transform.position.z), Vector3.up);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        
    }
}
