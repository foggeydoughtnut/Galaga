using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaga.Utilities;

public static class RenderUtilities
{
    public static void CreateBorderOnWord(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position)
    {
        const int offset = 4;
        var newPosition = new Vector2(position.X, position.Y);
        // Move left and up
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y - offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right and up
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y - offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move left and down
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y + offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right and down
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y + offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move down        
        newPosition.Y = position.Y - offset;
        newPosition.X = position.X;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move up        
        newPosition.Y = position.Y + offset;
        newPosition.X = position.X;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move right
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Move left      
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y;
        spriteBatch.DrawString(font, text, newPosition, Color.Black);
        // Main string
        spriteBatch.DrawString(font, text, position, Color.White);
    }
}