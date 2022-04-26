using UnityEngine;
using UnityEngine.UI;

public class Prise : MonoBehaviour
{
    [SerializeField] private Text _label;
    [SerializeField] private GameItem item;

    public Text label
    { 
        get
        {
            return _label;
        }
    }

    [SerializeField] private Image _icon;
    [SerializeField] private Sprite coinsSprite;
    [SerializeField] private Sprite hintsSprite;

    public GameItem priseType
    {
        set
        {
            if (value == item)
                return;

            item = value;
            if (item == GameItem.Coins)
            {
                _icon.sprite = coinsSprite;
            }
            else if (item == GameItem.Hints)
            {
                _icon.sprite = hintsSprite;
            }
        }
    }
}
