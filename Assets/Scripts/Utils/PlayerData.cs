using UnityEngine;
using PointData;

public static class PlayerData
{
    public static PointObj GetPoint(Index index)
    {
        int y = index.y;
        int x = index.x;

        PointObj point = new PointObj(
            PlayerPrefs.GetInt($"point{y}{x}tier"),
            (ColorType)PlayerPrefs.GetInt($"point{y}{x}color"),
            index,
            PlayerPrefs.GetInt($"point{y}{x}isEmpty") == 1,
            PlayerPrefs.GetInt($"point{y}{x}isControl") == 1,
            PlayerPrefs.GetInt($"point{y}{x}isCombine") == 1,
            PlayerPrefs.GetInt($"point{y}{x}isBomb") == 1
        );

        return point;
    }

    public static void SetPoint(PointObj point)
    {
        int y = point.index.y;
        int x = point.index.x;
        PlayerPrefs.SetInt($"point{y}{x}tier", point.tier);
        PlayerPrefs.SetInt($"point{y}{x}color", (int)point.color);
        PlayerPrefs.SetInt($"point{y}{x}isEmpty", point.isEmpty ? 1 : 0);
        PlayerPrefs.SetInt($"point{y}{x}isControl", point.isControl ? 1 : 0);
        PlayerPrefs.SetInt($"point{y}{x}isCombine", point.isCombine ? 1 : 0);
        PlayerPrefs.SetInt($"point{y}{x}isBomb", point.isBomb ? 1 : 0);
    }

    public static bool HasSavedGame()
    {
        return PlayerPrefs.GetInt("savedGame", 0) == 1;
    }

    public static int ColorDifficult
    {
        get
        {
            return PlayerPrefs.GetInt("colorDifficult");
        }
        set
        {
            PlayerPrefs.SetInt("colorDifficult", value);
        }
    }

    public static int ReachedTier
    {
        get
        {
            return PlayerPrefs.GetInt("reachedTier", 0);
        }
        set
        {
            PlayerPrefs.SetInt("reachedTier", value);
        }
    }

    public static int MovesCount
    {
        get
        {
            return PlayerPrefs.GetInt("currentMovesCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("currentMovesCount", value);
        }
    }

    #region Stats

    public static int CurrentScore
    {
        get
        {
            return PlayerPrefs.GetInt("currentScore");
        }
        set
        {
            PlayerPrefs.SetInt("currentScore", value);
        }
    }

    public static int BestScore
    {
        get
        {
            return PlayerPrefs.GetInt("bestScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("bestScore", value);
        }
    }

    public static int GetBestScore(int colorTier)
    {
        return PlayerPrefs.GetInt($"bestScore{colorTier}", 0);
    }

    public static void SetBestScore(int colorTier, int value)
    {
        PlayerPrefs.SetInt($"bestScore{colorTier}", value);
    }

    public static int[] GetMaxTier()
    {
        int[] maxTier = new int[Props.PointColorsLength];

        for (int i = 0; i < maxTier.Length; i++)
        {
            maxTier[i] = PlayerPrefs.GetInt($"currentMaxTier{i}");
        }

        return maxTier;
    }

    public static void SetMaxTier(int[] maxTier)
    {
        for (int i = 0; i < maxTier.Length; i++)
        {
            PlayerPrefs.SetInt($"currentMaxTier{i}", maxTier[i]);
        }
    }

    public static int TotalHighestLevel
    {
        get
        {
            return PlayerPrefs.GetInt("totalHighestLevel", 0);
        }
        set
        {
            PlayerPrefs.SetInt("totalHighestLevel", value);
        }
    }
    public static int GetHighestLevel(int colorTier)
    {
        return PlayerPrefs.GetInt($"highestLevel{colorTier}", 0);
    }

    public static void SetHighestLevel(int colorTier, int value)
    {
        PlayerPrefs.SetInt($"highestLevel{colorTier}", value);
    }

    public static int TotalGamesFinished
    {
        get
        {
            return PlayerPrefs.GetInt("totalGamesFinished", 0);
        }
        set
        {
            PlayerPrefs.SetInt("totalGamesFinished", value);
        }
    }

    public static int GetGamesFinished(int colorTier)
    {
        return PlayerPrefs.GetInt($"gamesFinished{colorTier}", 0);
    }

    public static void SetGamesFinished(int colorTier, int value)
    {
        PlayerPrefs.SetInt($"gamesFinished{colorTier}", value);
    }

    public static int GetMostMoves(int colorTier)
    {
        return PlayerPrefs.GetInt($"mostMoves{colorTier}", 0);
    }

    public static void SetMostMoves(int colorTier, int value)
    {
        PlayerPrefs.SetInt($"mostMoves{colorTier}", value);
    }

    #endregion

    public static bool Sound
    {
        get
        {
            return PlayerPrefs.GetInt("sounds", 1) == 1;
        }
        set
        {            
            PlayerPrefs.SetInt("sounds", value ? 1 : 0);
        }
    }

    public static bool Mode
    {
        get
        {
            return PlayerPrefs.GetInt("lightMode", 0) == 1;
        }
        set
        {
            int mode = value ? 1 : 0;
            PlayerPrefs.SetInt("lightMode", mode);
            Messenger.Broadcast(GameEvent.MODE_SWITCHED);
        }
    }

    public static int Coins
    {
        get
        {
            return PlayerPrefs.GetInt("Coins", 150);
        }
        set
        {
            PlayerPrefs.SetInt("Coins", value);
            //Messenger<int>.Broadcast(GameEvent.ON_COIN_VALUE, value);
        }
    }

    public static System.EventHandler<ValueChangedEventArgs> OnHintsValueChanged;
    public static int Hints
    {
        get
        {
            return PlayerPrefs.GetInt("Hints", 10);
        }
        set
        {
            PlayerPrefs.SetInt("Hints", value);
            if (OnHintsValueChanged != null)
            {
                OnHintsValueChanged.Invoke(null, new ValueChangedEventArgs(value));
            }
            //Messenger<int>.Broadcast(GameEvent.ON_HINTS_VALUE, value);
        }
    }

    public static int Theme
    {
        get
        {
            return PlayerPrefs.GetInt("theme", 0);
        }
        set
        {
            PlayerPrefs.SetInt("theme", value);
            Messenger.Broadcast(GameEvent.THEME_CHANGED);
        }
    }

    public static bool GetOwnTheme(int themeIndex)
    {
        return PlayerPrefs.GetInt("theme" + themeIndex, themeIndex == 0 ? 1 : 0) == 1;
    }

    public static void SetOwnTheme(int themeIndex, bool value)
    {
        PlayerPrefs.SetInt("theme" + themeIndex, value ? 1 : 0);
    }

    public static bool Ads
    {
        get
        {
            return PlayerPrefs.GetInt("ads", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("ads", value ? 1 : 0);
            if (!value) Messenger.Broadcast(GameEvent.ON_DISABLE_ADS);
        }
    }
}
