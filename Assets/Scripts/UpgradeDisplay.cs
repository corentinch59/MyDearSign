using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private TMP_Text upgradeDescription;
    [SerializeField] private TMP_Text upgradeCost;
    private int cost;

    public void SetUpgrade(PanalUpgrade upgrade)
    {
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.description;
        upgradeCost.text = upgrade.cost.ToString();
        cost = upgrade.cost;
    }
    
    private void Update()
    {
        if (GameManager.instance.money < cost)
        {
            upgradeCost.color = Color.red;
        }
        else
        {
            upgradeCost.color = Color.green;
        }
    }
}
