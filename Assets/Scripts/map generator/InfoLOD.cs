using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InfoLOD
{
    [SerializeField]
    private int _LOD;
    [SerializeField]
    private float _limitDistanceOfLOD;
    [SerializeField]
    private bool _useForCollider;

    //getters y setters
    public int LOD
    {
        get
        {
            return _LOD;
        }

        set
        {
            _LOD = value;
        }
    }

    public float LimitDistanceOfLOD
    {
        get
        {
            return _limitDistanceOfLOD;
        }

        set
        {
            _limitDistanceOfLOD = value;
        }
    }

    public bool UseForCollide
    {
        get
        {
            return _useForCollider;
        }

        set
        {
            _useForCollider = value;
        }
    }
}
