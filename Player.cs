﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player information
/// </summary>
public class Player : Destructible
{
    #region Constants

    private const int wallLayer = 13;
    private const int playerLayer = 9;
    private const int entityLayer = 10;
    #endregion

    #region Variables

    //Editor variables
    [SerializeField, Header("Player")]
    private int initialGrowth;
    public int growthMeter;
    public int maxGrowth;

    [SerializeField, Header("Gun")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float rateOfFire;
    public float offset = 0.5f;
    public int burstSize = 1;

    [SerializeField, Header("Weapon")]
    public int damage = 2;


    #endregion

    #region Properties

    /// <summary>
    /// Current growth level of the player
    /// </summary>
    public int CurrentGrowth { get; set; }


    #endregion

    #region Functions

    protected void Awake()
    {
        base.Awake();
        CurrentGrowth = initialGrowth;
    }

    protected void Update() {

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Fire();
            
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(BurstFire(bulletPrefab, burstSize, rateOfFire));
        }
    }

    protected void Fire()
    {
        Vector3 spawnPos = this.transform.position + this.transform.forward * offset;
        Projectile bullet = Instantiate(bulletPrefab, spawnPos, this.transform.localRotation).GetComponent<Projectile>();

        bullet.SetSpeed(bulletSpeed, damage);
    }

    protected IEnumerator BurstFire(GameObject bulletPrefab, int burstSize, float rateOfFire)
    {

        float bulletDelay = 1 / rateOfFire;

        for (int i = 0; i < burstSize; i++)
        {
            Vector3 spawnPos = this.transform.position + this.transform.forward * offset;
            Projectile playerBullet = Instantiate(bulletPrefab, spawnPos, transform.localRotation).GetComponent<Projectile>();
            playerBullet.SetSpeed(bulletSpeed, damage);
            yield return new WaitForSeconds(bulletDelay); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == entityLayer && other.gameObject.name == "Food(Clone)")
        {
            UpdateGrowth();
            Destroy(other.gameObject);
        }
    }
    protected void UpdateGrowth()
    {
       
        if (maxGrowth < 4)
        {
            //For testing, simply change the code below to maxGrowth += 1
            growthMeter += 1;

            if (growthMeter >= 10)
            {
                maxGrowth += 1;
                growthMeter = 0;
            }

            if (maxGrowth == 1)
            {
                transform.localScale += new Vector3(0.5F, 0, 0.5F);
                //maxHealth += 10;
                //damage += 2;
                //bulletSpeed += 5;
                TopdownController moveScript = gameObject.GetComponent<TopdownController>();
                moveScript.speed += 2;
            }
            else if (maxGrowth == 2)
            {
                burstSize = 2;
                //maxHealth += 10;
                //bulletSpeed += 5;
            }
            else if (maxGrowth == 3)
            {
                burstSize = 3;
                //maxHealth += 10;
                //bulletSpeed += 5;
            }
        }
    }
    #endregion

}