using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MissileLauncher : IPanalUpgrade
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject missileDecal;
    [SerializeField] private float travelDuration = .5f;
    [SerializeField] private float reloadTime = 10.0f;
    [SerializeField] private float range = 2.0f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem shootEffect;

    private float _lastFireTime = 0.0f;
    private GameObject missileDecalInstance;

    private void Update()
    {
        if (GameManager.instance.state != GameManager.GameState.FIGHTING) return;

        if (Time.time - _lastFireTime > reloadTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        _lastFireTime = Time.time;
        
        shootEffect.Play();
        
        var mobs = FindObjectsOfType<Mob>();

        if (mobs.Length == 0) return;
        
        var missile = Instantiate(missilePrefab, transform.position, transform.rotation);

        float closestDistance = float.MaxValue;
        Vector3 targetPosition = Vector3.zero;
        foreach (var mob in mobs)
        {
            var distance = Vector3.Distance(mob.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetPosition = mob.transform.position;

               
            }
        }

        Quaternion rot = Quaternion.Euler(90, 0, 0);
        missileDecalInstance = Instantiate(missileDecal, targetPosition, rot);
        missileDecalInstance.GetComponent<DecalProjector>().size = new Vector3(range * 2, range * 2, 4);

        var upPoint = transform.position + Vector3.up * 2.0f;
        
        missile.transform.DOPath(new[] { transform.position, upPoint, targetPosition }, travelDuration, PathType.CatmullRom)
            .SetLookAt(0.01f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                var mobs2 = FindObjectsOfType<Mob>();
                foreach (var mob in mobs2)
                {
                    if (Vector3.Distance(mob.transform.position, targetPosition) <= range)
                    {
                        if(mob)
                            mob.Kill(missile.transform);
                    }
                }
                
                Instantiate(explosionPrefab, targetPosition, Quaternion.identity);

                if(missileDecalInstance)
                    Destroy(missileDecalInstance);
                if(missile)
                    Destroy(missile);
            });
    }
}