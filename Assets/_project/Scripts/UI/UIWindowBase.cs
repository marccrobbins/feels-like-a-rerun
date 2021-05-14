using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TOJam.FLR
{
    public class UIWindowBase : MonoBehaviour
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private bool _startVisible;
        
        protected EventSystem _eventSystem;
        
        private void Awake()
        {
            _eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Start()
        {
            _window.SetActive(_startVisible);
        }

        public virtual void Show()
        {
            _window.SetActive(true);
        }

        public virtual void Hide()
        {
            _window.SetActive(false);
        }

        public void SetFirstInteractable(GameObject interactableObject)
        {
            StartCoroutine(SetInteractableRoutine(interactableObject));
        }

        private IEnumerator SetInteractableRoutine(GameObject interactableObject)
        {
            _eventSystem.SetSelectedGameObject(null);
            yield return null;
            _eventSystem.SetSelectedGameObject(interactableObject);
        }
    }
}
