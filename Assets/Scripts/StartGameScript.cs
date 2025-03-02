using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameScript : IInteractable
{

    [SerializeField] public Canvas helpPrompt;
    
    public override void Interact(PlayerInteraction player)
    {
        if (GameManager.instance.state == GameManager.GameState.BUYING)
        {
            GameManager.instance.ChangeState(GameManager.GameState.FIGHTING);
        }
    }

    public override bool CanInteract()
    {
        return GameManager.instance.state == GameManager.GameState.BUYING;
    }

    private void Update()
    {
        helpPrompt.gameObject.SetActive(GameManager.instance.state == GameManager.GameState.BUYING);
    }
}
