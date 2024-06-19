using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetsLinkMap
{
    internal static class AssetsLinkMapUtils
    {
        public static List<Scene> GetCurrentOpenedScenes()
        {
            List<Scene> scenes = new List<Scene>();
            for (int sceneIdx = 0; sceneIdx < SceneManager.sceneCount; ++sceneIdx)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIdx);
                scenes.Add(scene);
            }
            return scenes;
        }
    }
}