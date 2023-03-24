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
    public override void LoadContent(ContentManager contentManager)
    {
        Fonts.Add("default", contentManager.Load<SpriteFont>("Fonts/DemoFont1"));
        Fonts.Add("big", contentManager.Load<SpriteFont>("Fonts/DemoFont2"));    
        _tracker = HighScoreTracker.GetTracker();
    }

    public override GameStates Update(GameTime gameTime)
    {
        ProcessInput();
        return IsKeyPressed(Keys.Escape) ? GameStates.MainMenu : GameStates.HighScores;
    }

    protected override void ProcessInput()
    {
        if(IsKeyPressed(Keys.F5))
            _tracker.ResetHighScores();
        base.ProcessInput();
    }

    public override void Render(GameTime gameTime)
    {   
        SpriteBatch.Begin();
        // High Score Title
        var bigFont = Fonts["big"];
        var titleSize = bigFont.MeasureString("High Scores");
        CreateBorderOnWords(bigFont, "High Scores",
            new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - titleSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 4) + 150));
        
        // No high scores
        var font = Fonts["default"];
        var highScoreCount = _tracker.HighScores.Count;
        if (highScoreCount == 0)
        {
            var noScoreSize = font.MeasureString("No high scores");
            CreateBorderOnWords(font, "No high scores",
                new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - noScoreSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 2)));
            SpriteBatch.End();
            return;
        }
        
        var middle = highScoreCount / 2;
        var highScores = _tracker.HighScores.ToList();
        int diff;
        // Show Scores
        for (var i = 0; i < highScoreCount; i++)
        {
            var stringSize = font.MeasureString(highScores[i].ToString());
            diff = i - middle;
            CreateBorderOnWords(font, highScores[i].ToString(), new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - stringSize.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 2) + diff * Constants.MENU_BUFFER));
        }
        
        diff = highScoreCount - middle;
        var stringSizeReset = font.MeasureString("Press F5 to Reset Scores");
        CreateBorderOnWords(font, "Press F5 to Reset Scores", new Vector2(Convert.ToInt32(Graphics.PreferredBackBufferWidth / 2) - stringSizeReset.X / 2, Convert.ToInt32(Graphics.PreferredBackBufferHeight / 2) + diff * Constants.MENU_BUFFER));

        SpriteBatch.End();
    }
}