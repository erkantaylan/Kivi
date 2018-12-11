using System.Collections.ObjectModel;

namespace KiviTR.Common.Models
{

    public class VocabularyTranslate
    {
        public VocabularyTranslate(string word)
        {
            Word = word;
        }

        public string Word { get; set; }

        public ObservableCollection<Meaning> MeaningsA { get; set; }
        public ObservableCollection<Meaning> MeaningsB { get; set; }
    }

}