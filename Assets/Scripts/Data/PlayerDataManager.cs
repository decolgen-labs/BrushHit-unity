using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

public class PlayerDataManager : MonoBehaviorInstance<PlayerDataManager>
{
    private int _playerSahPoint;
    private int _playerIngamePoint;

    public bool IsConnected()
    {
#if UNITY_EDITOR
        return true;
#else
        return JSInteropManager.IsConnected();
#endif
    }

    #region Set
    public void SetPlayerAddress(string playerAddress)
    {
        PlayerPrefs.SetString("PlayerAddress", playerAddress);
    }
    public void SetPlayerSahPoint(int point)
    {
        _playerSahPoint = point;
    }
    public void SetPlayerIngamePoint(int point)
    {
        Debug.Log("SetPlayerIngamePoint: " + point);
        _playerIngamePoint = point;
    }
    #endregion

    #region Get
    public string GetPlayerAddress()
    {
#if UNITY_EDITOR
        return "0x010110334";
#else
        return PlayerPrefs.GetString("PlayerAddress");
#endif
    }
    public int GetPlayerIngamePoint()
    {
        return _playerIngamePoint;
    }
    public int GetPlayerSahPoint()
    {
        return _playerSahPoint;
    }
    #endregion

}

public class PlayerPoint
{
    public int point;
}
