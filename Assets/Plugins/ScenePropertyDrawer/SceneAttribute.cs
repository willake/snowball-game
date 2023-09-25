
using UnityEngine;

namespace WillakeD.ScenePropertyDrawler
{
    public class SceneAttribute : PropertyAttribute { }

    public static class Extensions
    {
        public static string GetSceneNameByPath(this string path)
        {
            string[] array = path.Split('/');
            return array[array.Length - 1].Replace(".unity", "");
        }
    }
}