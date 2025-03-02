using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fouteball : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ParticleSystem goalEffect;
    [SerializeField] private UnityEvent onGoal;

    private GameObject b;

    private void Start()
    {
        SpawnBall();
    }

    public void SpawnBall()
    {
        if (b) Destroy(b);

        b = Instantiate(ball, spawnPoint.position, Quaternion.identity);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == b)
        {
            goalEffect.Play();
            Destroy(b);
            b = null;
            onGoal?.Invoke();
            if (GameManager.instance)
            {
                GameManager.instance.money = GameManager.instance.money + 1;
            }
        }
    }
}