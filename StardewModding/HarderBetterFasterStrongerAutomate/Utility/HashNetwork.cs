using System;
using System.Collections.Generic;
using System.Linq;

namespace Dannnno.StardewMods.HarderBetterFasterStrongerAutomate.Utility
{
    public class HashNetwork<TNode> : INetwork<TNode> where TNode : IEquatable<TNode>, IComparable<TNode>
    {
        private HashSet<TNode> _nodeList;
        private Dictionary<TNode, HashSet<TNode>> _edgeList;
        public IEnumerable<TNode> Nodes => _nodeList;

        public IEnumerable<Tuple<TNode, TNode>> Edges => _edgeList.SelectMany(pair => pair.Value.Select(toEdge => Tuple.Create(pair.Key, toEdge)));

        public HashNetwork()
        {
            _nodeList = new HashSet<TNode>();
            _edgeList = new Dictionary<TNode, HashSet<TNode>>();
        }

        private void _AddEdge(TNode one, TNode two)
        {
            if (!_edgeList.ContainsKey(one))
            {
                _edgeList[one] = new HashSet<TNode>();
            }

            _edgeList[one].Add(two);
        }

        public void AddEdge(TNode one, TNode two)
        {
            if (!_nodeList.Contains(one))
            {
                throw new ArgumentOutOfRangeException("one", one, "This is not a node present in the network");
            }

            if (!_nodeList.Contains(two))
            {
                throw new ArgumentOutOfRangeException("two", two, "This is not a node present in the network");
            }

            if (one.Equals(two))
            {
                throw new ArgumentOutOfRangeException("one", one, "Self-referential edges cannot be created");
            }

            // The way we're going to handle edges is by always having the key be less than the value
            if (two.CompareTo(one) < 0)
            {
                _AddEdge(two, one);
            }
            else
            {
                _AddEdge(one, two);
            }
        }

        public void AddNode(TNode node)
        {
            if (!_nodeList.Contains(node))
            {
                _nodeList.Add(node);
            }
        }


        public void RemoveEdge(TNode one, TNode two)
        {
            if (two.CompareTo(one) < 0)
            {
                RemoveEdge(two, one);
                return;
            }

            _edgeList[one].Remove(two);
            if (_edgeList[one].Count == 0)
            {
                _edgeList.Remove(one);
            }

        }

        public void RemoveNode(TNode node)
        {
            _nodeList.Remove(node);

            if (_edgeList.ContainsKey(node))
            {
                _edgeList.Remove(node);
            }

            // Now remove our edges referencing this node
            foreach (var othernode in _nodeList)
            {
                TryRemoveEdge(node, othernode);
            }
        }

        public bool TryRemoveEdge(TNode one, TNode two)
        {
            try
            {
                RemoveEdge(one, two);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryRemoveNode(TNode node)
        {
            try
            {
                RemoveNode(node);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
