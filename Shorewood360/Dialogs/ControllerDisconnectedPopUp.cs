using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood
{
    class ControllerDisconnectedPopUp:PopUpDialog
    {
        public ControllerDisconnectedPopUp()
            : base(null, @"screens\menus\largePopupBackground", "")
        {
            staticTextColor = Color.White;
            staticFont = Shorewood.fonts[FontTypes.MenuButtonFont];
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogController], true); 
        }

        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                Close(a.gameTime);
            }
            base.OnSelect(sender, a);
        }
    }
}