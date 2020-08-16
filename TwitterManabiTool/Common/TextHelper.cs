using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterManabiTool.Common
{
    public static class TextHelper
    {
        public static string LimitedChars(string text,int numOfChars) 
        {
            var temp = String.Empty;
            for (int i = 0; i < numOfChars; i++)
            {
                temp += " ";
            }

            string limitedtext = String.Empty;
            if (text.Length > numOfChars)
            {
                limitedtext = text.Substring(0, numOfChars - 1) + "…";
            }
            else 
            {
                limitedtext = (text + temp).Substring(0, numOfChars);
            }

            return limitedtext;
        }

        public static string MoveRight(string text, int numOfChars)
        {
            var temp = String.Empty;
            for (int i = 0; i < numOfChars; i++)
            {
                temp += " ";
            }
            string limitedtext = String.Empty;
            limitedtext = temp + text;

            return limitedtext.Substring(limitedtext.Length - numOfChars);
        }

        public static string MoveLeft(string text, int numOfChars)
        {
            var temp = String.Empty;
            for (int i = 0; i < numOfChars; i++)
            {
                temp += " ";
            }

            return (text + temp).Substring(0, numOfChars);
        }
    }
}
