using UnityEngine;

namespace Arcadian.Enums
{
    public enum Direction2D
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class Direction2DExtensions
    {
        public static Direction2D ToDirection2D(this Vector2 vector2)
        {
            vector2 = vector2.normalized;
            
            if (Mathf.Abs(vector2.x) > Mathf.Abs(vector2.y))
            {
                return vector2.x > 0 ? Direction2D.Right : Direction2D.Left;
            }

            return vector2.y > 0 ? Direction2D.Up : Direction2D.Down;
        }
    }
}