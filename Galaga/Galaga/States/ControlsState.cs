using Galaga.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Galaga.States
{


    public class ControlsState : GameState
    {
        RenderTarget2D renderTarget;
        private int _indexOfChoice;
        private List<string> _controls;
        private bool _listening;
        KeyboardState previousKeyboardState;
        private MouseState _previousMouseState;
        private Dictionary<int, Tuple<int, int>> _optionPositions;
        Keys pressedKey;
        Controls controls;



        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window)
        {
            if (!File.Exists("controls.json"))
            {
                Controls newControls = new Controls {Left = Keys.Left, Right = Keys.Right, Fire = Keys.Space };
                string json = JsonConvert.SerializeObject(newControls);
                File.WriteAllText("controls.json", json);
            }

            string controlsFileData = File.ReadAllText("controls.json");
            controls = JsonConvert.DeserializeObject<Controls>(controlsFileData);


            // Todo read controls from json file
            _listening = false;
            _controls = new()
            {
                $"Move Left: {controls.Left}",
                $"Move Right: {controls.Right}",
                $"Fire: {controls.Fire}"
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
            previousKeyboardState = Keyboard.GetState();
            pressedKey = new();
            _optionPositions = new Dictionary<int, Tuple<int, int>>();
            base.Initialize(graphicsDevice, graphics, window);
        }
        public override void LoadContent(ContentManager contentManager)
        {
            Textures.Add("background", contentManager.Load<Texture2D>("Images/Background"));
            Fonts.Add("galaga", contentManager.Load<SpriteFont>("Fonts/File"));
            Fonts.Add("galagaBig", contentManager.Load<SpriteFont>("Fonts/File2"));
            Fonts.Add("galagaSmall", contentManager.Load<SpriteFont>("Fonts/File3"));
        }


        protected override void ProcessInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            var currentMouseState = Mouse.GetState();
            if (!_listening)
            {
                if (currentKeyboardState.IsKeyUp(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up) && _indexOfChoice - 1 >= 0)
                    _indexOfChoice -= 1;

                if (currentKeyboardState.IsKeyUp(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down) && _indexOfChoice + 1 < _controls.Count)
                    _indexOfChoice += 1;

                if (!_optionPositions.Any())
                    _optionPositions = MouseMenu.DefineOptionPositions(Fonts, _controls.Count, renderTarget);
                if (currentMouseState.Position != _previousMouseState.Position)
                    _indexOfChoice = MouseMenu.UpdateIndexBasedOnMouse(currentMouseState, Window, _optionPositions, _indexOfChoice);
                
                if (currentKeyboardState.IsKeyUp(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter)
                    || currentMouseState.LeftButton == ButtonState.Released 
                    && _previousMouseState.LeftButton == ButtonState.Pressed 
                    && currentMouseState.X < Window.ClientBounds.Width
                    && currentMouseState.X > 0)
                {
                    _listening = true;
                }
            }
            else // listening
            {
                foreach (Keys key in currentKeyboardState.GetPressedKeys())
                {
                    // Take what the user inputs after and then put that into a variable.
                    pressedKey = key;
                    break;
                }
                //Debug.WriteLine(pressedKey);
                if (pressedKey != Keys.None)
                {
                    if (_indexOfChoice == 0) // Left
                    {
                        controls.Left = pressedKey;
                        _controls[0] = $"Move Left: {controls.Left}";
                    }
                    else if (_indexOfChoice == 1) // Right
                    {
                        controls.Right = pressedKey;
                        _controls[1] = $"Move Right: {controls.Right}";
                    }
                    else if (_indexOfChoice == 2) // Fire
                    {
                        controls.Fire = pressedKey;
                        _controls[2] = $"Fire: {controls.Fire}";
                    }
                    else
                    {
                        Debug.WriteLine("Invalid index");
                    }
                    pressedKey = Keys.None;
                    _listening = false;

                    // Write that value to be the new input for the specific command
                    string json = JsonConvert.SerializeObject(controls);
                    File.WriteAllText("controls.json", json);
                    Controls.ChangeInControls = true;
                }
            }
            previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            base.ProcessInput();

        }

        public override void Render()
        {
            this.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            this.Graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            this.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(Textures["background"], new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);

            SpriteFont bigFont = Fonts["galagaBig"];
            Vector2 titleSize = bigFont.MeasureString("Controls");
            RenderUtilities.CreateBorderOnWord(SpriteBatch, bigFont, "Controls",
                new Vector2(Convert.ToInt32(renderTarget.Width / 2) - titleSize.X / 2, Convert.ToInt32(renderTarget.Height / 8)));

            SpriteFont font = Fonts["galaga"];
            var middle = _controls.Count / 2;
            if (_listening)
            {
                string message = "Listening...";
                Vector2 stringSize = font.MeasureString(message);
                var diff = -1 - middle;
                RenderUtilities.CreateBorderOnWord(SpriteBatch, font, message, new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
            }
            
            for (int i = 0; i < _controls.Count; i++)
            {
                SpriteFont optionFont = _indexOfChoice == i ? bigFont : font;

                string control = _controls[i];
                Vector2 stringSize = optionFont.MeasureString(control);
                int diff = i - middle;
                RenderUtilities.CreateBorderOnWord(SpriteBatch, optionFont, _controls[i], new Vector2(Convert.ToInt32(renderTarget.Width / 2) - stringSize.X / 2, Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER));
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

        public override GameStates Update(GameTime gameTime)
        {
            ProcessInput();
            return Keyboard.GetState().IsKeyDown(Keys.Escape) ? GameStates.MainMenu : GameStates.Controls;
        }
    }
}
