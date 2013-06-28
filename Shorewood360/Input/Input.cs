using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace IETGames.Shorewood.Input
{
    public class ControllerDisconnectedEventArgs : EventArgs
    {
        public PlayerIndex PlayerIndex
        {
            get;
            set;
        }
        public ControllerDisconnectedEventArgs()
            : base()
        {
        }
    }

    //public delegate void OnControllerDisconnect(PlayerIndex index);
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ButtonFireEventArgs : EventArgs
    {
        
        public GameTime gameTime;
        public GamePadState previousGamePadState;
        public KeyboardState previousKeyboardState;        
        public bool previouslyDown;
        public ButtonFireEventArgs(GameTime gameTime, GamePadState previousGamePadState, bool previouslyDown)
        {
            this.previousGamePadState = previousGamePadState;
            this.gameTime = gameTime;
            this.previouslyDown = previouslyDown;
        }

        //public ButtonFireEventArgs(Buttons button, GameTime gameTime, GamePadState previousGamePadState)
        //{
        //    this.previousGamePadState = previousGamePadState;
        //    this.gameTime = gameTime;
        //    this.button = button;
        //}

        public ButtonFireEventArgs( GameTime gameTime, KeyboardState previousKeyboardState, bool previouslyDown)
        {
            this.previousKeyboardState = previousKeyboardState;
            this.gameTime = gameTime;
            this.previouslyDown = previouslyDown;
        }
    }
    
    public class InputHandler : Microsoft.Xna.Framework.GameComponent
    {        
        GamePadState previousGamePadState;
        KeyboardState previousKeyboardState;
        private Dictionary<Buttons, EventHandler<ButtonFireEventArgs>> buttonHandlers = new Dictionary<Buttons, EventHandler<ButtonFireEventArgs>>();
        private Dictionary<Keys, EventHandler<ButtonFireEventArgs>> keyHandlers = new Dictionary<Keys, EventHandler<ButtonFireEventArgs>>();
        public EventHandler<ControllerDisconnectedEventArgs> ControllerDisconnect
        {
            get;
            set;
        }
        public Dictionary<Buttons, Keys> ButtonToKeysTranslator
        {
            get;
            set;
        }
        public InputHandler(Game game)
            : base(game)
        {
            Dictionary<Buttons, Keys> translator = new Dictionary<Buttons, Keys>();
            translator.Add(Buttons.A, Keys.Space);
            translator.Add(Buttons.B, Keys.B);
            translator.Add(Buttons.X, Keys.X);
            translator.Add(Buttons.Y, Keys.Y);
            translator.Add(Buttons.Back, Keys.Escape);
            translator.Add(Buttons.Start, Keys.S);
            translator.Add(Buttons.RightShoulder, Keys.L);
            translator.Add(Buttons.LeftShoulder, Keys.R);
            translator.Add(Buttons.DPadUp, Keys.E);
            translator.Add(Buttons.DPadDown, Keys.D);            
            translator.Add(Buttons.DPadLeft, Keys.A);
            translator.Add(Buttons.DPadRight, Keys.D);
            translator.Add(Buttons.LeftThumbstickUp, Keys.Up);
            translator.Add(Buttons.LeftThumbstickDown, Keys.Down);            
            translator.Add(Buttons.LeftThumbstickRight, Keys.Right);
            translator.Add(Buttons.LeftThumbstickLeft, Keys.Left);
            translator.Add(Buttons.RightThumbstickUp, Keys.O);
            translator.Add(Buttons.RightThumbstickDown, Keys.L);            
            translator.Add(Buttons.LeftTrigger, Keys.T);
            translator.Add(Buttons.RightTrigger, Keys.G);
            
            ButtonToKeysTranslator = translator;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void AddEvent(Buttons button, EventHandler<ButtonFireEventArgs> handler)
        {
            if (!buttonHandlers.Keys.Contains(button))
            {
                buttonHandlers.Add(button, handler);
            }
            else
            {
                buttonHandlers[button] += handler;
            }
            AddEvent(ButtonToKeysTranslator[button], handler);
        }

        private void AddEvent(Keys button, EventHandler<ButtonFireEventArgs> handler)
        {
            if (!keyHandlers.Keys.Contains(button))
            {
                keyHandlers.Add(button, handler);
            }
            else
            {
                keyHandlers[button] += handler;
            }
        }


        public void RemoveEvent(Buttons button, EventHandler<ButtonFireEventArgs> handler)
        {
            buttonHandlers[button] -= handler;
            if (buttonHandlers[button] == null)
            {
                buttonHandlers.Remove(button);
            }
            RemoveEvent(ButtonToKeysTranslator[button], handler);
        }

        private void RemoveEvent(Keys button, EventHandler<ButtonFireEventArgs> handler)
        {
            keyHandlers[button] -= handler;
            if (keyHandlers[button] == null)
            {
                keyHandlers.Remove(button);
            }
        }


        private void CheckGamePad(GameTime gameTime, GamePadState state)
        {
            foreach (var buttonEvent in buttonHandlers)
            {
                if (state.IsButtonDown(buttonEvent.Key))
                {
                    buttonEvent.Value(this, new ButtonFireEventArgs(gameTime, previousGamePadState, previousGamePadState.IsButtonDown(buttonEvent.Key)));
                }
                else
                {
                    if (previousGamePadState.IsButtonDown(buttonEvent.Key))
                    {
                        buttonEvent.Value(this, new ButtonFireEventArgs(gameTime, previousGamePadState, true));
                    }
                }
            }          
        }

        private void CheckKeyboard(GameTime gameTime, KeyboardState state)
        {
            foreach (var keyEvent in keyHandlers)
            {
                if (state.IsKeyDown(keyEvent.Key))
                {
                    keyEvent.Value(this, new ButtonFireEventArgs(gameTime, previousKeyboardState, previousKeyboardState.IsKeyDown(keyEvent.Key)));
                }
                else
                {
                    if (previousKeyboardState.IsKeyDown(keyEvent.Key))
                    {
                        keyEvent.Value(this, new ButtonFireEventArgs(gameTime, previousKeyboardState, true));
                    }
                }
            }            
        }

        private void UpdateGamePad(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(Shorewood.mainPlayer);
            if (state.IsConnected)
            {
                CheckGamePad(gameTime, state);
            }
            else if (previousGamePadState.IsConnected)
            {
                ControllerDisconnectedEventArgs e = new ControllerDisconnectedEventArgs();
                e.PlayerIndex = Shorewood.mainPlayer;
                ControllerDisconnect(this, e);
            }
            previousGamePadState = state;
        }

        private void UpdateKeyboard(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            CheckKeyboard(gameTime, state);
            previousKeyboardState = state;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
#if XBOX
            UpdateGamePad(gameTime);
#else
            UpdateKeyboard(gameTime);      
#endif
            base.Update(gameTime);
        }
    }
}