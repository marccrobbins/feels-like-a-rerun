using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WobblyGamerStudios;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace TOJam.FLR
{
    public class UIManager : Manager<UIManager>
    {
        [SerializeField] private UIWindowBase _mainMenuUI;
        [SerializeField] private UIWindowBase _inGameUI;
        [SerializeField] private PopUpWindow _popUpWindow;

        [SerializeField] private InputActionProperty _menuAction;

        protected override IEnumerator InitializeManager()
        {
            _menuAction.action.performed += OnMenuButtonPress;
            GameManager.OnGameStateChange += OnGameStateChange;
            
            return base.InitializeManager();
        }

        private void OnMenuButtonPress(Context context)
        {
            if (!context.performed || GameManager.Instance.GameState != GameState.Game) return;
            
            _mainMenuUI.Hide();
            
            if (_inGameUI.IsVisible) _inGameUI.Hide();
            else _inGameUI.Show();
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.Menu:
                    _mainMenuUI.Show();
                    _inGameUI.Hide();
                    break;
                case GameState.Pregame:
                case GameState.Game:
                    _mainMenuUI.Hide();
                    _inGameUI.Hide();
                    break;
            }
        }
    }
}
