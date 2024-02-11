using System.Collections.Generic;
using System.Numerics;

namespace NEAGame
{
    /// <summary>
    /// A <c>Node</c> represents a single point on the graph where a player or building can be placed.
    /// </summary>
    public class Node
    {
        public Vector3 position;
        public Building status = new Building();

        // constructor for creating a node from a position
        public Node(int x, int y, int z)
        {
            position = new Vector3(x, y, z);
        }

        // constructor for creating a node from a node wrapper
        public Node(NodeWrapper node) 
        {
            position = new Vector3(node.position.x, node.position.y, node.position.z);
            status = new Building(node.status);
        }

        // method to get the node neighbours of a node based on the parity of the position
        public IEnumerable<Vector3> GetNodeNeighbours()
        {
            // determine parity of position
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 1)
            {
                yield return position + new Vector3(1, 0, 0);
                yield return position + new Vector3(0, 1, 0);
                yield return position + new Vector3(0, 0, 1);
            }
            else
            {
                yield return position + new Vector3(-1, 0, 0);
                yield return position + new Vector3(0, -1, 0);
                yield return position + new Vector3(0, 0, -1);
            }
        }

        // method to get the hex neighbours of a node based on the parity of the position
        public IEnumerable<Vector3> GetHexNeighbours()
        {
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 1)
            {
                yield return position + new Vector3(-1, 0, 0);
                yield return position + new Vector3(0, -1, 0);
                yield return position + new Vector3(0, 0, -1);
            }
            else
            {
                yield return position + new Vector3(-1, -1, 0);
                yield return position + new Vector3(0, -1, -1);
                yield return position + new Vector3(-1, 0, -1);
            }
        }

        // method to check if the node is empty
        public bool isEmpty()
        {
            return status.ToString() == "Empty";
        }

        // override the ToString method to return the status of the node
        public override string ToString()
        {
            if (!status.IsEmpty())
            {
                return $"{status} at {position}";

            }
            else
            {
                return position.ToString();
            }
        }
    }
}
