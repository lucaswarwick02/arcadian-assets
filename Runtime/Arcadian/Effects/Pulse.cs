using Arcadian.Maths;
using UnityEngine;

namespace Arcadian.Effects
{
    public class Pulse : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        [SerializeField] private float scale = 1.15f;
        
        private void Update()
        {
            var t = Curves.In.Evaluate(Mathf.PingPong(Time.time * speed, 1f));
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scale, t);
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }
    }
}