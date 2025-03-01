using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class PanneauDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textUI;
    
    // Start is called before the first frame update
    void Start()
    {
        textUI.text = GameManager.instance.cityName;
    }
}