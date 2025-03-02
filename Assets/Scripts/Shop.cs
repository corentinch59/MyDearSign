using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Shop : IInteractable
{
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject grid;
    [SerializeField] public List<PanalUpgrade> upgrades;

    private List<Button> _buttons = new List<Button>();

    
    public void SetPanneauText(string text)
    {
        Panneau.instance.textUI.text = text;
    }
    
    public void Start()
    {
        foreach (PanalUpgrade upgrade in upgrades)
        {
            var button = Instantiate(buttonPrefab, grid.transform).GetComponent<Button>();
            button.GetComponentInChildren<UpgradeDisplay>().SetUpgrade(upgrade);
            button.onClick.AddListener(() => { GameManager.instance.BuyUpgrade(upgrade); });
            _buttons.Add(button);
        }
    }

    public override void Interact(PlayerInteraction player)
    {
        canvas.gameObject.SetActive(true);
        // focus input on ui
        var pi = player.GetComponent<PlayerInput>();
        pi.SwitchCurrentActionMap("UI");

        _buttons.First().Select();
    }

    public void CloseUI()
    {
        FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("PlayerMap");
        canvas.gameObject.SetActive(false);
    }

    public override bool CanInteract()
    {
        return GameManager.instance.state == GameManager.GameState.BUYING && !canvas.gameObject.activeSelf;
    }
}