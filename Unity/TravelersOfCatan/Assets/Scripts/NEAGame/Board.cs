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
                            nodes.Add(new Vector3(x, y, z), n);
                            i++;
                        }
                    }
                }
            }

            

        }

        public Board (BoardWrapper board)
        {

            int i = 0;
            foreach (HexagonUnitWrapper hexagonUnitWrapper in board.board)
            {
                HexagonUnit unit = new HexagonUnit(hexagonUnitWrapper);
                this.board[i] = unit;
                i++;
            }
            for (int x = -2; x < 4; x++)
            {
                for (int y = -2; y < 4; y++)
                {
                    for (int z = -2; z < 4; z++)
                    {
                        if ((x + y + z == 1) || (x + y + z == 2))
                        {

                            Node n = new Node(x, y, z);
                            nodes.Add(new Vector3(x, y, z), n);
                        }
                    }
                }
            }
            i = 0;
            foreach (NodeWrapper node in board.nodes._Values)
            {
                Node n = new Node(node);
                nodes[new Vector3(board.nodes._Keys[i].x, board.nodes._Keys[i].y, board.nodes._Keys[i].z)] = n;
                i++ ;
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
                return new Connection();
            }
        }

        public void UpdateConnection(Vector3 v1, Vector3 v2, Connection con) // weakest function in entire project
        {

            if (connections.ContainsKey(v1))
            {
                if (connections[v1].ContainsKey(v2))
                {
                    connections[v1][v2] = con;
                }
                else
                {
                    connections[v1].Add(v2, con);
                }
            }
            else
            {
                connections.Add(v1, new Dictionary<Vector3, Connection>());
                connections[v1].Add(v2, con);
            }
            if (connections.ContainsKey(v2))
            {
                if (connections[v2].ContainsKey(v1))
                {
                    connections[v2][v1] = con;
                }
                else
                {
                    connections[v2].Add(v1, con);
                }
            }
            else
            {
                connections.Add(v2, new Dictionary<Vector3, Connection>());
                connections[v2].Add(v1, con);
            }
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


    }
}