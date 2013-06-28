using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IETGames.Shorewood;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood.Localization
{
    class UsEnglishLocalization
    {
        public static Dictionary<GameStrings, StringBuilder> StringTable
        {
            get
            {
                Dictionary<GameStrings, StringBuilder> strings = new Dictionary<GameStrings, StringBuilder>();
#if XBOX
                strings.Add(GameStrings.MenuPlay, new StringBuilder("Start Game"));
#else
                strings.Add(GameStrings.MenuPlay, new StringBuilder("Start Game"));
#endif
                strings.Add(GameStrings.MenuTutorial, new StringBuilder("Press & for a quick tutorial!"));
                strings.Add(GameStrings.MenuHighScores, new StringBuilder("High Scores"));
                strings.Add(GameStrings.MenuQuit, new StringBuilder("Return To Game Library"));
                strings.Add(GameStrings.MenuPlayerSelect, new StringBuilder("Press [Start] to Continue"));
                strings.Add(GameStrings.MenuCredits, new StringBuilder("Credits"));
                strings.Add(GameStrings.MenuSwitch, new StringBuilder("Switch Controller or Profile"));
                strings.Add(GameStrings.GoalCount, new StringBuilder("Spell <X> words with at least <Y> letters"));
                strings.Add(GameStrings.GoalAnyBackwards, new StringBuilder("Spell <X> word(s) backwards"));
                strings.Add(GameStrings.GoalSpecificLengthBackwards, new StringBuilder("Spell <X> word(s) backwards with at least <Y> letters"));
                strings.Add(GameStrings.GoalPalindrome, new StringBuilder("Spell a palindrome of at least <Y> letters"));
                strings.Add(GameStrings.GoalSpecific, new StringBuilder("Spell the word <X>"));
                strings.Add(GameStrings.GoalPoints, new StringBuilder("Spell <X> word(s) that scores <Y> or more points"));
                strings.Add(GameStrings.GoalLetter, new StringBuilder("Spell a word using the letter <X>"));
                strings.Add(GameStrings.GoalStatusPrefix, new StringBuilder("Goal "));
                strings.Add(GameStrings.GoalStatusPostfix, new StringBuilder("% completed"));
                strings.Add(GameStrings.GoalContainsWord, new StringBuilder ("Spell a word that contains the word <X>"));
                strings.Add(GameStrings.GoalSurvival, new StringBuilder("Survive for <X> minutes"));
                strings.Add(GameStrings.Goal1337, new StringBuilder("Spell <X> words with at least <Y> letters in l33t!"));
                strings.Add(GameStrings.MainMenuDialogTitle, new StringBuilder("Main Menu"));
                strings.Add(GameStrings.GameOverDialogTitle, new StringBuilder("Game Over"));
                strings.Add(GameStrings.GameOverDialogTryAgain, new StringBuilder("Try Again?"));
                strings.Add(GameStrings.GameOverDialogReturnToMainMenu, new StringBuilder("Return to Main Menu"));
                strings.Add(GameStrings.GoalNew, new StringBuilder("New Challenge!"));
                strings.Add(GameStrings.DialogCancel, new StringBuilder(") Cancel"));
                strings.Add(GameStrings.DialogOk, new StringBuilder("' Ok"));
                strings.Add(GameStrings.DialogBack, new StringBuilder(") Back"));
                strings.Add(GameStrings.MenuSelect, new StringBuilder("' Select"));  
                strings.Add(GameStrings.DialogReady, new StringBuilder("' Ready?!"));
                strings.Add(GameStrings.CurrentScore, new StringBuilder("Score: "));
                strings.Add(GameStrings.PauseContinue, new StringBuilder("Continue Playing"));
                strings.Add(GameStrings.PauseQuit, new StringBuilder("Return to Main Menu"));
                strings.Add(GameStrings.PauseTitle, new StringBuilder("Paused"));
                strings.Add(GameStrings.PauseRestart, new StringBuilder("Restart"));
                strings.Add(GameStrings.TutorialTitle, new StringBuilder("Pro Tips"));
                strings.Add(GameStrings.DifficultyEasy, new StringBuilder("Free Spell"));
                strings.Add(GameStrings.DifficultyMedium, new StringBuilder("Easy"));
                strings.Add(GameStrings.DifficultyHard, new StringBuilder("Casual"));
                strings.Add(GameStrings.DifficultyLOL, new StringBuilder("Hard"));
                strings.Add(GameStrings.DifficultyWTF, new StringBuilder("1337"));
                strings.Add(GameStrings.MenuNext, new StringBuilder("Next"));
                strings.Add(GameStrings.MenuPrevious, new StringBuilder("Previous"));
                strings.Add(GameStrings.Difficulties, new StringBuilder("How Hard?"));
                strings.Add(GameStrings.DialogYes, new StringBuilder("Yes"));
                strings.Add(GameStrings.DialogNo, new StringBuilder("No"));
                strings.Add(GameStrings.DialogConfirmation, new StringBuilder("Are you sure?"));
                strings.Add(GameStrings.DialogController, new StringBuilder("Press ' to continue!"));
                strings.Add(GameStrings.SettingsMusicVolume, new StringBuilder("Music Volume"));
                strings.Add(GameStrings.DialogPurchaseConfirmation, new StringBuilder("Go to Marketplace?"));
                strings.Add(GameStrings.DialogPurchaseWarnTitle, new StringBuilder("Doh!"));
                strings.Add(GameStrings.DialogPurchaseWarn0, new StringBuilder("This profile is not allowed to "));
                strings.Add(GameStrings.DialogPurchaseWarn1, new StringBuilder("purchase LIVE Marketplace content."));
                strings.Add(GameStrings.PuchaseNag, new StringBuilder("( To Purchase Now!"));
                strings.Add(GameStrings.NextChallenge, new StringBuilder("Challenge in "));
                strings.Add(GameStrings.HighScoresDisabledInTrial, new StringBuilder("High scores are disabled in trial mode!"));
                return strings;
            }
        }

        public static Language Language
        {
            get
            {
                return Language.USEnglish;
            }
        }

        public static Dictionary<char, Letter> Alphabet
        {
            get
            {
                Dictionary<char, Letter> alphabet = new Dictionary<char, Letter>();
                alphabet.Add('a', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\A"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseA1337"), 'a'));
                alphabet.Add('b', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\B"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseB1337"), 'b'));
                alphabet.Add('c', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\C"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseC1337"), 'c'));
                alphabet.Add('d', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\D"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseD1337"), 'd'));
                alphabet.Add('e', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\E"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseE1337"), 'e'));
                alphabet.Add('f', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\F"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseF1337"), 'f'));
                alphabet.Add('g', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\G"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseG1337"), 'g'));
                alphabet.Add('h', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\H"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseH1337"), 'h'));
                alphabet.Add('i', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\I"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseI1337"), 'i'));
                alphabet.Add('j', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\J"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseJ1337"), 'j'));
                alphabet.Add('k', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\K"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseK1337"), 'k'));
                alphabet.Add('l', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\L"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseL1337"), 'l'));
                alphabet.Add('m', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\M"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseM1337"), 'm'));
                alphabet.Add('n', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\N"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseN1337"), 'n'));
                alphabet.Add('o', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\O"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseO1337"), 'o'));
                alphabet.Add('p', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\P"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseP1337"), 'p'));
                alphabet.Add('q', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\Q"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseQ1337"), 'q'));
                alphabet.Add('r', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\R"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseR1337"), 'r'));
                alphabet.Add('s', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\S"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseS1337"), 's'));
                alphabet.Add('t', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\T"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseT1337"), 't'));
                alphabet.Add('u', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\U"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseU1337"), 'u'));
                alphabet.Add('v', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\V"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseV1337"), 'v'));
                alphabet.Add('w', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\W"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseW1337"), 'w'));
                alphabet.Add('x', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\X"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseX1337"), 'x'));
                alphabet.Add('y', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\Y"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseY1337"), 'y'));
                alphabet.Add('z', new Letter(Shorewood.Content.Load<Texture2D>("Sprites\\Z"), Shorewood.Content.Load<Texture2D>("Sprites\\1337\\baseZ1337"), 'z'));
                return alphabet;
            }
        }
    }
}