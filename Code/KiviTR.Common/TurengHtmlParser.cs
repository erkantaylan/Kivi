using System;
using System.Collections.ObjectModel;
using KiviTR.Common.Models;

namespace KiviTR.Common
{

    public class TurengHtmlParser
    {
        private readonly HtmlParser parser;
        private readonly string word;

        public TurengHtmlParser(string html, string word)
        {
            this.word = word;
            parser = new HtmlParser(html);
        }

        public VocabularyTranslate Translate()
        {
            VocabularyTranslate result = new VocabularyTranslate(word);

            try
            {
                int sourceIndex = GetTableIndex(TurengHtmlXPaths.FirstTable);
                result.MeaningsA = ParseMeaningList(sourceIndex, TurengHtmlXPaths.FirstTableXPathNumber);
            }
            catch (NullReferenceException)
            { }

            try
            {
                int destinationIndex = GetTableIndex(TurengHtmlXPaths.SecondTable);
                result.MeaningsB = ParseMeaningList(destinationIndex, TurengHtmlXPaths.SecondTableXPathNumber);
            }
            catch (NullReferenceException)
            { }

            return result;
        }

        private ObservableCollection<Meaning> ParseMeaningList(int SourceIndex, int table)
        {
            ObservableCollection<Meaning> list = new ObservableCollection<Meaning>();
            for (int i = 0; list.Count != SourceIndex; i++)
            {
                int currentIndex = i + TurengHtmlXPaths.StarterXpathMeanningNumber;
                try
                {
                    Meaning meaning = GetMeaning(table, currentIndex);
                    list.Add(meaning);
                }
                catch
                {
                    // ignored
                }
            }

            return list;
        }

        private int GetTableIndex(string xpath)
        {
            //sample text:
            //Meanings of "meaning" in Turkish English Dictionary : 21 result(s)
            string fulltext = parser.GetHtmlSpanText(xpath);
            string[] splitted = fulltext.Split(':');
            if (splitted.Length > 0)
            {
                string last = splitted[splitted.Length - 1].Trim();
                string numbers = string.Empty;
                foreach (char c in last)
                {
                    if (char.IsDigit(c))
                    {
                        numbers += c;
                    }
                    else
                    {
                        break;
                    }
                }

                return int.Parse(numbers);
            }

            throw new Exception("table index not found");
        }

        private Meaning GetMeaning(int table, int index)
        {
            Meaning meaning = new Meaning
            {
                Index = int.Parse(parser.GetHtmlSpanText(string.Format(TurengHtmlXPaths.Index, table, index)).Trim()),
                Category = parser.GetHtmlSpanText(string.Format(TurengHtmlXPaths.Category, table, index)),
                Source = parser.GetHtmlSpanText(string.Format(TurengHtmlXPaths.Source, table, index)),
                Destination = parser.GetHtmlSpanText(string.Format(TurengHtmlXPaths.Destination, table, index))
            };


            return meaning;
        }
    }

    public static class TurengHtmlXPaths
    {
        public static string FirstTable = "/html[1]/body[1]/div[5]/div/div[1]/h2[1]";
        public static string SecondTable = "/html[1]/body[1]/div[5]/div/div[1]/h2[2]";

        public static string Index = "/html[1]/body[1]/div[5]/div[1]/div[1]/table[{0}]/tr[{1}]/td[1]";
        public static string Category = "/html[1]/body[1]/div[5]/div[1]/div[1]/table[{0}]/tr[{1}]/td[2]";
        public static string Source = "/html[1]/body[1]/div[5]/div[1]/div[1]/table[{0}]/tr[{1}]/td[3]";
        public static string Destination = "/html[1]/body[1]/div[5]/div[1]/div[1]/table[{0}]/tr[{1}]/td[4]";

        public static int FirstTableXPathNumber = 1;
        public static int SecondTableXPathNumber = 2;

        public static int StarterXpathMeanningNumber = 4;
        /*

public int Index { get; set; }
public string Category { get; set; }
//translated from
public string Source { get; set; }
//meaning of source
public string Destination { get; set; }*/
    }

}