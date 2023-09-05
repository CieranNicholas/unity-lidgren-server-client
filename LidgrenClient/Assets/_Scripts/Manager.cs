using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Network Info")]
    public int port = 1337;
    public string server = "127.0.0.1";
    public string gameName = "game";

    void Awake()
    {
        Debug.Log("Starting Game Manager.");
        StaticManager.InitGameManager(port, server, gameName);
    }

    private void OnApplicationQuit()
    {
        StaticManager.Client.SendDisconnect();
    }
}
