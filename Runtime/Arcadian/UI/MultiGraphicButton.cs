using UnityEngine;
using UnityEngine.UI;

namespace Arcadian.UI
{
    public class MultiGraphicButton : Button
    {
        private Graphic[] _graphics;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //get the graphics, if it could not get the graphics, return here
            if (!GetGraphics())
                return;
 
            var targetColor = state switch { 
                SelectionState.Disabled => colors.disabledColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Normal => colors.normalColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                _ => Color.white 
            };
 
            foreach (var graphic in _graphics)
                graphic.CrossFadeColor(targetColor, instant ? 0 : colors.fadeDuration, true, true);
        }
 
        private bool GetGraphics()
        {
            _graphics = GetComponentsInChildren<Graphic>();

            return _graphics is { Length: > 0 };
        }
    }
}