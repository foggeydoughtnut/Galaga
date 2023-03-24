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
            _graphics.PreferredBackBufferWidth = 2130;
            _graphics.PreferredBackBufferHeight = 1880;
            _graphics.ApplyChanges();

            _states = new Dictionary<GameStates, GameState>
            {
                { GameStates.MainMenu, new MenuState() },
                { GameStates.GamePlay, new PlayState() },
                { GameStates.HighScores, new HighScoreState() },
                { GameStates.About, new CreditState() },
            };

            foreach (var item in _states)
            {
                item.Value.Initialize(GraphicsDevice, _graphics);
            }

            _currentState = _states[_nextState];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (var item in _states)
            {
                item.Value.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _nextState = _currentState.Update(gameTime);
            if (_nextState == GameStates.Exit)
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _currentState.Render(gameTime);
            _currentState = _states[_nextState];
            base.Draw(gameTime);
        }
    }
}