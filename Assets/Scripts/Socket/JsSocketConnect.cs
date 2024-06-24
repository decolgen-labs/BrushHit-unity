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
    public static extern void EmitUpdateBrushPosition(float x1, float y1, float x2, float y2);
    [DllImport("__Internal")]
    public static extern void EmitUpdatePlatformPos(float x, float y);
    [DllImport("__Internal")]
    public static extern void EmitUpdateLevel(int level);
    [DllImport("__Internal")]
    public static extern void EmitPlayerTouch();
    [DllImport("__Internal")]
    public static extern void EmitCoinCollect(float positionX, float positionY);
    [DllImport("__Internal")]
    public static extern void EmitClaim(string accountAddress);
    #endregion
}
