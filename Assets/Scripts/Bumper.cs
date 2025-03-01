using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bumper : IPanalUpgrade
{
    [SerializeField] private float force = 1;
    [SerializeField] private float radius = 2;
    [SerializeField] private float distance = 2;
    [SerializeField] private float duration = 1;
    [SerializeField] private GameObject _bumperModel;
    [SerializeField] private float _cooldown = 5;
    [SerializeField] public Vector3 punchScale = new  Vector3(.5f,.5f,.5f);
    [SerializeField] public float punchDuration = .5f;
    [SerializeField] public int punchVibrato = 6;
    [SerializeField] public float punchElasticity = 0.05f;

    private float _timer;
    
    private void Update()
    {
        if (!isEnabled) return;
        
        _timer += Time.deltaTime;

        if (_timer >= _cooldown)
        {
            transform.DOPunchScale(punchScale, punchDuration, punchVibrato, punchElasticity);
            var mobs = FindObjectsOfType<Mob>();
            foreach (var mob in mobs)
            {
                if (Vector3.Distance(mob.transform.position, Panneau.instance.transform.position) < radius)
                {
                    Vector3 direction = mob.transform.position - Panneau.instance.transform.position;
                    mob.transform.DOJump(mob.transform.position + direction.normalized, force, 1, duration);
                }
            }
            _timer = 0;
        }
    }
}