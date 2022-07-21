using Playnite.SDK.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;

namespace TitleNormaliser
{
    public static class TitleNormaliserHelper
    {
        private static readonly String pattern = @"(?i)\b(?=[ivxlcdm]+)M{0,4}(?:CM|CD|D?C{0,3})(?:XC|XL|L?X{0,3})(?:IX|IV|V?I{0,3})\b";

        private static ArrayList lowerCaseWords = new ArrayList();

        private static ArrayList capitalCaseWords = new ArrayList();

        private static ArrayList ignoredWords = new ArrayList();



        public static void NormaliseTitle(IItemCollection<Game> games, TitleNormaliserSettings savedSettings)
        {
            String[] ignoredArray = savedSettings.Option1.Split();
            String[] capataliseArray = savedSettings.Option2.Split();

            PopulateIgnoredWordsList(ignoredArray);
            PopulateLowerCaseWordsList();
            PopulateCapitalWordsList(capataliseArray);

            foreach (Game x in games)
            {
                if (x.Name != null)
                {
                    String[] words = x.Name.Split();
                    StringBuilder sb = new StringBuilder();


                    foreach (String word in words)
                    {

                        String lowerWord = word.ToLower();
                        String capitalWord = word.ToUpper();

                        if (!ignoredWords.Contains(lowerWord) && lowerCaseWords.Contains(lowerWord))
                        {
                            sb.Append(lowerWord);
                        }
                        
                        else if (!ignoredWords.Contains(lowerWord) && capitalCaseWords.Contains(lowerWord))
                        {
                            sb.Append(capitalWord);
                        }

                        else if (!ignoredWords.Contains(lowerWord))
                        {
                            sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerWord));
                        }
                        else
                        {
                            sb.Append(word);
                        }
                        sb.Append(" ");
                    }

                    sb.Remove(sb.Length - 1, 1);

                    if (Regex.IsMatch(sb.ToString(), pattern))
                    {
                        sb.Replace(sb.ToString(), Regex.Replace(sb.ToString(), pattern, c => c.ToString().ToUpper()));
                    }
                    x.Name = sb.ToString();
                    games.Update(x);
                }
            }
        }

        private static void PopulateLowerCaseWordsList()
        {
            lowerCaseWords.Clear();


            lowerCaseWords.Add("and");
            lowerCaseWords.Add("of");
            lowerCaseWords.Add("an");
            lowerCaseWords.Add("a");
            lowerCaseWords.Add("the");
            lowerCaseWords.Add("but");
            lowerCaseWords.Add("or");
            lowerCaseWords.Add("nor");
            lowerCaseWords.Add("for");
            lowerCaseWords.Add("yet");
            lowerCaseWords.Add("as");
            lowerCaseWords.Add("in");
            lowerCaseWords.Add("on");
            lowerCaseWords.Add("to");
            lowerCaseWords.Add("for");
            lowerCaseWords.Add("from");
            lowerCaseWords.Add("into");
            lowerCaseWords.Add("like");
            lowerCaseWords.Add("over");
            lowerCaseWords.Add("with");
            lowerCaseWords.Add("upon");
        }

        private static void PopulateCapitalWordsList(String[] capatalisedArray)
        {
            capitalCaseWords.Clear();

            capitalCaseWords.Add("hd");

            foreach (String word in capatalisedArray)
            {
                capitalCaseWords.Add(word.ToLower());
            }
        }

        private static void PopulateIgnoredWordsList(String[] ignoredArray)
        {
            ignoredWords.Clear();

            foreach (String word in ignoredArray)
            {
                ignoredWords.Add(word.ToLower());
            }
        }
    }

    
}
