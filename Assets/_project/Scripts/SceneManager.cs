using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WobblyGamerStudios;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TOJam.FLR
{
    public class SceneManager : Manager<SceneManager>
    {
        [SerializeField] private SceneReference _levelScene;
        
        public void LoadGameLevel()
        {
            USceneManager.LoadScene(_levelScene.SceneName);
        }
    }
}
