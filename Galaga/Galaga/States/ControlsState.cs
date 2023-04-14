using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.States
{
    public class ControlsState : GameState
    {
        RenderTarget2D renderTarget;
        private int _indexOfChoice;
        private List<string> _controls;
        private bool _listening;


        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window)
        {
            _listening = false;
            _controls = new()
            {
                $"Move Left: {Keys.Left}",
                $"Move Right: {Keys.Right}",
                $"Fire: {Keys.Space}"
            };
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
            Fonts.Add("veryBig", contentManager.Load<SpriteFont>("Fonts/DemoFont3"));
        }

        // Example of keyReleased event
        private bool keyReleased(Keys key)
        {
            return (Keyboard.GetState().IsKeyUp(key) && true);// state previous was down)
        }

        protected override void ProcessInput()
        {
            if (!_listening)
            {
                if (IsKeyPressed(Keys.Up) && _indexOfChoice - 1 >= 0)
                    _indexOfChoice -= 1;

                if (IsKeyPressed(Keys.Down) && _indexOfChoice + 1 < _controls.Count)
                    _indexOfChoice += 1;

                if (IsKeyPressed(Keys.Enter))
                {
                    _listening = true;
                }
            }
            else
            {
                // listening
            }
            // TODO Take what the user inputs after and then put that into a variable.
            // Write that value to be the new input for the specific command
            // Write that new data to the file

            base.ProcessInput();

        }

        public override void Render()
        {
            this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

            SpriteFont bigFont = Fonts["big"];
            Vector2 titleSize = bigFont.MeasureString("Controls");
            RenderUtilities.CreateBorderOnWord(SpriteBatch, bigFont, "Controls",
                new Vector2(Convert.ToInt32(renderTarget.Width / 2) - titleSize.X / 2, Convert.ToInt32(renderTarget.Height / 4)));

            SpriteFont font = Fonts["default"];
            for (int i = 0; i < _controls.Count; i++)
            {
                SpriteFont optionFont = _indexOfChoice == i ? bigFont : font;

                string control = _controls[i];
                Vector2 stringSize = font.MeasureString(control);
                RenderUtilities.CreateBorderOnWord(SpriteBatch, optionFont, control, new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2 + (1.5f * stringSize.Y * i))));
            }
            SpriteBatch.End();
            Graphics.GraphicsDevice.SetRenderTarget(null);

            // Render render target to screen
            SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(
                    renderTarget,
                    new Rectangle(Window.ClientBounds.Width / 8, 0, (Window.ClientBounds.Height / 3 * 4), Window.ClientBounds.Height),
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1f
                );
            SpriteBatch.End();
        }

        public override GameStates Update(GameTime gameTime)
        {
            ProcessInput();
            return Keyboard.GetState().IsKeyDown(Keys.Escape) ? GameStates.MainMenu : GameStates.Controls;
        }
    }
}
