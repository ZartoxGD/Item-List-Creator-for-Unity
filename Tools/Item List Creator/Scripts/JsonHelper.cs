using UnityEngine;

namespace ItemListCreator
{
    public static class JsonHelper
    {
        /// <summary>
        /// Deserialize a JSON string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The string to Deserialize</param>
        /// <returns>An array of Objects</returns>
        public static T[] FromJson<T>(string json)
        {
            // Deserialize the JSON string into a Wrapper object
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            // Return the array of items contained in the wrapper object
            return wrapper.Items;
        }

        /// <summary>
        /// Serialize an array of Objects 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">the array of Objects to serialize</param>
        /// <returns>a JSON formatted string</returns>
        public static string ToJson<T>(T[] array)
        {
            // Create a new Wrapper object and set its Items field to the input array
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            // Use JsonUtility to serialize the Wrapper object into a JSON string
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Serialize an array of Objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">the array of Objects to serialize</param>
        /// <param name="prettyPrint">if true will print each properties on a new line</param>
        /// <returns></returns>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            // Create a new Wrapper object and set its Items field to the input array
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            // Use JsonUtility to serialize the Wrapper object into a JSON string
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
