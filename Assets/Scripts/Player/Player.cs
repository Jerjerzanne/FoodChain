using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the player information
/// </summary>
public class Player : Destructible
{
    #region Constants

    private const int wallLayer = 13;
    private const int playerLayer = 9;
    private const int entityLayer = 10;
    private const int thresholdInterval = 5;
    #endregion

    #region Variables

    //Editor variables
    [SerializeField, Header("Player")]
    private int initialGrowth;
    private const int maxGrowMeter = 5;
    public int growthMeter;
    public int maxGrowth;

    [SerializeField, Header("Gun")]
    public List<Gun> playerGuns;
    public Gun playerGun;
    public GameObject bulletPrefab;
    public float rateOfFire;
    public float offset = 0.5f;
    public int burstSize = 1;

    [SerializeField, Header("Weapon")]
    public int damage = 2;
    public float bulletSpeed = 2;

    [Header("Ammo")]
    public Text ammoText;
    public int ammoCount;
    public int refreshAmmoRate = 2;
    protected bool cooldown;

    [Header("UI")]
    public Image damageImage;
    public Text growthText;
    public Image growthBar;
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     //The colour the damageImage is set to, to flash.
    #endregion

    #region Properties

    /// <summary>
    /// Current growth level of the player
    /// </summary>
    public int CurrentGrowth { get; set; }


    #endregion

    #region Functions

    void UpdateMaxAmmo()
    {
        ammoCount = playerGun.maxAmmo;

        ammoText.text = "Ammo: " + ammoCount.ToString();
    }

    protected void Awake()
    {
        base.Awake();
        CurrentGrowth = initialGrowth;
        UpdateMaxAmmo();
    }

    protected void Update()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Debug.Log("Do you reach FireSingle()");
            if (ammoCount > 0)
            {
                playerGun.FireSingle();
                ammoCount --;
                
            }
            //FireSingle();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Debug.Log("Do you reach Burst()");
            if (ammoCount > 0 && ammoCount >= playerGun.burstSize)
            {
                playerGun.FireBurst();
                Debug.Log("Burst size is" + playerGun.burstSize);
                ammoCount -= playerGun.burstSize;
            }
        }
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            playerGun = playerGuns[0];
        }
        if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
        {
            if (maxGrowth >= thresholdInterval * 1)
            {
                playerGun = playerGuns[1];
            }
        }
        if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
        {
            if (maxGrowth >= thresholdInterval * 2)
            {
                playerGun = playerGuns[2];
            }
        }
        
        damagedUI();
        AutoReload();
    }
    public void damagedUI()
    {
        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
        damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }
    public void AutoReload()
    {

        if (ammoCount < playerGun.maxAmmo && cooldown == false)
        {
            cooldown = true;
            StartCoroutine(ReloadAmmo());
        }

    }

    public IEnumerator ReloadAmmo()
    {
        ammoCount ++;
        yield return new WaitForSeconds(refreshAmmoRate);
        cooldown = false;
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

        if (maxGrowth <= 15)
        {
            //For testing, simply change the code below to maxGrowth += 1

            growthMeter += 1;
            maxGrowth += 1;

            if (true) //(growthMeter >= 10)
            {
                //maxGrowth += 1;
                //growthMeter = 0;

                if (maxGrowth % thresholdInterval == 0)
                {
                    // Grows in size after 5 growths
                    transform.localScale += new Vector3(0.5F, 0, 0.5F);


                    int growthIndex = (int)maxGrowth / thresholdInterval;
                    if (growthIndex < 3)
                    {
                        CurrentGrowth = growthIndex;
                        playerGun = playerGuns[growthIndex];
                    }

                }
                else if (maxGrowth % thresholdInterval == 1)
                {
                    maxHealth += 10;

                }
                else if (maxGrowth % thresholdInterval == 2)
                {
                    TopdownController moveScript = gameObject.GetComponent<TopdownController>();
                    moveScript.speed += 2;
                }
                else if (maxGrowth % thresholdInterval == 3)
                {
                    playerGun.damage += 1;

                }
                else if (maxGrowth % thresholdInterval == 4)
                {
                    playerGun.burstSize++;
                }
            }
        }

        if (growthBar != null)
        {
            Debug.Log(this.name + " has fill amount" + growthMeter + " " + maxGrowMeter);
            growthBar.fillAmount = (float)growthMeter / maxGrowMeter;
            Debug.Log(this.name + " has fill amount" + growthBar.fillAmount);
        }
        growthText.text = "Growth Level: " + maxGrowth.ToString();
    }

    #endregion
}
