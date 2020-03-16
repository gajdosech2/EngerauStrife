using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    public GameObject menu;

    public void Host()
    {
        NetworkManager.singleton.networkAddress = "90.64.193.109";
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.StartHost();
        menu.SetActive(false);
    }

    public void Join()
    {
        NetworkManager.singleton.networkAddress = "90.64.193.109";
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.StartClient();
        menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Host();
        }
    }
}
