using System;
using UnityEngine;

namespace Game
{
    public class Consts
    {
        public static TimeSpan THROTTTLE_IN_SCEOND = TimeSpan.FromSeconds(0.2f);
        public static string PERSISTANT_DATA_PATH = Application.persistentDataPath;
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string GAME_FOLDER_PATH()
        {
            string[] dataPath = Application.dataPath.Split('/');
            string path = "";
            for (int i = 0; i < dataPath.Length - 1; i++)
            {
                path += $"{dataPath[i]}/";
            };
            return path;
        }
        public static string VERSION = "v1.1.0";
    }
}