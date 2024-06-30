using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JsSocketConnect 
{

    [DllImport("__Internal")]
    public static extern void SocketIOInit();

    [DllImport("__Internal")]
    public static extern void RegisterUpdateBrushPosition(string objectName, string methodName);
    [DllImport("__Internal")]
    public static extern void RegisterSpawnCoin(string objectName, string methodName);
    [DllImport("__Internal")]
    public static extern void RegisterUpdateCoin(string objectName, string methodName);
    [DllImport("__Internal")]
    public static extern void RegisterUpdateProof(string objectName, string methodName);

    #region Emit
    [DllImport("__Internal")]
    public static extern void EmitUpdate();
    [DllImport("__Internal")]
    public static extern void EmitUpdateBrushPosition(string x1, string y1, string x2, string y2);
    [DllImport("__Internal")]
    public static extern void EmitUpdatePlatformPos(string x, string y);
    [DllImport("__Internal")]
    public static extern void EmitUpdateLevel(string level);
    [DllImport("__Internal")]
    public static extern void EmitPlayerTouch();
    [DllImport("__Internal")]
    public static extern void EmitCoinCollect(string positionX, string positionY);
    [DllImport("__Internal")]
    public static extern void EmitClaim(string accountAddress);
    [DllImport("__Internal")]
    public static extern void EmitAfterClaim();
    #endregion
}
