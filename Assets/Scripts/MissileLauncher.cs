using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class MissileLauncher : IPanalUpgrade
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject missileDecal;
    [SerializeField] private float travelDuration = .5f;
    [SerializeField] private float reloadTime = 10.0f;
    [SerializeField] private float range = 2.0f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource shootSource;
    public UnityEvent MissileLand;
    
    [SerializeField] public float minTargetingRange = 2.0f;

    private float _lastFireTime = 0.0f;
    // private GameObject missileDecalInstance;

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
        
        var mobs = FindObjectsOfType<Mob>();

        if (mobs.Length == 0) return;
        
        // Choose closest mob that if at least at minRange of the ground
        NavMesh.SamplePosition(transform.position, out var groundHit, Mathf.Infinity, NavMesh.AllAreas);
        
        Mob target = null;
        float minDistance = float.MaxValue;
        foreach (var mob in mobs)
        {
            if (Vector3.Distance(mob.transform.position, groundHit.position) >= minTargetingRange)
            {
                if (Vector3.Distance(mob.transform.position, groundHit.position) < minDistance)
                {
                    target = mob;
                    minDistance = Vector3.Distance(mob.transform.position, transform.position);
                }
            }
        }

        if (!target) return;
        
        shootEffect.Play();
        
        Vector3 targetPosition = target ? target.transform.position : groundHit.position;
        
        Vector3 upPoint = new Vector3(targetPosition.x, targetPosition.y + 2, targetPosition.z);

        shootSource.time = 0.0f;
        shootSource.Play();
        shootSource.pitch = Random.Range(0.9f, 1.1f);
        
        var missile = Instantiate(missilePrefab, transform.position, transform.rotation);
        
        Quaternion rot = Quaternion.Euler(90, 0, 0);
        var missileDecalInstance = Instantiate(missileDecal, targetPosition, rot);
        missileDecalInstance.GetComponent<DecalProjector>().size = new Vector3(range * 2, range * 2, 4);
        
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
                MissileLand?.Invoke();
                missile.GetComponent<AudioSource>().PlayOneShot(explosionSound);
                missile.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                missile.GetComponent<MeshRenderer>().enabled = false;
                missile.GetComponentInChildren<ParticleSystem>().Stop();

                if(missileDecalInstance)
                    Destroy(missileDecalInstance);
                if(missile)
                    Destroy(missile, explosionSound.length);
            });
    }
}