using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    public GameObject menu;

    void SetPortAndAndress()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.networkPort = 7777;
    }

    public void HostGame()
    {
        SetPortAndAndress();
        NetworkManager.singleton.StartHost();
        menu.SetActive(false);
    }

    public void JoinGame()
    {
        SetPortAndAndress();
        NetworkManager.singleton.StartClient();
        menu.SetActive(false);
    }
}
