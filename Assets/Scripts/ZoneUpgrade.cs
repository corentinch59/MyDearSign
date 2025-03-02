using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ZoneUpgrade : IPanalUpgrade
{
    [Header("Upgrade Config")]
    [SerializeField] private float _areaIncrease;

    private object _modifier;

    private void OnEnable()
    {
        _modifier = Panneau.instance.internalZoneSize.AddTransformator((x) => x + _areaIncrease , 1);
        Panneau.instance.UpdateDecal();
    }

    private void OnDisable()
    {
        Panneau.instance.internalZoneSize.RemoveTransformator(_modifier);
        Panneau.instance.UpdateDecal();
    }
}
