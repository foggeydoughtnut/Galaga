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
    protected readonly Dictionary<string, Texture2D> Textures = new();
    protected readonly Dictionary<string, SpriteFont> Fonts = new();
    protected readonly Dictionary<string, Song> Songs = new();

    public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        Graphics = graphics;
        SpriteBatch = new SpriteBatch(graphicsDevice);
    }

    public abstract void LoadContent(ContentManager contentManager);
    
    public abstract GameStates Update(GameTime gameTime);

    public abstract void Render(GameTime gameTime);


    protected virtual void ProcessInput()
    {
        _previousKeyState = Keyboard.GetState();
    }

    protected bool IsKeyPressed(IEnumerable<Keys> keys)
    {
        var currentState = Keyboard.GetState();
        return keys.Any(key => _previousKeyState.IsKeyUp(key) && currentState.IsKeyDown(key));
    }

    protected bool IsKeyPressed(Keys key)
    {
        return IsKeyPressed(new List<Keys> { key });
    }
    
    protected void CreateBorderOnWords(SpriteFont font, string text, Vector2 position)
    {
        const int offset = 4;
        var newPosition = new Vector2(position.X, position.Y);
        // Move left and up
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y - offset;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right and up
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y - offset;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move left and down
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y + offset;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right and down
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y + offset;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move down        
        newPosition.Y = position.Y - offset;
        newPosition.X = position.X;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move up        
        newPosition.Y = position.Y + offset;
        newPosition.X = position.X;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move left      
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y;
        SpriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Main string
        SpriteBatch.DrawString(font, text, position, Color.White);
    }
}