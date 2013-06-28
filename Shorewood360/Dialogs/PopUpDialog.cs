using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood
{
    public class PopUpResultEventArgs : EventArgs
    {
        public GameTime GameTime
        {
            get;
            set;
        }
        public DialogResult Result
        {
            get;
            set;
        }
        public int SelectedOption
        {
            get;
            set;
        }
        public PopUpResultEventArgs()
            :base()
        {
        }
    }
    public delegate void PopUpCallback(PopUpResultEventArgs result);
    public class PopUpDialog:Dialog 
    {
        private PopUpType transitionToPop = PopUpType.None;
        public PopUpCallback PopUpCallback
        {
            get;
            set;
        }

        public PopUpDialog(StringBuilder popupTitle)
            : base(popupTitle, new Vector2(300, 200))
        {
            dialogBackground = Shorewood.Content.Load<Texture2D>(@"screens\menus\popupBackground");
        }

        public PopUpDialog(StringBuilder popupTitle, Vector2 size)
            : base(popupTitle, size)
        {
            blur = false;
            fadeDuration = 500;
        }


        public PopUpDialog(StringBuilder popupTitle, Texture2D background, Texture2D foreground)
            : base(popupTitle, background, foreground)
        {
            blur = false;
            fadeDuration = 500;
        }

        public PopUpDialog(StringBuilder popupTitle, string background, string foreground)
            : base(popupTitle, background, foreground)
        {
            blur = false;
            fadeDuration = 500;
        }

        protected override void OnMoveDown(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                MoveDown();
            }
            base.OnMoveDown(sender, a);
        }

        protected override void OnMoveUp(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                MoveUp();
            }
            base.OnMoveUp(sender, a);
        }

        public void TransitionTo(PopUpType popUpType, GameTime gameTime)
        {
            Shorewood.popUpManager.CloseDialog(gameTime);
            transitionToPop = popUpType;
        }

        protected override void OnDeactivated(GameTime gameTime)
        {
            if (transitionToPop != PopUpType.None)
            {
                Shorewood.popUpManager.ShowDialog(transitionToPop, gameTime, null);
                transitionToPop = PopUpType.None;
            }
            base.OnDeactivated(gameTime);
        }
        
        public virtual void Close(GameTime gameTime)
        {
            PopUpResultEventArgs result = new PopUpResultEventArgs();
            result.GameTime = gameTime;
            result.Result = DialogResult;
            result.SelectedOption = SelectedOption;
            if (PopUpCallback != null)
            {
                PopUpCallback(result);
            }
            Shorewood.popUpManager.CloseDialog(gameTime);
        }
    }
}