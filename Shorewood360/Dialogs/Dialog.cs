using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IETGames.Shorewood.Utilities;
using IETGames.Shorewood.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace IETGames.Shorewood
{
    public enum DialogState {Active, Inactive, FadingIn, FadingOut }
    public enum DialogResult { A, B, X, Y, LeftBumper, RightBumper, LeftTrigger, RightTrigger, Back, Start, Nothing };

    public struct DialogOption
    {
        public StringBuilder optionText;
        public Texture2D textureRepresentation;
        public Vector2 position;
        public Vector2 origin;
        public Vector2 bounds;
        public bool isStatic;
        public bool isText;
        public EventHandler optionSelected;
        
        public DialogOption(StringBuilder optionText)
        {
            isText = true;
            this.optionText = optionText;            
            position = Vector2.Zero;
            origin = Vector2.Zero;
            isStatic = false;            
            textureRepresentation = null;
            optionSelected = null;
            bounds = Vector2.Zero;

        }

        public DialogOption(Texture2D texture)
        {
            isText = false;
            this.textureRepresentation = texture;
            this.optionText = new StringBuilder(60);
            position = Vector2.Zero;
            origin = Vector2.Zero;
            isStatic = false;
            bounds = Vector2.Zero;
            optionSelected = null;
        }

        public override bool Equals(object obj)
        {
            DialogOption option = (DialogOption)obj;
            return option.optionText == optionText;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Dialog
    {
        public static Rectangle ScaleRectangle(Rectangle rectangle, float scale, Vector2 center)
        {
            return new Rectangle((int)center.X - (int)((rectangle.Width / 2) * scale), (int)center.Y - (int)((rectangle.Height / 2) * scale), (int)(rectangle.Width * scale), (int)(rectangle.Height * scale));
        }
        public bool blur = false;
        public Texture2D dialogBackground;
        public Texture2D dialogHighlight;
        public Rectangle bounds;
        //public Rectangle scaledBounds;
        public Vector2 center;
        public Color color = Color.White;
        public bool wrapText = false;
        public int selectedItem = -1;
        public DialogState state = DialogState.Inactive;
        public float fadeDuration = 500;
        public float fadeStartTime = 0;
        public SpriteFont titleFont = Shorewood.fonts[FontTypes.MenuFont];
        public SpriteFont optionFont = Shorewood.fonts[FontTypes.MenuFont];
        public SpriteFont selectedFont = Shorewood.fonts[FontTypes.MenuFont];
        public SpriteFont staticFont = Shorewood.fonts[FontTypes.MenuFont];
        public SpriteFont font1337 = Shorewood.fonts[FontTypes.Font1337];
        public float currentScale = 0;
        public Vector2 size;
        public DialogType parentDialogType;
        protected bool isAllStatic = true;
        protected EventHandler<ButtonFireEventArgs> y;
        protected bool isModal = false;
        protected float textSpacing = 0;       
        protected StringBuilder dialogTitle = new StringBuilder(200);
        protected Color titleColor = Color.Black;
        protected Color textColor = Color.Black;
        protected Color staticTextColor = Color.DarkGray;
        protected Color selectedTextColor = Color.Black;
        protected float bounceScaleMax = 1.05f;
        protected float bounceScaleMin = 0.7f;
        protected float bounceSpeed = 500;
        protected KeyboardState previousKeyboardState;
        protected KeyboardState keyboardState;
        protected GamePadState gamePadState;
        protected GamePadState previousGamePadState;
        protected bool ignore1337 = true;
        protected EventHandler<ButtonFireEventArgs> onMoveUpHandler;
        protected EventHandler<ButtonFireEventArgs> onMoveDownHandler;
        protected EventHandler<ButtonFireEventArgs> onSelectHandler;
        protected bool AllowPurchase
        {
            get;
            set;
        }

        private DialogType transitionToType = DialogType.None;
        private List<DialogOption> dialogOptions = new List<DialogOption>(20);
        private Vector2 titlePosition;
        //private Vector2 titleScaledPosition;
        private Vector2 titleOrigin;
        private Vector2 nagOrigin;
        private Vector2 nagPosition;
        private int selectedOption = 0;
        private bool bounceExpanding = true;
        
        private float bounceStart = 0;
        private float selectedScale = 1;
        private bool selectionChanged = true;
        private DialogResult dialogResult = DialogResult.Nothing;
        private Matrix rotationMatrix = Matrix.Identity;

        public Dialog(StringBuilder dialogTitle, Vector2 size)
        {
            this.size = size;
            Initalize(dialogTitle, null, null);
            y = new EventHandler<ButtonFireEventArgs>(OnY);
            AllowPurchase = true;
        }

        public Dialog(StringBuilder dialogTitle, string dialogBackgroundContentLocation, string dialogHighlightContentLocation)
        {
            Texture2D backgroundTexture = dialogBackgroundContentLocation != "" ? Shorewood.Content.Load<Texture2D>(dialogBackgroundContentLocation) : null;
            Texture2D foregroundTexture = dialogHighlightContentLocation != "" ? Shorewood.Content.Load<Texture2D>(dialogHighlightContentLocation) : null;
            Initalize(dialogTitle, backgroundTexture, foregroundTexture);
            y = new EventHandler<ButtonFireEventArgs>(OnY);
            AllowPurchase = true;
        }


        public Dialog(StringBuilder dialogTitle, Texture2D dialogBackground, Texture2D dialogHighlight)
        {
            Initalize(dialogTitle, dialogBackground, dialogHighlight);
        }

        #region Public Members
        public virtual void Scale(float scale)
        {            
            currentScale = scale;            
            selectedScale = scale;            
            color.A = (byte)(scale * byte.MaxValue);
            titleColor.A = (byte)(scale * byte.MaxValue);
            textColor.A = (byte)(scale * byte.MaxValue);            
        }


        public virtual void Activate(GameTime gameTime)
        {
            fadeStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            state = DialogState.FadingIn;
            selectionChanged = true;
            nagOrigin = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PuchaseNag]) / 2;
            nagPosition = new Vector2(bounds.Center.X - nagOrigin.X, bounds.Bottom + 10);
            //Shorewood.inputHandler.AddEvent(Buttons.Y, OnY);
        }

        public virtual void Deactivate(GameTime gameTime)
        {
            fadeStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            state = DialogState.FadingOut;
            
        }

        public virtual void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(Shorewood.mainPlayer);
            switch (state)
            {
                case DialogState.FadingIn:
                    UpdateFadeIn(gameTime);
                    break;
                case DialogState.FadingOut:
                    UpdateFadeOut(gameTime);
                    break;
                case DialogState.Active:                    
                    if (!isModal)
                    {
                        UpdateSelectedOption(gameTime);
                    }
                    break;
                case DialogState.Inactive:
                    break;
            }
            previousGamePadState = gamePadState;
            previousKeyboardState = keyboardState;
        }

        public virtual void CustomDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public virtual void DrawTextOption(SpriteBatch spriteBatch, GameTime gameTime, int i)
        {
            if ((i == SelectedOption) && (!isModal) && !isAllStatic)
            {
                spriteBatch.DrawString(selectedFont, dialogOptions[i].optionText, dialogOptions[i].position, selectedTextColor, 0, dialogOptions[i].origin, selectedScale, SpriteEffects.None, 1);
            }
            else
            {
                if (dialogOptions[i].isStatic)
                {
                     spriteBatch.DrawString(staticFont, dialogOptions[i].optionText, dialogOptions[i].position, staticTextColor, 0, dialogOptions[i].origin, 1, SpriteEffects.None, 1);
                }
                else
                {
                    spriteBatch.DrawString(optionFont, dialogOptions[i].optionText, dialogOptions[i].position, textColor, 0, dialogOptions[i].origin, 1, SpriteEffects.None, 1);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (dialogBackground != null)
            {
                spriteBatch.Draw(dialogBackground, bounds, color);
            }
            
            for (int i = 0; i < dialogOptions.Count; i++)
            {
                if (dialogOptions[i].isText)
                {
                    DrawTextOption(spriteBatch, gameTime, i);
                }
            }
            CustomDraw(spriteBatch, gameTime);
            if (dialogHighlight != null)
            {
                spriteBatch.Draw(dialogHighlight, bounds, color);
            }
            if (dialogTitle != null)
            {
                spriteBatch.DrawString(titleFont, dialogTitle, titlePosition, titleColor, 0, titleOrigin, 1, SpriteEffects.None, 1);
            }
            if (AllowPurchase)
            {
                DrawNag(spriteBatch);
            }
        }

        public virtual void DrawNag(SpriteBatch spriteBatch)
        {
            if (Shorewood.IsTrial && !Shorewood.popUpManager.IsActive)
            {
                spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont],
                    Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PuchaseNag],
                    nagPosition, Color.White);
            }
        }

        public virtual void Reset()
        {
            state = DialogState.Inactive;
            dialogResult = DialogResult.Nothing;
            bounceExpanding = true;
            selectionChanged = true;
            selectedScale = 1;
            SelectedOption = 0;
        }

        public virtual int SelectedOption
        {
            get
            {
                return selectedOption;
            }
            set
            {
                if (isAllStatic)
                {
                    selectedOption = 0;
                }
                int oldOption = selectedOption;                
                if (value > selectedOption)
                {
                    if (LookForward(value, out selectedOption))
                    {
                        selectionChanged = true;
                        if (state != DialogState.Inactive)
                        {
                            Shorewood.tick.Play();
                        }
                    }
                    else
                    {
                        selectedOption = oldOption;
                    }
                }
                else if (value == selectedOption)
                {
                    return;
                }
                else
                {
                    if (LookReverse(value, out selectedOption))
                    {
                        selectionChanged = true;
                        if (state != DialogState.Inactive)
                        {
                            Shorewood.tick.Play();
                        }
                    }
                    else
                    {
                        selectedOption = oldOption;
                    }
                }
            }
        }

        public virtual DialogResult DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value;
            }
        }

        public virtual bool IsActive
        {
            get
            {
                return state != DialogState.Inactive;
            }
        }
        #endregion 

        #region Private Members
        private void Initalize(StringBuilder dialogTitle, Texture2D dialogBackground, Texture2D dialogHighlight)
        {
            this.onMoveDownHandler = new EventHandler<ButtonFireEventArgs>(OnMoveDown);
            this.onMoveUpHandler = new EventHandler<ButtonFireEventArgs>(OnMoveUp);
            this.onSelectHandler = new EventHandler<ButtonFireEventArgs>(OnSelect);
            this.dialogBackground = dialogBackground;
            this.dialogHighlight = dialogHighlight;
            center = new Vector2(Shorewood.titleSafeArea.Center.X, Shorewood.titleSafeArea.Center.Y);
            if (dialogBackground != null)
            {
                bounds = new Rectangle((int)center.X - dialogBackground.Width / 2, (int)center.Y - dialogBackground.Height / 2, dialogBackground.Width, dialogBackground.Height);
            }
            else
            {
                bounds = new Rectangle((int)center.X - (int)size.X / 2, (int)center.Y - (int)size.Y / 2, (int)size.X, (int)size.Y);
            }
            if (dialogTitle != null)
            {
                this.dialogTitle.Append(dialogTitle);
                Vector2 titleBounds = Shorewood.fonts[FontTypes.MenuFont].MeasureString(dialogTitle);
                titleOrigin = titleBounds / 2;
                titlePosition = new Vector2(center.X, bounds.Y + titleOrigin.Y);
            }
            Scale(0);
            previousGamePadState = GamePad.GetState(Shorewood.mainPlayer);
            previousKeyboardState = Keyboard.GetState();
        }

        private void UpdateOptionPositions()
        {            
            float[] heights = new float[dialogOptions.Count];
            float totalOptionsHeight = 0, currentOptionHeight = 0;
            
            for (int i = 0; i < dialogOptions.Count; i++)
            {
                float height = dialogOptions[i].bounds.Y;
                if (i < dialogOptions.Count - 1)
                {
                    height =  height + textSpacing;
                    totalOptionsHeight += height;
                }
                heights[i] = height;
            }            
            totalOptionsHeight = (center.Y - totalOptionsHeight / 2);
            
            for (int i = 0; i < dialogOptions.Count; i++)
            {
                Vector2 position = new Vector2(center.X, 
                    totalOptionsHeight + currentOptionHeight);
                DialogOption option = dialogOptions[i];
                option.position = position;                
                dialogOptions[i] = option;
                currentOptionHeight += heights[i];
            }
        }

        private bool LookReverse(int start, out int nonStaticOption)
        {
            for (int i = start; i >= 0; i--)
            {
                if (!dialogOptions[i].isStatic)
                {
                    nonStaticOption = i;
                    return true;
                }
            }
            nonStaticOption = start;
            return false;
        }

        private bool LookForward(int start, out int nonStaticOption)
        {
            for (int i = start; i < dialogOptions.Count; i++)
            {
                if (!dialogOptions[i].isStatic)
                {
                    nonStaticOption = i;
                    return true;
                }
            }
            nonStaticOption = start;
            return false;
        }
        #endregion

        #region Protected Members
        protected void AddOption(StringBuilder option)
        {
            AddOption(option, false);
        }

        protected void AddOption(StringBuilder option, bool isStatic)
        {
            AddOption(option, isStatic, 0);
        }

        protected void AddOption(StringBuilder option, bool isStatic, int pageNumber)
        {
            DialogOption newOption = new DialogOption(option);
            if (wrapText)
            {
                StringBuilder wrappedOption = new StringBuilder();
                WordWrapper.WrapWord(option, wrappedOption, optionFont, bounds, 1);
                newOption.optionText = wrappedOption;
            }
            if (isStatic)
            {
                Vector2 optionBounds = staticFont.MeasureString(newOption.optionText);

                newOption.origin = optionBounds / 2;
                newOption.bounds = optionBounds;
            }
            else
            {
                Vector2 optionBounds = optionFont.MeasureString(newOption.optionText);

                newOption.origin = optionBounds / 2;
                newOption.bounds = optionBounds;

            }
            isAllStatic &= isStatic;
            newOption.isStatic = isStatic;
            dialogOptions.Add(newOption);
            UpdateOptionPositions();
        }

        protected void AddOption(DialogOption option)
        {
            dialogOptions.Add(option);
            UpdateOptionPositions();
        }

        protected void RemoveOption(StringBuilder option)
        {            
            dialogOptions.Remove(new DialogOption(option));
        }

        protected void RemoveAllOptions()
        {
            dialogOptions.Clear();
        }

        protected virtual void UpdateSelectedOption(GameTime gameTime)
        {
            if (isAllStatic)
            {
                return;
            }
            if (selectionChanged)
            {
                selectionChanged = false;
                bounceStart = (float)gameTime.TotalGameTime.TotalMilliseconds;
                selectedScale = 1;
                bounceExpanding = false;
                
                return;
            }

            if (bounceExpanding)
            {
                selectedScale = MathHelper.SmoothStep(bounceScaleMin, bounceScaleMax, ((float)gameTime.TotalGameTime.TotalMilliseconds - bounceStart) / bounceSpeed);
            }
            else
            {
                selectedScale = MathHelper.SmoothStep(bounceScaleMax, bounceScaleMin, ((float)gameTime.TotalGameTime.TotalMilliseconds - bounceStart) / bounceSpeed);
            }
            selectedScale = MathHelper.Clamp(selectedScale, bounceScaleMin, bounceScaleMax);
            if (selectedScale == bounceScaleMax)
            {
                bounceExpanding = false;
                bounceStart = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
            else if (selectedScale == bounceScaleMin)
            {
                bounceExpanding = true;
                bounceStart = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        protected virtual void UpdateFadeIn(GameTime gameTime)
        {
            float scale = MathHelper.SmoothStep(0, 1, ((float)gameTime.TotalGameTime.TotalMilliseconds - fadeStartTime) / fadeDuration);
            MathHelper.Clamp(scale, 0, 1);
            Scale(scale);
            if (scale == 1)
            {
                state = DialogState.Active;
                OnActivated(gameTime);
            }
        }

        protected virtual void UpdateFadeOut(GameTime gameTime)
        {
            float scale = MathHelper.SmoothStep(1, 0, ((float)gameTime.TotalGameTime.TotalMilliseconds - fadeStartTime) / fadeDuration);
            MathHelper.Clamp(scale, 0, 1);
            Scale(scale);
            if (scale == 0)
            {
                state = DialogState.Inactive;
                Reset();
                OnDeactivated(gameTime);
            }
        }

        protected virtual void OnActivated(GameTime gameTime)
        {
            Shorewood.inputHandler.AddEvent(Buttons.DPadUp, onMoveUpHandler );
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickUp, onMoveUpHandler);
            Shorewood.inputHandler.AddEvent(Buttons.RightThumbstickUp, onMoveUpHandler);
            Shorewood.inputHandler.AddEvent(Buttons.DPadDown, onMoveDownHandler);
            Shorewood.inputHandler.AddEvent(Buttons.LeftThumbstickDown, onMoveDownHandler);
            Shorewood.inputHandler.AddEvent(Buttons.RightThumbstickDown, onMoveDownHandler);
            Shorewood.inputHandler.AddEvent(Buttons.A, onSelectHandler);
            Shorewood.inputHandler.AddEvent(Buttons.Y, OnY);
            Shorewood.inputHandler.AddEvent(Buttons.B, OnB);
            Shorewood.inputHandler.AddEvent(Buttons.X, OnX);
        }

        protected virtual void OnDeactivated(GameTime gameTime)
        {
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadUp, onMoveUpHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickUp, onMoveUpHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.RightThumbstickUp, onMoveUpHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.DPadDown, onMoveDownHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.LeftThumbstickDown, onMoveDownHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.RightThumbstickDown, onMoveDownHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.A, onSelectHandler);
            Shorewood.inputHandler.RemoveEvent(Buttons.Y, OnY);
            Shorewood.inputHandler.RemoveEvent(Buttons.B, OnB);
            Shorewood.inputHandler.RemoveEvent(Buttons.X, OnX);

            if (transitionToType != DialogType.None)
            {
                Shorewood.dialogManager.ShowDialog(transitionToType, gameTime);
                transitionToType = DialogType.None;
            }
        }

        protected virtual void MoveDown()
        {
            SelectedOption++;
        }

        protected virtual void MoveUp()
        {
            SelectedOption--;
        }

        protected virtual void OnMoveUp(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                if (!a.previouslyDown)
                {
                    MoveUp();
                }
            }
        }

        protected virtual void OnMoveDown(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                if (!a.previouslyDown)
                {
                    MoveDown();
                }
            }
        }

        protected virtual void OnSelect(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                if (!a.previouslyDown)
                {
                    DialogResult = DialogResult.A;
                }
            }
        }

        public virtual void TransitionTo(DialogType dialogType, GameTime gameTime)
        {
            //parentDialogType = Shorewood.dialogManager.ActiveDialogType;
            Shorewood.dialogManager.CloseDialog(gameTime);
            transitionToType = dialogType;
        }

        public virtual void OnY(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                if (Shorewood.IsTrial && AllowPurchase )
                {
                    Shorewood.popUpManager.ShowDialog(PopUpType.Purchase, a.gameTime, null);
                }
            }
        }
        public virtual void OnB(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                
            }
        }
        public virtual void OnX(object sender, ButtonFireEventArgs a)
        {
            if (!Shorewood.popUpManager.IsActive)
            {
                
            }
        }


        #endregion
    }
}