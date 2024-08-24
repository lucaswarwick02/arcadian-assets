using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arcadian.GameObjects
{
    public abstract class UniqueGameObject : MonoBehaviour
    {
        [SerializeField] private string uniqueID;

        public string UniqueID => uniqueID;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheckForDuplicateIDs();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckForDuplicateIDs()
        {
            var uniqueIDs = FindObjectsByType<UniqueGameObject>(FindObjectsSortMode.None)
                .GroupBy(o => o.uniqueID)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (uniqueIDs.Count != 0)
            {
                Debug.LogError($"Duplicate Unique IDs found ({uniqueIDs.Count}): {string.Join(", ", uniqueIDs)}");
            }
        }
    }
}