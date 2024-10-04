using System.Collections;
using Arcadian.Maths;
using Arcadian.System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Arcadian.UI.Transition
{
    public class TransitionEffect : MonoBehaviour
    {
        private const float FadeTime = 0.25f;

        private static Vector3 Position => new(Screen.width / 2f, Screen.height / 2f, 0);

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TransitionEffectText transitionEffectText;
        
        public static void ChangeScene(string sceneName, string header, string body, float speed = 1f, float textSpeed = 1f)
        {
            if (string.IsNullOrWhiteSpace(ArcadianAssets.Config.TransitionEffectPath))
            {
                Debug.LogError("You must set ArcadianAssetsConfig.TransitionEffectPath in order to use TransitionEffect.ChangeScene()");
                return;
            }
            
            Addressables.InstantiateAsync(ArcadianAssets.Config.TransitionEffectPath, Position, Quaternion.identity).Completed +=
                handle =>
                {
                    var transitionEffect = handle.Result.GetComponent<TransitionEffect>();
                    
                    DontDestroyOnLoad(transitionEffect.gameObject);
                    transitionEffect.canvasGroup.alpha = 0f;
                    transitionEffect.transitionEffectText.Close();

                    transitionEffect.StartCoroutine(transitionEffect.Animation(sceneName, header, body, speed, textSpeed));
                };
        }

        private IEnumerator Animation(string sceneName, string header, string body, float speed, float textSpeed)
        {
            var timer = 0f;
            while (timer < FadeTime)
            {
                timer += Time.deltaTime * speed;

                canvasGroup.alpha = Curves.In.Evaluate(timer / FadeTime);
                
                yield return null;
            }

            SceneManager.LoadScene(sceneName);

            yield return new WaitForSeconds(0.75f / speed);
            
            var showText = transitionEffectText.SetTexts(header, body);

            if (showText)
            {
                transitionEffectText.Open();

                yield return new WaitForSeconds(2f * textSpeed);
            }

            timer = 0f;
            while (timer < FadeTime)
            {
                timer += Time.deltaTime * speed;

                canvasGroup.alpha = Curves.Out.Evaluate(timer / FadeTime);
                
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}