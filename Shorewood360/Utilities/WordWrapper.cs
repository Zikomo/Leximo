using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace IETGames.Shorewood.Utilities
{
    public static class WordWrapper
    {
        static StringBuilder builder = new StringBuilder(" ");        
        public static char[] NewLine = { '\r', '\n' };        
        static Vector2 MeasureCharacter(this SpriteFont font, char character)
        {
            builder[0] = character;
            return font.MeasureString(builder);
        }
        
        public static void WrapWord(StringBuilder original, StringBuilder target, SpriteFont font, Rectangle bounds, float scale)
        {
            int lastWhiteSpace = 0;
            float currentLength = 0;
            float lengthSinceLastWhiteSpace = 0;
            float characterWidth = 0;
            for (int i = 0; i < original.Length; i++)
            {
                //get the character
                char character = original[i];
                //measure the length of the current line
                characterWidth = font.MeasureCharacter(character).X * scale;
                currentLength += characterWidth;
                //find the length since last white space
                lengthSinceLastWhiteSpace += characterWidth;
                //are we at a new line?
                if ((character != '\r') && (character != '\n'))
                {
                    //time for a new line?
                    if (currentLength > bounds.Width)
                    {
                        //if so are we at white space?
                        if (char.IsWhiteSpace(character))
                        {
                            //if so insert newline here
                            target.Insert(i, NewLine);
                            //reset lengths
                            currentLength = 0;
                            lengthSinceLastWhiteSpace = 0;
                            // return to the top of the loop as to not append white space
                            continue;
                        }
                        else
                        {
                            //not at white space so we insert a new line at the previous recorded white space
                            target.Insert(lastWhiteSpace, NewLine);
                            //remove the white space
                            target.Remove(lastWhiteSpace + NewLine.Length, 1);
                            //make sure the the characters at the line break are accounted for
                            currentLength = lengthSinceLastWhiteSpace;
                            lengthSinceLastWhiteSpace = 0;
                        }
                    }
                    else
                    {
                        //not time for a line break? are we at white space?
                        if (char.IsWhiteSpace(character))
                        {
                            //record it's location
                            lastWhiteSpace = target.Length;
                            lengthSinceLastWhiteSpace = 0;
                        }
                    }
                }
                else
                {
                    lengthSinceLastWhiteSpace = 0;
                    currentLength = 0;
                }
                //always append 
                target.Append(character);
            }
        }
        
        public static void WrapWordOld(StringBuilder original, StringBuilder target, SpriteFont font, Rectangle bounds)
        {
            int lastWhiteSpace = 0;
            Vector2 currentTargetSize;
            for (int i = 0; i < original.Length; i++)
            {
                char character = original[i];
                if (char.IsWhiteSpace(character))
                {
                    lastWhiteSpace = target.Length;
                }
                target.Append(character);
                currentTargetSize = font.MeasureString(target);
                if (currentTargetSize.X > bounds.Width)
                {
                    target.Insert(lastWhiteSpace, NewLine);
                    target.Remove(lastWhiteSpace + NewLine.Length, 1);
                }
            }
        } 
    }
}