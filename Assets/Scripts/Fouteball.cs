using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fouteball : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ParticleSystem goalEffect;

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
            if (GameManager.instance)
            {
                GameManager.instance.money++;
            }
        }
    }
}
