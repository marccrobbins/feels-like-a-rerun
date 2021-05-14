using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace TOJam.FLR
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private PopUpWindow _popUpWindow;

        public void StartRun()
        {
            GameManager.Instance.StartRun();
        }
        
        public void Quit()
        {
            _popUpWindow.OpenPopup("Quit", "Are you sure you want to quit?", acceptCallback: Application.Quit);
        }
    }
}
