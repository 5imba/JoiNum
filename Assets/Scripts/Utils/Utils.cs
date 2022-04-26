using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public static class Utils
{
    public static void LogIndex(Index i)
    {
        Debug.Log(string.Format("y:{0} x:{1}", i.y, i.x));
    }
    public static void LogIndex(string prefix, PointData.Point p)
    {
        Debug.Log(string.Format("{0} i:[{1},{2}] t:{3} c:{4} empty:{5}", prefix, p.index.y, p.index.x, p.Tier, p.color, p.IsEmpty));
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Vector3 AddVec2(this Vector3 v3, Vector2 v2)
    {
        return new Vector3(v3.x + v2.x, v3.y + v2.y, v3.z);
    }

    public static Vector3 ToVector3(float value)
    {
        return new Vector3(value, value, value);
    }

    public static void CreateWindow(string windowText, ModalWindow.WindowType windowType)
    {
        ModalWindow modalWindow = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ModalWindow")).GetComponent<ModalWindow>();
        modalWindow.Show(windowText, windowType);
    }

    public static void CreateWindowWithItem(string title, string description, GameItem item, ModalWindow.WindowType windowType)
    {
        ModalWindow modalWindow = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ModalWindow")).GetComponent<ModalWindow>();
        modalWindow.ShowWithItem(title, description, item, windowType);
    }

    public static string GetLocalizedString(string table, string key, KeyValuePair<string, IVariable>[] args = null)
    {
        var localizedString = new LocalizedString
        { TableReference = table, TableEntryReference = key };

        if (args?.Length > 0)
            foreach (var arg in args)
                localizedString.Add(arg);

        return localizedString.GetLocalizedString();
    }

    public static LocalizedString GetLocalizedStringObject(string table, string key, KeyValuePair<string, IVariable>[] args = null)
    {
        var localizedString = new LocalizedString
        { TableReference = table, TableEntryReference = key };

        if (args?.Length > 0)
            foreach (var arg in args)
                localizedString.Add(arg);

        return localizedString;
    }

    public static string IntToStringShortener(int amount)
    {
        if (amount < 10000)
        {
            return amount.ToString();
        }
        else if (amount < 10000000)
        {
            float newAmount = amount * 0.001f;
            return newAmount.ToString("0.0") + "k";
        }
        else if (amount < 1000000000)
        {
            float newAmount = amount * 0.000001f;
            return newAmount.ToString("0.000") + "m";
        }
        else
        {
            float newAmount = amount * 0.000000001f;
            return newAmount.ToString("0.000") + "b";
        }
    }

    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }

    public static GameObject[] FindInActiveObjectsByLayer(int layer)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].gameObject.layer == layer)
            {
                gameObjects.Add(objs[i].gameObject);
            }
        }

        return gameObjects.Count > 0 ? gameObjects.ToArray() : null;
    }

    public static GameObject[] FindInActiveObjectsByTag(string tag)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].tag == tag)
            {
                gameObjects.Add(objs[i].gameObject);
            }
        }

        return gameObjects.Count > 0 ? gameObjects.ToArray() : null;
    }
}
