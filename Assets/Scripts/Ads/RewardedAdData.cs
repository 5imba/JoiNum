using UnityEngine;

public class RewardedAdData : MonoBehaviour
{
    public GameItem rewardType;
    public int rewardAmount;

    public void ShowRewardedAd()
    {
        AdManager.ShowRewardedAd(this);
    }
}