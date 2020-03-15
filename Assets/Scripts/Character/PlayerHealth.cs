using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public float health = 100f;
    private float lastHealth = 100f;
    private int deaths = 0;

    public AudioClip injuryClip;
    private AudioSource audio;

    private GameObject[] spawns;
    public GameObject gui;
    public Slider healthSlider;
    public Text deathsText;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        spawns = GameObject.FindGameObjectsWithTag("Respawn");
        gui.SetActive(isLocalPlayer);
    }

    void Update()
    {
        if (isLocalPlayer && health < lastHealth)
        {
            audio.clip = injuryClip;
            audio.Play();
            if (health < 0)
            {
                CmdRefresh();
                deaths += 1;
                deathsText.text = "" + deaths;
                transform.position = spawns[Random.Range(0, spawns.Length - 1)].transform.position;
                health = 100f;
                healthSlider.value = 100f;
                lastHealth = 100f;
            }
            else
            {
                healthSlider.value = health;
                lastHealth = health;
            }
        }
    }

    [Command]
    void CmdRefresh()
    {  
        health = 100f;
    }

    public void TakeDamage(float damage)
    {
        if (isServer)
        {
            health -= damage;
        }
    }
}
