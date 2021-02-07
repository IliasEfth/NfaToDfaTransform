using System.Collections.Generic;
using System.Text;
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
