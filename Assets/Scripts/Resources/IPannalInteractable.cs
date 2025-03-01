using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPannalInteractable
{
    public void EnableResource(int batteryLevel);

    public void DisableResource();
}
