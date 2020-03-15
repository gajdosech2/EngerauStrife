using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHands : MonoBehaviour
{
    public AudioClip shootClip, reloadClip;
    public AudioSource audio;
    private GameObject muzzleFlash;

    private Animator anim;

    private string SHOOT = "Shoot";
    private string RELOAD = "Reload";
    
    void Awake()
    {
        muzzleFlash = transform.Find("MuzzleFlash").gameObject;
        muzzleFlash.SetActive(false);

        anim = GetComponent<Animator>();
    }

    public void Shoot()
    {
        audio.clip = shootClip;
        audio.Play();
        StartCoroutine(Flash());
        anim.SetTrigger(SHOOT);
    }

    IEnumerator Flash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    public void Reload()
    {
        StartCoroutine(ReloadSound());
        anim.SetTrigger(RELOAD);
    }

    IEnumerator ReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        audio.clip = reloadClip;
        audio.Play();
    }
}
