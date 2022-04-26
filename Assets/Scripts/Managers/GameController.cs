using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PointData;

public class GameController : Controller
{
    [SerializeField] private AudioController audioCtrl;
    [SerializeField] private Transform pointPrefab;
    [SerializeField] private Transform controlPointPrefab;
    [SerializeField] private Transform combinePointPrefab;
    [SerializeField] private Transform pointsHolder;
    [SerializeField] private ControlPointHolder[] controlPointHolders;

    [Header("UI")]
    [SerializeField] private Button undoButton;
    [SerializeField] private ScoreLabel score;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Text gameOverScore;
    [SerializeField] private Text gameOverBestScore;
    [SerializeField] private Transform prisesHolder;
    [SerializeField] private Prise prisePrefab;
    [SerializeField] private Achieve achieve;

    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 9;
    [SerializeField] private float pointScale = 0.5f;

    [Header("Point Settings")]
    //[SerializeField] private bool random = false;
    //[SerializeField] private int[] numColors;
    //[SerializeField] private int[] numTiers;
    [SerializeField] private int achiveLimitTier = 8;
    [SerializeField] private int minControlPointTier = 1;
    [SerializeField] private int maxStartControlPointTier = 3;

    [Header("Animations")]
    [SerializeField] private float combineDuration = 0.3f;
    [SerializeField] private float multipleCombinesDelay = 0.25f;
    [SerializeField] private float scoreChangingDuration = 0.35f;

    [Header("Configs")]
    [SerializeField] private int historyCapacity = 10;
    [SerializeField] private float adsChance = 0.3f;

    private Point[,] pointGrid;
    private Index i = Index.negative;
    private Index bombPointIndex = Index.negative;
    private bool hasSuperPoint = false;

    private bool allowInput = true;
    private bool allowPointSetting = false;
    private ControlPointHolder currentCPHolder;

    private int scoreNum = 0;
    private int[] maxTier;
    private int movesCount = 0;
    private int reachedTier = 0;

    private int colorDifficult;

    private List<History> history = new List<History>();
    private List<Point> samePoints = new List<Point>();
    private List<Point> combinePoints = new List<Point>();
    private List<List<Point>> combinePointsGrouped = new List<List<Point>>();
    private Point currentPoint;

    void Start()
    {
        GameTempData.IsPause = false;
        bool hasSavedGame = PlayerData.HasSavedGame() && !GameTempData.IsNewGame;

        // Set values
        colorDifficult = GameTempData.IsNewGame ? GameTempData.ColorDifficult : PlayerData.ColorDifficult;
        maxTier = hasSavedGame ? PlayerData.GetMaxTier() : 
            Enumerable.Repeat(maxStartControlPointTier, Props.PointColorsLength).ToArray();

        // Create points
        CreateGrid(hasSavedGame);
        InitControlPoints(hasSavedGame);

        if (hasSavedGame) 
        {
            reachedTier = PlayerData.ReachedTier;
            movesCount = PlayerData.MovesCount;
            StartCoroutine(StepChangingScore(PlayerData.CurrentScore, scoreChangingDuration));
        }
        else
        {
            reachedTier = maxStartControlPointTier;
            PlayerData.BestScore = Mathf.Max(PlayerData.CurrentScore, PlayerData.BestScore);
            PlayerData.CurrentScore = 0;
        }
    }

