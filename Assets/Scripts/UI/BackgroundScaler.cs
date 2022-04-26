using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundScaler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private RectTransform _parent;
    private Image _image;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parent = _rectTransform.parent.GetComponent<RectTransform>();
        _image = GetComponent<Image>();

        Resize();
        CanvasHelper.OnScreenStateChanged.AddListener(Resize);
    }

    private void OnDestroy()
    {
        CanvasHelper.OnScreenStateChanged.RemoveListener(Resize);
    }

    private void Resize()
    {
        Rect parentRect = _parent.rect;
        Rect spriteRect = _image.sprite.rect;

        _rectTransform.sizeDelta = Screen.width / spriteRect.width
            > Screen.height / spriteRect.height
            ? new Vector2(parentRect.width, (parentRect.width * spriteRect.height) / spriteRect.width)      // width
            : new Vector2((parentRect.height * spriteRect.width) / spriteRect.height, parentRect.height);   // height
    }
}
