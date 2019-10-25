using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dannnno.StardewMods.HarderBetterFasterStrongerAutomate.Utility
{
    /// <summary>
    /// Interface that represents a network or graph structure
    /// No directionality is implied or guaranteed by the graph
    /// </summary>
    /// <typeparam name="TNode">The type of entity that is a node in the graph; must be comparable to other TNodes</typeparam>
    public interface INetwork<TNode> where TNode: IEquatable<TNode>, IComparable<TNode>
    {
        /// <summary>
        /// Get the enumerable nodes of the network, in any order
        /// </summary>
        public IEnumerable<TNode> Nodes { get; }

        /// <summary>
        /// Get the enumerable edges of the network, in any order
        /// </summary>
        public IEnumerable<Tuple<TNode, TNode>> Edges { get; }

        /// <summary>
        /// Add a node to the graph
        /// </summary>
        /// <param name="node">The node to add</param>
        public void AddNode(TNode node);

        /// <summary>
        /// Remove a node from the graph
        /// </summary>
        /// <param name="node">The node to remove</param>
        public void RemoveNode(TNode node);

        /// <summary>
        /// Try to remove a node from the graph
        /// </summary>
        /// <param name="node">The node to try to remove</param>
        /// <returns>Whether removing the node was successful</returns>
        public bool TryRemoveNode(TNode node);

        /// <summary>
        /// Add an edge to the graph
        /// </summary>
        /// <param name="one">One of the nodes in the edge</param>
        /// <param name="two">The other node in the edge</param>
        public void AddEdge(TNode one, TNode two);

        /// <summary>
        /// Remove an edge from the graph
        /// </summary>
        /// <param name="one">One of the nodes in the edge</param>
        /// <param name="two">The other node in the edge</param>
        public void RemoveEdge(TNode one, TNode two);

        /// <summary>
        /// Try to remove an edge from the graph
        /// </summary>
        /// <param name="one">One of the ndoes in the edge</param>
        /// <param name="two">The other node in the edge</param>
        /// <returns>Whether the edge was removed</returns>
        public bool TryRemoveEdge(TNode one, TNode two);
    }
}
