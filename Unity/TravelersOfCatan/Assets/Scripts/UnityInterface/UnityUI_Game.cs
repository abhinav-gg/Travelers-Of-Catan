using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NEAGame;
using System;
using System.Linq;


[Serializable]
public partial class UnityUI
{

    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;
    public GameObject PlayerPrefab;
    public GameObject PlayerUI;
    public GameObject inventoryPopup;

    public Tile[] resources = new Tile[6];
    public GridLayout gridLayout;
    public Tilemap tilemap;

    public List<NodeButton> nodes = new List<NodeButton>();
    public List<ConnectionButton> connections = new List<ConnectionButton>();

    bool hasInitializedBoard = false;

    public float Timer = 0.0f;
    public bool TimerActive = false;



    PlayerUIOverlay overlay;
    NodeButton SelectedNode;


    void FixedUpdate()
    {
        if (TimerActive)
        {
            if (!LeanTween.isTweening(Camera.main.gameObject))
                LeanTween.move(Camera.main.gameObject, GetCurrentPlayerGlobalPos() + new Vector3(0, 0, -10), 0.3f).setEase(LeanTweenType.easeInSine);
            
            Timer -= Time.deltaTime;
        }
    }

    public void BeginTurn()
    {
        overlay = Instantiate(PlayerUI).GetComponent<PlayerUIOverlay>();
        overlay.EndTurnInput.onClick.AddListener(EndTurn);
        overlay.MoveInput.onClick.AddListener(OnPlayerMove);
        overlay.InventoryInput.onClick.AddListener(OpenInventory);
        overlay.PlayerName.text = Interface.game.GetCurrentPlayerName();


        Timer = 300.0f; // ASSIGN TO CONSTANT
        TimerActive = true;
        StartCoroutine(WaitForTurnToEnd());

        // set camera position to player position on baord!
        Camera.main.transform.position = GetCurrentPlayerGlobalPos() + new Vector3(0, 0, -10);
    }

    public void ConnectionButtonPressed()
    {

    }

    IEnumerator WaitForTurnToEnd()
    {
        while (Timer > 0.0f)
        {
            yield return null;
        }
        EndTurn();
    }

    public void OnPlayerMove()
    {
        //anim.MoveButtonPlay();
        Interface.game.attemptPlayerMove();
    }

    public Vector3 GetCurrentPlayerGlobalPos()
    {
        return FindPlayerGameObject(Interface.game.GetCurrentPlayerName()).transform.position;
    }

    public string GetTime()
    {
        // take float time of seconds and convert to string of minutes and seconds
        int minutes = Mathf.FloorToInt(Timer / 60F);
        int seconds = Mathf.FloorToInt(Timer - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        return niceTime;
    }

    public void EndTurn()
    {

        foreach (NodeButton n in nodes)
        {
            n.DisableButton();
        }
        // anim.EndTurnButtonPlay();
        LeanTween.moveLocalY(overlay.EndTurnInput.gameObject, 10, 0.5f).setEase(LeanTweenType.easeInBack);
        LeanTween.scale(overlay.EndTurnInput.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutBounce).setOnComplete(() => { 
        
            Destroy(FindObjectOfType<PlayerUIOverlay>().gameObject);
            Interface.game.EndTurn();
        
        
        });

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

    public PlayerButton FindPlayerGameObject(string PlayerName)
    {
        foreach (var a in FindObjectsOfType<PlayerButton>())
        {
            if (a.player.playerName == PlayerName)
            {
                return a;
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
                if (n.status.GetStatus() == "City")
                {
                    nodeui.SetVillage();
                    nodeui.UpgradeVillage();
                }
                nodes.Add(nodeui);
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

                PlayerButton playUI = Instantiate(PlayerPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("PlayerParent").transform).GetComponent<PlayerButton>();
                playUI.player = pl;
                playUI.gameObject.name = pl.playerName;
                playUI.transform.position = GetNodeGlobalPos(Interface.game.board.GetNode(pl.position));
            
            }

        }

    }


    public PlayerButton GetPlayerGameObject(string PlayerName)
    {
        foreach (var a in FindObjectsOfType<PlayerButton>())
        {
            if (a.player.playerName == PlayerName)
            {
                return a;
            }
        }
        return null;
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
        foreach (NodeButton n in nodes)
        {
            if (n.NodePos == pos)
            {
                return n;
            }
        }
        Debug.LogError(pos);
        return null;
    }

    void UI.GetUserNodeChoice(Node[] options, Action<Node> method)
    {

        // Add LeanTween Animation to the selected nodes here!

        SelectedNode = null;
        foreach (Node choice in options)
        {
            NodeButton node = FindNodeGameObject(ConvertVector(choice.position));
            node.EnableButton();
        }
        StartCoroutine(WaitForNodeChoice(method));
    }

    IEnumerator WaitForNodeChoice(Action<Node> method) // pass in function for moving vs buying
    {

        while (SelectedNode is null)
        {
            yield return new WaitForSeconds(0.01f);
        }
        method(SelectedNode.node);
        LeanTween.move(GetPlayerGameObject(Interface.game.GetCurrentPlayerName()).gameObject, SelectedNode.transform.position, 0.5f).setEase(LeanTweenType.easeInOutElastic);
    }

    public void OnNodeClick(NodeButton node)
    {
        SelectedNode = node;
        foreach (NodeButton n in nodes)
        {
            n.DisableButton();
        }
    }


    public void OpenInventory()
    {
        StartCoroutine(DisplayInventory());
    }


    IEnumerator DisplayInventory()
    {
        InventoryPopup inv = Instantiate(inventoryPopup).GetComponent<InventoryPopup>();
        foreach (KeyValuePair<Resource, int> entry in Interface.game.GetCurrentPlayerInventory())
        {
            inv.Display(entry.Key.ToString(), entry.Value);
            yield return new WaitForSeconds(0.1f);
        }

    }

}
