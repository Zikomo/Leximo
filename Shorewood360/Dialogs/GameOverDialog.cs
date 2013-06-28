using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public class GameOverDialog:Dialog
    {
        public GameOverDialog()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.GameOverDialogTitle],
                @"screens\menus\largeDialog",
                @"screens\menus\largeDialogHighlight")
        {
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.GameOverDialogTryAgain]);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.GameOverDialogReturnToMainMenu]);            
        }
    }
}