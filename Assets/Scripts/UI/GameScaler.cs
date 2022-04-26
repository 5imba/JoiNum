using System.Collections;
using UnityEngine;
using PointData;

public class GameScaler : MonoBehaviour
{
    [SerializeField] private RectTransform gameField;
    [SerializeField] private RectTransform controlPointsHolder;
    [SerializeField] private Transform pointsHolder;


    [SerializeField] private ControlPointHolder[] controlPointHolders;
    [SerializeField] private int gridSize = 9;
    [SerializeField] private float pointScale = 0.5f;
    [SerializeField] private bool transformControlPoints = true;

    private RectTransform rectTransform;
    private AdManager.BannerInfo bannerInfo = AdManager.BannerInfo.zero;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Messenger<AdManager.BannerInfo>.AddListener(GameEvent.ON_BANNER_SWITCH, OnBannerSwitch);
        CanvasHelper.OnScreenStateChanged.AddListener(Resize);
    }

    private void Start()
    {
        Resize();
    }

    private void OnDestroy()
    {
        Messenger<AdManager.BannerInfo>.RemoveListener(GameEvent.ON_BANNER_SWITCH, OnBannerSwitch);
        CanvasHelper.OnScreenStateChanged.RemoveListener(Resize);
    }

    private void Resize()
    {
        StopAllCoroutines();
        StartCoroutine(Resizing());
    }

    private void OnBannerSwitch(AdManager.BannerInfo bannerInfo)
    {
        if (!this.bannerInfo.Equals(bannerInfo))
        {
            rectTransform.offsetMin = bannerInfo.isShowing
                ? new Vector2(0f, bannerInfo.height)
                : Vector2.zero;

            this.bannerInfo = bannerInfo;
            Resize();
        }
    }

    private float prevScale = 0f;
    private IEnumerator Resizing()
    {
        Rect rect = gameField.GetWorldRect();
        float pointsStartWidth = pointScale * gridSize;
        float gameFieldWidth = rect.size.x;
        float scale = gameFieldWidth / pointsStartWidth;

        if (scale == prevScale)
        {
            // Rescale and transform Points Holder
            Vector3 pos = gameField.position;
            pos.z = 0f;
            pointsHolder.transform.position = pos;
            pointsHolder.localScale = new Vector3(scale, scale, scale);

            // Recalculate banner safeArea
            rectTransform.offsetMin = bannerInfo.isShowing
                ? new Vector2(0f, bannerInfo.height)
                : Vector2.zero;

            // Transform control point container to center
            if (transformControlPoints)
            {
                float gameFieldBottom = Mathf.Abs(gameField.rect.height * 0.5f - gameField.anchoredPosition.y);
                float gameScreenBottom = Mathf.Abs(rectTransform.rect.height * 0.5f);
                float middleDelta = (gameScreenBottom - gameFieldBottom) * 0.5f;
                float newY = gameFieldBottom + middleDelta;
                controlPointsHolder.anchoredPosition = new Vector2(0f, -newY);                
            }

            for (int i = 0; i < controlPointHolders.Length; i++)
            {
                controlPointHolders[i].Init();
            }
        }
        else
        {
            yield return null;
            prevScale = scale;
            Resize();
        }
    }
}
