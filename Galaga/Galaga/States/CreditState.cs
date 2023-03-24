using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States;

public class CreditState : GameState
{
    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));
        Fonts.Add("veryBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
    }

    public override GameStates Update(GameTime gameTime)
    {
        ProcessInput();
        return Keyboard.GetState().IsKeyDown(Keys.Escape) ? GameStates.MainMenu : GameStates.About;
    }

    public override void Render(GameTime gameTime)
    {
        SpriteBatch.Begin();
        var bigFont = Fonts["big"];
        var titleSize = bigFont.MeasureString("Credits");
        CreateBorderOnWords(bigFont, "Credits",
            new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - titleSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 4) + 150));

        var font = Fonts["default"];
        var stringSize = font.MeasureString("I Did This. Trey Crowther.");
        CreateBorderOnWords(font, "I Did This. Trey Crowther.", new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - stringSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 2)));

        
        SpriteBatch.End();
    }

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }
}