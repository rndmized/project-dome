using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class PlayerProfile {

    private static string loginServerAddress = "http://localhost:3000/";
    private static string gameServerAddress = "";

    public static string uID { get; set; }
    public static string cID { get; set; }

    public static string GetLoginServerAddress()
    {
        return loginServerAddress;
    }

    public static string GetGameServerAddress()
    {
        return gameServerAddress;
    }


}
