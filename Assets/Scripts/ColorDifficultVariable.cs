using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using System.Collections.Generic;

[DisplayName("Current Color Difficult")]
public class ColorDifficultVariable : IVariable
{
    public object GetSourceValue(ISelectorInfo selector) => PlayerData.ColorDifficult;
}

public class Test
{
    void Teser()
    {
        var localizedString = new LocalizedString("My Table", "My Entry");

        // An example of a Smart String using the variable would be: "You have {player-money:C}.".
        // :C will apply the current Locale currency and number formatting.
        localizedString.Add("player-money", new FloatVariable { Value = 100.45f });
        new KeyValuePair<string, IVariable>("num", new IntVariable { Value = 3 });

    }
}
