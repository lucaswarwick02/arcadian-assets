using TMPro;
using UnityEngine;

namespace Arcadian.UI.Transition
{
    public class TransitionEffectText : AbstractUI
    {
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI body;

        public bool SetTexts(string headerText, string bodyText)
        {
            header.text = headerText;
            body.text = bodyText;

            return !(string.IsNullOrEmpty(headerText) && string.IsNullOrEmpty(bodyText));
        }
    }
}