using System;
using UnityEngine;
using System.Collections.Generic;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A set of extension methods for <c>Sprite</c> to manipulate pivots, locate pixels by color, covert pixel positions to UV offsets, an remove specific pixel colors. Useful for dynamically adjusting sprite origins, detecting color positions, or creating modified textures at runtime.
    /// </summary>
    public static class SpriteExtensions
    {
        /// <summary>
        /// Vector2 representing the center of a sprite.
        /// </summary>
        public static readonly Vector2 PivotCenter = new(0.5f, 0.5f);

        /// <summary>
        /// Centers the pivot of a sprite to (0.5, 0.5).
        /// </summary>
        /// <param name="sprite">Original sprite.</param>
        /// <returns>Sprite with a pivot of 0.5, 0.5</returns>
        public static Sprite CenterSprite(this Sprite sprite)
        {
            var texture = sprite.texture;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), PivotCenter, sprite.pixelsPerUnit);
        }

        /// <summary>
        /// Find the (first) pixel coordinate for a given color.
        /// </summary>
        /// <param name="sprite">Sprite to search.</param>
        /// <param name="color">Color to look for.</param>
        /// <returns>Pixel coordinate of the first found color.</returns>
        public static Vector2Int GetPixelPoint(this Sprite sprite, Color32 color)
        {
            var rect = sprite.textureRect;
            var pixels = sprite.texture.GetPixels32();
            int texWidth = sprite.texture.width;

            for (int y = 0; y < (int)rect.height; y++)
            {
                int rowOffset = ((int)rect.y + y) * texWidth + (int)rect.x;
                for (int x = 0; x < (int)rect.width; x++)
                {
                    if (pixels[rowOffset + x].Equals(color)) return new Vector2Int(x, y);
                }
            }

            Debug.LogError("Could not find the pixel point!");
            return Vector2Int.zero;
        }

        /// <summary>
        /// Given a pixel point on a sprite, find the normalized offset.
        /// </summary>
        /// <param name="sprite">Sprite to grab the texture rect from.</param>
        /// <param name="pixelPoint">Pixel point to use as reference.</param>
        /// <returns>Normalized offset.</returns>
        public static Vector2 PixelPointToOffset(this Sprite sprite, Vector2Int pixelPoint)
        {
            var rect = sprite.textureRect;
            return new Vector2(
                (pixelPoint.x + rect.x) / rect.width,
                (pixelPoint.y + rect.y) / rect.height
            );
        }

        /// <summary>
        /// Remove a list of colours from a sprite.
        /// </summary>
        /// <param name="sprite">Sprite to search and filter.</param>
        /// <param name="colors">Colors to find and remove.</param>
        /// <returns>Sprite with removed colours.</returns>
        public static Sprite RemovePixelPoints(this Sprite sprite, params Color32[] colors)
        {
            var rect = sprite.textureRect;
            var texPixels = sprite.texture.GetPixels32();
            int texWidth = sprite.texture.width;
            int width = (int)rect.width, height = (int)rect.height;

            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                Array.Copy(texPixels, ((int)rect.y + y) * texWidth + (int)rect.x, pixels, y * width, width);
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                foreach (var c in colors)
                {
                    if (pixels[i].Equals(c))
                    {
                        pixels[i] = Color.clear;
                        break;
                    }
                }
            }

            var newTexture = new Texture2D(width, height)
            {
                filterMode = sprite.texture.filterMode
            };
            newTexture.SetPixels32(pixels);
            newTexture.Apply();

            var pivot = new Vector2((sprite.pivot.x - sprite.textureRectOffset.x) / width, (sprite.pivot.y - sprite.textureRectOffset.y) / height);
            return Sprite.Create(newTexture, new Rect(0, 0, width, height), pivot, sprite.pixelsPerUnit);
        }

        /// <summary>
        /// Convert the sprite so that all pixels with alpha greater than <c>alphaThreshold</c> become the provided <c>color</c>,
        /// and all other pixels become clear (transparent).
        /// </summary>
        /// <param name="sprite">Source sprite whose texture rect will be processed.</param>
        /// <param name="color">Target color to apply to pixels above the threshold.</param>
        /// <param name="alphaThreshold">Alpha threshold in range [0,1]. Pixels with alpha &gt; threshold become <paramref name="color"/>. Default is 0.5.</param>
        /// <returns>A new <see cref="Sprite"/> with converted pixels.</returns>
        public static Sprite ToColor(this Sprite sprite, Color32 color, float alphaThreshold = 0.5f)
        {
            var rect = sprite.textureRect;
            int width = (int)rect.width, height = (int)rect.height;
            var srcPixels = sprite.texture.GetPixels32();
            int texWidth = sprite.texture.width;

            // Copy sprite rect into a compact buffer
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                Array.Copy(srcPixels, ((int)rect.y + y) * texWidth + (int)rect.x, pixels, y * width, width);
            }

            // Convert pixels based on alpha threshold
            byte thresh = (byte)Mathf.Clamp(Mathf.RoundToInt(alphaThreshold * 255f), 0, 255);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > thresh)
                    pixels[i] = color;
                else
                    pixels[i] = Color.clear;
            }

            var newTexture = new Texture2D(width, height)
            {
                filterMode = sprite.texture.filterMode,
                wrapMode = sprite.texture.wrapMode
            };
            newTexture.SetPixels32(pixels);
            newTexture.Apply();

            var pivot = new Vector2((sprite.pivot.x - sprite.textureRectOffset.x) / width, (sprite.pivot.y - sprite.textureRectOffset.y) / height);
            return Sprite.Create(newTexture, new Rect(0, 0, width, height), pivot, sprite.pixelsPerUnit);
        }

        /// <summary>
        /// Adds a pixel outline to a sprite, expanding the texture bounds to ensure outlines appear even at edges.
        /// </summary>
        /// <param name="sprite">The source sprite to add an outline to.</param>
        /// <param name="outlineColor">The color to use for the outline.</param>
        /// <param name="cutCorners">If true, uses 4-neighbour outline (cross pattern). If false, uses 8-neighbour (including diagonals).</param>
        /// <param name="alphaThreshold">Alpha value below which pixels are treated as transparent. Allows semi-transparent pixels to be ignored.</param>
        /// <returns>A new sprite with the outline applied.</returns>
        public static Sprite AddOutline(this Sprite sprite, Color32 outlineColor, bool cutCorners = false, byte alphaThreshold = 0)
        {
            var rect = sprite.textureRect;
            int width = (int)rect.width, height = (int)rect.height;
            var srcPixels = sprite.texture.GetPixels32();
            int texWidth = sprite.texture.width;

            // Copy sprite rect into a compact buffer
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                Array.Copy(srcPixels, ((int)rect.y + y) * texWidth + (int)rect.x, pixels, y * width, width);
            }

            // Create expanded buffer with 1 pixel padding on all sides
            int expandedWidth = width + 2;
            int expandedHeight = height + 2;
            var expandedPixels = new Color32[expandedWidth * expandedHeight];
            
            // Copy original pixels into center of expanded buffer
            for (int y = 0; y < height; y++)
            {
                Array.Copy(pixels, y * width, expandedPixels, (y + 1) * expandedWidth + 1, width);
            }

            // Output buffer starts as a copy of the expanded pixels
            var outPixels = new Color32[expandedWidth * expandedHeight];
            Array.Copy(expandedPixels, outPixels, expandedPixels.Length);

            // helper to check if a pixel is opaque (alpha > threshold)
            bool IsOpaque(in Color32 c) => c.a > alphaThreshold;

            // Check transparent pixels only; if any neighbour (4 or 8 depending on cutCorners) is opaque set outline color
            for (int y = 0; y < expandedHeight; y++)
            {
                int row = y * expandedWidth;
                for (int x = 0; x < expandedWidth; x++)
                {
                    int idx = row + x;
                    if (IsOpaque(expandedPixels[idx])) continue; // preserve existing opaque

                    bool neighbourOpaque = false;

                    if (cutCorners)
                    {
                        // 4-neighbour check (up, down, left, right)
                        if (x > 0 && IsOpaque(expandedPixels[idx - 1])) neighbourOpaque = true;
                        else if (x < expandedWidth - 1 && IsOpaque(expandedPixels[idx + 1])) neighbourOpaque = true;
                        else if (y > 0 && IsOpaque(expandedPixels[idx - expandedWidth])) neighbourOpaque = true;
                        else if (y < expandedHeight - 1 && IsOpaque(expandedPixels[idx + expandedWidth])) neighbourOpaque = true;
                    }
                    else
                    {
                        // 8-neighbour check (including diagonals)
                        int y0 = Math.Max(0, y - 1);
                        int y1 = Math.Min(expandedHeight - 1, y + 1);
                        int x0 = Math.Max(0, x - 1);
                        int x1 = Math.Min(expandedWidth - 1, x + 1);

                        for (int ny = y0; ny <= y1 && !neighbourOpaque; ny++)
                        {
                            int nRow = ny * expandedWidth;
                            for (int nx = x0; nx <= x1; nx++)
                            {
                                if (nx == x && ny == y) continue;
                                if (IsOpaque(expandedPixels[nRow + nx]))
                                {
                                    neighbourOpaque = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (neighbourOpaque)
                    {
                        outPixels[idx] = outlineColor;
                    }
                }
            }

            var newTexture = new Texture2D(expandedWidth, expandedHeight)
            {
                filterMode = sprite.texture.filterMode,
                wrapMode = sprite.texture.wrapMode
            };
            newTexture.SetPixels32(outPixels);
            newTexture.Apply();

            // Adjust pivot: sprite.pivot is relative to the logical rect. 
            // Subtract textureRectOffset to get the pivot relative to the extracted pixels,
            // then add 1 pixel for the padding, then normalize.
            float px = (sprite.pivot.x - sprite.textureRectOffset.x + 1) / expandedWidth;
            float py = (sprite.pivot.y - sprite.textureRectOffset.y + 1) / expandedHeight;
            return Sprite.Create(newTexture, new Rect(0, 0, expandedWidth, expandedHeight), new Vector2(px, py), sprite.pixelsPerUnit);
        }

        /// <summary>
        /// Divides a sprite sheet into a 2D list of sprites (rows x columns).
        /// Only non-empty cells (pixels with alpha &gt; 0.01) are included.
        /// Notes:
        /// - The texture used by the sprite must be Read/Write enabled in import settings for pixel checks to work.
        /// - If the texture is not readable, the function will still create sprites but will not be able to detect emptiness and will include the cell.
        /// </summary>
        /// <param name="sheet">Source sprite (texture) containing the sheet.</param>
        /// <param name="rows">Number of rows to divide into (vertical slices).</param>
        /// <param name="columns">Number of columns to divide into (horizontal slices).</param>
        /// <param name="pixelsPerUnit">Pixels per unit for created sprites (defaults to 100).</param>
        /// <param name="pivot">Optional pivot point for created sprites (defaults to Unity’s (0.5, 0.5)).</param>
        /// <returns>List of rows, each row is a list of Sprites (empty rows are omitted).</returns>
        public static List<List<Sprite>> DivideSpriteSheet(this Sprite sheet, int rows, int columns, int pixelsPerUnit = 100, Vector2? pivot = null)
        {
            var result = new List<List<Sprite>>();
            if (sheet == null || rows <= 0 || columns <= 0)
                return result;

            Texture2D tex = sheet.texture;
            if (tex == null)
                return result;

            int texW = tex.width;
            int texH = tex.height;

            int cellW = texW / columns;
            int cellH = texH / rows;

            if (cellW <= 0 || cellH <= 0)
                return result;

            Vector2 actualPivot = pivot ?? new Vector2(0.5f, 0.5f);

            bool readable = tex.isReadable;
            Color[] allPixels = readable ? tex.GetPixels() : null;

            for (int r = 0; r < rows; r++)
            {
                var rowList = new List<Sprite>();
                for (int c = 0; c < columns; c++)
                {
                    int x = c * cellW;
                    int y = r * cellH;

                    int w = (c == columns - 1) ? texW - x : cellW;
                    int h = (r == rows - 1) ? texH - y : cellH;

                    if (w <= 0 || h <= 0)
                        continue;

                    bool hasVisible = true;
                    if (readable)
                    {
                        hasVisible = false;
                        int yEnd = y + h;
                        int xEnd = x + w;
                        for (int yy = y; yy < yEnd; yy++)
                        {
                            int rowOffset = yy * texW;
                            for (int xx = x; xx < xEnd; xx++)
                            {
                                if (allPixels[rowOffset + xx].a > 0.01f)
                                {
                                    hasVisible = true;
                                    goto FoundVisible; // break both loops fast
                                }
                            }
                        }
                    }
                FoundVisible:

                    if (!hasVisible)
                        continue;

                    Rect rect = new Rect(x, y, w, h);
                    Sprite s = Sprite.Create(tex, rect, actualPivot, pixelsPerUnit);
                    rowList.Add(s);
                }

                if (rowList.Count > 0)
                    result.Add(rowList);
            }

            return result;
        }

        /// <summary>
        /// Calculates the pixel width of a sprite within its texture rect, counting only pixels with alpha above the threshold.
        /// </summary>
        /// <param name="sprite">The sprite to measure.</param>
        /// <param name="alphaThreshold">The alpha threshold (0-1). Pixels with alpha above this value are counted.</param>
        /// <returns>The width in pixels of the sprite's visible content within its texture rect.</returns>
        public static int GetPixelWidth(this Sprite sprite, float alphaThreshold = 0.5f)
        {
            if (sprite == null || sprite.texture == null)
                return 0;

            Texture2D texture = sprite.texture;
            Rect rect = sprite.rect;
            
            int rectWidth = (int)rect.width;
            int rectHeight = (int)rect.height;
            int rectX = (int)rect.x;
            int rectY = (int)rect.y;
            int texWidth = texture.width;
            
            // Get all pixels at once - vastly more efficient than individual GetPixel calls
            Color32[] pixels = texture.GetPixels32();
            
            byte threshold = (byte)(alphaThreshold * 255);
            int width = 0;

            // Check each column for opaque pixels
            for (int x = 0; x < rectWidth; x++)
            {
                for (int y = 0; y < rectHeight; y++)
                {
                    // Calculate index in the 1D pixels array (row-major order)
                    int pixelIndex = (rectY + y) * texWidth + (rectX + x);
                    if (pixels[pixelIndex].a > threshold)
                    {
                        width++;
                        break;
                    }
                }
            }

            return width;
        }
    }
}