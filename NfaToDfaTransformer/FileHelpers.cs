using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NfaToDfaTransformer
{
    public class FileHelpers
    {
        public static void parseDfaFromFileTo(ref Language lang, ref IList<Node> nodes, string file)
        {
            if (file == null) throw new ArgumentNullException();
            if (object.ReferenceEquals(lang, null))
            {
                lang = new Language();
            }
            if (object.ReferenceEquals(nodes, null))
            {
                nodes = new List<Node>();
            }
            if (File.Exists(file))
            {
                int statesCount = 0;
                string startState = null;
                string endState = null;
                //move to
                using (StreamReader stream = File.OpenText(file))
                {
                    if (!int.TryParse(stream.ReadLine(), out statesCount))
                    {
                        throw new Exception();
                    }
                    if (statesCount == 0)
                    {
                        throw new Exception();
                    }
                    string autonomusLang = stream.ReadLine();
                    if (autonomusLang == null)
                    {
                        throw new Exception();
                    }
                    foreach (string lan in autonomusLang.Split(" "))
                    {
                        lang.Symbols.Add(lan);
                    }
                    startState = stream.ReadLine();
                    if (startState == null)
                    {
                        throw new Exception();
                    }
                    endState = stream.ReadLine();
                    if (endState == null)
                    {
                        throw new Exception();
                    }
                    List<string> endStateList = endState.Split(" ").ToList();
                    for (int counter = 0; counter < statesCount; counter++)
                    {
                        int start = 0;
                        if (!int.TryParse(startState, out start))
                        {
                            throw new Exception();
                        }
                        bool isStartState = counter == start;
                        bool isEndState = endStateList.SingleOrDefault((s) => s.Equals(counter.ToString())) != null;
                        Node node = new Node($"{counter}", isStartState, isEndState);
                        nodes.Add(node);
                    }
                    string str = "";
                    while ((str = stream.ReadLine()) != null)
                    {
                        string[] info = str.Split(" ");
                        Node node = nodes.SingleOrDefault((s) => s.Name.Equals(info[0]));
                        node.MoveTo.Add(new Move()
                        {
                            State = info[2],
                            Symbol = info[1]
                        });
                    }
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        public static void ExportFinaleNfaToFile(IList<Node> nfa, Language lang, string file)
        {
            if (object.ReferenceEquals(nfa, null)
                || object.ReferenceEquals(file, null))
            {
                throw new ArgumentNullException();
            }
            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.WriteLine(nfa.Count.ToString());
                {
                    string tmp = "";
                    foreach (string symbol in lang.Symbols)
                    {
                        if (string.IsNullOrWhiteSpace(tmp))
                        {
                            tmp = symbol;
                        }
                        else
                        {
                            tmp = tmp + $" {symbol}";
                        }
                    }
                    writer.WriteLine(tmp);
                }
                writer.WriteLine(nfa.SingleOrDefault((s) => s.isStartState.Equals(true)).Name);
                IEnumerable<Node> endStates = nfa.Where((s) => s.isEndState.Equals(true));
                string str = "";
                foreach (Node node in endStates)
                {
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        str = node.Name;
                    }
                    else
                    {
                        str = str + $" {node.Name}";
                    }
                }
                writer.WriteLine(str);
                foreach (Node node in nfa)
                {
                    foreach (Move move in node.MoveTo)
                    {
                        writer.WriteLine($"{node.Name} {move.Symbol} {move.State}");
                    }
                }
            }
        }
    }
}
