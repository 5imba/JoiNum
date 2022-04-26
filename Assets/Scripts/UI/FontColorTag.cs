using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FontColorTag : MonoBehaviour
{
    [SerializeField] private UITag dayColor = UITag.FornAdditional;
    [SerializeField] private UITag nightColor = UITag.FornAdditional;
    private Text _text;


    public void SetColor()
    {
        if (_text == null) _text = GetComponent<Text>();
        _text.color = PlayerData.Mode ? Props.GetUIColor(nightColor) : Props.GetUIColor(dayColor);
    }
}
