using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSController : NetworkBehaviour
{
    private Transform view;
    private Transform camera;
    private FPSShooting shooter;

    private Vector3 view_rotation = Vector3.zero;

    public float walkSpeed = 6.75f;
    public float runSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private float speed;
    private bool moving, grounded, crouching;

    private float inputX, inputY;

    private CharacterController controller;
    private Vector3 direction = Vector3.zero;

    public LayerMask groundLayer;
    private float rayDistance;
    private float defaultHeight;
    private float camHeight;
    private Vector3 defaultCamPos;

    private FPSAnimations anims;

    [HideInInspector]
    public float nextTimeToFire = 0.0f;
    private float fireRate = 3f;

    [SerializeField]
    private WeaponManager weaponsManager;
    private FPSWeapon weapon;

    [SerializeField]
    private WeaponManager handsWeaponsManager;
    private FPSHands handsWeapon;

    public GameObject playerHolder, weaponsHolder;
    public GameObject[] weapons;
    public FPSMouseLook[] mouseLook;

    void Start()
    {
        view = transform.Find("FPS View").transform;
        controller = GetComponent<CharacterController>();
        shooter = GetComponent<FPSShooting>();
        anims = GetComponent<FPSAnimations>();

        speed = walkSpeed;
        moving = false;
        rayDistance = controller.height * 0.5f + controller.radius;
        defaultHeight = controller.height;
        defaultCamPos = view.localPosition;
   
        nextTimeToFire = Time.time + 1.25f;
        weaponsManager.weapons[0].SetActive(true);
        weapon = weaponsManager.weapons[0].GetComponent<FPSWeapon>();
        handsWeaponsManager.weapons[0].SetActive(true);
        handsWeapon = handsWeaponsManager.weapons[0].GetComponent<FPSHands>();

        playerHolder.layer = LayerMask.NameToLayer(isLocalPlayer ? "Player" : "Default");
        foreach (Transform child in playerHolder.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(isLocalPlayer ? "Player" : "Default");
        }
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].layer = LayerMask.NameToLayer(isLocalPlayer ? "Player" : "Default");
        }

        weaponsHolder.layer = LayerMask.NameToLayer(isLocalPlayer ? "Default" : "Player");
        foreach (Transform child in weaponsHolder.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(isLocalPlayer ? "Default" : "Player");
        }

        if (!isLocalPlayer)
        {
            for (int i = 0; i < mouseLook.Length; i++)
            {
                mouseLook[i].enabled = false;
            }
        }

        transform.Find("FPS View").Find("FPS Camera").gameObject.SetActive(isLocalPlayer);
    }

    public override void OnStartLocalPlayer()
    {
        tag = "Player";
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            Movement();
            Animations();
            Shooting();
            Weapons();
        }
    }

    void Movement()
    {
        float inputX_Set = 0f;
        float inputY_Set = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            inputY_Set = (Input.GetKey(KeyCode.W)) ? 1f : -1f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputX_Set = (Input.GetKey(KeyCode.A)) ? -1f : 1f;
        }

        inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 19f);
        inputX = Mathf.Lerp(inputX, inputX_Set, Time.deltaTime * 19f);

        float inputModifyFactor = (inputY_Set != 0 && inputX_Set != 0) ? 0.75f : 1.0f;
        float antiBumpFactor = 0.75f;

        view_rotation = Vector3.Lerp(view_rotation, Vector3.zero, Time.deltaTime * 5f);
        view.localEulerAngles = view_rotation;

        if (grounded)
        {
            CrouchSprint();
            direction = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            direction = transform.TransformDirection(direction) * speed;
            Jump();
        }

        direction.y -= gravity * Time.deltaTime;
        grounded = (controller.Move(direction * Time.deltaTime) & CollisionFlags.Below) != 0;
        moving = controller.velocity.magnitude > 0.15f;
    }

    void CrouchSprint()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!crouching)
            {
                crouching = true;
            }
            else if (CanGetUp())
            {
                crouching = false;
            }

            StopCoroutine(CameraCrouch());
            StartCoroutine(CameraCrouch());
        }

        if (crouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            speed = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
        }
    }

    bool CanGetUp()
    {
        Ray groundRay = new Ray(transform.position, transform.up);
        RaycastHit groundHit;

        if (Physics.SphereCast(groundRay, controller.radius + 0.05f, out groundHit, rayDistance, groundLayer))
        {
            if (Vector3.Distance(transform.position, groundHit.point) < 2.3f)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator CameraCrouch()
    {
        controller.height = crouching ? defaultHeight / 1.5f : defaultHeight;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        camHeight = crouching ? defaultCamPos.y / 1.5f : defaultCamPos.y;
        while(Mathf.Abs(camHeight - view.localPosition.y) > 0.01f)
        {
            view.localPosition = Vector3.Lerp(view.localPosition, new Vector3(defaultCamPos.x, camHeight, defaultCamPos.z), Time.deltaTime * 11f);
            yield return null;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (crouching)
            {
                if (CanGetUp())
                {
                    crouching = false;
                    
                    StopCoroutine(CameraCrouch());
                    StartCoroutine(CameraCrouch());
                }
            }
            else
            {
                direction.y = jumpSpeed;
            }
        }
    }

    void Animations()
    {
        anims.Movement(new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude);
        anims.Jump(controller.velocity.y);
        anims.Crouch(crouching);
        anims.CrouchWalk(controller.velocity.magnitude);
    }

    void Shooting()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            shooter.Shoot();
            anims.Shoot(!crouching);
            weapon.Shoot();
            handsWeapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            anims.Reload();
            handsWeapon.Reload();
        }
    }

    void Weapons()
    {
        int active = -1 + (Input.GetKeyDown(KeyCode.Alpha1) ? 1 : 0) + 2*(Input.GetKeyDown(KeyCode.Alpha2) ? 1 : 0) + 3*(Input.GetKeyDown(KeyCode.Alpha3) ? 1 : 0);
        if (active > -1 && active < 3)
        {
            if (!handsWeaponsManager.weapons[active].activeInHierarchy)
            {
                for (int i = 0; i < handsWeaponsManager.weapons.Length; i++)
                {
                    handsWeaponsManager.weapons[i].SetActive(false);
                }
                handsWeaponsManager.weapons[active].SetActive(true);
                handsWeapon = handsWeaponsManager.weapons[active].GetComponent<FPSHands>();
            }

            if (!weaponsManager.weapons[active].activeInHierarchy)
            {
                for (int i = 0; i < weaponsManager.weapons.Length; i++)
                {
                    weaponsManager.weapons[i].SetActive(false);
                }
                weaponsManager.weapons[active].SetActive(true);
                weapon = weaponsManager.weapons[active].GetComponent<FPSWeapon>();
                anims.ChangeController((active == 0) ? true : false);
                nextTimeToFire = Time.time + 1.25f;
            }
        }
    }
}
