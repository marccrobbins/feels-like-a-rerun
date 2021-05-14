using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WobblyGamerStudios;

namespace TOJam.FLR
{
    public class GameManager : Manager<GameManager>
    {
        [SerializeField] private SceneReference _levelScene;
        
        public void StartRun()
        {
            SceneManager.LoadScene(_levelScene.SceneName);
        }
    }
}
