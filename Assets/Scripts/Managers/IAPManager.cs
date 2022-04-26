using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    private const string coin1000 = "com.wildraion.joinum.coin1000";
    private const string coin3000 = "com.wildraion.joinum.coin3000";
    private const string coin10000 = "com.wildraion.joinum.coin10000";
    private const string coin15000 = "com.wildraion.joinum.coin15000";
    private const string coin50000 = "com.wildraion.joinum.coin50000";
    private const string removeAds = "com.wildraion.joinum.removeads";

    public void OnPurchaseComplete(Product product)
    {
        switch (product.definition.id)
        {
            case coin1000:
                PlayerData.Coins += 1000;
                Utils.CreateWindowWithItem(Utils.GetLocalizedString("UITable", "youPurchse"),
                    "+1000", GameItem.Coins, ModalWindow.WindowType.Success);
                break;
            case coin3000:
                PlayerData.Coins += 3000;
                Utils.CreateWindowWithItem(Utils.GetLocalizedString("UITable", "youPurchse"),
                    "+3000", GameItem.Coins, ModalWindow.WindowType.Success);
                break;
            case coin10000:
                PlayerData.Coins += 10000;
                Utils.CreateWindowWithItem(Utils.GetLocalizedString("UITable", "youPurchse"),
                    "+10000", GameItem.Coins, ModalWindow.WindowType.Success);
                break;
            case coin15000:
                PlayerData.Coins += 15000;
                Utils.CreateWindowWithItem(Utils.GetLocalizedString("UITable", "youPurchse"),
                    "+15000", GameItem.Coins, ModalWindow.WindowType.Success);
                break;
            case coin50000:
                PlayerData.Ads = false;
                PlayerData.Coins += 50000;
                Utils.CreateWindowWithItem(Utils.GetLocalizedString("UITable", "youPurchse"),
                    "+50000", GameItem.Coins, ModalWindow.WindowType.Success);
                break;
            case removeAds:
                PlayerData.Ads = false;
                Utils.CreateWindow(Utils.GetLocalizedString("UITable", "adsRemoved"),
                    ModalWindow.WindowType.Success);
                break;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Utils.CreateWindow(Utils.GetLocalizedString("UITable", "purchaseFailed"), ModalWindow.WindowType.Error);
    }
}
