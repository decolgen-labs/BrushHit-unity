using System;
using System.Collections.Generic;
using System.Linq;
using NOOD;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SocketConnectManager : MonoBehaviorInstance<SocketConnectManager>
{
    public Action<int[]> onValidRubberIndex;

    public SocketIOUnity socket;
    (Vector3, Vector3) brushPosition;
    private object[] _completeRubberIndexArray;

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

        Debug.Print("Connecting...");
        socket.Connect();

        socket.On(SocketEnum.updateRubberIndexList.ToString(), (data) =>
        {
            _completeRubberIndexArray = data.GetValue<SocketCompleteRubberArray>().indexArray;
            onValidRubberIndex?.Invoke(ConvertToIntArray(_completeRubberIndexArray));
        });

        socket.On(SocketEnum.updateBrushPosition.ToString(), (data) =>
        {
            (Vector2, Vector2) brushPos = data.GetValue<SocketBrushPosition>().GetTuple();
            brushPosition = (new Vector3(brushPos.Item1.x, brushPosition.Item1.y, brushPos.Item1.y), new Vector3(brushPos.Item2.x, brushPosition.Item2.y, brushPos.Item2.y));
        });

        socket.OnAnyInUnityThread((name, response) =>
        {
            // UnityEngine.Debug.Log(name + " " + response.ToString());
        });
    }
    void FixedUpdate()
    {
        socket.Emit(SocketEnum.update.ToString());
    }

    private int[] ConvertToIntArray(object[] objects)
    {
        int[] intArray = new int[objects.Length];
        for(int i = 0; i < objects.Length; i++)
        {
            intArray[i] = int.Parse(objects[i].ToString());
        }
        return intArray;
    }

    public void UpdateBrushPosition(Vector3 position1, Vector3 position2)
    {
        brushPosition = (position1, position2);
        socket.Emit(SocketEnum.updateBrushPosition.ToString(), new object[] { position1.x, position1.z, position2.x, position2.z });
    }
    public void UpdateBrushRadius(int radius)
    {
        socket.Emit(SocketEnum.updateBrushRadius.ToString(), radius);
    }

    public (Vector3 mainBrush, Vector3 otherBrush) GetBrushPosition()
    {
        return brushPosition;
    }

    public void AddNewRunner(int index, Vector3 position)
    {
        socket.Emit(SocketEnum.addRubber.ToString(), index, position.x, position.z);
    }

    public void CheckWin()
    {

    }

    public bool IsBetweenBrush(int index, Vector3 position)
    {
        if (_completeRubberIndexArray != null && isContainIndex(index)) return true;
        return false;
    }

    private bool isContainIndex(int index)
    {
        foreach(object obj in _completeRubberIndexArray)
        {
            if(int.Parse(obj.ToString()) == index) return true;
        }
        return false;
    }
}
