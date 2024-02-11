using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace NEAGame
{
    /// <summary>
    /// The <c>Board</c> class is used to represent the graph of nodes and connections that make up the main game board.
    /// </summary>
    public class Board // A graph of nodes and connections that makes up the main game board.
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// nested dictionary for the connections between nodes in the board implementing an adjacency matrix
        /// these data structures are also composed of other class objects
        /// Skill A: Complex Data Structure 
        /// Skill A: Complex User-Defined OOP: Composition + Encapsulation
        
        private Dictionary<Vector3, Resource> board = new Dictionary<Vector3, Resource>();
        private Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
        private Dictionary<Vector3, Dictionary<Vector3, Connection>> connections = new Dictionary<Vector3, Dictionary<Vector3, Connection>>();
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor for the <c>Board</c> class. This creates a new board with 19 hexagon cells and 54 nodes using a mathematical representation of the Catan board.
        /// This is more efficient than creating a 2D array of hexagons and nodes and allows for easier calculations of the positions of nodes and connections.
        /// Skill A: Complex Mathematical Model
        /// </summary>
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
                                // center of board is Empty in classical Catan
                                unit = new Resource(0); 
                            }
                            else
                            {
                                // generate a random resource at each position in the hexagon grid
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
                            i++; 
                            // create a node at each position on the board using cubic coordinates
                        }
                    }
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
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
    /// A player may purchase a <c>Settlement</c> to place on the board. This is an abstract class which is inherited by <c>Building</c> and <c>Connnection</c>
    /// </summary>
    public abstract class Settlement
    {
        protected int id { get; set; }
        protected string[] statuses { get; set; }
        protected int occupantID { get; set; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A <c>Building</c> represents the status of a node on the board. It can be empty, a village or a city.<br/>
    /// Skill A: Complex User-Defined OOP - Inheritance
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Building : Settlement
    {
        // constructor for the building class
        public Building(string i = "Empty", int o = -1)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            id = Array.IndexOf(statuses, i);
            occupantID = o;

        }

        // constructor to create a building from the loaded serialized data
        public Building(SettlementWrapper sw)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            occupantID = sw.occupantID;
            id = Array.IndexOf(statuses, sw.status);
        }

        // method to upgrade a village to a city
        public void UpgradeVillage()
        {
            if (id == 1)
            {
                id++;
            }
        }

        // method to check if a node is empty
        public bool IsEmpty()
        {
            return id == 0;
        }

        // override the ToString method to return the status
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

        // method to get the status of the building
        public string GetStatus()
        {
            return statuses[id];
        }

        // method to get the occupant of the building
        public int GetOccupant()
        {
            return occupantID;
        }

        // method to undo the upgrade of a village to a city
        public void DowngradeVillage()
        {
            if (id == 2)
            {
                id--;
            }
        }
    }

    /// <summary>
    /// A <c>Resource</c> represents the type of resource that a hexagon cell on the board produces.
    /// </summary>
    public class Resource
    {
        public static readonly string[] RESOURCES = { "Empty", "Brick", "Sheep", "Ore", "Wood", "Wheat" };
        private int id;
        private static readonly Random rng = new Random();


        // constructor for the resource class using the resource id
        public Resource(int i = 0)
        {
            id = i;
        }

        // constructor to create a resource from the loaded serialized data
        public Resource(ResourceWrapper res)
        {
            id = res.id;
        }

        // override the ToString method to return the name of the resource
        public override string ToString()
        {
            return RESOURCES[id];
        }

        // method to get the id of the resource
        public override int GetHashCode()
        {
            return id;
        }

        // method to create a random resource
        public static Resource GetRandom()
        {
            Resource temp = new Resource(rng.Next(1, RESOURCES.Length));
            return temp;
        }

        // override the Equals method to compare two resources
        public override bool Equals(object obj)
        {
            // used in comparisons to check if two resources are the same in dictionary lookups
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Resource r = (Resource)obj;
            return r.id == id;
        }
    }
}
