using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NfaToDfaTransformer
{
    public class Node : IEquatable<Node>
    {
        public string Name { get; set; }
        public List<Move> MoveTo { get; set; }
        public bool isStartState { get; private set; }
        public bool isEndState { get; private set; }
        public Node(string name = "", bool isStartState = false, bool isEndState = false)
        {
            this.isStartState = isStartState;
            this.isEndState = isEndState;
            this.Name = string.IsNullOrWhiteSpace(name) ? StateHelpers.Default : name;
            this.MoveTo = new List<Move>();
        }
        public bool Equals([AllowNull] Node other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(this, other)) return true;
            string[] objStates = null;
            bool equal = true;
            {
                string[] states = this.Name.Split(StateHelpers.DefaultSpliter);
                objStates = other.Name.Split(StateHelpers.DefaultSpliter);
                if (!states.Length.Equals(objStates.Length)) return false;
            }
            foreach (string state in objStates)
            {
                if (!this.Name.Contains(state))
                {
                    equal = false;
                }
            }
            return equal;
        }
    }
}
