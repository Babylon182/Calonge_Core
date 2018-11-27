using System.Collections.Generic;
using UnityEngine;

namespace CalongeCore.Pathfinding
{
    public class Node : MonoBehaviour
    {
        [SerializeField]
        private List<Node> neighbours = new List<Node>();
        
        public List<Node> Neighbours => neighbours;
        public Node Parent { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public float F => G + H;
        public float X => transform.position.x;
        public float Z => transform.position.z;
        public bool IsTransitable { get; } = true;

        public void SetFirstNodeValues()
        {
            G = 0;
            H = 0;
            Parent = null;
        }

        public void AddNeighbour(Node n)
        {
            neighbours.Add(n);
        }
        
    #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (neighbours.Count == 0 || neighbours == null) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < neighbours.Count; i++)
            {
                Gizmos.DrawRay(transform.position, neighbours[i].transform.position - transform.position);
            }
        }
    #endif
    }
}