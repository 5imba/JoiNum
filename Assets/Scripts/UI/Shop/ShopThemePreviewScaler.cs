using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ShopThemePreviewScaler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private RectTransform _parent;
    private Image _image;
    private Vector2 parentSizeDelta;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parent = transform.parent.GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        parentSizeDelta = _parent.sizeDelta;
    }

    private void FixedUpdate()
    {
        if (parentSizeDelta != _parent.sizeDelta)
        {
            parentSizeDelta = _parent.sizeDelta;
            Resize();
        }
    }

    public void Resize()
    {
        Rect spriteRect = _image.sprite.rect;
        Rect parentRect = _parent.rect;

        _rectTransform.sizeDelta = new Vector2(parentRect.width, (parentRect.width * spriteRect.height) / spriteRect.width);
    }

    public Image image
    {
        get
        {
            return _image;
        }
    }
}
