using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Turret : IPanalUpgrade
{
    [SerializeField] private GameObject head;
    [SerializeField] public float lockOnDuration;
    [SerializeField] public float waitDuration;
    [SerializeField] public Vector3 punchScale = new  Vector3(.5f,.5f,.5f);
    [SerializeField] public float punchDuration = .5f;
    [SerializeField] public int punchVibrato = 6;
    [SerializeField] public float punchElasticity = 0.05f;
    [SerializeField] public List<ParticleSystem> particles;
    
    private Mob target;
    private float timer = 0;
    
    private void Update()
    {
        if (!isEnabled) return;

        if (target == null)
        {
            if (timer >= waitDuration)
            {
                // find a random target
                var mobs = FindObjectsOfType<Mob>();
                if (mobs.Length > 0)
                {
                    target = mobs[Random.Range(0, mobs.Length)];
                    timer = 0;
                
                    // Dotween look at
                    head.transform.DOLookAt(target.transform.position, lockOnDuration);
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            if (timer >= lockOnDuration)
            {
                var direction = target.transform.position - head.transform.position;
                head.transform.DOPunchScale(punchScale, punchDuration, punchVibrato, punchElasticity);
                timer = 0;
                particles.ForEach(p => p.Play());
                target.Kill(transform);
                target = null;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        
        
    }
}