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
    /// <summary>
    /// Class to represent the game board
    /// </summary>
    [System.Serializable]
    public class Board // A graph of nodes and connections that makes up the main game board.
    {

        Dictionary<Vector3, Resource> board = new Dictionary<Vector3, Resource>();
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
                            Resource unit;
                            if (x == 0 && y == 0 && z == 0)
                            {
                                unit = new Resource(0); // center of board is Empty in classical Catan
                            }
                            else
                            {
                                unit = Resource.GetRandom();

                            }
                            board.Add(new Vector3(x, y, z), unit);
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
                            i++; // create a node at each position on the board
                        }
                    }
                }
            }
        }

        // constructor to create a board from the loaded serialized data
        public Board (BoardWrapper board) : this()
        {
            int i = 0;
            foreach (ResourceWrapper res in board.board._Values)
            {
                Resource n = new Resource(res);
                this.board[new Vector3(board.board._Keys[i].x, board.board._Keys[i].y, board.board._Keys[i].z)] = n;
                i++;
            }
            
            i = 0;
            foreach (NodeWrapper node in board.nodes._Values)
            {
                Node n = new Node(node);
                nodes[new Vector3(board.nodes._Keys[i].x, board.nodes._Keys[i].y, board.nodes._Keys[i].z)] = n;
                i++;
            }
        }

        // method to get the resource at a given position or return null if it doesn't exist
        public Resource GetHexAtPosition(Vector3 pos)
        {
            if (board.ContainsKey(pos))
                return board[pos];
            else
                return null;
        }

        // method to get the node at a given position or return null if it doesn't exist
        public Node GetNode(Vector3 pos)
        {

            if (nodes.ContainsKey(pos))
                return nodes[pos];
            else
                return null;
        }

        // method to get an array of all the nodes on the board
        public Node[] GetAllNodes()
        {
            return nodes.Values.ToArray();
        }

        // method to get key value pairs of all the resources on the board with their positions
        public Dictionary<Vector3, Resource> GetResourcesOnBoard()
        {
            return board;
        }

        // method to get the connection between two nodes
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

        // method to update an existing connection between two nodes or create a new one if it doesn't exist
        public void UpdateConnection(Vector3 v1, Vector3 v2, Connection con)
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

        // method to serialize the board object directly to a JSON serializable BoardWrapper object
        public BoardWrapper SoftSerialize()
        {
            BoardWrapper b = new BoardWrapper();
            b.board = new HexagonUnitWrapper(board);
            b.connections = new AdjMatrixWrapper(connections);
            b.nodes = new AllNodesWrapper(nodes);


            return b;
        }


    }

    /// <summary>
    /// Minor classes for the game that are not worth their own file and are all serializable
    /// </summary>

    public abstract class Settlement
    {
        protected int id { get; set; }
        protected string[] statuses { get; set; }
        protected int occupantID { get; set; }
    }

    /// <summary>
    /// class to represent a building on the board
    /// </summary>
    public class Building : Settlement
    {
        
        public Building(string i = "Empty", int o = -1)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            id = Array.IndexOf(statuses, i);
            occupantID = o;

        }

        public Building(SettlementWrapper sw)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            occupantID = sw.occupantID;
            id = Array.IndexOf(statuses, sw.status);
        }


        public void UpgradeVillage()
        {
            if (id == 1)
            {
                id++;
            }
        }

        public bool IsEmpty()
        {
            return id == 0;
        }
        public override string ToString()
        {
            if (occupantID != -1)
            {
                return $"{statuses[id]} owned by Player {occupantID}";
            }
            else
            {
                return $"{statuses[id]}";
            }
        }

        public string GetStatus()
        {
            return statuses[id];
        }

        public int GetOccupant()
        {
            return occupantID;
        }

        public void DowngradeVillage()
        {
            if (id == 2)
            {
                id--;
            }
        }
    }

    public class Resource
    {
        public static readonly string[] resources = { "Empty", "Brick", "Sheep", "Ore", "Wood", "Wheat" };
        private int id;
        private static readonly Random rng = new Random();



        public Resource(int i = 0)
        {
            id = i;
        }

        public Resource(string name)
        {
            id = Array.IndexOf(resources, name);
        }

        public Resource(ResourceWrapper res)
        {
            id = res.id;
        }


        public override string ToString()
        {
            return resources[id];
        }


        public void CreateRandomResource()
        {
            id = rng.Next(1, resources.Length);
        }

        public override bool Equals(System.Object otherItem)
        {
            if (otherItem == null)
            {
                return false;
            }

            Resource otherResource = otherItem as Resource;

            return (id == otherResource.id);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public static Resource GetRandom()
        {
            Resource temp = new Resource();
            temp.CreateRandomResource();
            return temp;
        }
    }
}