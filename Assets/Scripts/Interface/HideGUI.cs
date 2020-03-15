using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGUI : MonoBehaviour
{
    public GameObject gui;
    private GameObject[] players;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        if (players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        else
        {
            gui.SetActive(false);
        }
    }
}
