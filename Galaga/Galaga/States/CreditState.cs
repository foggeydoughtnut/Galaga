using System;
using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.States;

public class CreditState : GameState
{
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
    }

    public override GameStates Update(GameTime gameTime)
    {
        ProcessInput();
        return Keyboard.GetState().IsKeyDown(Keys.Escape) ? GameStates.MainMenu : GameStates.About;
    }

    public override void Render()
    {
        this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(Textures["background"], new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

        SpriteFont bigFont = Fonts["galagaBig"];
        Vector2 titleSize = bigFont.MeasureString("Credits");
        RenderUtilities.CreateBorderOnWord(SpriteBatch, bigFont, "Credits",
            new Vector2(Convert.ToInt32(renderTarget.Width / 2) - titleSize.X / 2, Convert.ToInt32(renderTarget.Height / 4)));

        SpriteFont font = Fonts["galaga"];
        Vector2 stringSize = font.MeasureString("We Did This. Trey Crowther and Jeff Anderson.");
        RenderUtilities.CreateBorderOnWord(SpriteBatch, font, "We Did This. Trey Crowther and Jeff Anderson.", new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2)));

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

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }
}