using UnityEngine;

public static class Pointer
{
    public static bool position(out Vector2 pos)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;

                float totalX = 0f;
                float totalY = 0f;
                for (int i = 0; i < touches.Length; i++)
                {
                    totalX += touches[i].position.x;
                    totalY += touches[i].position.y;
                }
                float centerX = totalX / touches.Length;
                float centerY = totalY / touches.Length;

                pos = new Vector2(centerX, centerY);
                return true;
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            pos = Input.mousePosition;
            return Input.GetMouseButton(0);
        }

        pos = Vector2.zero;
        return false;
    }
}
