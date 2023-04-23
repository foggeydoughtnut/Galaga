using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States;

public class HighScoreState : GameState
{
    private HighScoreTracker _tracker;
    RenderTarget2D renderTarget;


    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window)
    {
        this.renderTarget = new RenderTarget2D(
            graphicsDevice,
            1440,
            1080,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            graphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.DiscardContents
        );
        base.Initialize(graphicsDevice, graphics, window);
    }

    public override void LoadContent(ContentManager contentManager)
    {
        Textures.Add("background", contentManager.Load<Texture2D>("Images/Background"));
        Fonts.Add("galaga", contentManager.Load<SpriteFont>("Fonts/File"));
        Fonts.Add("galagaBig", contentManager.Load<SpriteFont>("Fonts/File2"));
        Fonts.Add("galagaSmall", contentManager.Load<SpriteFont>("Fonts/File3"));
        _tracker = HighScoreTracker.GetTracker();
    }

    public override GameStates Update(GameTime gameTime)
    {
        GameStates nextState = IsKeyPressed(Keys.Escape) ? GameStates.MainMenu : GameStates.HighScores; 
        ProcessInput();
        return nextState;
    }

    protected override void ProcessInput()
    {
        if(IsKeyPressed(Keys.F5))
            _tracker.ResetHighScores();
        base.ProcessInput();
    }

    public override void Render()
    {
        Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(Textures["background"], new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        // High Score Title
        SpriteFont bigFont = Fonts["galagaBig"];
        Vector2 titleSize = bigFont.MeasureString("High Scores");
        RenderUtilities.CreateBorderOnWord(SpriteBatch, bigFont, "High Scores",
            new Vector2(Convert.ToInt32(renderTarget.Width / 2) - titleSize.X / 2, Convert.ToInt32(renderTarget.Height / 8)));
        
        // No high scores
        SpriteFont font = Fonts["galaga"];
        int highScoreCount = _tracker.HighScores.Count;
        if (highScoreCount == 0)
        {
            Vector2 noScoreSize = font.MeasureString("No high scores");
            RenderUtilities.CreateBorderOnWord(SpriteBatch, font, "No high scores",
                new Vector2(Convert.ToInt32(renderTarget.Width / 2) - noScoreSize.X / 2, Convert.ToInt32(renderTarget.Height / 2)));
        }
        else
        {
            int middle = highScoreCount / 2;
            List<int> highScores = _tracker.HighScores.ToList();
            int diff;
            // Show Scores
            for (int i = 0; i < highScoreCount; i++)
            {
                Vector2 stringSize = font.MeasureString(highScores[i].ToString());
                diff = i - middle;
                RenderUtilities.CreateBorderOnWord(SpriteBatch,font, highScores[i].ToString(), new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
            }
        
            diff = highScoreCount - middle;
            Vector2 stringSizeReset = font.MeasureString("Press F5 to Reset Scores");
            RenderUtilities.CreateBorderOnWord(SpriteBatch, font, "Press F5 to Reset Scores", new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSizeReset.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
        }
        

        SpriteBatch.End();
        Graphics.GraphicsDevice.SetRenderTarget(null);

        // Render render target to screen
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(
                renderTarget,
                new Rectangle(Window.ClientBounds.Width / 8, 0, 4 * Window.ClientBounds.Height / 3, Window.ClientBounds.Height),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );
        SpriteBatch.End();

    }
}