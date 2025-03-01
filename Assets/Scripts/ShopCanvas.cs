using UnityEngine;
using UnityEngine.EventSystems;

public class ShopCanvas : MonoBehaviour, ICancelHandler 
{
    [SerializeField] private Shop shop;
    
    public void OnCancel(BaseEventData eventData)
    {
        shop.CloseUI();
    }
}