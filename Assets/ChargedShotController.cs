using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedShotController : MonoBehaviour {

    private Vector3 targetPos;
    private float speed;
    private Vector3 startPos;
    private float totalDistance;
    private float distance = 0;
    private float percent = 0;
    private Vector3 direction;

    private bool active = false;

    private GameObject target = null;

    private Vector3 targetOffset = Vector3.zero;
    private Vector3 prevTargetPos;

    private Vector3 targetScale;

    public GameObject hitPrefab;

    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!active)
            return;

        transform.localScale = (transform.localScale + targetScale) / 2f;

        distance += speed * Time.deltaTime;
        speed += 1000f * Time.deltaTime;

        if (distance >= totalDistance)
        {
            distance = totalDistance;

            percent = distance / totalDistance;

            UpdateOffset();

            transform.position = Vector3.Lerp(startPos, targetPos, percent) + Vector3.Lerp(Vector3.zero, targetOffset, percent);

            if (target != null && target.CompareTag("Enemy"))
            {

                Enemy e = target.GetComponentInParent<Enemy>();

                e.Hit(5, targetPos, direction);
                //enemy.GetComponent<Rigidbody>().freezeRotation = false;
                //enemy.GetComponent<Rigidbody>().AddForce(Vector3.down * 1000f, ForceMode.Impulse);
                //enemy.GetComponent<Rigidbody>().AddForceAtPosition(direction * 1000f, targetPos, ForceMode.Impulse);
                //GameObject.Destroy(enemy.GetComponent<SimpleMoveToDestination>());
                //GameObject.Destroy(enemy);
            }

            audioSource.Play();

            Object nHitPrefab = GameObject.Instantiate(hitPrefab, transform.position, transform.rotation, null);
            ((GameObject)nHitPrefab).GetComponent<ParticleSystem>().Play();
            GameObject.Destroy(nHitPrefab, 1);

            PoolManager.Pools["Bullet"].Despawn(transform, 1f);
            //Transform.Destroy(this.gameObject, 5f);

            //GetComponent<Light>().intensity /= 2f;

            MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].enabled = false;
            }

            //GetComponentInChildren<ParticleSystem>().Stop();

            active = false;
        }
        else
        {

            percent = distance / totalDistance;

            UpdateOffset();

            transform.position = Vector3.Lerp(startPos, targetPos, percent) + Vector3.Lerp(Vector3.zero, targetOffset, percent);
        }
        //gameObject.transform.LookAt(targetPos);
    }

    public void Initialize(Vector3 targetPos, float speed)
    {
        targetScale = transform.localScale;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        this.targetPos = targetPos;
        this.speed = speed;

        startPos = transform.position;
        Vector3 displacement = targetPos - startPos;
        direction = displacement.normalized;
        totalDistance = displacement.magnitude;



        active = true;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        prevTargetPos = target.transform.position;
    }

    private void UpdateOffset()
    {
        if (target != null)
        {
            targetOffset += -prevTargetPos + target.transform.position;
            prevTargetPos = target.transform.position;
        }
    }

    void OnDespawned()
    {
        transform.localScale = targetScale;
        active = false;
        distance = 0;
        percent = 0;
        targetOffset = Vector3.zero;
        target = null;

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = true;
        }
    }
}
