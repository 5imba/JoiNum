using UnityEngine;

public class ResetStatsButton : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private StatsMenuManager statsMenu;

    public void ShowWindow()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        window.SetActive(true);
    }

    public void CloseWindow()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        window.SetActive(false);
    }

    public void ResetStats()
    {
        Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        PlayerData.BestScore = 0;
        PlayerData.TotalHighestLevel = 0;
        PlayerData.TotalGamesFinished = 0;

        for (int i = 3; i < 6; i++)
        {
            PlayerData.SetBestScore(i, 0);
            PlayerData.SetHighestLevel(i, 0);
            PlayerData.SetGamesFinished(i, 0);
            PlayerData.SetMostMoves(i, 0);
        }

        statsMenu.Load();
        window.SetActive(false);
    }
}
