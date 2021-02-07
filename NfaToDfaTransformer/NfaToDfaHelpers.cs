using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace NfaToDfaTransformer
{
    public class NfaToDfaHelpers
    {
        public static void generateAllCombinations(IList<Node> states)
        {
            if (object.ReferenceEquals(states, null)) throw new ArgumentNullException("states cannot be null");
            int bits = states.Count;
            IList<Node> copyiedStates = null;
            {
                Node[] nodes = new Node[bits];
                states.CopyTo(nodes, 0);
                copyiedStates = nodes.ToList();
            }
            bool[] fillZero = new bool[bits];
            int actualCombines = (int)Math.Pow(2, bits);

            for (int counter = 0; counter < bits; counter++) { fillZero[counter] = false; }

            for (int combine = 0; combine < actualCombines; combine++)
            {
                string tmp = "";
                for (int bit = bits; bit > 0; bit--)
                {
                    int bitIndex = (int)Math.Pow(2, bits - bit);
                    if (combine % bitIndex == 0)
                    {
                        fillZero[bit - 1] = !fillZero[bit - 1];
                    }
                    if (!fillZero[bit - 1])
                    {
                        if (string.IsNullOrWhiteSpace(tmp))
                        {
                            tmp = states.ElementAt(bits - bit).Name;
                        }
                        else
                        {
                            tmp = tmp + $",{states.ElementAt(bits - bit).Name}";
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(tmp))
                {
                    bool startNodeIsEndState = states.SingleOrDefault((s) => s.isStartState.Equals(true)).isEndState;
                    states.Add(new Node(isEndState: startNodeIsEndState));
                }
                else
                {
                    Node node = copyiedStates.SingleOrDefault((s) => s.Name.Equals(tmp));
                    bool NodeDoesntExists = node == null;
                    if (NodeDoesntExists)
                    {
                        IEnumerable<Node> endState = copyiedStates.Where((s) => s.isEndState.Equals(true));
                        bool NodeIsEndState = false;
                        foreach (Node endNode in endState)
                        {
                            if (tmp.Contains(endNode.Name))
                            {
                                NodeIsEndState = true;
                            }
                        }
                        states.Add(new Node(tmp, isEndState: NodeIsEndState));
                    }
                }
            }
            //sort the results in order to show them in correct way
            (states as List<Node>).Sort((a, b) =>
            {
                if (a.Name.Equals(StateHelpers.Default) || b.Name.Equals(StateHelpers.Default))
                {
                    return -1;//return -1 to find it at first row
                }
                string[] aStates = a.Name.Split(StateHelpers.DefaultSpliter);
                string[] bStates = b.Name.Split(StateHelpers.DefaultSpliter);
                return aStates.Length < bStates.Length ? 1 : 2;
            });
        }
        public static IList<Node> generateTheDfaFromNfaStates(IList<Node> states, Language lang)
        {
            IList<Node> result = new List<Node>();
            foreach (Node state in states)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (string symbol in lang.Symbols)
                {
                    string[] complexStates = state.Name.Split(StateHelpers.DefaultSpliter);
                    bool isSimpleState = complexStates.Length == 1;
                    if (isSimpleState)
                    {
                        var moveTo = state.MoveTo.Where((s) => s.Symbol.Equals(symbol));
                        if (moveTo.Count() != 0)
                        {
                            foreach (Move move in moveTo)
                            {
                                if (dictionary.ContainsKey(symbol))
                                {
                                    string value = null;
                                    dictionary.TryGetValue(symbol, out value);
                                    dictionary.Remove(symbol);
                                    dictionary.Add(symbol, $"{value},{move.State}");
                                }
                                else
                                {
                                    dictionary.Add(symbol, move.State);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string simpleState in complexStates)
                        {
                            Node nodeIsSimpleState = result.SingleOrDefault((s) => s.Name.Equals(simpleState));
                            var moveTo = nodeIsSimpleState.MoveTo.Where((s) => s.Symbol.Equals(symbol));
                            if (moveTo.Count() != 0)
                            {
                                foreach (Move move in moveTo)
                                {
                                    if (dictionary.ContainsKey(symbol))
                                    {
                                        string value = null;
                                        dictionary.TryGetValue(symbol, out value);
                                        if (!value.Contains(move.State))
                                        {
                                            dictionary.Remove(symbol);
                                            dictionary.Add(symbol, $"{value},{move.State}");
                                        }
                                    }
                                    else
                                    {
                                        dictionary.Add(symbol, move.State);
                                    }
                                }
                            }
                        }
                    }
                }
                if (dictionary.Count == 0)
                {
                    result.Add(new Node(state.Name, state.isStartState, state.isEndState)
                    {
                        MoveTo = new List<Move>()
                    });
                }
                else
                {
                    Node node = new Node(state.Name, isStartState: state.isStartState, isEndState: state.isEndState);
                    foreach (KeyValuePair<string, string> values in dictionary)
                    {
                        node.MoveTo.Add(new Move()
                        {
                            State = values.Value,
                            Symbol = values.Key
                        });
                    }
                    result.Add(node);
                }
            }
            (result as List<Node>).ForEach((node) =>
            {
                Node tmp = states.SingleOrDefault((s) => s.Equals(node));
                if (object.ReferenceEquals(tmp, null)) throw new NullReferenceException();
                node.Name = tmp.Name;
            });
            return result;
        }
        public static IList<Node> getFinalStates(IList<Node> states)
        {
            if (states?.Count == 0) throw new Exception();
            IList<Node> front = new List<Node>();
            IList<Node> closed = new List<Node>();
            {
                Node node = states.SingleOrDefault((s) => s.isStartState.Equals(true));
                if (node.isEndState)
                {
                    closed.Add(states.SingleOrDefault((s) => s.Name.Equals(StateHelpers.Default)));
                }
                front.Add(node);
            }
            while (front.Count != 0)
            {
                Node tmp = front.FirstOrDefault();
                if (closed.SingleOrDefault((s) => s.Name.Equals(tmp.Name)) != null)
                {
                    front.Remove(tmp);
                }
                else
                {
                    foreach (var move in tmp.MoveTo)
                    {
                        Node child = states.SingleOrDefault((s) => s.Equals(new Node(move.State)));
                        if (front.Where((s) => s.Name.Equals(child.Name)).Count() == 0)
                        {
                            front.Add(child);
                        }
                    }
                    front.Remove(tmp);
                    closed.Add(tmp);
                }
            }
            return closed;
        }
    }
}
