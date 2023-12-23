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

        public Board (BoardWrapper board) : this()
        {
            int i = 0;
            foreach (HexagonUnitWrapper hexagonUnitWrapper in board.board)
            {
                HexagonUnit unit = new HexagonUnit(hexagonUnitWrapper);
                this.board[i] = unit;
                i++;
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

    /// <summary>
    /// Minor classes for the game that are not worth their own file and are all serializable
    /// </summary>



    public class HexagonUnit
    {
        public Vector3 position;
        public Resource resource;

        public HexagonUnit(Resource R, int x, int y, int z)
        {
            resource = R;
            position = new Vector3(x, y, z);
        }

        public HexagonUnit(HexagonUnitWrapper hex)
        {
            resource = new Resource(hex.resource);
            position = new Vector3(hex.position.x, hex.position.y, hex.position.z);
        }

        public override string ToString()
        {
            return $"{resource} at {position}";
        }

    }

    public abstract class Settlement
    {
        protected int id { get; set; }
        protected string[] statuses { get; set; }
        protected int occupantID { get; set; }
    }

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