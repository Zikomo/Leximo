using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public class OptionsDialog:Dialog
    {
        public OptionsDialog()
            :base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuCredits],
            @"screens\menus\baseMainDialog",
            @"screens\menus\baseMainDialogHighlight")
        {
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.SettingsMusicVolume], false);
        }

        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {

            base.OnSelect(sender, a);
        }
    }
}