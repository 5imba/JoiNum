using UnityEngine;

[CreateAssetMenu(menuName ="Color Settings")]
public class ColorSettings : ScriptableObject
{
    public Color[] pointColors;
    public Theme[] themes;

    [System.Serializable]
    public class Theme
    {
        public Color2 backMain;
        public Color2 backUI;
        public Color2 backUIPopup;
        public Color2 backUIFade;
        public Color2 buttonMain;
        public Color2 buttonColored;
        public Color2 fontMain;
        public Color2 fontAdditional;
        public Color2 fontContrast;
        public Sprite2 mainMenuBackground;
        public Sprite2 gameBackground;
        public int price;
        public string nameLocalizedId;

        public Theme(Color2 backMain, Color2 backUI, Color2 backUIPopup, Color2 backUIFade,
            Color2 buttonMain, Color2 buttonColored, Color2 fontMain, Color2 fontAdditional,
            Color2 fontContrast, Sprite2 mainMenuBackground, Sprite2 gameBackground, int price, 
            string nameLocalizedId)
        {
            this.backMain = backMain;
            this.backUI = backUI;
            this.backUIPopup = backUIPopup;
            this.backUIFade = backUIFade;
            this.buttonMain = buttonMain;
            this.buttonColored = buttonColored;
            this.fontMain = fontMain;
            this.fontAdditional = fontAdditional;
            this.fontContrast = fontContrast;
            this.mainMenuBackground = mainMenuBackground;
            this.gameBackground = gameBackground;
            this.price = price;
            this.nameLocalizedId = nameLocalizedId;
        }
    }

    [System.Serializable]
    public struct Color2
    {
        public Color light;
        public Color dark;

        public Color2(Color light, Color dark)
        {
            this.light = light;
            this.dark = dark;
        }

        public Color this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return light;
                }
                else if (index == 1)
                {
                    return dark;
                }
                else
                {
                    return new Color(0f, 0f, 0f, 0f);
                }
            }
            set
            {
                if (index == 0)
                {
                    light = value;
                }
                else if (index == 1)
                {
                    dark = value;
                }
            }
        }
    }

    [System.Serializable]
    public struct Sprite2
    {
        public Sprite light;
        public Sprite dark;

        public Sprite2(Sprite light, Sprite dark)
        {
            this.light = light;
            this.dark = dark;
        }

        public Sprite this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return light;
                }
                else if (index == 1)
                {
                    return dark;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (index == 0)
                {
                    light = value;
                }
                else if (index == 1)
                {
                    dark = value;
                }
            }
        }
    }
}
