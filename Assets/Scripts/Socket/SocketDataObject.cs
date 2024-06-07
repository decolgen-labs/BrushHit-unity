using UnityEngine;

public class SocketBooleanObject
{
    public bool data { get; set; }
}

public class SocketBrushPositionData
{
    public Vector2 mainBrush;
    public Vector2 otherBrush;

    public (Vector2, Vector2) GetTuple()
    {
        return (mainBrush, otherBrush);
    }
}
