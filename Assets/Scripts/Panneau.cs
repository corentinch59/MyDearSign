using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class Panneau : MonoBehaviour
{
    [SerializeField]
    public string cityName = "FEUR";
    [SerializeField]
    private TextMeshPro textUI;

    [SerializeField] private float zoneSize = 10;
    [SerializeField] private DecalProjector decal;

    private List<Resource> _resources = new List<Resource>();
    
    public List<Resource> GetResources()
    {
        List<Resource> resources = new List<Resource>();
        
        // Get all resources around the pannel with distance < zoneSize
        // Get all resource scripts
        var r = FindObjectsOfType<Resource>();
        foreach (var resource in r)
        {
            var distance = Vector3.Distance(resource.transform.position, transform.position);
            if (distance < zoneSize)
            {
                resources.Add(resource);
            }
        }

        return resources;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        textUI.text = cityName;
        decal.size = new Vector3(zoneSize, zoneSize, 1);

        _resources = GetResources();
        
        foreach (var resource in _resources)
        {
            resource.EnableResource(this);
        }
    }

    private void OnDisable()
    {
        foreach (var resource in _resources)
        {
            resource.DisableResource(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
