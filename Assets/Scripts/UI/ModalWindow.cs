using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModalWindow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private GameObject[] icons;
    [SerializeField] private Image background;

    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite coinsSprite;
    [SerializeField] private Sprite hintsSprite;

    public enum WindowType {
        None = -1,
        Success = 0, 
        Warning = 1, 
        Error = 2 
    }

    private float time = 0f;

    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Color color = Props.BackUIPopup;
        color.a = 0.98f;
        background.color = color;
        title.color = Props.FontContrast;
        description.color = Props.FontContrast;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void Show(string title, WindowType windowType)
    {
        if (title != string.Empty)
        {
            this.title.text = title;
        }
        if (windowType != WindowType.None)
        {
            icons[(int)windowType].SetActive(true);
        }
    }

    public void ShowWithItem(string title, string description, GameItem item, WindowType windowType)
    {
        if (title != string.Empty)
        {
            this.title.text = title;
        }
        if (description != string.Empty)
        {
            this.description.gameObject.SetActive(true);
            this.description.text = description;            
        }

        if (item != GameItem.None)
        {
            icon.gameObject.SetActive(true);

            if (item == GameItem.Coins)
                icon.sprite = coinsSprite;

            if (item == GameItem.Hints)
                icon.sprite = hintsSprite;
        }
        if (windowType != WindowType.None)
        {
            icons[(int)windowType].SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(gameObject);
    }

}
