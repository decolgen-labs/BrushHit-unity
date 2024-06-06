using UnityEngine;

public class SocketBooleanObject
{
    public bool data { get; set; }
}

public class SocketDeltaTime
{
    public float deltaTime { get; set; }
}

public class SocketBrushPosition
{
    public Vector2 mainBrush { get; set; }
    public Vector2 otherBrush { get; set; }

    public (Vector2, Vector2) GetTuple()
    {
        return (mainBrush, otherBrush);
    }
}

public class SocketCompleteRubberArray
{
    public object[] indexArray { get; set; }
}