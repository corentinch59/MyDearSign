using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnimationEvent : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider;

    public void Attack()
    {
        // Check all mobs in the attack collider
        var mobs = FindObjectsOfType<Mob>();
        
        // find closest of collider
        float closestDistance = float.MaxValue;
        Mob closestMob = null;
        foreach (var mob in mobs)
        {
            if (_attackCollider.bounds.Intersects(mob.GetComponent<Collider>().bounds))
            {
                var distance = Vector3.Distance(mob.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMob = mob;
                }
            }
        }
        
        if (closestMob != null)
        {
            closestMob.Kill();
        }
    }
}
