using System;
using UnityEngine;
using AnimationState = Arcadian.Animation.AnimationState;

namespace Arcadian.Extensions
{
    public static class TextureExtensions
    {
        public static AnimationState[] ToCharacterAnimations(this Texture2D texture2D)
        {
            if (texture2D.width != 64 && texture2D.height != 192)
            {
                Debug.LogError("Texture must be 64x192 (provided {}x{}).");
                return Array.Empty<AnimationState>();
            }

            const int spriteWidth = 16;
            const int spriteHeight = 24;

            const int rows = 8;
            const int columns = 4;

            var animationStates = new AnimationState[rows];

            for (var r = 0; r < rows; r++)
            {
                var animationState = new AnimationState
                {
                    name = r switch
                    {
                        0 => "Walk Up",
                        1 => "Walk Right",
                        2 => "Walk Left",
                        3 => "Walk Down",
                        4 => "Idle Up",
                        5 => "Idle Right",
                        6 => "Idle Left",
                        7 => "Idle Down",
                        _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
                    }
                };

                var sprites = new Sprite[columns];

                for (var c = 0; c < columns; c++)
                {
                    sprites[c] = Sprite.Create(texture2D, new Rect(c*spriteWidth, r*spriteHeight, spriteWidth, spriteHeight), new Vector2(0.5f, 0f), 16);
                }

                animationState.sprites = sprites;

                animationStates[r] = animationState;
            }

            return animationStates;
        }
    }
}