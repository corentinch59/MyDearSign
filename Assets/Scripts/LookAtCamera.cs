using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void OnEnable()
    {
        // make object be facing camera look vector
        if (Camera.main != null)
        {
            Vector3 lookDirection = Camera.main.transform.forward;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        else
        {
            Debug.LogWarning("No main camera found. LookAtCamera script will not function properly.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // make object be facing camera look vector
        if (Camera.main != null)
        {
            Vector3 lookDirection = Camera.main.transform.forward;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        else
        {
            Debug.LogWarning("No main camera found. LookAtCamera script will not function properly.");
        }
    }
}
