using System.Collections;
using WobblyGamerStudios;

namespace TOJam.FLR
{
    public delegate void GameStateChangeDelegate(GameState state);
    
    public class GameManager : Manager<GameManager>
    {
        private GameState _gameState = GameState.Invalid;
        public GameState GameState
        {
            get => _gameState;
            private set
            {
                if (value == _gameState) return;
                _gameState = value;
                OnGameStateChange?.Invoke(_gameState);
            }
        }

        protected override IEnumerator InitializeManager()
        {
            GameState = GameState.Menu;
            
            return base.InitializeManager();
        }

        public void BeginPreGame()
        {
            GameState = GameState.Pregame;
        }

        public void BeginGame()
        {
            GameState = GameState.Game;
        }

        public void BeginPostGame()
        {
            GameState = GameState.PostGame;
        }

        #region Events

        public static GameStateChangeDelegate OnGameStateChange;

        #endregion Events
    }

    public enum GameState
    {
        Invalid = 0,
        Menu,
        Pregame,
        Game,
        PostGame
    }
}
