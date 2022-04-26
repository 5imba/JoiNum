using UnityEngine;
using UnityEngine.UI;

public class ThemesShopLoader : MonoBehaviour
{
    [SerializeField] private GameObject themeItemPrefab;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        var themes = Props.settings.themes;

        for (int i = 0; i < themes.Length; i++)
        {
            var themeShopItem = Instantiate(themeItemPrefab);
            themeShopItem.transform.SetParent(transform, false);

            var previewScalers = themeShopItem.GetComponentsInChildren<ShopThemePreviewScaler>();
            for(int ps = 0; ps < previewScalers.Length; ps++)
            {
                previewScalers[ps].image.sprite = themes[i].gameBackground[ps];
            }

            var shopToggle = themeShopItem.GetComponentInChildren<ShopToggle>();
            shopToggle.LocalizeStringReference = themes[i].nameLocalizedId;
            shopToggle.ThemeIndex = i;
            shopToggle.Price = themes[i].price;

            //var nameLabel = themeShopItem.transform.Find("Background/TopBar/ThemeName");
            //nameLabel.GetComponent<Text>().text = themes[i].name;
        }

        float childHeight = 0f;
        for (int i = 0; i < rect.childCount; i++)
        {
            RectTransform childRect = rect.GetChild(i).GetComponent<RectTransform>();
            childHeight += childRect.sizeDelta.y;
        }

        rect.sizeDelta = new Vector2(0f, childHeight);
    }
}
