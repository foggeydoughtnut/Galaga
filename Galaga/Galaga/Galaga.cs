using System.Collections.Generic;
using Galaga.States;
using Microsoft.Xna.Framework;

namespace Galaga
{
    public class Galaga : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private GameState _currentState;
        private GameStates _nextState = GameStates.MainMenu;
        private Dictionary<GameStates, GameState> _states;

        public Galaga()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;


            _states = new Dictionary<GameStates, GameState>
            {
                { GameStates.MainMenu, new MenuState() },
                { GameStates.GamePlay, new PlayState() },
                { GameStates.HighScores, new HighScoreState() },
                { GameStates.About, new CreditState() },
                { GameStates.Controls, new ControlsState() },
            };

            foreach (KeyValuePair<GameStates, GameState> item in _states)
            {
                item.Value.Initialize(GraphicsDevice, _graphics, Window);
            }

            _currentState = _states[_nextState];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (KeyValuePair<GameStates, GameState> item in _states)
            {
                item.Value.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _nextState = _currentState.Update(gameTime);
            if (_currentState is MenuState { UseAi: true } menuState && _nextState == GameStates.GamePlay)
            {
                menuState.UseAi = false;
                ((PlayState)_states[GameStates.GamePlay]).UseAi = true;
            }
            
            if (_nextState == GameStates.Exit)
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _currentState.Render();
            _currentState = _states[_nextState];
            base.Draw(gameTime);
        }
    }
}