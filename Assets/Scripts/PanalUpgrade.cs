using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PanalUpgrade", menuName = "PanalUpgrade")]
public class PanalUpgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int cost;
    public GameObject upgradePrefab;
}
