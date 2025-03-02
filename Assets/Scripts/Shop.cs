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
    [SerializeField] public Canvas helpPrompt;
    [SerializeField] public GameObject achatReussi;
    [SerializeField] public Button closeAchatReussi;

    private List<Button> _buttons = new List<Button>();

    public void CloseAchatReussi()
    {
        achatReussi.SetActive(false);
        _buttons.First().Select();
    }
    
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
            button.onClick.AddListener(() =>
            {
                if (GameManager.instance.BuyUpgrade(upgrade))
                {
                    achatReussi.SetActive(true);
                    closeAchatReussi.Select();
                }
            });
            _buttons.Add(button);
        }
    }

    private void Update()
    {
        helpPrompt.gameObject.SetActive(GameManager.instance.state == GameManager.GameState.BUYING);
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