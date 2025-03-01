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
        foreach (var mob in mobs)
        {
            if (_attackCollider.bounds.Intersects(mob.GetComponent<Collider>().bounds))
            {
                Debug.Log("Killing");
                mob.Kill();
            }
        }
    }
}
