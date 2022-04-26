using UnityEngine;

public static class Props
{
    private static ColorSettings colorSettings = Resources.Load<ColorSettings>("Props/ColorSettings");

    private static int theme
    {
        get
        {
            return PlayerData.Theme;
        }
    }

    private static int mode
    {
        get
        {
            return PlayerData.Mode ? 1 : 0;
        }
    }

    public static ColorSettings settings
    {
        get
        {
            return colorSettings;
        }
    }

    public static Color BackMain
    {
        get
        {
            return colorSettings.themes[theme].backMain[mode];
        }
    }
    public static Color BackUI
    {
        get
        {
            return colorSettings.themes[theme].backUI[mode];
        }
    }
    public static Color BackUIPopup
    {
        get
        {
            return colorSettings.themes[theme].backUIPopup[mode];
        }
    }
    public static Color BackUIFade
    {
        get
        {
            return colorSettings.themes[theme].backUIFade[mode];
        }
    }
    public static Color ButtonMain
    {
        get
        {
            return colorSettings.themes[theme].buttonMain[mode];
        }
    }
    public static Color ButtonColored
    {
        get
        {
            return colorSettings.themes[theme].buttonColored[mode];
        }
    }
    public static Color FontMain
    {
        get
        {
            return colorSettings.themes[theme].fontMain[mode];
        }
    }
    public static Color FontAdditional
    {
        get
        {
            return colorSettings.themes[theme].fontAdditional[mode];
        }
    }
    public static Color FontContrast
    {
        get
        {
            return colorSettings.themes[theme].fontContrast[mode];
        }
    }

    public static Sprite MainMenuBackground
    {
        get
        {
            return colorSettings.themes[theme].mainMenuBackground[mode];
        }
    }
    public static Sprite GameBackground
    {
        get
        {
            return colorSettings.themes[theme].gameBackground[mode];
        }
    }

    public static Color GetPointColor(PointData.ColorType color)
    {
        return colorSettings.pointColors[(int)color];
    }

    public static Color GetUIColor(UITag uiTag)
    {
        switch (uiTag)
        {
            case UITag.BackMain:        return BackMain;
            case UITag.BackUI:          return BackUI;
            case UITag.BackUIPopup:     return BackUIPopup;
            case UITag.BackUIFade:      return BackUIFade;
            case UITag.ButtonMain:      return ButtonMain;
            case UITag.ButtonColored:   return ButtonColored;
            case UITag.FontMain:        return FontMain;
            case UITag.FornAdditional:  return FontAdditional;
            default:                    return Color.black;
        }
    }
    public static Sprite GetSprite(UITag uiTag)
    {
        switch (uiTag)
        {
            case UITag.MainMenuBackground:  return MainMenuBackground;                
            case UITag.GameBackground:      return GameBackground;
            default:                        return null;
        }
    }

    public static int PointColorsLength
    {
        get
        {
            return colorSettings.pointColors.Length;
        }
    }

}



/*
    private static ColorTheme[] colorThemes =
    {
        // Standart theme
        new ColorTheme(
             // Back Main      
             new Color[] { new Color(0.8980392f, 0.8823529f, 0.8823529f, 1f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f) },
             // Back UI
             new Color[] { new Color(0.6196079f, 0.6196079f, 0.6196079f, 0.4f), new Color(0.4588235f, 0.4588235f, 0.4588235f, 0.4f) },
             // Back UI Popup
             new Color[] { new Color(0.8980392f, 0.8980392f, 0.8980392f, 0.7f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 0.99f) },
             // BackUI Fade
             new Color[] { new Color(0.509804f, 0.509804f, 0.509804f, 0.5f), new Color(0f, 0f, 0f, 0.5f) },
             // Button Main
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(0.3098039f, 0.3098039f, 0.3098039f, 1f) },
             // Button Colored
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Font Main
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Fort Additional
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 1f) },
             // Main Menu Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/standart"), Resources.Load<Sprite>("Sprites/Themes/standart") },
             // Game Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/standart"), Resources.Load<Sprite>("Sprites/Themes/standart") }
        ),

        // Abstract theme
        new ColorTheme(
             // Back Main      
             new Color[] { new Color(0.8980392f, 0.8823529f, 0.8823529f, 1f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f) },
             // Back UI
             new Color[] { new Color(0.6196079f, 0.6196079f, 0.6196079f, 0.4f), new Color(0.4588235f, 0.4588235f, 0.4588235f, 0.4f) },
             // Back UI Popup
             new Color[] { new Color(0.8980392f, 0.8980392f, 0.8980392f, 0.7f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 0.99f) },
             // BackUI Fade
             new Color[] { new Color(0.509804f, 0.509804f, 0.509804f, 0.5f), new Color(0f, 0f, 0f, 0.5f) },
             // Button Main
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(0.3098039f, 0.3098039f, 0.3098039f, 1f) },
             // Button Colored
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Font Main
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Fort Additional
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 1f) },
             // Main Menu Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/standart"), Resources.Load<Sprite>("Sprites/Themes/standart") },
             // Game Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/abstract_2_1"), Resources.Load<Sprite>("Sprites/Themes/abstract_2_2") }
        ),

        // Ocean theme
        new ColorTheme(
             // Back Main      
             new Color[] { new Color(0.8980392f, 0.8823529f, 0.8823529f, 1f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f) },
             // Back UI
             new Color[] { new Color(0.6196079f, 0.6196079f, 0.6196079f, 0.4f), new Color(0.4588235f, 0.4588235f, 0.4588235f, 0.4f) },
             // Back UI Popup
             new Color[] { new Color(0.8980392f, 0.8980392f, 0.8980392f, 0.7f), new Color(0.1960784f, 0.1960784f, 0.1960784f, 0.99f) },
             // BackUI Fade
             new Color[] { new Color(0.509804f, 0.509804f, 0.509804f, 0.5f), new Color(0f, 0f, 0f, 0.5f) },
             // Button Main
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(0.3098039f, 0.3098039f, 0.3098039f, 1f) },
             // Button Colored
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Font Main
             new Color[] { new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f), new Color(0.4392157f, 0.6666667f, 0.9568627f, 1f) },
             // Fort Additional
             new Color[] { new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 1f) },
             // Main Menu Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/ocean"), Resources.Load<Sprite>("Sprites/Themes/ocean") },
             // Game Background
             new Sprite[] { Resources.Load<Sprite>("Sprites/Themes/ocean"), Resources.Load<Sprite>("Sprites/Themes/ocean") }
        )
    };
    */