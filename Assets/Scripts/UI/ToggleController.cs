using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleController : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvent;
    [SerializeField] private int themeIndex = 0;

    [SerializeField] private Image targetGrapic;

    [Header("Sprite swap")]
    [SerializeField] private bool spriteSwap = false;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [Header("Color swap")]
    [SerializeField] private bool colorSwap = false;
    [SerializeField] private Color swapOnColor;
    [SerializeField] private Color swapOffColor;

    [Header("Text swap")]
    [SerializeField] private bool textSwap = false;
    [SerializeField] private Text targetLabel;
    [SerializeField] private string textOn;
    [SerializeField] private string textOff;

    [Header("Switch handle")]
    [SerializeField] private bool switchHandle = false;
    [SerializeField] private RectTransform handleRectTransform;
    [SerializeField] private Image handleIconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float switchDuration;
    [SerializeField] private float handleOffset;
    [SerializeField] private Color onIconColor;
    [SerializeField] private Color offIconColor;
    [SerializeField] private UITag onBackColor;
    [SerializeField] private UITag offBackColor;
    private Toggle toggle;
    private bool initialized = false;

    public void UpdateValue()
    {
        bool isOn = true;
        switch (gameEvent)
        {
            case GameEvents.SOUND_SWITCHED:
                isOn = PlayerData.Sound;
                break;
            case GameEvents.MODE_SWITCHED:
                isOn = PlayerData.Mode;
                break;
            case GameEvents.THEME_CHANGED:
                isOn = PlayerData.Theme == themeIndex;
                break;
        }
        
        if (toggle == null) toggle = GetComponent<Toggle>();
        toggle.isOn = isOn;
        SetValues(isOn);
    }


    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnSwitch);
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

    private void OnSwitch(bool isOn)
    {
        switch (gameEvent)
        {
            case GameEvents.SOUND_SWITCHED:
                PlayerData.Sound = isOn;
                break;
            case GameEvents.MODE_SWITCHED:
                PlayerData.Mode = isOn;
                break;
            case GameEvents.THEME_CHANGED:
                if (isOn) PlayerData.Theme = themeIndex;
                break;
        }
        if (initialized) Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        SetValues(isOn);
    }

    private IEnumerator SwitchToggle(Vector2 newHandlePos, Color newHandleColor, Color newBackgroundColor)
    {
        Vector2 startHandlePos = handleRectTransform.anchoredPosition;
        Color startHandleColor = handleIconImage.color;
        Color startBackgroundColor = backgroundImage.color;

        float time = 0.0f;
        while (time <= switchDuration)
        {
            time += Time.deltaTime;
            float t = time / switchDuration;

            handleRectTransform.anchoredPosition = Vector2.Lerp(startHandlePos, newHandlePos, t);
            handleIconImage.color = Color.Lerp(startHandleColor, newHandleColor, t);
            backgroundImage.color = Color.Lerp(startBackgroundColor, newBackgroundColor, t);

            yield return null;
        }
    }

    private void SetValues(bool isOn)
    {
        if (isOn)
        {
            if (spriteSwap) targetGrapic.sprite = spriteOn;
            if (colorSwap) targetGrapic.color = swapOnColor;
            if (textSwap) targetLabel.text = textOn;
            if (switchHandle)
            {
                StartCoroutine(SwitchToggle(new Vector2(handleOffset, 0), onIconColor,
                Props.GetUIColor(onBackColor)));
            }
        }
        else
        {
            if (spriteSwap) targetGrapic.sprite = spriteOff;
            if (colorSwap) targetGrapic.color = swapOffColor;
            if (textSwap) targetLabel.text = textOff;
            if (switchHandle)
            {
                StartCoroutine(SwitchToggle(new Vector2(-handleOffset, 0), offIconColor,
                Props.GetUIColor(offBackColor)));
            }
        }
    }

    public void Initialize()
    {
        bool isOn = true;
        switch (gameEvent)
        {
            case GameEvents.SOUND_SWITCHED:
                isOn = PlayerData.Sound;
                break;
            case GameEvents.MODE_SWITCHED:
                isOn = PlayerData.Mode;
                break;
            case GameEvents.THEME_CHANGED:
                isOn = PlayerData.Theme == themeIndex;
                break;
        }

        if (toggle == null) toggle = GetComponent<Toggle>();
        toggle.isOn = isOn;

        if (isOn)
        {
            if (spriteSwap) targetGrapic.sprite = spriteOn;
            if (colorSwap) targetGrapic.color = swapOnColor;
            if (textSwap) targetLabel.text = textOn;
            if (switchHandle)
            {
                handleRectTransform.anchoredPosition = new Vector2(handleOffset, 0);
                handleIconImage.color = onIconColor;
                backgroundImage.color = Props.GetUIColor(onBackColor);
            }
        }
        else
        {
            if (spriteSwap) targetGrapic.sprite = spriteOff;
            if (colorSwap) targetGrapic.color = swapOffColor;
            if (textSwap) targetLabel.text = textOff;
            if (switchHandle)
            {
                handleRectTransform.anchoredPosition = new Vector2(-handleOffset, 0);
                handleIconImage.color = offIconColor;
                backgroundImage.color = Props.GetUIColor(offBackColor);
            }
        }
        initialized = true;
    }
}
