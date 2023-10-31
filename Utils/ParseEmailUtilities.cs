using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommunicationThreadTagExtraction.Utils
{
    //ParseEmailUtilities accepts a string of threaded email text as a parameter and uses regular expressions to split that string into separate messages stored in a list of strings.
    public class ParseEmailUtilities
    {
        public static List<string> ParseEmailText(string emailText)
        {
            List<string> parsedEmails = new List<string>();

            List<int> matchIndexes = new List<int>();

            var regexes = GetRegexList();

            //Iterate through the list of regexes and add the index of each match to a list of integers
            foreach (var regex in regexes)
            {
                Match m = regex.Match(emailText);
                while(m.Success)
                {
                    matchIndexes.Add(m.Index);
                    m = m.NextMatch();
                }
            }

            int currentIndex = 0;
            int counter = 1;

            //Sort the list of integers and use it to split the email text into separate messages
            if(matchIndexes.Count > 0)
            {
                matchIndexes.Sort();

                foreach (var matchIndex in matchIndexes)
                {
                    parsedEmails.Add(emailText.Substring(currentIndex, matchIndex - currentIndex));
                    currentIndex = matchIndex;
                    counter++;
                }
            }

            //Add the remaining text to the list of parsed emails
            parsedEmails.Add(emailText.Substring(currentIndex, emailText.Length - currentIndex));

            return parsedEmails;
        }

        public static List<string> ParseTagsFromEmailText(string emailText, string tagStartMarker, bool replaceUnderscoresWithSpaces = true, bool replaceDashesWithSpaces = true, bool capitalizeFirstLetterOfEachWord = true)
        {
            List<string> parsedTags = new List<string>();
            var regex = new Regex($@"{tagStartMarker}\S+");

            Match m = regex.Match(emailText);
            while(m.Success)
            {
                string tagString = emailText.Substring(m.Index, m.Length);

                tagString = tagString.Replace(tagStartMarker, "");
                
                if (replaceUnderscoresWithSpaces)
                {
                    tagString = tagString.Replace("_", " ");
                }

                if (replaceDashesWithSpaces)
                {
                    tagString = tagString.Replace("-", " ");
                }

                //capitalize the first letter of each word in the tag
                if (capitalizeFirstLetterOfEachWord)
                {
                    if(!replaceUnderscoresWithSpaces)
                    {
                        tagString = tagString.Split('_').Select(x => x.First().ToString().ToUpper() + x.Substring(1)).Aggregate((x, y) => x + "_" + y);
                    }
                    if(!replaceDashesWithSpaces)
                    {
                        tagString = tagString.Split('-').Select(x => x.First().ToString().ToUpper() + x.Substring(1)).Aggregate((x, y) => x + "-" + y);
                    }
                    tagString = tagString.Split(' ').Select(x => x.First().ToString().ToUpper() + x.Substring(1)).Aggregate((x, y) => x + " " + y);
                }

                parsedTags.Add(tagString);
                
                m = m.NextMatch();
            }

            return parsedTags;
        }


        //Create a new static method that returns a list of Regex objects
        //Informed by https://stackoverflow.com/questions/278788/parse-email-content-from-quoted-reply
        public static List<Regex> GetRegexList()
        {

            //Create a regex that matches the following: From: FS (Farnam Street) <newsletter@farnamstreetblog.com> 
            //Regex regex = new Regex("From:\\s*" + Regex.Escape(_mail), RegexOptions.IgnoreCase);
            List<Regex> regexList = new List<Regex>();
            regexList.Add(new Regex(@"^\s*[oO]n[ \t][\S \t]*?wrote:", RegexOptions.IgnoreCase | RegexOptions.Multiline));
            //regexList.Add(new Regex(@"^\s*[fF]rom:[ \t]\S", RegexOptions.IgnoreCase));
            //regexList.Add(new Regex(@"[oO]n[ \t][\S \t]*?wrote:", RegexOptions.IgnoreCase | RegexOptions.Multiline));
            regexList.Add(new Regex(@"[fF]rom:[ \t]\S", RegexOptions.IgnoreCase));
            //regexList.Add(new Regex(@"From:", RegexOptions.IgnoreCase));
            return regexList;
        }
    }    
}