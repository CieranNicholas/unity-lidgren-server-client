using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LidgrenClient;

public class StaticManager 
{
    public static string LocalPlayerID {  get; set; }
    public static Client Client { get; set; }
    public static Dictionary<string, GameObject> Players { get; set; }
    public static void InitGameManager(int port, string server, string gameName)
    {
        Debug.Log("Starting static game manager");

        LocalPlayerID = "";
        Client = new Client(port, server, gameName);
        Players = new Dictionary<string, GameObject>();
    }
}
