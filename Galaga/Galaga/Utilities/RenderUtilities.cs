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
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move right and up
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y - offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move left and down
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y + offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move right and down
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y + offset;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move down        
        newPosition.Y = position.Y - offset;
        newPosition.X = position.X;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move up        
        newPosition.Y = position.Y + offset;
        newPosition.X = position.X;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move right
        newPosition.X = position.X + offset;
        newPosition.Y = position.Y;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Move left      
        newPosition.X = position.X - offset;
        newPosition.Y = position.Y;
        spriteBatch.DrawString(font, text, newPosition, Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        // Main string
        spriteBatch.DrawString(font, text, position, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.5f); // Needed to draw main string on different layer to make it so it drew on top always
    }

}