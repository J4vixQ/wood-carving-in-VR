using UnityEngine;

public class GridPoint
{
    private Vector3 _position; // The position in Chunk's local frame
    private float _value;

    public Vector3 Position
    {
        get
        {
            return _position;
        }
        set
        {
            this._position = value;
        }
    }
    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }
}