    private bool adsNotShowed = true;
    private void Update()
    {
        // Show ads
        if (adsNotShowed)
        {
            adsNotShowed = false;
            if (PlayerData.Ads)
            {
                float adsPercent = Random.Range(0f, 1f);
                if (adsPercent < adsChance)
                {
                    AdManager.ShowInterstitialAd();
                }
            }
        }

        if (!GameTempData.IsPause)
        {
            // Input Management
            Vector2 resultPos;
            if (Pointer.position(out resultPos))
            {
                Vector3 pointerPos = Camera.main.ScreenToWorldPoint(resultPos);
                pointerPos.z = 0f;

                // Check point entering
                if (allowPointSetting)
                {
                    bool hoverAnyPoint = false;
                    for (int y = 0; y < pointGrid.GetLength(0); y++)
                    {
                        for (int x = 0; x < pointGrid.GetLength(1); x++)
                        {
                            Point p = GetPoint(y, x);
                            if (p.bounds.Contains(pointerPos))
                            {
                                p.OnPointerEnter();
                                hoverAnyPoint = true;
                            }
                            else
                            {
                                p.OnPointerExit();
                            }
                        }
                    }

                    // Reset Current Index
                    if (!hoverAnyPoint) i = Index.negative;
                }

                // Check pointer entering to control point 
                else if (allowInput)
                {
                    for (int i = 0; i < controlPointHolders.Length; i++)
                    {
                        ControlPointHolder cph = controlPointHolders[i];
                        if (cph.bounds.Contains(pointerPos))
                        {
                            if (i == 0 ? PlayerData.Hints > 0 : true)
                            {
                                allowPointSetting = true;
                                allowInput = false;
                                currentCPHolder = cph;
                                currentCPHolder.FollowPointer = true;
                            }
                        }
                    }
                }

                // control point following pointer
                if (currentCPHolder != null)
                {
                    currentCPHolder.PointPos = new Vector3(pointerPos.x, pointerPos.y, -1f);
                }
            }
            else
            {
                // On pointer up
                if (currentCPHolder != null)
                {
                    if (!i.IsNegative()) CurrentPoint.OnPointerExit();

                    allowPointSetting = false;
                    currentCPHolder.OnPointerUp();
                    currentCPHolder = null;
                }
            }
        }
    }

    private void CreateGrid(bool hasSavedGame)
    {
        pointGrid = new Point[gridSize, gridSize];
        float borderPosition = (gridSize - 1) * pointScale * 0.5f;

        // Start form top left corner
        float yPos = borderPosition;
        for (int y = 0; y < pointGrid.GetLength(0); y++)
        {
            float xPos = -borderPosition;
            for (int x = 0; x < pointGrid.GetLength(1); x++)
            {
                Point p;
                if (hasSavedGame)
                {
                    p = CreatePoint(pointPrefab, pointsHolder, new Vector2(xPos, yPos),
                        PlayerData.GetPoint(new Index(y, x)));
                }
                else
                {
                    p = CreatePoint(pointPrefab, pointsHolder, new Vector2(xPos, yPos),
                    new PointObj(0, ColorType.Empty, new Index(y, x), true, false, false, false));
                }
    
                pointGrid[y, x] = p;
                xPos += pointScale;
            }
            yPos -= pointScale;
        }
    }

    private void InitControlPoints(bool hasSavedGame)
    {
        for (int i = 0; i < controlPointHolders.Length; i++)
        {
            if (hasSavedGame && i != 0)
            {
                CreateControlPoint(i, PlayerData.GetPoint(new Index(0, -i)));
            }
            else
            {
                CreateControlPoint(i);
            }
        }
    }

    private void CreateControlPoint(int i)
    {
        int colorIndex = Random.Range(0, colorDifficult);

        Point p = CreatePoint(controlPointPrefab, controlPointHolders[i].transform, Vector3.zero,
            new PointObj(/*numTiers[i], (ColorType)numColors[i],//*/Random.Range(minControlPointTier, maxTier[colorIndex] + 1), (ColorType)colorIndex,
            new Index(0, -i), false, true, false, i == 0));

        controlPointHolders[i].point = p;
    }

    private void CreateControlPoint(int i, PointObj point)
    {
        Point p = CreatePoint(controlPointPrefab, controlPointHolders[i].transform, Vector3.zero, point);
        controlPointHolders[i].point = p;
    }

