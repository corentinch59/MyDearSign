using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Resource : MonoBehaviour
{
    public enum ResourceType
    {
        COINS,
        UPGRADE,
        BONUS
    }
    
    [SerializeField]
    private ResourceType type = ResourceType.COINS;

    [SerializeField] public float reloadDuration = 5;
    
    [SerializeField]
    public int coinValue = 1;
    
    [SerializeField]
    public GameObject upgradePrefab;
    private GameObject _upgradeInstance;
    
    [SerializeField]
    public UnityEvent bonusEnableFunction;
    [SerializeField]
    public UnityEvent bonusDisableFunction;
    
    private bool _resourceEnabled = false;

    public void EnableResource(Panneau panal)
    {
        Debug.Log("Enable resource " + type);
        if (type == ResourceType.BONUS && bonusEnableFunction != null && bonusDisableFunction != null)
        {
            bonusEnableFunction.Invoke();
        } else if (type == ResourceType.UPGRADE && upgradePrefab != null)
        {
            _upgradeInstance = Instantiate(upgradePrefab, Vector3.zero, Quaternion.identity, panal.transform);
        }
        
        _lastReloadTime = Time.time;
        _resourceEnabled = true;
    }
    
    public void DisableResource(Panneau panal)
    {
        Debug.Log("Disable resource " + type);
        if (type == ResourceType.BONUS && bonusEnableFunction != null && bonusDisableFunction != null)
        {
            bonusDisableFunction.Invoke();
        } else if (type == ResourceType.UPGRADE && _upgradeInstance != null)
        {
            Destroy(_upgradeInstance);
        }
        _resourceEnabled = false;
    }
    
    private float _lastReloadTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state == GameManager.GameState.BUYING)
        {
            return;
        }
        
        if (_resourceEnabled && _lastReloadTime + reloadDuration < Time.time)
        {
            _lastReloadTime = Time.time;
            if (type == ResourceType.COINS)
            {
                // Add coins to the player
                Debug.Log("Add " + coinValue + " coins to the player");
                // TODO
            }
        }
    }
}
