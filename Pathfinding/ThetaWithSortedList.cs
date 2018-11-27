using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CalongeCore.Pathfinding
{
    public class ThetaWithSortedList
    {
        public static Stack<Node> SearchPath(Node startNode, Node endNode)
        {
            if (startNode == null || endNode == null)
                return null;
    
            SortedList<Node, float> openSet = new SortedList<Node, float>();
            HashSet<Node> closedSet = new HashSet<Node>();
            Node currentNode = startNode;
            
            currentNode.SetFirstNodeValues();
            openSet.Add(currentNode , currentNode.F);
    
            while (openSet.Count > 0)
            {
                currentNode = LookForLowerF(openSet);
                if (currentNode == endNode) break;
    
                for (int i = 0 , neighbourCount = currentNode.Neighbours.Count ; i < neighbourCount ; i++)
                {
                    Node neighNode = currentNode.Neighbours[i];
    
                    if (!neighNode.IsTransitable || closedSet.Contains(neighNode)) 
                        continue;
    
                    if (!openSet.ContainsKey(neighNode))
                    {
                        neighNode.Parent = currentNode;
                        neighNode.G = CalculateG(currentNode, neighNode);
                        neighNode.H = CalculateH(neighNode, endNode);
                        openSet.Add(neighNode, neighNode.F);
                    }
                    else
                    {
                        float potentialG = CalculateG(currentNode, neighNode);
                        if (potentialG < neighNode.G)
                        {
                            neighNode.Parent = currentNode;
                            neighNode.G = potentialG;
                            openSet.Remove(neighNode);
                            openSet.Add(neighNode, neighNode.F);
                        }
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
            }
    
            if (currentNode == endNode)
            {
                Node sourceNode = startNode;
                Node targetNode = endNode;

                while (sourceNode != targetNode)
                {
                    Vector3 from = sourceNode.transform.position;
                    Vector3 dir = targetNode.transform.position - from;

                    RaycastHit hit;
                    if (Physics.Raycast(from, dir.normalized, out hit, dir.magnitude))
                    {
                        if (targetNode.Parent != null && targetNode != targetNode.Parent) 
                            targetNode = targetNode.Parent;
                        else 
                            break;
                    }
                    else
                    {
                        targetNode.Parent = sourceNode;
                        sourceNode = targetNode;
                        targetNode = endNode;
                    }
                }
                
                return RebuildRoad(endNode);
            }
            else
            {
                Debug.Log("Impossible to find a way. The node may be an island");
                return null;
            }
        }

        private static float CalculateG(Node fromNode, Node toNode)
        {
            float calculatedG;

            if (fromNode != toNode)
            {
                Vector3 direction = toNode.transform.position - fromNode.transform.position;
                calculatedG = direction.sqrMagnitude + fromNode.G;
            }
            else
                calculatedG = fromNode.G;
  
            return calculatedG;
        }

        private static float CalculateH(Node fromNode, Node toNode)
        {
            Vector3 direction = toNode.transform.position - fromNode.transform.position;
            return direction.sqrMagnitude;
        }

        private static Node LookForLowerF(SortedList<Node, float> openSet)
        {
            return openSet.First().Key;
        }
    
        private static Stack<Node> RebuildRoad(Node end)
        {
            Stack<Node> nodesRoad = new Stack<Node>();
            while (end.Parent != null)
            {
                nodesRoad.Push(end);
                end = end.Parent;
            }
            nodesRoad.Push(end);
            return nodesRoad;
        }
    }
}