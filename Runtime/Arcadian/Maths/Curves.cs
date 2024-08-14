using UnityEngine;

namespace Arcadian.Maths
{
    /// <summary>
    /// Used to store animation curves for use in animations.
    /// </summary>
    public static class Curves
    {
        /// <summary>
        /// AnimationCurve that starts at 0 and ends at 1.
        /// </summary>
        public static readonly AnimationCurve In = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        /// <summary>
        /// AnimationCurve that starts at 1 and ends at 0.
        /// </summary>
        public static readonly AnimationCurve Out = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    }
}