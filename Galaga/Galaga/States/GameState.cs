using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Galaga.States;

public abstract class GameState
{
    private KeyboardState _previousKeyState;
    protected GraphicsDeviceManager Graphics;
    protected SpriteBatch SpriteBatch;
    protected GameWindow Window;
    protected readonly Dictionary<string, List<Texture2D>> Textures = new();
    protected readonly Dictionary<string, SpriteFont> Fonts = new();
    protected readonly Dictionary<string, Song> Songs = new();

    public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window)
    {
        Graphics = graphics;
        SpriteBatch = new SpriteBatch(graphicsDevice);
        Window = window;
    }

    public abstract void LoadContent(ContentManager contentManager);
    
    public abstract GameStates Update(GameTime gameTime);

    public abstract void Render();

    protected virtual void ProcessInput()
    {
        _previousKeyState = Keyboard.GetState();
    }

    private bool IsKeyPressed(IEnumerable<Keys> keys)
    {
        var currentState = Keyboard.GetState();
        return keys.Any(key => _previousKeyState.IsKeyUp(key) && currentState.IsKeyDown(key));
    }

    protected bool IsKeyPressed(Keys key)
    {
        return IsKeyPressed(new List<Keys> { key });
    }
}