using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public enum ItemType { Coins, Hints, Themes }

    [SerializeField] public ItemType itemType;
    [SerializeField] public int price;
    [SerializeField] public int amount;
}
