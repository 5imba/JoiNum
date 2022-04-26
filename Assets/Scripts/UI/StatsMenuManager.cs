using System;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenuManager : MonoBehaviour
{
    [Header("All Games")]
    [SerializeField] private Text bestScore;
    [SerializeField] private Text highestLevel;
    [SerializeField] private Text totalGamesPlayed;

    [SerializeField] private StatsBlock colors_3;
    [SerializeField] private StatsBlock colors_4;
    [SerializeField] private StatsBlock colors_5;

    public void Load()
    {
        bestScore.text = PlayerData.BestScore.ToString();
        highestLevel.text = PlayerData.TotalHighestLevel.ToString();
        totalGamesPlayed.text = PlayerData.TotalGamesFinished.ToString();

        colors_3.bestScore.text = PlayerData.GetBestScore(3).ToString();
        colors_3.highestLevel.text = PlayerData.GetHighestLevel(3).ToString();
        colors_3.finishedGames.text = PlayerData.GetGamesFinished(3).ToString();
        colors_3.mostMoves.text = PlayerData.GetMostMoves(3).ToString();

        colors_4.bestScore.text = PlayerData.GetBestScore(4).ToString();
        colors_4.highestLevel.text = PlayerData.GetHighestLevel(4).ToString();
        colors_4.finishedGames.text = PlayerData.GetGamesFinished(4).ToString();
        colors_4.mostMoves.text = PlayerData.GetMostMoves(4).ToString();

        colors_5.bestScore.text = PlayerData.GetBestScore(5).ToString();
        colors_5.highestLevel.text = PlayerData.GetHighestLevel(5).ToString();
        colors_5.finishedGames.text = PlayerData.GetGamesFinished(5).ToString();
        colors_5.mostMoves.text = PlayerData.GetMostMoves(5).ToString();
    }

    [Serializable]
    private class StatsBlock
    {
        public Text bestScore;
        public Text highestLevel;
        public Text finishedGames;
        public Text mostMoves;
    }
}
