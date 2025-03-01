using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

public class Alterable<T>
{
    class Transformateur
    {
        Func<T, T> _method;
        int _weight;
        object _label;

        public Transformateur(Func<T, T> method, int weight, object label)
        {
            _method = method;
            _weight = weight;
            _label = label;
        }

        public Func<T, T> Method { get => _method; set => _method = value; }
        public int Weight { get => _weight; set => _weight = value; }
        public object Label { get => _label; set => _label = value; }
    }

    T startValue;
    List<Transformateur> _data;

    public Alterable(T initialValue)
    {
        startValue = initialValue;
        _data = new List<Transformateur>();
    }

    public object AddTransformator(Func<T, T> method, int weight)
    {
        // Guard
        Assert.IsNotNull(method);
        Assert.IsTrue(weight > 0);

        var t = new Transformateur(method, weight, new Object());

        // Transformateur insertion
        int idx = 0;
        for (idx = 0; idx < _data.Count; idx++)
        {
            if (_data[idx].Weight > weight) break;
        }
        _data.Insert(idx, t);
        return t.Label;
    }

    public void RemoveTransformator(object label)
    {
        _data.Remove(_data.First(i => i.Label == label));
    }

    public T CalculateValue()
    {
        T tmp = startValue;
        foreach (var el in _data)
        {
            tmp = el.Method.Invoke(tmp);
        }
        return tmp;
    }

}