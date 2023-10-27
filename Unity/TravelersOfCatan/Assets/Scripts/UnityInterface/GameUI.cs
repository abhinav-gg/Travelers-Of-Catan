using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NEAGame;
using System;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public class GameUI : UnityUI
{

    [Obsolete("Unity does not allow for this")] [HideInInspector] internal new TravelersOfCatan game;

    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;
    public GameObject PlayerPrefab;

    public Tile[] resources = new Tile[6];
    public GridLayout gridLayout;
    public Tilemap tilemap;

    public GameObject[] hexes = new GameObject[19];
    public NodeButton[] nodes = new NodeButton[54];
    public List<ConnectionButton> connections = new List<ConnectionButton>();

    bool hasInitializedBoard = false;

    GameUIAnimator anim;

    private void Awake()
    {
        NameGetOverlay = base.NameGetOverlay;
    }

    private void Start()
    {
        anim = GetComponent<GameUIAnimator>();
    }


    public void BeginTurn()
    {

    }

    public void ConnectionButtonPressed()
    {

    }

    public void OnPlayerMove()
    {
        //anim.MoveButtonPlay();
        //game.movePlayer();
    }

    public ConnectionButton FindConnectionGameObject(Vector3 v1, Vector3 v2)
    {
        List<Vector3> list = new List<Vector3>();
        foreach (var con in connections)
        {
            list = (from node in con.nodes select node.NodePos).ToList<Vector3>();
            if (list.Contains(v1) && list.Contains(v2))
            {
                return con;
            }
        }
        return null;
    }


    internal void UpdateBoard(Board board)
    {

        if (!hasInitializedBoard)
        {
            hasInitializedBoard = true;

            int resourceID;
            Vector3Int gridPos;
            foreach (KeyValuePair<System.Numerics.Vector3, Resource> entry in board.GetResourcesOnBoard()) 
            {
                
                resourceID = Array.IndexOf(Resource.resources, entry.Value.ToString());// is the index of the resource in the list of resources
                gridPos = CubicToOddRow(entry.Key);
                tilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), resources[resourceID]);


            }
            
            
            foreach (Node n in board.GetAllNodes())
            {
                //node.transform.position = //  this is a pronblem;
                NodeButton nodeui = Instantiate(NodePrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("NodeParent").transform).GetComponent<NodeButton>();
                nodeui.node = n;
                nodeui.NodePos = ConvertVector(n.position);
                nodeui.transform.position = GetNodeGlobalPos(n);
            }

            Vector3 conPos;
            ConnectionButton GO;
            foreach (var nodeCon in board.connections)
            {
                 foreach (var con in nodeCon.Value)
                {
                    GO = FindConnectionGameObject(ConvertVector(nodeCon.Key), ConvertVector(con.Key));
                    conPos = GetConnectionGlobalPos(nodeCon.Key, con.Key);
                    if (GO is null)
                    {
                        ConnectionButton conui = Instantiate(ConnectionPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("ConnectionParent").transform).GetComponent<ConnectionButton>();
                        conui.connection = con.Value;
                        conui.transform.position = conPos;
                        conui.nodes = new NodeButton[2];
                        conui.nodes[0] = FindNodeGameObject(ConvertVector(nodeCon.Key));
                        conui.nodes[1] = FindNodeGameObject(ConvertVector(con.Key));
                        connections.Add(conui);

                    }

                }  
            }

            foreach (Player pl in Interface.game.gamePlayers)
            {
                Debug.Log(pl);
                PlayerButton playUI = Instantiate(PlayerPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("PlayerParent").transform).GetComponent<PlayerButton>();
                playUI.player = pl;
                playUI.gameObject.name = pl.playerName;
                playUI.transform.position = GetNodeGlobalPos(Interface.game.board.GetNode(pl.position));
            
            }



        }
    }

    public void UpdateConnection(Vector3 n1, Vector3 n2, Connection connection)
    {
        ConnectionButton conui = FindConnectionGameObject(n1, n2);
        conui.connection = connection;
        conui.UpdateConnection();
    }

    public Vector3 GetConnectionGlobalPos(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2)
    {
        HashSet < System.Numerics.Vector3 > starthexes = new HashSet<System.Numerics.Vector3>();
        HashSet < System.Numerics.Vector3 > endhexes = new HashSet<System.Numerics.Vector3>();
        float totalX = 0f;
        float totalY = 0f;
        float Z = 0f;

        foreach (var vec in Interface.game.board.GetNode(v1).GetHexNeighbours())
        {
            starthexes.Add(vec);
        }
        foreach (var vec in Interface.game.board.GetNode(v2).GetHexNeighbours())
        {
            endhexes.Add(vec);
        }
        Vector3 center;
        foreach (var HexPos in starthexes.Except(endhexes).Union(endhexes.Except(starthexes)))
        {
            center = GetHexGlobalPos(CubicToOddRow(HexPos));
            totalX += center.x;
            totalY += center.y;
            Z = center.z;

        }
        
        return new Vector3(totalX / 2, totalY / 2, Z);

    }

    public Vector3 GetNodeGlobalPos(Node node)
    {
        Vector3 center;
        float totalX = 0f;
        float totalY = 0f;
        float Z = 0f;
        foreach (var vec in node.GetHexNeighbours())
        {
            center = GetHexGlobalPos(CubicToOddRow(vec));

            totalX += center.x;
            totalY += center.y;
            Z = center.z;
        }
        float avgX = totalX / 3;
        float avgY = totalY / 3;
        return new Vector3(avgX, avgY, Z);
    }

    public Vector3 GetHexGlobalPos(Vector3 cellPos)
    {
        Vector3Int Pos = new Vector3Int((int)cellPos.x, (int)cellPos.y, 0);
        return gridLayout.CellToWorld(Pos);
    }



    public System.Numerics.Vector3 ConvertVector(Vector3 vec)
    {
        return new System.Numerics.Vector3(vec.x, vec.y, vec.z);
    }

    public static Vector3 ConvertVector(System.Numerics.Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }


    public static Vector3Int CubicToOddRow(System.Numerics.Vector3 vec)
    {
        int col = (int)(vec.Z + ((vec.X - ((int)vec.X & 1)) / 2));
        int row = (int)vec.X;
        return new Vector3Int(col, row, 0);
    }


    public NodeButton FindNodeGameObject(Vector3 pos)
    {
        foreach (NodeButton n in FindObjectsOfType<NodeButton>())
        {
            if (n.NodePos == pos)
            {
                return n;
            }
        }
        return null;
    }

    internal Node GetUserNodeChoice(Node[] options)
    {

        // Add LeanTween Animation to the selected nodes here!



        throw new NotImplementedException();
    }

    internal void DisplayPlayers(List<Player> players)
    {
        //foreach (Player player in players)
        //{
        //    Debug.Log(player.playerName);
        //
        //}
    }



}
