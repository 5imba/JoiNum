using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] private Image[] tabs;
    [SerializeField] private GameObject[] pages;

    private void Start()
    {
        // init
        SwitchTab(0, false);
    }
    public void SwitchTab(int tabIndex)
    {
        SwitchTab(tabIndex, true);
    }

    public void SwitchTab(int tabIndex, bool playSound)
    {
        // Play sound click
        if (playSound) Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);

        // Disactivate all tabs
        for (int i = 0; i < pages.Length; i++)
        {
            tabs[i].enabled = true;
            pages[i].SetActive(false);
        }

        // Activate current tab
        tabs[tabIndex].enabled = false;
        pages[tabIndex].SetActive(true);
    }
}
