using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target
{
    private Vector3 _position;
    private Collider _collider;
    private float _distance;
    private float _time;
    private TargetType _targetType;

    public Vector3 GetPosition { get { return _position; } }
    public Collider GetCollider { get { return _collider; } }
    public float Distance { get { return _distance; } set { _distance = value; } }
    public float GetTime { get { return _time; } }
    public TargetType GetTargetType { get { return _targetType; } }

    public void Set(Vector3 p, Collider c, float d, float t, TargetType tt)
    {
        _position = p;
        _collider = c;
        _distance = d;
        _time = t;
        _targetType = tt;
    }

    public void Clear()
    {
        _position = Vector3.zero;
        _collider = null;
        _distance = float.MaxValue;
        _time = 0f;
        _targetType = TargetType.None;
    }

    public enum TargetType
    {
        None,
        Sound,
        Visual
    }
}
