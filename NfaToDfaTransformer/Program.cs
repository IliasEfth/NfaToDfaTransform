using System;
using System.Collections.Generic;
using System.Linq;
namespace NfaToDfaTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<Node> finale = null;
            Language lang = null;
            {
                IList<Node> nfaStates = null;
                {
                    //read file and save on structures the states and language
                    IList<Node> nodes = null;
                    FileHelpers.parseDfaFromFileTo(ref lang, ref nodes, "nfa_autonomous.txt");
                    //generate all the posible combines
                    NfaToDfaHelpers.generateAllCombinations(nodes);//all combinations are stored on nodes reference
                    //convert dfa states to nfa
                    nfaStates = NfaToDfaHelpers.generateTheDfaFromNfaStates(nodes, lang);//we will return the actual npa STATES
                }
                //get final states .... that means without unaccessable states
                finale = NfaToDfaHelpers.getFinalStates(nfaStates);
            }
            //write to a file
            FileHelpers.ExportFinaleNfaToFile(finale, lang, "dfa_autonomous.txt");
        }
    }
}
