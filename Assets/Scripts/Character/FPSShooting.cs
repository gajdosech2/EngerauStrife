using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShooting : NetworkBehaviour
{
    private Camera mainCam;
    private AudioSource audio;
    public AudioClip hitClip;

    [SerializeField]
    private GameObject concreteImpact, bloodImpact;

    public float damageAmount = 10f;

    void Start()
    {
        mainCam = transform.Find("FPS View").Find("FPS Camera").GetComponent<Camera>();
        audio = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if (hit.transform.tag == "Enemy")
            {
                audio.clip = hitClip;
                audio.Play();
                Instantiate(concreteImpact, hit.point, Quaternion.LookRotation(hit.normal));
                CmdDamage(hit.transform.gameObject);
            }
            else
            {
                Instantiate(concreteImpact, hit.point, Quaternion.LookRotation(hit.normal));
            }   
        }
    }

    [Command]
    void CmdDamage(GameObject obj)
    {
        obj.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
    }
}
