using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOJam.FLR
{
    public class UIWindowBase : MonoBehaviour
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private bool _startVisible;
        
        public bool IsVisible { get; protected set; }
        
        protected EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Start()
        {
            _window.SetActive(_startVisible);
            
            foreach (var selectable in _window.GetComponentsInChildren<Selectable>())
            {
                if (!selectable) continue;
                selectable.interactable = _startVisible;
            }
            
            IsVisible = _startVisible;
        }

        public virtual void Show()
        {
            _window.SetActive(true);

            foreach (var selectable in _window.GetComponentsInChildren<Selectable>())
            {
                if (!selectable) continue;
                selectable.interactable = true;
            }
            
            IsVisible = true;
        }

        public virtual void Hide()
        {
            _window.SetActive(false);
            
            foreach (var selectable in _window.GetComponentsInChildren<Selectable>())
            {
                if (!selectable) continue;
                selectable.interactable = false;
            }
            
            IsVisible = false;
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