    private void CreateCombinePoint(Point pointFrom, float offsetZ, Point[] pointTo, bool setTarget)
    {
        Transform t = Instantiate(combinePointPrefab);

        Point tP = t.GetComponent<Point>();
        CombinePoint tCP = t.GetComponent<CombinePoint>();

        t.position = pointFrom.transform.position + new Vector3(0f, 0f, offsetZ);

        tP.color = pointFrom.color;
        tP.Tier = pointFrom.Tier;
        tP.IsCombine = true;
        tP.IsBomb = pointFrom.IsBomb;
        if (setTarget)
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.IsBomb, pointFrom.Tier);
        }
        else
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, false, -1);
        }
    }

    private void CreateCombinePoint(Vector3 posFrom, HistoryPoint pointFrom, Point[] pointTo, bool setTarget)
    {
        Transform t = Instantiate(combinePointPrefab);

        Point tP = t.GetComponent<Point>();
        CombinePoint tCP = t.GetComponent<CombinePoint>();

        t.position = posFrom;
        tP.color = pointFrom.color;
        tP.Tier = pointFrom.tier;
        tP.IsCombine = true;
        tP.IsBomb = pointFrom.isSuper;
        if (setTarget)
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.isSuper, pointFrom.tier);
        }
        else
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.isSuper, -1);
        }
    }

    private Point CreatePoint(Transform prefab, Transform parent, Vector3 position, PointObj pointObj)
    {
        Transform pointTransform = Instantiate(prefab);
        pointTransform.SetParent(parent, false);

        Point point = pointTransform.GetComponent<Point>();
        point.transform.localPosition = position;
        point.transform.localScale = new Vector3(pointScale, pointScale, pointScale);
        point.color = pointObj.color;
        point.Tier = pointObj.tier;
        point.index = pointObj.index;
        point.IsEmpty = pointObj.isEmpty;

        if (pointObj.isControl) point.IsControl = pointObj.isControl;
        if (pointObj.isCombine) point.IsCombine = pointObj.isCombine;
        if (pointObj.isBomb) point.IsBomb = pointObj.isBomb;

        return point;
    }

    public override void SetPoint(int index)
    {
        if (CurrentPoint.IsEmpty)
        {
            audioCtrl.PlaySound(Sound.Post);
            bool bombPoint = index == 0;

            Point p = controlPointHolders[index].point;
            CurrentPoint.Tier = p.Tier;
            CurrentPoint.color = p.color;
            CurrentPoint.IsEmpty = false;
            CurrentPoint.IsBomb = bombPoint;

            // History capacity limitation
            if (history.Count >= historyCapacity) history.RemoveAt(0);
            history.Add(new History(CurrentPoint, new HistoryPoint(p), index));

            CreateControlPoint(index);
            CheckForSame(CurrentIndex, true);

            undoButton.interactable = true;
            if (bombPoint) PlayerData.Hints -= 1;
        }
    }

    public void CheckForSame(Index i, bool writeToHistory)
    {
        samePoints.Clear();
        combinePoints.Clear();
        combinePointsGrouped.Clear();
        combinePointsGrouped.Add(null);

        List<HistoryPoint> historyPoints = new List<HistoryPoint>();
        currentPoint = GetPoint(i);
        bool hasSame = false;
        int scoreToAdd = 0;

        // Check for same points
        samePoints.Add(currentPoint); // skip current point
        combinePoints.Add(currentPoint);

        FindSamePoints(currentPoint);

        if (!hasSuperPoint)
        {
            for (int k = 1; k < samePoints.Count; k++)
            {
                // Check for Bomb Point
                if (samePoints[k].IsBomb)
                {
                    hasSuperPoint = true;
                    bombPointIndex = samePoints[k].index;
                }
            }
        }

        // If bomb point, reinit with bomb at center
        if (!currentPoint.IsBomb && hasSuperPoint)
        {
            // Add to history
            for (int k = 1; k < samePoints.Count; k++)
            {
                historyPoints.Add(new HistoryPoint(samePoints[k].index, samePoints[k].Tier,
                    samePoints[k].color, samePoints[k].transform.position, samePoints[k].IsBomb));
            }
            history.Last().AddCombinePoints(historyPoints);
            history.Last().AddTargetPoints(combinePointsGrouped.Skip(1).ToList());


            StartCoroutine(CheckRecursion(bombPointIndex, false));
            return;
        }
        // Create combine points
        else if (samePoints.Count > 2)
        {
            for (int k = 1; k < samePoints.Count; k++)
            {
                // Create combine points
                Point[] targetPoints = combinePointsGrouped[k].ToArray();
                CreateCombinePoint(samePoints[k], -0.1f * k, targetPoints, false);

                // Add to history
                historyPoints.Add(new HistoryPoint(samePoints[k].index, samePoints[k].Tier,
                    samePoints[k].color, samePoints[k].transform.position, samePoints[k].IsBomb));

                samePoints[k].IsEmpty = true;
            }

            hasSame = true;
        }

        // Apply the result
        if (hasSame)
        {
            if (writeToHistory)
            {
                history.Last().AddCombinePoints(historyPoints);
                history.Last().AddTargetPoints(combinePointsGrouped.Skip(1).ToList());
            }

            if (currentPoint.IsBomb)
            {
                audioCtrl.PlaySound(Sound.Super);
                currentPoint.SetEmptyWithAnimation();
                scoreToAdd += samePoints.Count;
            }
            else
            {
                audioCtrl.PlaySound(Sound.Combine);
                currentPoint.Tier += 1;

                if (currentPoint.Tier > Mathf.Max(achiveLimitTier, reachedTier))
                {
                    achieve.Show(colorDifficult);
                    reachedTier = currentPoint.Tier;
                    scoreToAdd += currentPoint.Tier * samePoints.Count * 2;
                }
                else
                {
                    scoreToAdd += currentPoint.Tier * samePoints.Count;
                }

                maxTier[(int)currentPoint.color] = Mathf.Max(currentPoint.Tier - 2, maxTier[(int)currentPoint.color]);
            }
            StartCoroutine(CheckRecursion(i, true));

            // Apply score
            history.Last().ScoreToAdd += scoreToAdd;
            StartCoroutine(StepChangingScore(scoreToAdd, scoreChangingDuration));
        }
        else
        {
            // Check for Game Over
            bool isEmptyPointsLeft = false;

            for (int y = 0; y < pointGrid.GetLength(0); y++)
            {
                for (int x = 0; x < pointGrid.GetLength(1); x++)
                {
                    if (pointGrid[y, x].IsEmpty)
                    {
                        isEmptyPointsLeft = true;
                        break;
                    }
                }
            }

            // Game Over
            if (!isEmptyPointsLeft)
            {
                if (PlayerData.GetBestScore(colorDifficult) < scoreNum)
                    PlayerData.SetBestScore(colorDifficult, scoreNum);
                if (PlayerData.GetHighestLevel(colorDifficult) < reachedTier)
                    PlayerData.SetHighestLevel(colorDifficult, reachedTier);
                if (PlayerData.GetMostMoves(colorDifficult) < movesCount)
                    PlayerData.SetMostMoves(colorDifficult, movesCount);
                if (PlayerData.TotalHighestLevel < reachedTier)
                    PlayerData.TotalHighestLevel = reachedTier;

                PlayerData.TotalGamesFinished += 1;
                PlayerData.SetGamesFinished(colorDifficult, PlayerData.GetGamesFinished(colorDifficult) + 1);

                PlayerPrefs.SetInt("savedGame", 0);
                uiManager.OpenGameOverMenu();

                int bestScore = PlayerData.BestScore;
                if (scoreNum > bestScore)
                {
                    gameOverScore.resizeTextForBestFit = false;
                    gameOverScore.text = $"{Utils.GetLocalizedString("UITable", "newBestScore")}\n{scoreNum}";
                    gameOverBestScore.gameObject.SetActive(false);

                    var prise = Instantiate(prisePrefab);
                    prise.transform.SetParent(prisesHolder, false);
                    prise.priseType = GameItem.Coins;
                    prise.label.text = "+5";
                    PlayerData.Hints += 5;
                    PlayerData.BestScore = scoreNum;
                }
                else
                {
                    string scoreText = Utils.GetLocalizedString("UITable", "score");
                    string bestScoreText = Utils.GetLocalizedString("UITable", "stats-bestScore");

                    gameOverScore.text = $"{scoreText}: {scoreNum}";
                    gameOverBestScore.text = $"{bestScoreText} {PlayerData.BestScore}";
                }

                GameTempData.IsPause = true;
            }
            else
            {
                movesCount += 1;
                SaveGameState();
            }

            // Reset values
            this.i = Index.negative;
            bombPointIndex = Index.negative;
            hasSuperPoint = false;
            allowInput = true;
        }
    }

    private void FindSamePoints(Point point)
    {
        Index i = point.index;
        Index[] compareIndexes = new Index[]
        {
            new Index(i.y, i.x - 1),
            new Index(i.y - 1, i.x),
            new Index(i.y, i.x + 1),
            new Index(i.y + 1, i.x),
        };

        for (int k = 0; k < compareIndexes.Length; k++)
        {
            if (IsValidIndex(compareIndexes[k]))
            {
                Point comparePoint = GetPoint(compareIndexes[k]);

                bool isSame = hasSuperPoint ? ComparePointsSuper(comparePoint, point) : ComparePoints(comparePoint, point);
                if (!samePoints.Contains(comparePoint) && isSame) 
                {
                    samePoints.Add(comparePoint);
                    combinePoints.Add(comparePoint);
                    combinePointsGrouped.Add(new List<Point>(combinePoints));

                    FindSamePoints(comparePoint);
                }
            }
        }

        combinePoints = new List<Point>();
        combinePoints.Add(currentPoint);
    }

    private void FindSuperPoint(Point point)
    {
        Index i = point.index;
        Index[] compareIndexes = new Index[]
        {
            new Index(i.y, i.x - 1),
            new Index(i.y - 1, i.x),
            new Index(i.y, i.x + 1),
            new Index(i.y + 1, i.x),
        };

        for (int k = 0; k < compareIndexes.Length; k++)
        {
            if (IsValidIndex(compareIndexes[k]))
            {
                Point comparePoint = GetPoint(compareIndexes[k]);
                if (!samePoints.Contains(comparePoint))
                {
                    if (comparePoint.IsBomb)
                    {
                        bombPointIndex = compareIndexes[k];
                        break;
                    }
                    else if (ComparePointsSuper(comparePoint, point))
                    {
                        samePoints.Add(comparePoint);
                        FindSuperPoint(comparePoint);
                    }
                }
            }
        }
    }

    private bool IsValidIndex(Index i)
    {
        if (i.y >= 0 && i.y < gridSize && i.x >= 0 && i.x < gridSize)
        {
            return true;
        }
        return false;
    }

    private Point GetPoint(Index i)
    {
        return pointGrid[i.y, i.x];
    }

    private Point GetPoint(int y, int x)
    {
        return pointGrid[y, x];
    }

    private bool ComparePoints(Point p1, Point p2)
    {
        if (!p1.IsEmpty && !p2.IsEmpty)
        {
            if (p1.color == p2.color && p1.Tier == p2.Tier || p1.IsBomb || p2.IsBomb)
            {
                return true;
            }
        }

        return false;
    }

    private bool ComparePointsSuper(Point p1, Point p2)
    {
        if (!p1.IsEmpty && !p2.IsEmpty)
        {
            if (p1.color == p2.color && p1.Tier == p2.Tier || p1.IsBomb || p2.IsBomb)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator CheckRecursion(Index index, bool writeToHistory)
    {
        yield return new WaitForSeconds(multipleCombinesDelay);

        CheckForSame(index, writeToHistory);
    }

    public override void SaveGameState()
    {
        PlayerPrefs.SetInt("savedGame", 1);
        PlayerData.ColorDifficult = colorDifficult;
        PlayerData.ReachedTier = reachedTier;
        PlayerData.MovesCount = movesCount;
        PlayerData.SetMaxTier(maxTier);

        for (int y = 0; y < pointGrid.GetLength(0); y++)
        {
            for (int x = 0; x < pointGrid.GetLength(1); x++)
            {
                PlayerData.SetPoint(new PointObj(pointGrid[y, x]));
            }
        }

        for (int i = 1; i < controlPointHolders.Length; i++)
        {
            PlayerData.SetPoint(new PointObj(controlPointHolders[i].point));
        }
    }

    #region Public accesors

    public override Index CurrentIndex
    {
        get
        {
            return i;
        }
        set
        {
            if (allowPointSetting)
            {
                i = value;
            }
        }
    }

    public Point CurrentPoint
    {
        get
        {
            return GetPoint(i);
        }
    }

    public override Vector3 CurrentPointPos
    {
        get
        {
            return GetPoint(i).transform.position;
        }
    }

    public override bool AllowPointSetting
    {
        get
        {
            return allowPointSetting;
        }
        set
        {
            allowPointSetting = value;
        }
    }

    public override bool IsCurrentControlPointBomb
    {
        get
        {
            return currentCPHolder.Index == 0;
        }
    }

    public bool PointerDown { get; set; }

    public override bool IsAnableToSet
    {
        get
        {
            bool b = !CurrentIndex.IsNegative() && CurrentPoint.IsEmpty;
            if (!b) allowInput = true;
            return b;
        }
    }

    #endregion

    /// <summary>
    /// Game History
    /// </summary>
    class History
    {
        Point point;
        HistoryPoint controlPoint;
        int controlPointHolderIndex, scoreToAdd = 0;
        List<List<HistoryPoint>> combinePoints = new List<List<HistoryPoint>>();
        List<List<List<Point>>> targetPoints = new List<List<List<Point>>>();

        public History(Point point, HistoryPoint controlPoint, int controlPointHolderIndex)
        {
            this.point = point;
            this.controlPoint = controlPoint;
            this.controlPointHolderIndex = controlPointHolderIndex;
        }

        public void AddCombinePoints(List<HistoryPoint> historyPoints)
        {
            combinePoints.Add(historyPoints);
        }

        public void AddTargetPoints(List<List<Point>> targetPoints)
        {
            this.targetPoints.Add(targetPoints);
        }

        public Point Point
        {
            get
            {
                return point;
            }
        }

        public HistoryPoint ControlPoint
        {
            get
            {
                return controlPoint;
            }
        }

        public int GetLength()
        {
            return combinePoints.Count;
        }

        public List<HistoryPoint> GetCombinePoints(int i)
        {
            return combinePoints[i];
        }

        public List<List<Point>> GetTargetPoints(int i)
        {
            return targetPoints[i];
        }

        public int ControlPointHolderIndex
        {
            get
            {
                return controlPointHolderIndex;
            }
        }

        public int ScoreToAdd
        {
            get
            {
                return scoreToAdd;
            }
            set
            {
                scoreToAdd = value;
            }
        }
    }

    public struct HistoryPoint
    {
        public int tier;
        public Index i;
        public ColorType color;
        public Vector3 pos;
        public bool isSuper;

        public HistoryPoint(Index i, int tier, ColorType color, Vector3 pos, bool isSuper)
        {
            this.i = i;
            this.tier = tier;
            this.color = color;
            this.pos = pos;
            this.isSuper = isSuper;
        }

        public HistoryPoint(Point point)
        {
            i = point.index;
            tier = point.Tier;
            color = point.color;
            pos = point.transform.position;
            isSuper = point.IsBomb;
        }
    }

    /// <summary>
    /// UI Management
    /// </summary>

    public void ReloadControlPoints()
    {
        if (PlayerData.Hints > 0)
        {
            audioCtrl.PlaySound(Sound.Click);
            for (int i = 1; i < controlPointHolders.Length; i++)
            {
                CreateControlPoint(i);
            }
            PlayerData.Hints -= 1;
            SaveGameState();
        }
    }

    public void Undo()
    {
        if (PlayerData.Hints > 0 && history.Count > 0)
        {
            audioCtrl.PlaySound(Sound.Click);
            History lastHistory = history.Last();

            for (int i = lastHistory.GetLength() - 1; i >= 0; i--)
            {
                List<HistoryPoint> combinePoints = lastHistory.GetCombinePoints(i);
                List<List<Point>> targetPoints = lastHistory.GetTargetPoints(i);
                for (int cp = 0; cp < combinePoints.Count; cp++)
                {
                    targetPoints[cp].Reverse();
                    CreateCombinePoint(lastHistory.Point.transform.position, combinePoints[cp], targetPoints[cp].ToArray(), true);
                }
            }

            StartCoroutine(StepChangingScore(-lastHistory.ScoreToAdd, scoreChangingDuration));
            CreateControlPoint(lastHistory.ControlPointHolderIndex, new PointObj(lastHistory.ControlPoint.tier,
                lastHistory.ControlPoint.color, new Index(0, -lastHistory.ControlPointHolderIndex), false, true, false,
                lastHistory.ControlPointHolderIndex == 0));

            lastHistory.Point.IsEmpty = true;
            history.Remove(lastHistory);
            PlayerData.Hints -= 1;
        }
    }

    public IEnumerator StepChangingScore(int scoreToAdd, float duration)
    {
        float time = 0f;
        int newScore = scoreNum + scoreToAdd;
        PlayerData.CurrentScore = newScore;

        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            scoreNum = (int)Mathf.Lerp(scoreNum, newScore, t);
            score.SetScore(scoreNum);

            yield return null;
        }
        scoreNum = newScore;
        score.SetScore(scoreNum);
    }
}


