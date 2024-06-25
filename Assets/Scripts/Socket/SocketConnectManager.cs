using System;
using Newtonsoft.Json;
using NOOD;
using SocketIOClient;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

using Debug = System.Diagnostics.Debug;

public class ProofClass
{
    public string address;
    public int point;
    public int timestamp;
    public string[] proof;
}

public class SocketConnectManager : MonoBehaviorInstance<SocketConnectManager>
{
    public Action<int> onUpdateCoin;
    public Action<ProofClass> onClaim;

    public SocketIOUnity socket;
    public (Vector2 mainBrush, Vector2 otherBrush) _brushTuple;
    public float brushHeigh;
    public bool isSpawnCoin;
    public ProofClass proofStruct;

    #region Unity function
    protected override void ChildAwake()
    {
        //TODO: check the Uri if Valid.
        Debug.Print("Connecting...");
        JsSocketConnect.SocketIOInit();

        JsSocketConnect.RegisterUpdateBrushPosition(this.gameObject.name, nameof(UpdateBrushPos));
        JsSocketConnect.RegisterSpawnCoin(this.gameObject.name, nameof(SpawnCoin));
        JsSocketConnect.RegisterUpdateCoin(this.gameObject.name, nameof(UpdateCoin));
        JsSocketConnect.RegisterUpdateProof(this.gameObject.name, nameof(UpdateProof));
    }
    void Update()
    {
        JsSocketConnect.EmitUpdate();
        if(Input.GetKeyDown(KeyCode.T))
        {
            UnityEngine.Debug.Log("TryClaim");
            Claim();
            // WalletConnectManager.Instance.SyncPlayerPoint();
        }
    }
    void OnDestroy()
    {
        socket.Disconnect();
    }
    #endregion

    #region SocketEvent
    private void UpdateBrushPos(string data)
    {
        UnityEngine.Debug.Log("UpdateBrushPos: " + data);
        if(!string.IsNullOrEmpty(data))
        {
            _brushTuple = JsonConvert.DeserializeObject<SocketBrushPositionData>(data).GetTuple();
            // UnityEngine.Debug.Log("receive: " + data);
        }
    }
    private void SpawnCoin()
    {
        isSpawnCoin = true;
    }
    private void UpdateCoin(SocketIOResponse data)
    {
        onUpdateCoin.Invoke(data.GetValue<int>());
    }
    private void UpdateProof(SocketIOResponse proof)
    {
        proofStruct = JsonConvert.DeserializeObject<ProofClass[]>(proof.ToString())[0];
        UnityEngine.Debug.Log(proofStruct.proof[1]);
        onClaim?.Invoke(proofStruct);
    }
    #endregion

    public void Claim()
    {
        JsSocketConnect.EmitClaim("0x04Ce066AF4C50AEe8febCB7F856109A312abc2011877955eCd2db6b2bAd56d87");
    }

    #region Update socket
    public void SetBrushPosition(Vector3 mainBrush, Vector3 otherBrush)
    {
        brushHeigh = mainBrush.y;
        UnityEngine.Debug.Log($"Send: {mainBrush.x} {mainBrush.z}, {otherBrush.x} {otherBrush.z}" );
        JsSocketConnect.EmitUpdateBrushPosition(mainBrush.x.ToString(), mainBrush.z.ToString(), otherBrush.x.ToString(), otherBrush.z.ToString());
    }
    public void UpdatePlatformOffset(Vector3 position)
    {
        JsSocketConnect.EmitUpdatePlatformPos(position.x.ToString(), position.y.ToString());
    }
    public void UpdateLevel(int level)
    {
        JsSocketConnect.EmitUpdateLevel(level.ToString());
    }
    #endregion

    public (Vector3 mainBrush, Vector3 otherBrush) GetBrushPosition()
    {
        Vector3 mainBrush = new Vector3(_brushTuple.mainBrush.x, brushHeigh, _brushTuple.mainBrush.y);
        Vector3 otherBrush = new Vector3(_brushTuple.otherBrush.x, brushHeigh, _brushTuple.otherBrush.y);
        return (mainBrush, otherBrush); 
    }
    public void PlayerInput()
    {
        JsSocketConnect.EmitPlayerTouch();
    }
    public void CoinCollect(Vector3 position)
    {
        JsSocketConnect.EmitCoinCollect(position.x.ToString(), position.y.ToString());
    }
    public bool IsSpawnCoinThisLevel()
    {
        bool result = isSpawnCoin;
        isSpawnCoin = false;
        return result;
    }

    public void Log(string value)
    {
        UnityEngine.Debug.Log(value);
    }
}
