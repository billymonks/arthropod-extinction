using UnityEngine;
using System.Collections;

public class SimpleMoveToDestination : MonoBehaviour {
    public GameObject destination;
    public float speed;
    private Rigidbody _rb;
	// Use this for initialization
	void Start () {
        _rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(destination.transform);
        _rb.AddForce(transform.forward * speed,ForceMode.Force);
            //destination.transform.position - transform.position
    }
}
