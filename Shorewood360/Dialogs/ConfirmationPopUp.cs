using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood
{
    

    public class ConfirmationPopUp:PopUpDialog
    {

        public ConfirmationPopUp()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogConfirmation])
        {
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogNo]);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogYes]);
            
            titleColor = Color.White;
            textColor = Color.White;
            selectedTextColor = Color.White;
        }

        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                DialogResult = DialogResult.A;
                Close(a.gameTime);
            }
            base.OnSelect(sender, a);
        }

        public override void OnB(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                DialogResult = DialogResult.B;
                Close(a.gameTime);
            }
            base.OnB(sender, a);
        }
    }
}