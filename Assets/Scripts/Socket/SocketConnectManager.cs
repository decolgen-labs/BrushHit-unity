using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NOOD;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

using Debug = System.Diagnostics.Debug;

public class SocketConnectManager : MonoBehaviorInstance<SocketConnectManager>
{
    public SocketIOUnity socket;

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

        socket.OnAnyInUnityThread((name, response) =>
        {
            UnityEngine.Debug.Log(name + " " + response.ToString());
        });
    }

    public void UpdateBrushPosition(Vector3 position1, Vector3 position2)
    {
        socket.Emit(SocketEnum.updateBrushPosition.ToString(), new object[] { position1.x, position1.z, position2.x, position2.z });
    }

    public void CheckWin()
    {

    }

    public void IsBetweenBrush(int index, Vector3 position)
    {
        socket.Emit(SocketEnum.isCollided.ToString(), new object[] { index, position.x, position.z });
    }
}
