using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood
{
    public class CannontPurchaseWarning:PopUpDialog 
    {
        public CannontPurchaseWarning()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogPurchaseWarnTitle], @"screens\menus\largePopupBackground", "")
        {
            staticTextColor = Color.White;
            titleColor = Color.White;
            staticFont = Shorewood.fonts[FontTypes.MenuButtonFont];
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogPurchaseWarn0], true);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogPurchaseWarn1], true);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogBack], true);
        }

        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                Close(a.gameTime);
            }
            base.OnSelect(sender, a);
        }

        public override void OnB(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                Close(a.gameTime);
            }
            base.OnB(sender, a);
        }
    }
}