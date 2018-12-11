namespace KiviTR.Common.Models
{

    public class Meaning
    {
        public int Index { get; set; }

        public string Category { get; set; }

        //translated from
        public string Source { get; set; }

        //meaning of source
        public string Destination { get; set; }
    }

}