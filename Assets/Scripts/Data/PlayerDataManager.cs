using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

public class PlayerDataManager : MonoBehaviorInstance<PlayerDataManager>
{
    private int _playerPoint;

    public bool IsConnected()
    {
#if UNITY_EDITOR
        return true;
#else
        return JSInteropManager.IsConnected();
#endif
    }

    #region Set
    public void SetPlayerData(string playerAddress)
    {
        PlayerPrefs.SetString("PlayerAddress", playerAddress);
    }
    public void SetPlayerPoint(int point)
    {
        _playerPoint = point;
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
    public float GetPlayerPoint()
    {
        return _playerPoint;
    }
    #endregion

}

public class PlayerPoint
{
    public int point;
}
