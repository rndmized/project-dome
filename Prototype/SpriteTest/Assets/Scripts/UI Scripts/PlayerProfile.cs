using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Session Storage of Player Data.
/// </summary>
public static  class PlayerProfile {

    /// <summary>
    /// Login Server Address.
    /// </summary>
    private static string loginServerAddress = "http://localhost:3000/";
    
    /// <summary>
    /// Game Server Address.
    /// </summary>
    private static string gameServerAddress = "";

    /// <summary>
    /// User ID.
    /// </summary>
    public static string uID { get; set; }

    /// <summary>
    /// Character ID.
    /// </summary>
    public static string cID { get; set; }

    /// <summary>
    /// User's Token.
    /// </summary>
    public static string token { get; set; }

    /// <summary>
    /// Return Login Server Address.
    /// </summary>
    /// <returns>string of Login Server Address. </returns>
    public static string GetLoginServerAddress()
    {
        return loginServerAddress;
    }

    /// <summary>
    /// Return Game Server Address.
    /// </summary>
    /// <returns>string of Game Server Address.</returns>
    public static string GetGameServerAddress()
    {
        return gameServerAddress;
    }


}
