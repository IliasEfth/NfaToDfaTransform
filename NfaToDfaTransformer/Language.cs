using System.Collections.Generic;

namespace NfaToDfaTransformer
{
    public class Language
    {
        public List<string> Symbols { get; set; }
        public Language()
        {
            this.Symbols = new List<string>();
        }
    }
}
