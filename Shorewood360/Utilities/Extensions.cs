using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood.Utilities
{
    public static class StringBuilderExtension
    {
        public static void Copy(this StringBuilder destination, StringBuilder source)
        {
            destination.Remove(0, destination.Length);            
            destination.EnsureCapacity(source.Length);
            for (int i = 0; i < source.Length; i++) 
            {
                destination.Append(source[i]);
            }
        }

        public static bool ZEquals(this StringBuilder builderOne, StringBuilder builderTwo)
        {
            bool rtnValue = true;
            if (builderOne.Length == builderTwo.Length)
            {
                for (int i = 0; i < builderOne.Length; i++)
                {
                    rtnValue &= builderOne[i] == builderTwo[i];
                }
            }
            else
            {
                rtnValue = false;
            }
            return rtnValue;
           
        }

        public static void Decorate(this StringBuilder builder)
        {
            char[] microsoftSucks = { '\n' };
            builder.Insert(0, microsoftSucks);
            builder.Append('\r');
        }
    }
}