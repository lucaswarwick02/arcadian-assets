using System.Collections;
using Arcadian.Maths;
using Arcadian.System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Arcadian.UI
{
    public static class FloatingText
    {
        public static void Create(
            string text,
            Vector3 worldPos,
            Color? color = null,
            Transform parent = null,
            Camera camera = null,
            float startOffset = 0f,
            float maxOffset = 1f,
            float maxRotation = 10f,
            float fadeTime = 0.25f,
            float stayTime = 0.5f)
        {
            if (string.IsNullOrWhiteSpace(ArcadianAssetsConfig.FloatingTextPath))
            {
                Debug.LogError("You must set ArcadianAssetsConfig.FloatingTextPath in order to use FloatingText.Create()");
                return;
            }

            // If not given, grab the Canvas and Camera
            if (!parent) parent = GameObject.Find("Canvas").transform;
            if (!camera) camera = Camera.main;
            
            // Calculate position + rotation
            var position = camera!.WorldToScreenPoint(worldPos);
            var rotation = Quaternion.Euler(0, 0, Random.Range(-maxRotation, maxRotation));

            var origin = worldPos + Vector3.one * startOffset;
            
            Addressables.InstantiateAsync(
                        ArcadianAssetsConfig.FloatingTextPath,
                        position,
                        rotation,
                        parent)
                    .Completed +=
                handle =>
                {
                    var floatingText = handle.Result.GetComponent<TextMeshProUGUI>();
                    floatingText.alpha = 0;
                    floatingText.text = text;

                    if (color.HasValue) floatingText.color = color.Value;

                    floatingText.StartCoroutine(Example(camera, floatingText, origin, maxOffset, fadeTime, stayTime));
                };
        }

        private static IEnumerator Example(
            Camera camera,
            TMP_Text tmpText,
            Vector3 origin,
            float maxOffset,
            float fadeTime,
            float stayTime)
        {
            tmpText.alpha = 0f;

            var target = origin + (Vector3.up * maxOffset);
            
            // Fade in
            var timer = 0f;
            while (timer < fadeTime)
            {
                var t = Curves.In.Evaluate(timer / fadeTime);

                tmpText.transform.position = camera.WorldToScreenPoint(Vector3.Lerp(origin, target, t));
                tmpText.alpha = t;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Stay
            timer = 0f;
            while (timer < stayTime)
            {
                tmpText.transform.position = camera.WorldToScreenPoint(target);
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Fade out
            timer = 0f;
            while (timer < fadeTime)
            {
                var t = Curves.Out.Evaluate(timer / fadeTime);

                tmpText.transform.position = camera.WorldToScreenPoint(target);
                tmpText.alpha = t;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            Object.Destroy(tmpText.gameObject);
        }
    }
}