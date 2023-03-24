using System;
using System.Collections.Generic;
using System.Linq;
using Galaga.Systems;
using Galaga.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States.SubPlayStates;

public class PlaySubPlayState : SubPlayState
{
    private readonly HighScoreTracker _tracker;
    private readonly List<Systems.System> _systems;

    public PlaySubPlayState(IReadOnlyDictionary<string, List<Texture2D>> textures)
    {
        _tracker = HighScoreTracker.GetTracker();
        
        _systems = new List<Systems.System>();
        var gameStats = new GameStatsSystem();
        var bulletSystem = new BulletSystem(textures["playerBullet"].First(), textures["enemyBullet"].First(), gameStats);
        var playerSystem = new PlayerSystem(textures["ship"].First(), gameStats, bulletSystem);
        var enemySystem = new EnemySystem(playerSystem, bulletSystem);
        var collisionDetectionSystem = new CollisionDetectionSystem(playerSystem, enemySystem, bulletSystem);
        _systems.Add(playerSystem);
        _systems.Add(enemySystem);
        _systems.Add(bulletSystem);
        _systems.Add(collisionDetectionSystem);
    }
    
    public override PlayStates Update(GameTime gameTime)
    {
        foreach(var system in _systems)
            system.Update(gameTime);
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            return PlayStates.Pause;
        return PlayStates.Play;
    }

    public override void Render(SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts)
    {
        foreach(var system in _systems)
            system.Render(spriteBatch);
        // Show high score
        var font = fonts["default"];
        var stringSize = font.MeasureString("Score: " + _tracker.CurrentGameScore);
        spriteBatch.DrawString(font, "Score: " + _tracker.CurrentGameScore,
            new Vector2(Constants.BOUNDS_X - stringSize.X, 0), Color.White);
    }
}