using UnityEngine;

public static class GameTempData
{
    static bool isPause = false;
    static bool isNewGame = true;
    static int colorDifficult = 3;

    public static bool IsPause
    {
        get
        {
            return isPause;
        }
        set
        {
            isPause = value;
        }
    }

    public static bool IsNewGame
    {
        get
        {
            return isNewGame;
        }
        set
        {
            isNewGame = value;

            if (isNewGame)
            {
                PlayerPrefs.SetInt("savedGame", 0);
            }
        }
    }

    public static int ColorDifficult
    {
        get
        {
            return colorDifficult;
        }
        set
        {
            colorDifficult = value;
        }
    }


    private static int hints = 10;
    public static int Hints
    {
        get
        {
            return hints;
        }
        set
        {
            hints = value;
            if (OnHintsValueChanged != null) OnHintsValueChanged.Invoke(null, new ValueChangedEventArgs(value));
        }
    }
    public static System.EventHandler<ValueChangedEventArgs> OnHintsValueChanged;
}
