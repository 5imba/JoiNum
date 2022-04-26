using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorTag : MonoBehaviour
{
    [SerializeField] private ObjectType objectType;
    [SerializeField] private UITag dayColor;
    [SerializeField] private UITag nightColor;
    private Image image;

    enum ObjectType { Color, Sprite }

    public void SetColor()
    {
        if (image == null) image = GetComponent<Image>();

        switch (objectType)
        {
            case ObjectType.Color:
                image.color = PlayerData.Mode ? Props.GetUIColor(nightColor) : Props.GetUIColor(dayColor);                
                break;
            case ObjectType.Sprite:
                image.sprite = PlayerData.Mode ? Props.GetSprite(nightColor) : Props.GetSprite(dayColor);
                break;
        }
    }
}
