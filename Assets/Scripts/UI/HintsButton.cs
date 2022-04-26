using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HintsButton : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Controller gameController;
    [SerializeField] private RewardedAdData rewardedAdData;

    [SerializeField] private ScoreLabel score;
    [SerializeField] private RectTransform hintsCounter;
    [SerializeField] private Text hintsCounterLabel;

    [Header("Hints menu")]
    [SerializeField] private RectTransform hintsMenuBackground;
    [SerializeField] private RectTransform undo;
    [SerializeField] private RectTransform superPoint;
    [SerializeField] private RectTransform refresh;

    [SerializeField] private GameObject superPointObj;
    [SerializeField] private GameObject hintsButton;
    [SerializeField] private GameObject adsLabel;

    private Coroutine checkForClick;
    private bool hintsMenuOpened = false;
    private bool adsReady = false;

    void Awake()
    { 
        hintsCounterLabel.text = PlayerData.Hints.ToString();
        HintsChanged(PlayerData.Hints);

        Messenger<bool>.AddListener(GameEvent.ON_REWARDED_AD, OnRewardAdReady);
        PlayerData.OnHintsValueChanged += OnHintsChanged;
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GameEvent.ON_REWARDED_AD, OnRewardAdReady);
        PlayerData.OnHintsValueChanged -= OnHintsChanged;
    }

    public void OpenHintsMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        if (PlayerData.Hints <= 0)
        {
            if (adsReady)
            {
                adsReady = false;
                adsLabel.SetActive(false);
                AdManager.ShowRewardedAd(rewardedAdData);
            }
            else
            {
                if (uiManager != null) uiManager.OpenShopMenu(1);
            }
        }
        else
        {
            hintsMenuOpened = false;
            StartCoroutine(OpenHints());
            checkForClick = StartCoroutine(CheckForClick());
        }
    }

    public void CloseHintsMenu()
    {
        if (hintsMenuOpened)
        {
            Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
            StopCoroutine(checkForClick);
            StartCoroutine(CloseHints());
        }
    }

    private IEnumerator CheckForClick()
    {
        yield return new WaitForEndOfFrame();

        Vector2 localPos;
        Vector2 screenPos;

        while (true)
        {

#if UNITY_ANDROID || UNITY_IOS

            if (hintsMenuOpened && (gameController.AllowPointSetting ? !gameController.IsCurrentControlPointBomb : true)
                && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                screenPos = touch.position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(hintsMenuBackground, screenPos, Camera.main, out localPos);

                if (!hintsMenuBackground.rect.Contains(localPos))
                {
                    StartCoroutine(CloseHints());
                    break;
                }
            }
#endif
#if UNITY_EDITOR

            if (hintsMenuOpened && (gameController.AllowPointSetting ? !gameController.IsCurrentControlPointBomb : true)
                && Input.GetMouseButton(0))
            {
                screenPos = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(hintsMenuBackground, screenPos, Camera.main, out localPos);

                if (!hintsMenuBackground.rect.Contains(localPos))
                {
                    StartCoroutine(CloseHints());
                    break;
                }
            }
#endif

            yield return null;
        }
    }

    private IEnumerator OpenHints()
    {
        float time = 0f;
        float duration = 0.2f;

        Vector2 backStartPos = new Vector2(280f, 0f);
        Vector2 backTargetPos = Vector2.zero;
        Vector2 counterStartPos = new Vector2(-48f, 48f);
        Vector2 counterTargetPos = Vector2.zero;

        Vector2 scoreStartPos = score.rectTransform.anchoredPosition;
        Vector2 scoreTargetPos = new Vector2(-373.62f, 0f);
        Vector3 scoreStartSize = score.rectTransform.sizeDelta;
        Vector3 scoreTargetSize = new Vector2(212.23f, 170f);

        adsLabel.transform.parent.gameObject.SetActive(false);
        score.IsShort = true;
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            score.rectTransform.anchoredPosition = Vector2.Lerp(scoreStartPos, scoreTargetPos, t);
            score.rectTransform.sizeDelta = Vector2.Lerp(scoreStartSize, scoreTargetSize, t);
            hintsCounter.anchoredPosition = Vector2.Lerp(counterStartPos, counterTargetPos, t);
            hintsMenuBackground.anchoredPosition = Vector2.Lerp(backStartPos, backTargetPos, t);
            hintsMenuBackground.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return null;
        }

        time = 0f;
        duration = 0.1f;
        Vector3 counterStartSize = new Vector2(50f, 50f);
        Vector3 counterTargetSize = new Vector2(115f, 115f);

        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 scale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            undo.localScale = scale;
            superPoint.localScale = scale;
            refresh.localScale = scale;
            hintsCounter.sizeDelta = Vector2.Lerp(counterStartSize, counterTargetSize, t);

            yield return null;
        }

        hintsButton.SetActive(false);
        superPointObj.SetActive(true);
        hintsMenuOpened = true;
    }

    private IEnumerator CloseHints()
    {
        float time = 0f;
        float duration = 0.1f;

        Vector3 counterStartSize = new Vector2(115f, 115f);
        Vector3 counterTargetSize = new Vector2(50f, 50f);

        hintsButton.SetActive(true);
        superPointObj.SetActive(false);

        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            undo.localScale = scale;
            superPoint.localScale = scale;
            refresh.localScale = scale;
            hintsCounter.sizeDelta = Vector2.Lerp(counterStartSize, counterTargetSize, t);

            yield return null;
        }

        time = 0f;
        duration = 0.2f;
        Vector2 backStartPos = new Vector2(-80f, 0f);
        Vector2 backTargetPos = new Vector2(280f, 0f);
        Vector2 counterStartPos = Vector2.zero;
        Vector2 counterTargetPos = new Vector2(-48f, 48f);

        Vector2 scoreStartPos = score.rectTransform.anchoredPosition;
        Vector2 scoreTargetPos = new Vector2(-130f, 0f);
        Vector3 scoreStartSize = score.rectTransform.sizeDelta;
        Vector3 scoreTargetSize = new Vector2(700f, 170f);

        score.IsShort = false;
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            score.rectTransform.anchoredPosition = Vector2.Lerp(scoreStartPos, scoreTargetPos, t);
            score.rectTransform.sizeDelta = Vector2.Lerp(scoreStartSize, scoreTargetSize, t);
            hintsCounter.anchoredPosition = Vector2.Lerp(counterStartPos, counterTargetPos, t);
            hintsMenuBackground.anchoredPosition = Vector2.Lerp(backStartPos, backTargetPos, t);
            hintsMenuBackground.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        adsLabel.transform.parent.gameObject.SetActive(true);
        hintsMenuOpened = false;
    }

    private void OnHintsChanged(object sender, ValueChangedEventArgs e)
    {
        HintsChanged(e.value);
    }

    private void HintsChanged(int balance)
    {
        hintsCounterLabel.text = Utils.IntToStringShortener(balance);

        bool zeroBalance = balance <= 0;
        if (adsReady) adsLabel.SetActive(zeroBalance);
        if (zeroBalance && hintsMenuOpened) StartCoroutine(CloseHints());
    }

    private void OnRewardAdReady(bool isReady)
    {
        adsReady = isReady;
        if (PlayerData.Hints <= 0)
        {
            adsLabel.SetActive(isReady);
        }
    }
}
