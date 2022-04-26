using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    public void BuyItem(ShopItem item)
    {
        switch (item.itemType)
        {
            case ShopItem.ItemType.Coins:
                if (true)
                {
                    PlayerData.Coins += item.amount;
                }
                break;
            case ShopItem.ItemType.Hints:
                int balance = PlayerData.Coins - item.price;
                if (balance >= 0)
                {
                    PlayerData.Coins = balance;
                    PlayerData.Hints += item.amount;
                    string text = Utils.GetLocalizedString("UITable", "shop-bought");
                    Utils.CreateWindowWithItem(text, $"+{item.amount}", GameItem.Hints,  ModalWindow.WindowType.Success);
                }
                else
                {
                    string text = Utils.GetLocalizedString("UITable", "shop-noCoins");
                    Utils.CreateWindow(text, ModalWindow.WindowType.Warning);
                }
                break;
        }
    }

    public void CloseShopMenu()
    {
        uiManager.CloseShopMenu();
    }
}
