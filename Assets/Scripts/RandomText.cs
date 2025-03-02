using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(TMP_Text))]
    public class RandomText : MonoBehaviour
    {
        public string[] texts;

        
        private void Start()
        {
            GetComponent<TMP_Text>().text = texts[UnityEngine.Random.Range(0, texts.Length)];
        }
    }
}