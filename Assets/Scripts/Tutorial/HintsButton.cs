using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class HintsButton : MonoBehaviour
    {
        [SerializeField] private ScoreLabel score;
        [SerializeField] private RectTransform hintsCounter;
        [SerializeField] private Text hintsCounterLabel;

        [Header("Hints menu")]
        [SerializeField] private RectTransform hintsMenuBackground;
        [SerializeField] private RectTransform undo;
        [SerializeField] private RectTransform refresh;
        [SerializeField] private RectTransform bomb;

        [SerializeField] private GameObject superPointObj;
        [SerializeField] private GameObject hintsButton;

        private bool hintsMenuOpened = false;

        void Awake()
        {
            hintsCounterLabel.text = GameTempData.Hints.ToString();
            HintsChanged(GameTempData.Hints);
            GameTempData.OnHintsValueChanged += OnHintsChanged;
        }

        private void OnDestroy()
        {
            GameTempData.OnHintsValueChanged -= OnHintsChanged;
        }

        private bool opened = false;
        public void OpenHintsMenu()
        {
            if (!opened)
            {
                //Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
                hintsMenuOpened = false;
                opened = true;
                StartCoroutine(OpenHints());
            }
        }

        public void HardOpenHintsMenu()
        {
            StopAllCoroutines();
            hintsMenuOpened = false;
            opened = true;

            score.IsShort = true;
            score.rectTransform.anchoredPosition = new Vector2(-373.62f, 0f);
            score.rectTransform.sizeDelta = new Vector2(212.23f, 170f);
            hintsCounter.anchoredPosition = Vector2.zero;
            hintsMenuBackground.anchoredPosition = Vector2.zero;
            hintsMenuBackground.localScale = Vector3.one;

            undo.localScale = Vector3.one;
            refresh.localScale = Vector3.one;
            bomb.localScale = Vector3.one;
            hintsCounter.sizeDelta = new Vector2(115f, 115f);

            hintsButton.SetActive(false);
            superPointObj.SetActive(true);
            hintsMenuOpened = true;
        }

        public void HardCloseHintsMenu()
        {
            StopAllCoroutines();

            hintsButton.SetActive(true);
            superPointObj.SetActive(false);

            undo.localScale = Vector3.zero;
            refresh.localScale = Vector3.zero;
            bomb.localScale = Vector3.zero;
            hintsCounter.sizeDelta = new Vector2(50f, 50f);

            score.IsShort = false;
            score.rectTransform.anchoredPosition = new Vector2(-130f, 0f);
            score.rectTransform.sizeDelta = new Vector2(700f, 170f);
            hintsCounter.anchoredPosition = new Vector2(-48f, 48f);
            hintsMenuBackground.anchoredPosition = new Vector2(280f, 0f);
            hintsMenuBackground.localScale = Vector3.zero;

            hintsMenuOpened = false;
            opened = false;
        }

        public void CloseHintsMenu()
        {
            if (hintsMenuOpened)
            {
                //Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
                StartCoroutine(CloseHints());
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
                refresh.localScale = scale;
                bomb.localScale = scale;
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
                refresh.localScale = scale;
                bomb.localScale = scale;
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

            hintsMenuOpened = false;
            opened = false;
        }

        private void OnHintsChanged(object sender, ValueChangedEventArgs e)
        {
            HintsChanged(e.value);
        }

        private void HintsChanged(int balance)
        {
            hintsCounterLabel.text = Utils.IntToStringShortener(balance);

            bool zeroBalance = balance <= 0;
            if (zeroBalance && hintsMenuOpened) StartCoroutine(CloseHints());
        }

        public bool isOpened
        {
            get
            {
                return opened;
            }
        }
    }
}
