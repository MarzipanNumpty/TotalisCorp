using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerScript : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    PlayerControls controls;
    Vector3 velocity;
    Vector3 movement;
    float gravity = -9.81f;
    Vector2 move;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.5f;
    bool isGrounded;
    float walkSpeed = 9.0f;
    float runSpeed = 12.0f;
    float currentSpeed;
    bool gunAimed;
    Animator anim;
    AmmoHandler ammoScript;
    bool inventoryOpen;
    [SerializeField] GameObject currentGun; //will need changed so it is referenced in code and not via inspector
    [SerializeField] GameObject meleeWeapon;
    [SerializeField] GameObject pistolWeapon;
    Animator meleeAnim;
    bool scrollInventory;
    int invNum;
    float invTimer;
    float buttonPressedTime;
    public float health = 100f;

    bool onElevator;
    bool haveRedKeyCard;
    bool haveYellowKeyCard;
    bool haveBlueKeyCard;

    public GameObject currentDoor;
    public bool haveWeapon;
    public bool weaponPistol;
    public bool weaponMelee;
    public int weaponSize;

    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject helmetUI;
    Animator helmetAnim;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    public bool gamePaused;
    [SerializeField] GameObject cam1;
    [SerializeField] GameObject cam2;
    mouseLook ml1;
    mouseLook ml2;
    [SerializeField] AudioClip[] breathingClips;
    AudioClip currentBreathingClip;
    public int currentBreathingClipPos;
    AudioSource speaker;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] AudioSource bulletSpeaker;
    [SerializeField] AudioClip hitByBullet;
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Movement.Jump.performed += ctx => Jump();

        controls.Movement.Interact.performed += ctx => Use();

        controls.Movement.Shoot.performed += ctx => ShootGun();
        controls.Movement.reload.performed += ctx => reloadGun();
        controls.Movement.aim.performed += ctx => AimGun();

        controls.Movement.Grenade.performed += ctx => throwGrenade();

        controls.Movement.InventoryLeft.started += ctx => inventory(-2);
        controls.Movement.InventoryLeft.canceled += ctx => cancelInv();
        controls.Movement.InventoryRight.started += ctx => inventory(2);
        controls.Movement.InventoryRight.canceled += ctx => cancelInv();
        controls.Movement.InventoryUp.started += ctx => inventory(1);
        controls.Movement.InventoryUp.canceled += ctx => cancelInv();
        controls.Movement.InventoryDown.started += ctx => inventory(-1);
        controls.Movement.InventoryDown.canceled += ctx => cancelInv();
        controls.Movement.Accept.performed += ctx => pickInvItem(false);
        controls.Movement.escape.performed += ctx => pickInvItem(true);
        controls.Movement.openInv.performed += ctx => OpenInventory();
        controls.Movement.dropItem.performed += ctx => dropItem();
        controls.Movement.swap.performed += ctx => swapInvSlots();
        controls.Movement.leftRotate.performed += ctx => rotateLeft();

        //controls.Movement.Sprint.started += ctx => Run();
        //controls.Movement.Sprint.canceled += ctx => Walk();

        controls.Movement.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => move = Vector2.zero;

        Application.targetFrameRate = 60;

        currentSpeed = walkSpeed;
        anim = GetComponent<Animator>();
        meleeAnim = meleeWeapon.GetComponent<Animator>();
        ammoScript = GetComponent<AmmoHandler>();
        helmetAnim = helmetUI.GetComponent<Animator>();
        healthSlider.maxValue = 100;
        healthSlider.value = health;
        meleeWeapon.SetActive(false);
        pistolWeapon.SetActive(false);
        ml1 = cam1.GetComponent<mouseLook>();
        ml2 = cam2.GetComponent<mouseLook>();
        speaker = GetComponent<AudioSource>();
        healthCalculation();
    }

    public void playParticle(int part)
    {
        particles[part].Play();
    }

    void healthCalculation()
    {
        currentBreathingClipPos = 0;
        for (int i = 20; i < health; i += 20)
        {
            currentBreathingClipPos++;
        }
        currentBreathingClip = breathingClips[currentBreathingClipPos];
        if(currentBreathingClipPos != 4 && health > 0)
        {
            speaker.loop = true;
            speaker.clip = currentBreathingClip;
            speaker.Play();
        }
        else
        {
            speaker.clip = null;
            speaker.loop = false;
        }
    }

    void Jump()
    {
        Debug.Log("jump");
    }

    void Use()
    {
        if(!inventoryOpen && !gamePaused)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Vector3.Distance(transform.position, hit.transform.position) < 5)
            {
                Debug.Log(hit.transform.gameObject.tag);
                Debug.Log(hit.transform.gameObject.name);
                if(hit.transform.CompareTag("helmet"))
                {
                    //takeDamage(5);
                    helmetAnim.SetBool("on", true);
                    Destroy(hit.transform.gameObject);
                }
                if (hit.transform.CompareTag("magazine"))
                {
                    ammoScript.addMagazine(hit.transform.gameObject);
                }
                if (hit.transform.CompareTag("itemSizeoftwo") || hit.transform.CompareTag("weapon") || hit.transform.CompareTag("healing") || hit.transform.CompareTag("grenade"))
                {
                    ammoScript.addMagazine(hit.transform.gameObject);
                }
                if (hit.transform.CompareTag("b10"))
                {
                    ammoScript.bulletPickupCollected(hit.transform.gameObject.GetComponent<bulletPickup>().bulletCount);
                    Destroy(hit.transform.gameObject);
                }
                if(hit.transform.CompareTag("elevatorButton"))
                {
                    Debug.Log("we are in");
                    hit.transform.GetComponentInParent<elevatorScript>().setTransform();
                }
                if(hit.transform.CompareTag("keyCard"))
                {
                    if(hit.transform.gameObject.GetComponent<keyCardScript>().redKeyCard)
                    {
                        haveRedKeyCard = true;
                    }
                    else if (hit.transform.gameObject.GetComponent<keyCardScript>().yellowKeyCard)
                    {
                        haveYellowKeyCard = true;
                    }
                    else if (hit.transform.gameObject.GetComponent<keyCardScript>().blueKeyCard)
                    {
                        haveBlueKeyCard = true;
                    }
                    Destroy(hit.transform.gameObject);
                    Debug.Log("red" + haveRedKeyCard + "yellow" + haveYellowKeyCard + "blue" + haveBlueKeyCard);
                }
                if(hit.transform.CompareTag("door"))
                {
                    doorScript dScript = hit.transform.gameObject.GetComponent<doorScript>();
                    if(dScript.noCardNeeded)
                    {
                        dScript.openDoor();
                        currentDoor = hit.transform.gameObject;
                    }
                    else
                    {
                        bool openDoor = false;
                        if(dScript.redKeyCardNeeded)
                        {
                            if(haveRedKeyCard)
                            {
                                openDoor = true;
                            }
                        }
                        else if (dScript.yellowKeyCardNeeded)
                        {
                            if (haveYellowKeyCard)
                            {
                                openDoor = true;
                            }
                        }
                        else if (dScript.blueKeyCardNeeded)
                        {
                            if (haveBlueKeyCard)
                            {
                                openDoor = true;
                            }
                        }
                        if(openDoor)
                        {
                            dScript.openDoor();
                            currentDoor = hit.transform.gameObject;                        }
                        else
                        {
                            Debug.Log("playSound");
                        }
                    }
                }
            }
        }
    }

    void throwGrenade()
    {
        if(!inventoryOpen && !gamePaused)
        {
            ammoScript.throwGrenade();
        }
    }
    void ShootGun()
    {
        if(!inventoryOpen && !gamePaused)
        {
            if(weaponPistol)
            {
                ammoScript.shoot();
            }
            else if(weaponMelee)
            {
                meleeAnim.SetBool("swing1", true);
            }
        }
    }

    public void ChangeWeapon(int weaponNum)
    {
        Debug.Log("in");
        if(weaponNum == 1)
        {
            currentGun = meleeWeapon;
        }
        else if(weaponNum == 2)
        {
            currentGun = pistolWeapon;
        }
    }

    void reloadGun()
    {
        if(!inventoryOpen && !gamePaused)
        {
            ammoScript.reloadGun();
        }
        else if(ammoScript.moveInvItem && !gamePaused)
        {
            ammoScript.inventoryChange(0, 1);
        }
    }

    void rotateLeft()
    {
        if (ammoScript.moveInvItem && !gamePaused)
        {
            ammoScript.inventoryChange(0, -1);
        }
    }

    void AimGun()
    {
        if(!inventoryOpen && !gamePaused)
        {
            if (gunAimed)
            {
                gunAimed = false;
                anim.SetBool("aim", false);
                ammoScript.changeFirePos(0);
            }
            else
            {
                gunAimed = true;
                anim.SetBool("aim", true);
                ammoScript.changeFirePos(1);
            }
        }
    }

    void OpenInventory()
    {
        if(inventoryOpen && !gamePaused)
        {
            inventoryOpen = false;
            if(currentGun != null)
            {
                currentGun.SetActive(true);
            }
        }
        else if(!gamePaused)
        {
            inventoryOpen = true;
            if (currentGun != null)
            {
                currentGun.SetActive(false);
            }
        }
        if (!gamePaused)
        {
            ammoScript.openInv();
        }
    }

    void inventory(int posChange)
    {
        if(inventoryOpen && !gamePaused)
        {
            if(buttonPressedTime == 0)
            {
                buttonPressedTime = Time.realtimeSinceStartup;
            }
            scrollInventory = true;
            if (ammoScript.invItemSelected)
            {
                if (posChange > 0)
                {
                    invNum = 1;
                }
                else
                {
                    invNum = -1;
                }
                ammoScript.changeBulletsInMag(posChange, true);
            }
            else
            {
                invNum = posChange;
                ammoScript.inventoryChange(posChange, 0);
            }
        }
        else if(!gamePaused)
        {
            scrollInventory = false;
            buttonPressedTime = 0;
        }
    }

    void cancelInv()
    {
        if(inventoryOpen && !gamePaused)
        {
            scrollInventory = false;
            buttonPressedTime = 0;
        }
    }

    void pickInvItem(bool deSelect)
    {
        if(inventoryOpen && !gamePaused)
        {
            if(deSelect)
            {
                ammoScript.selectInvItem(true);
                ammoScript.swapInventorySlots(true);
            }
            else
            {
                ammoScript.selectInvItem();
            }
        }
        else if (deSelect)
        {
            if(gamePaused)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                ml1.cameraLocked = false;
                ml2.cameraLocked = false;
            }
            else
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                ml1.cameraLocked = true;
                ml2.cameraLocked = true;
            }
            gamePaused = !gamePaused;
        }

    }

    void swapInvSlots()
    {
        if(inventoryOpen && !gamePaused)
        {
            ammoScript.swapInventorySlots();
        }
    }

    void dropItem()
    {
        if(inventoryOpen && !gamePaused)
        {
            if(ammoScript.invItemSelected)
            {
                ammoScript.dropInvItem();
            }
        }
    }

    void Run()
    {
        if(!inventoryOpen && !gamePaused)
        {
            currentSpeed = runSpeed;
        }
    }
    void Walk()
    {
        if(!inventoryOpen && !gamePaused)
        {
            currentSpeed = walkSpeed;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void resumeGame()
    {
        gamePaused = false;
        ml1.cameraLocked = false;
        ml2.cameraLocked = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void restartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if(currentDoor != null)
        {
            float dist = Vector3.Distance(transform.position, currentDoor.transform.position);
            if(dist > 5)
            {
                currentDoor.GetComponent<doorScript>().closeDoor();
                currentDoor = null;
            }
        }
        if (inventoryOpen && scrollInventory && !gamePaused)
        {
            invTimer += Time.deltaTime;
            float buttonHeldTime = Time.realtimeSinceStartup - buttonPressedTime;
            var div = ((buttonHeldTime / 0.5) / 20);
            var stopTimer = 0.5 - div; 
            if(stopTimer < 0.1)
            {
                stopTimer = 0.1;
            }
            if(invTimer >= stopTimer)
            {
                invTimer = 0;
                if (ammoScript.invItemSelected)
                {
                    ammoScript.changeBulletsInMag(invNum, true);
                }
                else
                {
                    ammoScript.inventoryChange(invNum, 0);
                }
            }
        }

        if (!inventoryOpen && !gamePaused)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
    }

    private void FixedUpdate()
    {
        if (!inventoryOpen && !gamePaused)
        {
            controller.Move(movement * currentSpeed * Time.deltaTime);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0;
            }
            move = Vector3.ClampMagnitude(move, 1f);
            movement = transform.right * move.x + transform.forward * move.y;
            velocity.y += gravity * Time.deltaTime * 2;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.gameObject.CompareTag("elevator"))
        {
            if(!onElevator)
            {
                onElevator = true;
                gameObject.transform.SetParent(hit.transform);
            }
        }
        else
        {
            onElevator = false;
            gameObject.transform.SetParent(null);
        }
    }

    public void takeDamage(int damagePercent, bool playSound = false)
    {
        float damage = 100 / damagePercent;
        health -= damage;
        if(playSound)
        {
            bulletSpeaker.Play();
        }
        if (health <= 0)
        {
            gamePaused = true;
            ml1.cameraLocked = true;
            ml2.cameraLocked = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            deathMenu.SetActive(true);
        }
        healthSlider.value = health;
        healthCalculation();
    }

    public void healDamage(int damagePercent)
    {
        float damage = 100 / damagePercent;
        health += damage;
        if (health >= 100)
        {
            health = 100;
        }
        healthSlider.value = health;
        healthCalculation();
    }

}
