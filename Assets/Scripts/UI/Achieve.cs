using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Achieve : MonoBehaviour
{
    [SerializeField] private GameObject _popup;
    [SerializeField] private Text _label;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _showingDuration = 2f;
    [SerializeField] private Vector2 popupOpenAnchoredPos = new Vector2(-70f, 0f);

    private RectTransform _popupRectTransform;
    private bool _isShowing = false;
    private int _amount = 0;
    private float _showingTime = 0f;

    private Coroutine waitAndHide;

    void Awake()
    {
        _popupRectTransform = _popup.GetComponent<RectTransform>();
    }

    public void Show(int amount)
    {
        string labelText = $"+{_amount+amount}";
        _label.text = labelText;

        _amount += amount;
        _showingTime = _showingDuration;
        PlayerData.Coins += amount;

        if (!_isShowing)
        {
            _isShowing = true;
            StartCoroutine(ShowTransform());
            waitAndHide = StartCoroutine(WaitAndHide());
        }
    }

    public void Hide()
    {
        if (waitAndHide != null) { StopCoroutine(waitAndHide); }
        StartCoroutine(HideTransform());
    }

    private IEnumerator ShowTransform()
    {
        _popupRectTransform.anchoredPosition = new Vector2(-_popupRectTransform.sizeDelta.x, 0f);
        _popup.SetActive(true);

        Vector2 startPos = _popupRectTransform.anchoredPosition;
        Vector2 targetPos = popupOpenAnchoredPos;

        float time = 0f;
        while (time <= _duration)
        {
            time += Time.deltaTime;
            float t = time / _duration;

            _popupRectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        _popupRectTransform.anchoredPosition = targetPos;
    }

    private IEnumerator WaitAndHide()
    {
        while(_showingTime > 0f)
        {
            _showingTime -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(HideTransform());
    }

    private IEnumerator HideTransform()
    {
        Vector2 startPos = _popupRectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(-_popupRectTransform.sizeDelta.x, 0f);

        float time = 0f;
        while (time <= _duration)
        {
            time += Time.deltaTime;
            float t = time / _duration;
            _popupRectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        _popup.SetActive(false);
        _isShowing = false;
        _amount = 0;
    }
}
