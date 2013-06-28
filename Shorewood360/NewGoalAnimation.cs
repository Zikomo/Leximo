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
using System.Text;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NewGoalDialog : ModalDialog
    {
        public NewGoalDialog()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.GoalNew],
                @"screens\menus\largeDialog",
                @"screens\menus\largeDialogHighlight")
        {
            FooterRight = Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DialogReady];
            blur = true;
        }

        public void SetGoals(List<IGoal> goals)
        {
            this.RemoveAllOptions();
            StringBuilder text = new StringBuilder(256);
            foreach (var goal in goals)
            {
                text.AppendLine(goal.GoalText.ToString());
                //this.AddOption(goal.GoalText, true);
            }
            this.AddOption(text, true);
        }
    }
}