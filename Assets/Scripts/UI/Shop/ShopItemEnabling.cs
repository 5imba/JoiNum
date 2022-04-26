using UnityEngine;

public class ShopItemEnabling : MonoBehaviour
{
    private bool active = false;
    private void FixedUpdate()
    {
        bool ads = PlayerData.Ads;
        if (active != ads)
        {
            active = ads;
            gameObject.SetActive(ads);
        }
    }
}
