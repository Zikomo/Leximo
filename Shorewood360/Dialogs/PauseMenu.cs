using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public class PauseMenu:Dialog
    {       
        public PauseMenu()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PauseTitle],
                @"screens\menus\largeDialog",
                @"screens\menus\largeDialogHighlight")
        {
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PauseContinue]);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PauseRestart]);
            AddOption(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.PauseQuit]);
            blur = true;
        }
        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs e)
        {            
            if ((SelectedOption != 0)&& !e.previouslyDown)
            {
                if (!Shorewood.popUpManager.IsActive)
                {
                    Shorewood.popUpManager.ShowDialog(PopUpType.Confirmation, e.gameTime, PopUpResult);
                }
            }
            base.OnSelect(sender, e);
        }

        void PopUpResult(PopUpResultEventArgs e)
        {
            if (e.SelectedOption == 1)
            {                
                DialogResult = DialogResult.A;
            }
            else
            {
                DialogResult = DialogResult.Nothing;
            }
        }
    }
}