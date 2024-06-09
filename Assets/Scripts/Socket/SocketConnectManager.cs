using System;
using System.Collections.Generic;
using NOOD;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

using Debug = System.Diagnostics.Debug;

public class SocketConnectManager : MonoBehaviorInstance<SocketConnectManager>
{
    public SocketIOUnity socket;
    public (Vector2 mainBrush, Vector2 otherBrush) _brushTuple;
    public float _brushHeigh;

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

        socket.OnAnyInUnityThread((name, response) =>
        {
            // UnityEngine.Debug.Log(name + " " + response.ToString());
        });
    }
    void Update()
    {
        socket.Emit(SocketEnum.update.ToString());    
    }

    public void UpdateBrushPosition(Vector3 mainBrush, Vector3 otherBrush)
    {
        _brushHeigh = mainBrush.y;
        socket.Emit(SocketEnum.updateBrushPosition.ToString(), new object[] { mainBrush.x, mainBrush.z, otherBrush.x, otherBrush.z });
    }
    public (Vector3 mainBrush, Vector3 otherBrush) GetBrushPosition()
    {
        Vector3 mainBrush = new Vector3(_brushTuple.mainBrush.x, _brushHeigh, _brushTuple.mainBrush.y);
        Vector3 otherBrush = new Vector3(_brushTuple.otherBrush.x, _brushHeigh, _brushTuple.otherBrush.y);
        return (mainBrush, otherBrush); 
    }
    public void PlayerInput()
    {
        socket.Emit(SocketEnum.playerTouch.ToString());
    }

    public void CheckWin()
    {

    }

    public void CheckTrue(Vector3 position)
    {
        socket.Emit(SocketEnum.isTrue.ToString(), new object[] { position.x, position.z });
    }

    public bool IsBetweenBrush(int index, Vector3 position, Action onTrue)
    {
        bool result = false;
        socket.Emit(SocketEnum.isCollided.ToString(), new object[] { index, position.x, position.z });
        socket.OnUnityThread(SocketEnum.isCollided.ToString(), (data) =>
        {
            result = data.GetValue<SocketBooleanObject>(0).data;
            if(result)
                onTrue?.Invoke();
        });
        UnityEngine.Debug.Log("Final result: " + result);
        return result;
    }
}
