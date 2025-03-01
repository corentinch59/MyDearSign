using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class Panneau : IInteractable
{
    [SerializeField] private float zoneSize = 10;
    [SerializeField] private DecalProjector decal;

    [SerializeField] private float angleRange = 10;

    [SerializeField] private GameObject pannalAnchor;
    
    // Start is called before the first frame update
    void Start()
    {
        decal.size = new Vector3(zoneSize, zoneSize, 1);
        
        var angle1 = UnityEngine.Random.Range(-angleRange, angleRange);
        var angle2 = UnityEngine.Random.Range(-angleRange, angleRange);
        pannalAnchor.transform.Rotate(Vector3.left, angle1);
        pannalAnchor.transform.Rotate(Vector3.forward, angle2);
    }

    public override void Interact()
    {
        Debug.Log("Interacting with Panneau");
    }

    public override bool CanInteract()
    {
        return GameManager.instance.state == GameManager.GameState.BUYING;
    }
}
