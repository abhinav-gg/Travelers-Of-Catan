using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace NEAGame
{
    [System.Serializable]
    public class Board // A graph of nodes
    {

        HexagonUnit[] board = new HexagonUnit[19];
        Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
        Dictionary<Vector3, Dictionary<Vector3, Connection>> connections = new Dictionary<Vector3, Dictionary<Vector3, Connection>>();

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

            foreach (Node n1 in nodes.Values)
            {
                foreach (Node n2 in nodes.Values)
                {
                    if (!connections.ContainsKey(n1.position))
                    {
                        connections.Add(n1.position, new Dictionary<Vector3, Connection>());
                    }
                    connections[n1.position].Add(n2.position, new Connection());
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
            try
            {
                return connections[v1][v2];
            }
            catch (KeyNotFoundException)
            {
                return null;
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

        public void UpdateConnection(Vector3 v1, Vector3 v2, SettlementWrapper con)
        {
            connections[v1][v2] = new Connection(con);
            connections[v2][v1] = new Connection(con);
        }


        public BoardWrapper SoftSerialize()
        {
            BoardWrapper b = new BoardWrapper();
            foreach (HexagonUnit u in board)
            {
                b.board.Add(new HexagonUnitWrapper(u));
            }
            b.connections = new AdjMatrixWrapper(connections);
            b.nodes = new AllNodesWrapper(nodes);


            return b;
        }

        public static Board SoftDeSerialize(BoardWrapper board)
        {
            Board b = new Board();
            int i = 0;
            int j = 0;
            foreach (HexagonUnitWrapper hex in board.board)
            {
                b.board[i] = new HexagonUnit(hex);
                i++;
            }
            i = 0;
            foreach (var v1 in board.connections._Keys)
            {
                j = 0;
                foreach (var v2 in board.connections._Values[i]._Keys)
                {
                    Vector3 pos1 = new Vector3(v1.x, v1.y, v1.z);
                    Vector3 pos2 = new Vector3(v2.x, v2.y, v2.z);
                    b.UpdateConnection(pos1, pos2, board.connections._Values[i]._Values[j]);
                    j++;
                }

                i++;
            }

            foreach (var v in board.nodes._Keys.Zip(board.nodes._Values, (k, v) => new { k, v }))
            {
                b.nodes[new Vector3(v.k.x, v.k.y, v.k.z)] = new Node(v.v);
            }


            return b;
        }


    }
}