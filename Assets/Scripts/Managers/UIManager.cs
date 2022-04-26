using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas difficultMenu;
    [SerializeField] private Canvas gameOverMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas shopMenu;
    [SerializeField] private Canvas statsMenu;
    [SerializeField] private TabsController tabsController;

    [Header("UI")]
    [SerializeField] private GameObject resurmeBtn;

    [Header("Blur UI")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera blurCam;
    [SerializeField] private int gameLayerOrder = 0;

    private Canvas[] uiLayers;
    private GameObject[] gameObjects;

    private static List<Canvas> uiWindows = new List<Canvas>();
    private ImageColorTag[] uiImages;
    private FontColorTag[] uiFonts;


    private void Awake()
    {
        uiWindows = new List<Canvas>();
        Messenger.AddListener(GameEvent.MODE_SWITCHED, SetUIColors);
        Messenger.AddListener(GameEvent.THEME_CHANGED, ChangeTheme);
    }

    private void Start()
    {
        uiLayers = Utils.FindInActiveObjectsByTag("UICanvas").Select(go => go.GetComponent<Canvas>()).ToArray();
        gameObjects = Utils.FindInActiveObjectsByTag("TopGameObject");

        List<ImageColorTag> icts = new List<ImageColorTag>();
        List<FontColorTag> fcts = new List<FontColorTag>();
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            ImageColorTag ict = objs[i].GetComponent<ImageColorTag>();
            if (ict != null)
            {
                icts.Add(ict);
            }

            FontColorTag fct = objs[i].GetComponent<FontColorTag>(); 
            if (fct != null)
            {
                fcts.Add(fct);
            }
        }

        uiImages = icts.ToArray();
        uiFonts = fcts.ToArray();
        SetUIColors();

        if (PlayerData.HasSavedGame() && resurmeBtn != null)
        {
            resurmeBtn.SetActive(true);
            Text resurmeInfo = resurmeBtn.transform.Find("ResurmeInfo").GetComponent<Text>();

            string colors = Utils.GetLocalizedString("UITable", "colors",
                new KeyValuePair<string, UnityEngine.Localization.SmartFormat.PersistentVariables.IVariable>[]
                {
                    new KeyValuePair<string, UnityEngine.Localization.SmartFormat.PersistentVariables.IVariable>
                    ("num", new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable { Value = PlayerData.ColorDifficult })
                });
            string score = Utils.GetLocalizedString("UITable", "score");

            resurmeInfo.text = $"{PlayerData.ColorDifficult} {colors} - {score}: {PlayerData.CurrentScore}";
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MODE_SWITCHED, SetUIColors);
        Messenger.RemoveListener(GameEvent.THEME_CHANGED, ChangeTheme);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (uiWindows.Count > 0)
            {
                Canvas window = uiWindows.Last();
                if (window == gameOverMenu)
                {
                    return;
                }

                Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
                window.gameObject.SetActive(false);

                // Stop game pausing
                if (window == pauseMenu) GameTempData.IsPause = false;
                RemoveWindow(window);
                SetLayer(PrevUILayer);
            }
            else if (GameObject.FindGameObjectWithTag("GameOverMenu"))
            {
                OpenMainMenu();
            }
            else if (pauseMenu != null)
            {
                OpenPauseMenu();
            }
        }
    }

    private void FixedUpdate()
    {
        if (uiWindows.Count > 0)
        {
            AdManager.HideBannerAd();
        }
        else
        {
            AdManager.ShowBannerAd();
        }
    }

    private void SetUIColors()
    {
        for (int i = 0; i < uiImages.Length; i++)
        {
            uiImages[i].SetColor();
        }

        for (int i = 0; i < uiFonts.Length; i++)
        {
            uiFonts[i].SetColor();
        }
    }

    public void OpenDifficultMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        difficultMenu.gameObject.SetActive(true);
        SetLayer(difficultMenu.sortingOrder);
        AddWindow(difficultMenu);
    }
    public void CloseDifficultMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        difficultMenu.gameObject.SetActive(false);
        RemoveWindow(difficultMenu);
        SetLayer(PrevUILayer);
    }

    public void OpenSettingsMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        settingsMenu.gameObject.SetActive(true);
        SetLayer(settingsMenu.sortingOrder);
        AddWindow(settingsMenu);
    }
    public void CloseSettingsMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        settingsMenu.gameObject.SetActive(false);
        RemoveWindow(settingsMenu);
        SetLayer(PrevUILayer);
    }

    public void OpenShopMenu(int tabIndex)
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        shopMenu.gameObject.SetActive(true);
        tabsController.SwitchTab(tabIndex, false);
        AddWindow(shopMenu);
    }
    public void CloseShopMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        shopMenu.gameObject.SetActive(false);
        RemoveWindow(shopMenu);
    }

    public void StartNewGame(int difficult)
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);

        if (PlayerData.HasSavedGame())
        {
            int colorDifficult = PlayerData.ColorDifficult;
            if (PlayerData.GetBestScore(colorDifficult) < PlayerData.CurrentScore)
                PlayerData.SetBestScore(colorDifficult, PlayerData.CurrentScore);
            if (PlayerData.GetHighestLevel(colorDifficult) < PlayerData.ReachedTier)
                PlayerData.SetHighestLevel(colorDifficult, PlayerData.ReachedTier);
            if (PlayerData.GetMostMoves(colorDifficult) < PlayerData.MovesCount)
                PlayerData.SetMostMoves(colorDifficult, PlayerData.MovesCount);
            if (PlayerData.TotalHighestLevel < PlayerData.ReachedTier)
                PlayerData.TotalHighestLevel = PlayerData.ReachedTier;

            PlayerData.TotalGamesFinished += 1;
            PlayerData.SetGamesFinished(colorDifficult, PlayerData.GetGamesFinished(colorDifficult) + 1);
        }

        GameTempData.ColorDifficult = difficult;
        GameTempData.IsNewGame = true;
        SceneManager.LoadScene( PlayerPrefs.GetInt("firstPlay", 0) == 0 ? 2 : 1);
    }

    public void StartPrevGame()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        GameTempData.IsNewGame = false;
        SceneManager.LoadScene(1);
    }

    public void OpenPauseMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        GameTempData.IsPause = true;
        pauseMenu.gameObject.SetActive(true);
        SetLayer(pauseMenu.sortingOrder);
        AddWindow(pauseMenu);
    }

    public void ClosePauseMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        GameTempData.IsPause = false;
        pauseMenu.gameObject.SetActive(false);
        RemoveWindow(pauseMenu);
        SetLayer(PrevUILayer);
    }
    public void OpenGameOverMenu()
    {
        gameOverMenu.gameObject.SetActive(true);
        SetLayer(gameOverMenu.sortingOrder);
        AddWindow(gameOverMenu);
    }

    public void OpenMainMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        SceneManager.LoadScene(0);
    }

    public void OpenStatsMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        statsMenu.gameObject.SetActive(true);
        statsMenu.GetComponent<StatsMenuManager>().Load();
        AddWindow(statsMenu);
    }
    public void CloseStatsMenu()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        statsMenu.gameObject.SetActive(false);
        RemoveWindow(statsMenu);
    }

    public void OnSoundValue(Toggle soundToggle)
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        PlayerData.Sound = soundToggle.isOn;
    }

    public static int WindowsCount
    {
        get
        {
            return uiWindows.Count;
        }
    }

    private void ChangeTheme()
    {
        SetUIColors();
    }

    private void AddWindow(Canvas window)
    {
        uiWindows.Add(window);
    }

    private void RemoveWindow(Canvas window)
    {
        uiWindows.Remove(window);
    }


    public void SetLayer(int layer)
    {
        if (layer >= uiLayers.Length) layer = uiLayers.Length - 1;

        for (int i = 0; i < uiLayers.Length; i++)
        {
            if (uiLayers[i].sortingOrder == layer)
            {
                uiLayers[i].gameObject.SetActive(true);
                SetBlur(uiLayers[i], true);
                SetLayerRecursively(uiLayers[i].gameObject, 8);
                uiLayers[i].worldCamera = blurCam;
            }
            else
            {
                SetBlur(uiLayers[i], false);
                SetLayerRecursively(uiLayers[i].gameObject, 0);
                uiLayers[i].worldCamera = mainCam;
            }
        }

        if (gameObjects != null)
        {
            foreach (var go in gameObjects)
                SetLayerRecursively(go, gameLayerOrder == layer ? 6 : 0);
        }
    }

    private void SetBlur(Canvas canvas, bool val)
    {
        var buis = canvas.GetComponentsInChildren<BlurUI.BlurUI>();
        var bs = canvas.GetComponentsInChildren<BlurUI.Blurring>();

        foreach (var bui in buis)
        {
            bui.enabled = val;
            if (val)
            {
                bui.AddBlurringComponent();
                bui.ApplyCameraProperties();
            }
        }

        foreach (var b in bs)
        {
            if (b.initialized)
            {
                b.UseBlurMaterial = val;
                b.enabled = val;
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private int PrevUILayer
    {
        get
        {
            return uiWindows.Count > 0 ? uiWindows.Last().sortingOrder : 0;
        }
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
        
    public void RemoveAds(bool isOn)
    {
        PlayerData.Ads = isOn;
    }

    public void AddCoins(Text text)
    {
        int res = 0;
        int.TryParse(text.text, out res);
        PlayerData.Coins += res;
    }
}
