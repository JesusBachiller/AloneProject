using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadInfo<T>
{
    private Action<T> _method;
    private T _parameterOfMethod;
    
    /// <summary>
    /// contructor
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <param name="parameter"></param>
    public ThreadInfo(Action<T> method, T parameter)
    {
        _method = method;
        _parameterOfMethod = parameter;
    }

    //getters
    public Action<T> Method
    {
        get
        {
            return _method;
        }
    }

    public T ParameterOfMethod
    {
        get
        {
            return _parameterOfMethod;
        }
    }

}
