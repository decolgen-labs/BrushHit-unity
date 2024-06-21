using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;
using NOOD;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
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

    public SocketIOUnity socket;
    public (Vector2 mainBrush, Vector2 otherBrush) _brushTuple;
    public float brushHeigh;
    public bool isSpawnCoin;
    public ProofClass proofStruct;

    #region Unity function
    protected override void ChildAwake()
    {
        //TODO: check the Uri if Valid.
        var uri = new Uri("http://localhost:11100");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            Debug.Print("socket.OnConnected");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Print("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Print("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Print("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Print($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        ////

        Debug.Print("Connecting...");
        socket.Connect();

        socket.On(SocketEnum.updateBrushPosition.ToString(), (data) =>
        {
            _brushTuple = data.GetValue<SocketBrushPositionData>().GetTuple();
        });
        socket.On(SocketEnum.spawnCoin.ToString(), (data) =>
        {
            isSpawnCoin = true;
        });
        socket.On(SocketEnum.updateCoin.ToString(), (coin) =>
        {
            onUpdateCoin?.Invoke(coin.GetValue<int>());
        });
        socket.On(SocketEnum.updateProof.ToString(), (proof) =>
        {
            proofStruct = JsonConvert.DeserializeObject<ProofClass[]>(proof.ToString())[0];
            UnityEngine.Debug.Log(proofStruct.proof[1]);
            WalletConnectManager.Instance.Claim();
        });
    }
    void Update()
    {
        socket.Emit(SocketEnum.update.ToString());    
        if(Input.GetKeyDown(KeyCode.T))
        {
            Claim();
        }
    }
    #endregion

    public void Claim()
    {
        socket.Emit("claim", "0x04Ce066AF4C50AEe8febCB7F856109A312abc2011877955eCd2db6b2bAd56d87");
    }

    #region Update socket
    public void UpdateBrushPosition(Vector3 mainBrush, Vector3 otherBrush)
    {
        brushHeigh = mainBrush.y;
        socket.Emit(SocketEnum.updateBrushPosition.ToString(), new object[] { mainBrush.x, mainBrush.z, otherBrush.x, otherBrush.z });
    }
    public void UpdatePlatformOffset(Vector3 position)
    {
        socket.Emit(SocketEnum.updatePlatformPosition.ToString(), new object[] { position.x, position.z });
    }
    public void UpdateLevel(int level)
    {
        socket.Emit(SocketEnum.updateLevel.ToString(), level);
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
        socket.Emit(SocketEnum.playerTouch.ToString());
    }
    public void CoinCollect()
    {
        socket.Emit(SocketEnum.coinCollect.ToString());
    }
    public bool IsSpawnCoinThisLevel()
    {
        bool result = isSpawnCoin;
        isSpawnCoin = false;
        return result;
    }
}
