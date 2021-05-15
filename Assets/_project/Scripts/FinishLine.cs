using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOJam.FLR
{
    public class FinishLine : MonoBehaviour
    {
        [SerializeField] private LevelController _levelController;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<Player>()) return;
            _levelController.FinishRun();
        }
    }
}
