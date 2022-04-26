using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BalanceLabel : MonoBehaviour
{
    [SerializeField] private LabelType labelType;
    [SerializeField] private string prefix = "<sprite index=0>";

    private enum LabelType {  Coins, Hints }

    private Text _text;
    private int value = 0;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        int temp = 0;
        switch (labelType)
        {
            case LabelType.Coins:
                temp = PlayerData.Coins;
                break;
            case LabelType.Hints:
                temp = PlayerData.Hints;
                break;
        }

        if (value != temp)
        {
            OnChangeValue(temp);
            value = temp;
        }
    }

    private void OnChangeValue(int amount)
    {
        _text.text = prefix + Utils.IntToStringShortener(amount);
    }
}
