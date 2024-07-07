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
    public Action<ProofClass> onClaim;

    [HideInInspector] public bool isSpawnCoin;
    public SocketIOUnity socket;
    public (Vector2 mainBrush, Vector2 otherBrush) _brushTuple;
    public float brushHeigh;
    public ProofClass proofStruct;

    #region Unity function
    protected override void ChildAwake()
    {
        //TODO: check the Uri if Valid.
        Debug.Print("Connecting...");
        JsSocketConnect.SocketIOInit();

        JsSocketConnect.RegisterUpdateBrushPosition(this.gameObject.name, nameof(UpdateBrushPos));
        // JsSocketConnect.RegisterSpawnCoin(this.gameObject.name, nameof(SpawnCoin));
        JsSocketConnect.RegisterUpdateCoin(this.gameObject.name, nameof(CollectCoinCallback));
        JsSocketConnect.RegisterUpdateProof(this.gameObject.name, nameof(UpdateProof));
    }

    void Update()
    {
        JsSocketConnect.EmitUpdate();
    }
    void OnDestroy()
    {
        socket.Disconnect();
    }
    #endregion

    #region SocketEvent

    private void UpdateBrushPos(string data)
    {
        if(!string.IsNullOrEmpty(data))
        {
            try{
                _brushTuple = JsonConvert.DeserializeObject<SocketBrushPositionData>(data).GetTuple();
            }catch{
                UnityEngine.Debug.Log("UpdateBrushPos: " + data);
            }
        }
    }
    private void SpawnCoin(string valueBool)
    {
        isSpawnCoin = JsonConvert.DeserializeObject<bool>(valueBool);
    }
    private void CollectCoinCallback(string data)
    {
        PlayerDataManager.Instance.SetPlayerIngamePoint(JsonConvert.DeserializeObject<int>(data));
        UIManager.Ins.UpdateScore();
    }
    private void UpdateProof(string proof)
    {
        proofStruct = JsonConvert.DeserializeObject<ProofClass>(proof.ToString());
        UnityEngine.Debug.Log(proofStruct.proof[1]);
        onClaim?.Invoke(proofStruct);
    }
    #endregion

    public void Claim()
    {
        JsSocketConnect.EmitClaim(PlayerDataManager.Instance.GetPlayerAddress());
    }
    public void EmitAfterClaim()
    {
        JsSocketConnect.EmitAfterClaim();
    }
    #region Update socket
    public void SetBrushPosition(Vector3 mainBrush, Vector3 otherBrush)
    {
        brushHeigh = mainBrush.y;
        JsSocketConnect.EmitUpdateBrushPosition(mainBrush.x.ToString(), mainBrush.z.ToString(), otherBrush.x.ToString(), otherBrush.z.ToString());
    }
    public void UpdatePlatformOffset(Vector3 position)
    {
        JsSocketConnect.EmitUpdatePlatformPos(position.x.ToString(), position.z.ToString());
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
        UnityEngine.Debug.Log("Socket emit coin collect");
        JsSocketConnect.EmitCoinCollect(position.x.ToString(), position.z.ToString());
    }
}
