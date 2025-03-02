using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MissileLauncher : IPanalUpgrade
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float travelDuration = .5f;
    [SerializeField] private float reloadTime = 10.0f;
    [SerializeField] private float range = 2.0f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem shootEffect;

    private float _lastFireTime = 0.0f;

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
        
        // Get all mobs, find the circle of radius range that contains the most mobs
        var mobs = FindObjectsOfType<Mob>();

        if (mobs.Length == 0) return;
        
        var missile = Instantiate(missilePrefab, transform.position, transform.rotation);

        // average position of mobs
        var averagePosition = Vector3.zero;
        foreach (var mob in mobs)
        {
            averagePosition += mob.transform.position;
        }

        averagePosition /= mobs.Length;

        Vector3 upPoint = (transform.position + averagePosition) / 2.0f + Vector3.up * 2.0f;
        
        missile.transform.DOPath(new[] { transform.position, upPoint, averagePosition }, travelDuration, PathType.CatmullRom)
            .SetLookAt(0.01f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                var mobs2 = FindObjectsOfType<Mob>();
                foreach (var mob in mobs2)
                {
                    if (Vector3.Distance(mob.transform.position, averagePosition) <= range)
                    {
                        mob.Kill(missile.transform);
                    }
                }
                
                Instantiate(explosionPrefab, averagePosition, Quaternion.identity);

                Destroy(missile);
            });
    }
}