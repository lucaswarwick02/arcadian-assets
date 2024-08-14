using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Arcadian.Extensions
{
    /// <summary>
    /// A class that contains methods for handling objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the first component of type T in the child object.
        /// </summary>
        /// <param name="gameObject">Original game object being queried</param>
        /// <param name="childName">Name of the child object</param>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Component</returns>
        public static T GetChildComponent<T>(this GameObject gameObject, string childName)
        where T : Component
        {
            var childObject = gameObject.transform.Find(childName);
            return childObject.GetComponent<T>();
        }

        /// <summary>
        /// Deep clones an object.
        /// </summary>
        /// <param name="obj">object to clone</param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>Deep Clone of the object</returns>
        public static T DeepClone<T>(this T obj)
        {
            using var ms = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }
}
