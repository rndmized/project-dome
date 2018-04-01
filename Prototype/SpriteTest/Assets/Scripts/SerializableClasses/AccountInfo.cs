/// <summary>
/// AccountInfo Class stores data response from Server.
/// </summary>
[System.Serializable]
public class AccountInfo
{
    /// <summary>
    /// Bool success stores whether the request has been successful or not.
    /// </summary>
    public bool success;
    /// <summary>
    /// Message string from server
    /// </summary>
    public string msg;
    /// <summary>
    /// Code string from server.
    /// </summary>
    public string code;

}