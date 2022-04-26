using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace LocalizationVariables
{
    public class ColorDifficult : IVariable
    {
        public object GetSourceValue(ISelectorInfo selector) => PlayerData.ColorDifficult;
    }

    public class CurrentScore : IVariable
    {
        public object GetSourceValue(ISelectorInfo selector) => PlayerData.CurrentScore;
    }

    [Serializable]
    public class Hints : IVariable, IVariableValueChanged
    {
        [SerializeField]
        bool intitialized = false;

        public int Value
        {
            get
            {
                if (!intitialized)
                    Init();

                return PlayerData.Hints;
            }
        }

        private void Init()
        {
            PlayerData.OnHintsValueChanged += (sender, args) => { ValueChanged?.Invoke(this); };
            intitialized = true;
        }
                
        public event Action<IVariable> ValueChanged;
        public object GetSourceValue(ISelectorInfo _) => Value;
    }
}
