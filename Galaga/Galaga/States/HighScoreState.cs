using System;
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
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));    
        _tracker = HighScoreTracker.GetTracker();
    }

    public override GameStates Update(GameTime gameTime)
    {
        var nextState = IsKeyPressed(Keys.Escape) ? GameStates.MainMenu : GameStates.HighScores; 
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
        this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        // High Score Title
        var bigFont = Fonts["big"];
        var titleSize = bigFont.MeasureString("High Scores");
        RenderUtilities.CreateBorderOnWord(SpriteBatch, bigFont, "High Scores",
            new Vector2(Convert.ToInt32(renderTarget.Width / 2) - titleSize.X / 2, Convert.ToInt32(renderTarget.Height / 4)));
        
        // No high scores
        var font = Fonts["default"];
        var highScoreCount = _tracker.HighScores.Count;
        if (highScoreCount == 0)
        {
            var noScoreSize = font.MeasureString("No high scores");
            RenderUtilities.CreateBorderOnWord(SpriteBatch, font, "No high scores",
                new Vector2(Convert.ToInt32(renderTarget.Width / 2) - noScoreSize.X / 2, Convert.ToInt32(renderTarget.Height / 2)));
        }
        else
        {
            var middle = highScoreCount / 2;
            var highScores = _tracker.HighScores.ToList();
            int diff;
            // Show Scores
            for (var i = 0; i < highScoreCount; i++)
            {
                var stringSize = font.MeasureString(highScores[i].ToString());
                diff = i - middle;
                RenderUtilities.CreateBorderOnWord(SpriteBatch,font, highScores[i].ToString(), new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
            }
        
            diff = highScoreCount - middle;
            var stringSizeReset = font.MeasureString("Press F5 to Reset Scores");
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