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



        public static void NormaliseTitle(List<Game> games, TitleNormaliserSettings savedSettings)
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

                        //If defined by users ignore list or the extensions ignore list then append only the word
                        if (ignoredWords.Contains(lowerWord)) {
                            sb.Append(word);
                        }
                        //If it's the first element in the string then make it Title case (i.e. The Lord of the Rings)
                        else if (word.Equals(words.ElementAt(0)))
                        {
                            sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerWord));
                        }
                        //If word is on users defined Capitlised word list then append the fully capitalised word
                        else if (capitalCaseWords.Contains(lowerWord))
                        {
                            sb.Append(capitalWord);
                        }
                        //Same as above but with all lower case words
                        else if (lowerCaseWords.Contains(lowerWord))
                        {
                            sb.Append(lowerWord);
                        }
                        //Otherwise Titlecase the word
                        else
                        {
                            sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerWord));
                        }
                        sb.Append(" ");
                    }

                    //Remove whitespace at the end
                    sb.Remove(sb.Length - 1, 1);

                    //Check for any Roman Numerals that need to be capatilised
                    if (Regex.IsMatch(sb.ToString(), pattern))
                    {
                        sb.Replace(sb.ToString(), Regex.Replace(sb.ToString(), pattern, c => c.ToString().ToUpper()));
                    }
                    x.Name = sb.ToString();
                    API.Instance.Database.Games.Update(x);
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
