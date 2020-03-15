using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSAnimations : NetworkBehaviour
{
    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH = "Crouch";
    private string CROUCH_WALK = "CrouchWalk";

    private string STAND_SHOOT = "StandShoot";
    private string CROUCH_SHOOT = "CrouchShoot";
    private string RELOAD = "Reload";

    public RuntimeAnimatorController pistolController, rifleController;

    private NetworkAnimator netAnim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
    }

    public void Movement(float magnitude)
    {
        anim.SetFloat(MOVE, magnitude);
    }

    public void Jump(float velocity)
    {
        anim.SetFloat(VELOCITY_Y, velocity);
    }

    public void Crouch(bool crouch)
    {
        anim.SetBool(CROUCH, crouch);
    }

    public void CrouchWalk(float magnitude)
    {
        anim.SetFloat(CROUCH_WALK, magnitude);
    }

    public void Shoot(bool standing)
    {
        if (standing)
        {
            anim.SetTrigger(STAND_SHOOT);
            netAnim.SetTrigger(STAND_SHOOT);
        }
        else
        {
            anim.SetTrigger(CROUCH_SHOOT);
            netAnim.SetTrigger(CROUCH_SHOOT);
        }
    }

    public void Reload()
    {
        anim.SetTrigger(RELOAD);
        netAnim.SetTrigger(RELOAD);
    }

    public void ChangeController(bool pistol)
    {
        if (pistol)
        {
            anim.runtimeAnimatorController = pistolController;
        }
        else
        {
            anim.runtimeAnimatorController = rifleController;
        }
    }
}
