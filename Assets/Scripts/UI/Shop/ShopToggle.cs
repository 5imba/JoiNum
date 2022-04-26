using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ShopToggle : MonoBehaviour
{
    [SerializeField] private int themeIndex = 0;
    [SerializeField] private int price;
    [SerializeField] private string _name;
    [SerializeField] private bool isOn = false;
    [SerializeField] private bool ownTheme = false;

    [SerializeField] private GameObject toggle;
    [SerializeField] private GameObject priceGameObject;
    [SerializeField] private Image checkmark;
    [SerializeField] private Text priceLabel;
    [SerializeField] private LocalizeStringEvent localizeStringEvent;

    [Header("Color swap")]
    [SerializeField] private Image targetColorGrapic;
    [SerializeField] private Color swapOnColor;
    [SerializeField] private Color swapOffColor;

    [Header("Text swap")]
    [SerializeField] private Text targetLabel;
    [SerializeField] private string onLocalizedKey;
    [SerializeField] private string offLocalizedKey;

    [Header("debug")]
    [SerializeField] bool reset = false;

    private void Start()
    {
        UpdateValue();
        Messenger.AddListener(GameEvent.THEME_CHANGED, UpdateValue);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.THEME_CHANGED, UpdateValue);
    }

    public void OnClick()
    {
        if (reset)
        {
            PlayerData.SetOwnTheme(themeIndex, false);
            ownTheme = false;
            priceGameObject.SetActive(!ownTheme);
            toggle.SetActive(ownTheme);
            return;
        }

        if (ownTheme)
        {
            bool newIsOn = !isOn;
            if (newIsOn)
            {
                Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
                PlayerData.Theme = themeIndex;
                SetValues(newIsOn);
            }
        }
        else
        {
            int balance = PlayerData.Coins - price;

            if (balance >= 0)
            {
                PlayerData.Coins = balance;
                PlayerData.SetOwnTheme(themeIndex, true);
                ownTheme = true;
                priceGameObject.SetActive(!ownTheme);
                toggle.SetActive(ownTheme);
                string text = Utils.GetLocalizedString("UITable", offLocalizedKey);
                targetLabel.text = text;


                string bought = Utils.GetLocalizedString("UITable", "shop-bought");
                string theme = Utils.GetLocalizedString("UITable", "shop-theme");
                Utils.CreateWindow($"{bought} {theme}\n{_name}", ModalWindow.WindowType.Success);
            }
            else
            {
                string text = Utils.GetLocalizedString("UITable", "shop-noCoins");
                Utils.CreateWindow(text, ModalWindow.WindowType.Warning);
            }
        }
    }

    public void UpdateValue()
    {
        ownTheme = PlayerData.GetOwnTheme(themeIndex);
        priceGameObject.SetActive(!ownTheme);
        toggle.SetActive(ownTheme);
        if (ownTheme) SetValues(PlayerData.Theme == themeIndex);
    }

    public int ThemeIndex
    {
        set
        {
            themeIndex = value;
        }
    }

    public int Price
    {
        set
        {
            price = value;
            priceLabel.text = value.ToString();
            if (value == 0) PlayerData.SetOwnTheme(themeIndex, true);
        }
    }

    public string LocalizeStringReference
    {
        set
        {
            localizeStringEvent.StringReference = Utils.GetLocalizedStringObject("UITable", value);
        }
    }

    private void SetValues(bool _isOn)
    {
        isOn = _isOn;
        if (_isOn)
        {
            checkmark.enabled = true;
            targetColorGrapic.color = swapOnColor;
            string text = Utils.GetLocalizedString("UITable", onLocalizedKey);
            targetLabel.text = text;
        }
        else
        {
            checkmark.enabled = false;
            targetColorGrapic.color = swapOffColor;
            string text = Utils.GetLocalizedString("UITable", offLocalizedKey);
            targetLabel.text = text;
        }
    }
}
