using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TOJam.FLR
{
    public class PopUpWindow : UIWindowBase
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private TextMeshProUGUI _acceptButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _cancelButtonText;
        
        public void OpenPopup(string title, string message, string acceptText = "Accept", UnityAction acceptCallback = null, string cancelText = "Cancel", UnityAction cancelCallback = null)
        {
            _titleText.text = title;
            _messageText.text = message;
            
            _acceptButtonText.text = acceptText;
            _acceptButton.onClick.RemoveAllListeners();
            _acceptButton.onClick.AddListener(Hide);
            _acceptButton.onClick.AddListener(acceptCallback);
            SetFirstInteractable(_acceptButton.gameObject);

            _cancelButtonText.text = cancelText;
            _cancelButton.onClick.RemoveAllListeners();
            _acceptButton.onClick.AddListener(Hide);
            _cancelButton.onClick.AddListener(acceptCallback);
            
            Show();
        }

        public override void Hide()
        {
            //Clear 
            _acceptButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            
            base.Hide();
        }
    }
}
