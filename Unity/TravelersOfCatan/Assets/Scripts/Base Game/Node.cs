using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using System.Collections;

namespace NEAGame
{
    [System.Serializable]
    public class Node
    {
        public Vector3 position;
        public Building status = new Building();
        public Dictionary<Vector3, Connection> connections = new Dictionary<Vector3, Connection>();

        public Node(int x, int y, int z)
        {
            position = new Vector3(x, y, z);


        }


        public void RegisterConnections(Board gameBoard)
        {
            foreach (Vector3 v in GetNodeNeighbours())
            {
                if (gameBoard.GetNode(v) != null)
                {
                    connections.Add(v, new Connection(this, gameBoard.GetNode(v)));
                }
            }
        }

        public List<Connection> GetConnections()
        {
            return Enumerable.ToList<Connection>(connections.Values);
        }


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

        public bool isEmpty()
        {
            return status.ToString() == "Empty";
        }

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