using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using PathologicalGames;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArthropodController : MonoBehaviour {
    enum PlayerState
    {
        Cutscene,
        Flight,
        Hit,
        Dead,
        Dodge
    }

    public float speed;
    public new Camera camera;

    private Vector3 prevMousePos = Vector3.zero;

    public Vector2 cameraLag;

    public float hitTime = 0.25f;
    public float flightResetIntensity = 1.25f; // range 1 to 2

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public GameObject projectileA, chargedShotProjectile;
    public float shootLength = 0.3f;

    public GameObject bloodEffect;
    public float timeScale = 1.0f;

    public GameObject splash, explosion, smokeEffect;

    public int maxHp;

    private Vector3 cameraOffset;
    private Rigidbody rb;
    private PlayerState state;
    private float hitResetTime = 0f;
    private GameObject crosshairA, crosshairB, crosshairC, crosshairD;
    private Vector3 rotation = Vector3.zero;
    private Vector3 hitToDirection = Vector3.zero;
    private float sAmt = 10f;
    public GameObject muzzleFlash, chargedMuzzleFlash;
    private GameObject playerModel;
    private Quaternion identityRotation;
    //private BGCurveFollow followScript;
    private AudioSource audioSource;
    private int hp;
    private GameObject focalPoint;
    private float shootTimer;
    private DepthOfField dof;
    private BloomOptimized bloom;

    private Text hpText;

    public GameObject deathUI;

    private GameObject smoke;

    public io.newgrounds.core ngio_core;

    private float targetSpeed, speedChangeMod = 1f;
    private BGCurveFollow _follow;

    private bool indAxis = false;

    private Vector3 movement = Vector3.zero, prevMovement = Vector3.zero;

    public Vector2 flightOffset = Vector2.zero;

    private Vector3 prevPos;

    public GameObject cameraFollowBirdy;

    private bool lockMovement = false, lockonMode = false, dodgeCooldown = false;

    private Vector3 dodgeDirection = Vector3.zero;

    private float burstFireTime = 0f;

    public float burstFireLength = 0.3f;

    public float chargeShotLength = 0.3f;

    private float chargeShotTime = 0.3f;

    public float dodgeSpeed = 100f;
    public float dodgeLength = 1f;
    public float dodgeCooldownLength = 0.5f;

    private float dodgeTimer = 0f, cooldownTimer = 0f;

    public int maxLock = 3;

    public Image playerCrosshair;
    public List<Image> uiLockTarget;

    private List<GameObject> enemyLocks;

    public GameObject lockChargedSfx, lockSfx;

    public bool cameraLookAhead = true;
    public float lookAheadDistance = 100f;
    private Vector3 lookAheadPos = Vector3.zero;

    private Quaternion cameraLockRotation;
    private Vector3 aimDiff = Vector3.zero;

    private float spinDirection = 1;

    public GameObject spinEffects;

    // Use this for initialization
    void Start () {
        hp = maxHp;
        cameraOffset = camera.transform.position - transform.position;
        enemyLocks = new List<GameObject>();

        if (!indAxis)
            cameraOffset += Vector3.up * 3f;

        rb = GetComponent<Rigidbody>();
        state = PlayerState.Flight;
        crosshairA = GameObject.Find("crosshair 1");
        crosshairB = GameObject.Find("crosshair 2");
        crosshairC = GameObject.Find("crosshair 3");
        crosshairD = GameObject.Find("crosshair 4");

        playerModel = GameObject.Find("choopa tank");
        identityRotation = playerModel.transform.localRotation;

        audioSource = GetComponent<AudioSource>();

        focalPoint = GameObject.Find("focal point");

        shootTimer = shootLength;

        dof = camera.GetComponent<DepthOfField>();
        bloom = camera.GetComponent<BloomOptimized>();

        Time.timeScale = this.timeScale;

        hpText = GameObject.Find("HP Text").GetComponent<Text>();

        _follow = GetComponentInParent<BGCurveFollow>();
        targetSpeed = _follow.speed;
        lookAheadPos = _follow.GetAheadPos(lookAheadDistance + _follow.speed);

        transform.position = prevPos;

        //unlockMedal(50260);
    }

    void unlockMedal(int medal_id)
    {
        io.newgrounds.components.Medal.unlock medal_unlock = new io.newgrounds.components.Medal.unlock();
        medal_unlock.id = medal_id;
        medal_unlock.callWith(ngio_core, onMedalUnlocked);
    }

    void onMedalUnlocked(io.newgrounds.results.Medal.unlock result)
    {
        io.newgrounds.objects.medal medal = result.medal;
        Debug.Log("Medal Unlocked: " + medal.name + " (" + medal.value + " points)");
    }


    // Update is called once per frame
    void Update () {
        prevMovement = movement;
        movement = Vector3.zero;
        Vector3 nPos, nCamPos;// = transform.position;
        lookAheadPos = _follow.GetAheadPos(lookAheadDistance + _follow.speed + this.transform.localPosition.magnitude * (1 + _follow.speed) * 0.05f);
        Vector2 targetPos = Vector2.zero;



        aimDiff += (Input.mousePosition - prevMousePos);

        prevMousePos = Input.mousePosition;

        if (indAxis)
        {
            movement.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            movement.y += Input.GetAxis("Vertical") * speed * Time.deltaTime;
        } else
        {
            float fWidth = (float)camera.pixelWidth;
            float fHeight = (float)camera.pixelHeight;
            
            targetPos = minBounds + new Vector2((Input.mousePosition.x / fWidth) * (maxBounds.x-minBounds.x), (Input.mousePosition.y / fHeight) * (maxBounds.y - minBounds.y)) + flightOffset;
            if (targetPos.x - transform.localPosition.x > 1f)
            {
                movement.x += System.Math.Min(targetPos.x - transform.localPosition.x, speed * Time.deltaTime);
            } else if (targetPos.x - transform.localPosition.x < -1f)
            {
                movement.x += System.Math.Max(targetPos.x - transform.localPosition.x, -speed * Time.deltaTime);
            } else
            {
                prevMovement.x = movement.x;
            }

            if(targetPos.y - transform.localPosition.y > 1f)
            {
                movement.y += System.Math.Min(targetPos.y - transform.localPosition.y, speed * Time.deltaTime);
            } else if (targetPos.y - transform.localPosition.y < -1f)
            {
                movement.y += System.Math.Max(targetPos.y - transform.localPosition.y, -speed * Time.deltaTime);
            } else
            {
                prevMovement.y = movement.y;
            }

            if (System.Math.Abs(Vector3.Magnitude(movement - prevMovement)) > 1f)
            {
                movement = (prevMovement * 6f + movement) / 7f;
            }
            
            
            //print(Input.mousePosition + ", " + targetPos + ", " + movement);
        }

        if(targetSpeed != _follow.speed)
        {
            float diffAmt = targetSpeed - _follow.speed;
            float changeAmt = Time.deltaTime * speedChangeMod;

            if (changeAmt < Mathf.Abs(diffAmt))
                _follow.speed = targetSpeed;
            else if(diffAmt < 0)
            {
                _follow.speed -= changeAmt;
            } else
            {
                _follow.speed += changeAmt;
            }


        }

        RayBasedHitDetection();
        
        prevPos = transform.position;

        switch (state)
        {
            case PlayerState.Flight:

                if (lockMovement)
                    movement = Vector3.zero;
                    

                nPos = new Vector3(transform.localPosition.x + movement.x, transform.localPosition.y + movement.y, 0);
                
                transform.localPosition = nPos;
                rotation = new Vector3(((movement.y * -25f) + rotation.x * sAmt) / (sAmt + 1f), ((movement.x * 25f) + rotation.y * sAmt) / (sAmt + 1f), ((movement.x * -25f) + rotation.z * sAmt) / (sAmt + 1f));

                if(dodgeCooldown)
                {
                    cooldownTimer -= Time.deltaTime;

                    if (cooldownTimer <= 0)
                    {
                        dodgeCooldown = false;
                    }
                }

                break;

            case PlayerState.Dodge:

                movement = dodgeDirection * dodgeSpeed * Time.deltaTime;
                //prevMovement = dodgeDirection * dodgeSpeed * Time.deltaTime;

                nPos = new Vector3(transform.localPosition.x + movement.x, transform.localPosition.y + movement.y, 0);

                transform.localPosition = nPos;

                dodgeTimer -= Time.deltaTime;

                if(dodgeTimer <= 0)
                {
                    dodgeCooldown = true;
                    cooldownTimer = dodgeCooldownLength;
                    state = PlayerState.Flight;

                    foreach(ParticleSystem ps in spinEffects.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.Stop();
                    }
                }

                rotation = new Vector3(((movement.y * -25f) + rotation.x * sAmt) / (sAmt + 1f), ((movement.x * 25f) + rotation.y * sAmt) / (sAmt + 1f), Mathf.Lerp(0, 360 * spinDirection, dodgeTimer/dodgeLength));


                break;

            case PlayerState.Hit:
                nPos = new Vector3(transform.localPosition.x / flightResetIntensity, transform.localPosition.y / flightResetIntensity, 0);
                //nPos = new Vector3(transform.localPosition.x + hitToDirection.x, transform.localPosition.y + hitToDirection.y, 0);
                //movement = hitToDirection;

                //movement = -transform.localPosition;
                hitToDirection /= flightResetIntensity;

                transform.localPosition = nPos;
                

                hitResetTime -= Time.deltaTime;

                if (hitResetTime <= 0)
                    state = PlayerState.Flight;

                rotation = new Vector3(((movement.y * -25f) + rotation.x * sAmt) / (sAmt + 1f), ((movement.x * 25f) + rotation.y * sAmt) / (sAmt + 1f), ((movement.x * -25f) + rotation.z * sAmt) / (sAmt + 1f));

                break;

            case PlayerState.Dead:
                camera.transform.LookAt(transform);
                camera.transform.Translate(camera.transform.forward * 1f);
                return;
                //break;
        }

        

        playerModel.transform.localRotation = identityRotation;
        playerModel.transform.Rotate(rotation);


        if (transform.localPosition.x < minBounds.x)
            transform.localPosition = new Vector3(minBounds.x, transform.localPosition.y, transform.localPosition.z);

        if (transform.localPosition.x > maxBounds.x)
            transform.localPosition = new Vector3(maxBounds.x, transform.localPosition.y, transform.localPosition.z);

        if (transform.localPosition.y < minBounds.y)
            transform.localPosition = new Vector3(transform.localPosition.x, minBounds.y, transform.localPosition.z);

        if (transform.localPosition.y > maxBounds.y)
            transform.localPosition = new Vector3(transform.localPosition.x, maxBounds.y, transform.localPosition.z);

        if (this.lockMovement)
        {
            nCamPos = new Vector3(
                (transform.localPosition.x + 0f - (targetPos.x / 30f)), 
                (transform.localPosition.y + 2f - (targetPos.y / 24f)), 
                (transform.localPosition.z - 3f)
                );
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, nCamPos, 0.3f);
            //nCamPos = nCamPos + 
        }
        else
        {
            nCamPos = new Vector3(transform.localPosition.x / cameraLag.x + cameraOffset.x, transform.localPosition.y / cameraLag.y + cameraOffset.y, transform.localPosition.z + cameraOffset.z);
            
            //nCamPos = new Vector3(transform.localPosition.x + cameraOffset.x, transform.localPosition.y + cameraOffset.y, transform.localPosition.z + cameraOffset.z);
            //camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, nCamPos, 0.3f);
            camera.transform.localPosition = nCamPos;
        }
        //camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, nCamPos, 0.3f);

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        crosshairA.transform.position = transform.position + ray.direction * 13f;
        crosshairB.transform.position = transform.position + ray.direction * 17f;
        crosshairC.transform.position = transform.position + ray.direction * 40f;
        crosshairD.transform.position = transform.position + ray.direction * 60f;

        crosshairA.transform.rotation = camera.transform.rotation;
        crosshairB.transform.rotation = camera.transform.rotation;
        crosshairC.transform.rotation = camera.transform.rotation;
        crosshairD.transform.rotation = camera.transform.rotation;

        crosshairA.transform.Rotate(-90, 0, 0);
        crosshairB.transform.Rotate(-90, 0, 0);
        crosshairC.transform.Rotate(-90, 0, 0);
        crosshairD.transform.Rotate(-90, 0, 0);


        //dof.focalTransform = hit.transform;
        //if()
        focalPoint.transform.position = (hit.point + focalPoint.transform.position)/2f;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(ray, hit);
            shootTimer = shootLength;
            burstFireTime = burstFireLength;
            chargeShotTime = chargeShotLength;
        }

        if (Input.GetMouseButton(0))
        {
            chargeShotTime -= Time.deltaTime;
            if(chargeShotTime <= 0f && !lockonMode)
            {
                //lockon on: play sfx
                playerCrosshair.color = Color.red;
                lockonMode = true;

                GameObject lockChargedSound = PoolManager.Pools["Sfx"].Spawn(lockChargedSfx, transform.position, Quaternion.identity, transform).gameObject;
                PoolManager.Pools["Sfx"].Despawn(lockChargedSound.transform, 1f);
            }
            if(lockonMode)
            {
                LockonRay(ray, hit);
                VerifyLocks();
            }
        } else
        {
            if(lockonMode)
            {
                //fire charged shot(s)
                foreach (GameObject g in enemyLocks)
                {
                    ShootChargedShots(g);
                }
                playerCrosshair.color = new Color(.81f, .99f, .16f);
                lockonMode = false;
                RemoveLocks();
            }
        }

        if (burstFireTime > 0f)
        {
            burstFireTime -= Time.deltaTime;
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot(ray, hit);
                shootTimer = shootLength;
            }
        }

        if (Input.GetMouseButton(1) && !dodgeCooldown && state != PlayerState.Dodge)
        {
            dodgeDirection = new Vector3( targetPos.x - transform.localPosition.x, targetPos.y - transform.localPosition.y, transform.localPosition.z).normalized;
            spinDirection = Mathf.Sign(dodgeDirection.x);
            dodgeTimer = dodgeLength;
            state = PlayerState.Dodge;

            foreach (ParticleSystem ps in spinEffects.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }

            /*if (lockMovement == false)
            {
                cameraLockRotation = camera.transform.localRotation;
                aimDiff = Vector3.zero;
            }

            lockMovement = true;
            dof.enabled = true;*/
        } else
        {
            lockMovement = false;
            dof.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            bloom.enabled = !bloom.enabled;
        }

        //if (Input.GetKeyDown(KeyCode.P))
        //{
            //dof.enabled = !dof.enabled;
        //}

        hpText.text = "HP: " + hp;

        if (!lockMovement)
        {
            if (cameraFollowBirdy != null)
            {
                camera.transform.LookAt(cameraFollowBirdy.transform.position);
            }

            if (cameraLookAhead)
            {
                camera.transform.LookAt(lookAheadPos);
                Quaternion lookAheadRot = camera.transform.rotation;
                camera.transform.LookAt(transform);
                camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAheadRot, 0.3f);

            }
        } else
        {
            camera.transform.localRotation = cameraLockRotation;

            camera.transform.Rotate(new Vector3(-aimDiff.y / 15f, aimDiff.x / 20f, 0f));
            //camera.transform.LookAt(hit.point);
        }
    }

    private void RayBasedHitDetection()
    {
        Vector3 dir = transform.position - prevPos;
        Ray r = new Ray(prevPos, dir.normalized);
        RaycastHit rH;

        if (Physics.Raycast(r, out rH, dir.magnitude))
        {
            if (rH.collider.CompareTag("SafeToTouch"))
                return;

            if ((state == PlayerState.Flight || state == PlayerState.Dodge) && (rH.collider.CompareTag("Environment") || rH.collider.CompareTag("Water")))
            {
                state = PlayerState.Flight;
                Hit(1, transform.position, transform.forward);

                foreach (ParticleSystem ps in spinEffects.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Stop();
                }
            }
            else if (state == PlayerState.Flight && rH.collider.CompareTag("Enemy") && !rH.collider.gameObject.GetComponentInParent<Enemy>().isDead())
            {
                Hit(1, transform.position, transform.forward);
            }
            else if (state == PlayerState.Flight && rH.collider.CompareTag("Water"))
            {
                Hit(1, transform.position, transform.forward);
            }
            else if (state == PlayerState.Dead && rH.collider.CompareTag("Environment"))
            {
                GameObject explosionObj = PoolManager.Pools["Effects"].Spawn(explosion, transform.position, Quaternion.identity).gameObject;

                PoolManager.Pools["Effects"].Despawn(explosionObj.transform, 8f);

                smoke.transform.SetParent(null, false);

                Transform.Destroy(gameObject);
            }
            else if (state == PlayerState.Dead && rH.collider.CompareTag("Water"))
            {
                GameObject splashObj = PoolManager.Pools["Effects"].Spawn(splash, transform.position, Quaternion.identity).gameObject;

                PoolManager.Pools["Effects"].Despawn(splashObj.transform, 2f);

                smoke.transform.SetParent(null, false);

                Transform.Destroy(gameObject);
            }
        }
    }

    public void Hit(int dmg, Vector3 hitPos, Vector3 direction)
    {
        if (state == PlayerState.Dead || state == PlayerState.Hit)
            return;

        if(state == PlayerState.Dodge)
        {
            //play happy sfx, get points / deflect?

            return;

        }

        GameObject bloodObj = PoolManager.Pools["Effects"].Spawn(bloodEffect, hitPos, Quaternion.identity, transform).gameObject;

        bloodObj.transform.LookAt(direction);

        PoolManager.Pools["Effects"].Despawn(bloodObj.transform, 2f);

        hp -= dmg;

        state = PlayerState.Hit;
        hitResetTime = hitTime;

        hitToDirection = Vector3.Normalize(transform.position - hitPos) * 6f;

        if (hp <= 0)
        {
            state = PlayerState.Dead;
            transform.parent.GetComponent<BGCurveFollow>().enabled = false;
            rb.useGravity = true;
            smoke = PoolManager.Pools["Effects"].Spawn(smokeEffect, hitPos, Quaternion.identity, transform).gameObject;
            smoke.transform.localScale = Vector3.one * 1.5f;
            PoolManager.Pools["Effects"].Despawn(smoke.transform, 5f);
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(camera.transform.forward * 100f, ForceMode.Impulse);
            rb.AddForce(Vector3.down * 100f, ForceMode.Impulse);
            hpText.enabled = false;
            deathUI.SetActive(true);

            for (int i = 0; i < camera.transform.childCount; i++)
                Transform.Destroy(camera.transform.GetChild(i).gameObject);
        }

        //rend.material.color = Color.red;
        //print("Hit! HP=" + hp);

        //if (hp <= 0)
        //Die(hitPos, direction);
    }

    void LockonRay (Ray ray, RaycastHit hit)
    {
        if (enemyLocks.Count >= maxLock)
            return;

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            if(!enemyLocks.Contains(hit.collider.gameObject))
            {
                enemyLocks.Add(hit.collider.gameObject);

                GameObject lockSound = PoolManager.Pools["Sfx"].Spawn(lockSfx, transform.position, Quaternion.identity, transform).gameObject;
                PoolManager.Pools["Sfx"].Despawn(lockSound.transform, 1f);
            }
        }
    }

    void VerifyLocks()
    {
        for (int i = enemyLocks.Count - 1; i >= 0; i--)
        {
            if(enemyLocks[i] == null || !enemyLocks[i].activeInHierarchy)
            {
                enemyLocks.Remove(enemyLocks[i]);
                continue;
            }

            Vector3 screenPoint = camera.WorldToViewportPoint(enemyLocks[i].transform.position);
            bool enemyVisible = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if(!enemyVisible)
            {
                enemyLocks.Remove(enemyLocks[i]);
            }
        }

        for(int i = 0; i < enemyLocks.Count; i++)
        {
            if (enemyLocks[i] == null)
                continue;

            Vector3 screenPoint = camera.WorldToViewportPoint(enemyLocks[i].transform.position);
            uiLockTarget[i].gameObject.SetActive(true);
            uiLockTarget[i].rectTransform.localPosition = new Vector3(screenPoint.x * Screen.width - Screen.width /2f, screenPoint.y * Screen.height - Screen.height/2f, 0);
        }

        for(int i = enemyLocks.Count; i < uiLockTarget.Count; i++)
        {
            if (uiLockTarget[i] == null)
                continue;

            uiLockTarget[i].gameObject.SetActive(false);
        }

    }

    void RemoveLocks()
    {
        enemyLocks.Clear();
        foreach(Image i in uiLockTarget)
        {
            i.gameObject.SetActive(false);
        }
    }

    void Shoot (Ray ray, RaycastHit hit)
    {

        GameObject nFlash = PoolManager.Pools["Effects"].Spawn(muzzleFlash, transform.position + transform.forward, Quaternion.identity, transform).gameObject;

        nFlash.transform.LookAt(hit.point);
        nFlash.SetActive(true);
        nFlash.GetComponent<ParticleSystem>().Play();
        nFlash.GetComponent<AudioSource>().Play();

        PoolManager.Pools["Effects"].Despawn(nFlash.transform, 1f);
       

        if (hit.collider != null)
        {
            //GameObject nProjectile = PoolManager.Pools["Bullet"].Spawn(projectileA, transform.position, Quaternion.identity, null).gameObject;
            GameObject nProjectile = PoolManager.Pools["Bullet"].Spawn(projectileA, transform.position, Quaternion.identity, transform).gameObject;

            //nProjectile.transform.position = transform.position;
            nProjectile.transform.localPosition = Vector3.zero;
            //nProjectile.transform.SetParent(null);

            PlayerHitscanProjectileController projectileController = nProjectile.GetComponent<PlayerHitscanProjectileController>();
            projectileController.Initialize(hit.point, 800);

            nProjectile.transform.LookAt(hit.point);
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                projectileController.SetTarget(hit.collider.gameObject);

            }
        } else
        {
            
        }

    }

    void ShootChargedShots(GameObject g)
    {
        if (g == null)
            return;

        GameObject nFlash = PoolManager.Pools["Effects"].Spawn(chargedMuzzleFlash, transform.position + transform.forward, Quaternion.identity, transform).gameObject;

        nFlash.transform.LookAt(g.transform.position);
        nFlash.SetActive(true);
        nFlash.GetComponent<ParticleSystem>().Play();
        nFlash.GetComponent<AudioSource>().Play();

        PoolManager.Pools["Effects"].Despawn(nFlash.transform, 1f);


        //if (g.collider != null)
        //{
            GameObject nProjectile = PoolManager.Pools["Bullet"].Spawn(chargedShotProjectile, transform.position, Quaternion.identity, null).gameObject;
            nProjectile.transform.position = transform.position;

            ChargedShotController projectileController = nProjectile.GetComponent<ChargedShotController>();
            projectileController.Initialize(g.transform.position, 800);

            nProjectile.transform.LookAt(g.transform.position);
            
            projectileController.SetTarget(g);


        //}
        //else
        //{

        //}
    }

    public void ChangeTargetSpeed(float speed, float mod)
    {
        targetSpeed = speed;
        speedChangeMod = mod;
    }

    public void RestoreHp(int amt)
    {
        hp += amt;
        if (hp > maxHp)
            hp = maxHp;
    }
}
