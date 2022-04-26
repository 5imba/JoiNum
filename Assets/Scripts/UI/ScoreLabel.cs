using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreLabel : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Text tmpro;
    private bool isShort = false;
    private int score = 0;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        tmpro = GetComponent<Text>();
        SetScore(score);
    }

    public void SetScore(int value)
    {
        string prefix = Utils.GetLocalizedString("UITable", "score") + ": ";
        tmpro.text = isShort ? value.ToString() : prefix + value.ToString();
        score = value;
    }

    public bool IsShort
    {
        set
        {
            isShort = value;
            SetScore(score);
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform != null)
            {
                return _rectTransform;
            }

            _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
}
