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
    [SerializeField] public MonoBehaviour owner = null;
    [SerializeField] public Transform upgradeAnchor;
    [SerializeField] public float upgradeDistance = 1;
    
    private Vector3 defaultRotation;
    
    static public Panneau instance = null;

    public void PickUp(Transform parent, MonoBehaviour newOwner)
    {
        if (owner != null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneSize, LayerMask.GetMask("Resource"));
        foreach (Collider hitCollider in hitColliders)
        {
            hitCollider.GetComponent<IPannalInteractable>().DisableResource();
        }

        owner = newOwner;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        decal.gameObject.SetActive(false);
        
        EnableUpgrades(false);
    }

    public void Drop(Vector3 position)
    {
        owner = null;
        transform.SetParent(null);
        
        // Ray cast to find the ground
        var ray = new Ray(position + Vector3.up * 100, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 200, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
        }
        transform.rotation = Quaternion.identity;
        decal.gameObject.SetActive(true);
        PositionPanal();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneSize, LayerMask.GetMask("Resource"));
        foreach (Collider hitCollider in hitColliders)
        {
            hitCollider.GetComponent<IPannalInteractable>().EnableResource(0);
        }

        if (GameManager.instance.state == GameManager.GameState.FIGHTING)
        {
            EnableUpgrades(true);
        }
    }

    public void EnableUpgrades(bool value)
    {
        // Destory all children
        foreach (Transform child in upgradeAnchor)
        {
            child.GetComponent<IPanalUpgrade>().isEnabled = value;
        }
    }

    public void SetupUpgrades()
    {
        // Destory all children
        foreach (Transform child in upgradeAnchor)
        {
            Destroy(child.gameObject);
        }

        int i = 0;
        foreach (var upgrade in GameManager.instance.panalUpgrades)
        {
            var o = Instantiate(upgrade.upgradePrefab, Vector3.zero, Quaternion.identity, upgradeAnchor);
            o.transform.localPosition = new Vector3(0, i++ * upgradeDistance, 0);
            
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        defaultRotation = pannalAnchor.transform.rotation.eulerAngles;
    }

    // Start is called before the first frame update
    void Start()
    {
        decal.size = new Vector3(zoneSize, zoneSize, 1);
        
        PositionPanal();
    }

    void PositionPanal()
    {
        pannalAnchor.transform.rotation = Quaternion.Euler(defaultRotation);
        
        var angle1 = UnityEngine.Random.Range(-angleRange, angleRange);
        var angle2 = UnityEngine.Random.Range(-angleRange, angleRange);
        pannalAnchor.transform.Rotate(Vector3.left, angle1);
        pannalAnchor.transform.Rotate(Vector3.forward, angle2);
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!CanInteract()) return;

        if (owner == null)
        {
            PickUp(player.GetInteractableTransformAnchor(), player);
        }
        else
        {
            Drop(player.transform.position);
        }
    }

    public override bool CanInteract()
    {
        return GameManager.instance.state == GameManager.GameState.BUYING;
    }
}
