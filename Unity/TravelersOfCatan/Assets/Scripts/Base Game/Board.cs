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
    public class Board // A graph of nodes
    {

        private HexagonUnit[] board = new HexagonUnit[19];
        Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
        // array of nodes and hexagon centers in graph. Fixed length means no need to resize
        public Dictionary<Vector3, Dictionary<Vector3, Connection>> connections = new Dictionary<Vector3, Dictionary<Vector3, Connection>>();
        // nested dictionary for the connections between nodes in the board with a default state of new Connection() which can be updated as the game progresses
        // this acts as an adjacency matrix of the graph of nodes but omits all the null values

        public Board()
        {

            int i = 0;
            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    for (int z = -2; z < 3; z++)
                    {
                        if (x + y + z == 0)
                        {
                            HexagonUnit unit;
                            if (x == 0 && y == 0 && z == 0)
                            {
                                unit = new HexagonUnit(new Resource(0), x, y, z); // center of board is Empty in classical Catan
                            }
                            else
                            {
                                unit = new HexagonUnit(Resource.GetRandom(), x, y, z);

                            }
                            board[i] = unit;
                            i++;
                        }
                    }
                }
            }

            i = 0;
            for (int x = -2; x < 4; x++)
            {
                for (int y = -2; y < 4; y++)
                {
                    for (int z = -2; z < 4; z++)
                    {
                        if ((x + y + z == 1) || (x + y + z == 2))
                        {

                            Node n = new Node(x, y, z);

                            // register a list of all existing connections for the AI to use



                            nodes.Add(new Vector3(x, y, z), n);
                            i++;
                        }
                    }
                }
            }

            foreach (Node n in nodes.Values)
            {
                n.RegisterConnections(this);
                foreach (var con in n.connections)
                {
                    if (!connections.ContainsKey(n.position))
                    {
                        connections.Add(n.position, new Dictionary<Vector3, Connection>());
                    }
                    connections[n.position].Add(con.Key, con.Value);
                }

            }



        }


        public HexagonUnit GetHexAtPosition(Vector3 pos)
        {
            foreach (HexagonUnit unit in board)
            {
                if (unit.position == pos)
                {
                    return unit;
                }
            }
            return null;
        }

        public Node GetNode(Vector3 pos)
        {

            if (nodes.ContainsKey(pos))
                return nodes[pos];
            else
                return null;
        }

        public Node[] GetAllNodes()
        {
            return nodes.Values.ToArray();
        }


        public IEnumerable<KeyValuePair<Vector3, Resource>> GetResourcesOnBoard()
        {
            foreach (HexagonUnit hex in board)
            {
                yield return new KeyValuePair<Vector3, Resource>(hex.position, hex.resource);
                
            }
        }


        public Connection GetConnection(Vector3 v1, Vector3 v2)
        {
            var x = connections[v1];
            if (x == null)
            {
                return null;
            }
            else
            {
                if (x.ContainsKey(v2))
                {
                    return x[v2];
                }
                else
                {
                    return null;
                }
            }
        }


        public void UpdateConnection(Vector3 v1, Vector3 v2, string status, Player currentPlayer) // weakest function in entire project
        {
            Connection x = connections[v1][v2];
            x.SetOccupant(currentPlayer);
            x.SetStatus(status); 
            Connection y = connections[v2][v1];
            y.SetOccupant(currentPlayer);
            y.SetStatus(status);
        }

    }
}