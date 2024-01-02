using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Threading;
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
    public GameObject shoppingPopup;
    public GameObject pausePopup;


    public Tile[] resources = new Tile[6];
    public GridLayout gridLayout;
    public Tilemap tilemap;

    public List<NodeButton> nodes = new List<NodeButton>();

    bool hasInitializedBoard = false;

    public float Timer = 0.0f;
    public bool TimerActive = false;

    private IEnumerator coroutine;

    PlayerUIOverlay overlay;
    NodeButton SelectedNode;



    void Update()
    {
        if (TimerActive)
        {
            
            Timer = Mathf.Clamp(Timer - Time.deltaTime, 0, int.MaxValue);
        }
    }


    void UI.BeginTurn(float time)
    {
        Timer = time;
        TimerActive = true;
        overlay = Instantiate(PlayerUI).GetComponent<PlayerUIOverlay>();
        if (game.GetCurrentPlayer().isPlayerAI())
        {
            overlay.SetAI();
            StartCoroutine(BeginAI());

        }
        else
        {
            overlay.ShopInput.onClick.AddListener(OpenShop);
            overlay.UndoInput.onClick.AddListener(game.UndoPlayerAction);
            overlay.EndTurnInput.onClick.AddListener(EndTurn);
            overlay.InventoryInput.onClick.AddListener(OpenInventory);
            overlay.PauseInput.onClick.AddListener(PauseButton);
            StartCoroutine(WaitForTurnToEnd());
        }

        overlay.PlayerName.text = game.GetCurrentPlayer().playerName;
        GetPlayerGameObject(game.GetCurrentPlayer().GetID()).isCurrentPlayer = true;

        // set camera position to player position on board!
        LeanTween.move(Camera.main.gameObject, GetPlayerGameObject(game.GetCurrentPlayer().GetID()).transform.position + new Vector3(0f, 0f, -10f), 0.3f).setEase(LeanTweenType.easeInSine);

    }

    IEnumerator BeginAI()
    {

        // call the AI BRS in a thread and wait for it to finish
        Thread t = new Thread(() => ((AI)game.GetCurrentPlayer()).BRS(int.MinValue, int.MaxValue));
        t.Start();
        while (t.IsAlive)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        game.DisplayAIMoves();

        yield return new WaitForSeconds(7.5f);
        EndTurn();

    }


    void SetupGameScene()
    {
        gridLayout = FindObjectOfType<GridLayout>();
        tilemap = FindObjectOfType<Tilemap>();

    }

    IEnumerator WaitForTurnToEnd()
    {
        while (Timer > 0.0f)
        {
            yield return null;
        }
        EndTurn();
    }

    public void PauseButton()
    {
        TimerActive = false;
        GameObject overlay = Instantiate(pausePopup);
    }

    public void OnPlayerMove()
    {
        
        StopAllPlayerCoroutines();
        //anim.MoveButtonPlay();
        game.attemptPlayerMove();
    }

    public string GetTime()
    {
        // take float time of seconds and convert to string of minutes and seconds
        int minutes = Mathf.FloorToInt(Timer / 60F);
        int seconds = Mathf.FloorToInt(Timer - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        return niceTime;
    }

    float UI.GetTimer()
    {
        return Timer;
    }

    public void EndTurn()
    {
        if (!overlay.isZoomed)
        {
            overlay.ZoomButton();
        }
        GetPlayerGameObject(Interface.game.GetCurrentPlayer().GetID()).isCurrentPlayer = false;

        StopAllPlayerCoroutines();

        // Close all GUIS
        FindAnyObjectByType<InventoryPopup>()?.CloseGUI();
        FindAnyObjectByType<ShopOverlay>()?.CloseGUI();
        // FindAnyObjectByType<TradeOverlay>()?.CloseGUI();



        if (!game.GetCurrentPlayer().isPlayerAI())
        {
            LeanTween.moveLocalY(overlay.EndTurnInput.gameObject, 10, 0.5f).setEase(LeanTweenType.easeInBack);
            LeanTween.scale(overlay.EndTurnInput.gameObject, new Vector3(0f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInOutBounce).setOnComplete(() => {
                Destroy(FindObjectOfType<PlayerUIOverlay>().gameObject);
                Interface.game.EndTurn();

            });
        }
        else
        {
            Destroy(FindObjectOfType<PlayerUIOverlay>().gameObject);
            Interface.game.EndTurn();
        }   
        
    }

    public ConnectionAnimator FindConnectionGameObject(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2)
    {
        Vector3 searchingpos = GetConnectionGlobalPos(v1, v2);
        foreach (ConnectionAnimator con in FindObjectsOfType<ConnectionAnimator>())
        {
            if (con.transform.position == searchingpos)
            {
                return con;
            }
        }
        return null;
    }

    void UI.DisplayBoard(Board board)
    {
        if (!hasInitializedBoard)
        {
            hasInitializedBoard = true;

            int resourceID;
            Vector3Int gridPos;
            foreach (var entry in board.GetResourcesOnBoard())
            {

                resourceID = Array.IndexOf(Resource.resources, entry.Value.ToString());// is the index of the resource in the list of resources
                gridPos = CubicToOddRow(entry.Key);
                tilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), resources[resourceID]);

            }

            foreach (Node n in board.GetAllNodes())
            {
                
                NodeButton nodeui = Instantiate(NodePrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("NodeParent").transform).GetComponent<NodeButton>();
                nodeui.node = n;
                nodeui.NodePos = ConvertVector(n.position);
                nodeui.transform.position = GetNodeGlobalPos(n);
                nodeui.UpdateSettlement();

                nodes.Add(nodeui);

            }

        }

    }

    void UI.DisplayPlayers(List<Player> gamePlayers)
    {
        foreach (Player pl in gamePlayers)
        {

            PlayerAnimator playUI = Instantiate(PlayerPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("PlayerParent").transform).GetComponent<PlayerAnimator>();
            playUI.player = pl;
            playUI.gameObject.name = pl.playerName;
            playUI.transform.position = GetNodeGlobalPos(game.board.GetNode(pl.position));

        }
    }

    void UI.UpdatePlayer(Stack<Node> path)
    {
        StartCoroutine(MovePlayerAlongPath(path));
    }

    public IEnumerator MovePlayerAlongPath(Stack<Node> path)
    {
        overlay.FinishMove();
        while (path.Count > 0)
        {
            Node otherNode = path.Pop();
            Vector3 pos = GetNodeGlobalPos(otherNode);
            LeanTween.move(GetPlayerGameObject(Interface.game.GetCurrentPlayer().GetID()).gameObject, pos, 0.5f).setEase(LeanTweenType.easeInOutElastic);
            LeanTween.move(Camera.main.gameObject, pos + new Vector3(0f, 0f, -10f), 0.3f).setEase(LeanTweenType.easeInSine).setDelay(0.5f);
            yield return new WaitForSeconds(0.55f);
        }
        yield return null;

    }



    void UI.UpdateConnection(Node otherNode, Node current, Connection con)
    {
        var x = current.position;
        var y = otherNode.position;
        ConnectionAnimator conui = FindConnectionGameObject(x, y);
        if (conui == null)
        {
            conui = Instantiate(ConnectionPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("ConnectionParent").transform).GetComponent<ConnectionAnimator>();
            conui.UpdateConnection(ConvertVector(x), ConvertVector(y));
            conui.transform.position = GetConnectionGlobalPos(x, y);
        }
        conui.connection = con;
        conui.UpdateDisplay();
        
    }

    public Color GetPlayerColor(int playerID)
    {
        // return color from string
        return Color.cyan;

    }
    void UI.UpdateSettlement(Node otherNode)
    {
        var x = otherNode.position;
        NodeButton nodeui = FindNodeGameObject(ConvertVector(x)); // no need to instantiate as the nodes were made on startup
        nodeui.UpdateSettlement();
    }

    public PlayerAnimator GetPlayerGameObject(int playerID = -1)
    {
        if (playerID == -1)
        {
            playerID = game.GetCurrentPlayer().GetID();
        }
        foreach (var a in FindObjectsOfType<PlayerAnimator>())
        {
            if (a.player.GetID() == playerID)
            {
                return a;
            }
        }
        return null;
    }

    public Vector3 GetConnectionGlobalPos(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2)
    {
        HashSet<System.Numerics.Vector3> starthexes = new HashSet<System.Numerics.Vector3>();
        HashSet<System.Numerics.Vector3> endhexes = new HashSet<System.Numerics.Vector3>();
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


    void UI.GetUserNodeChoice(Node[] options, Action<Node> callback)
    {

        // Add LeanTween Animation to the selected nodes here!

        SelectedNode = null;
        foreach (Node choice in options)
        {
            NodeButton node = FindNodeGameObject(ConvertVector(choice.position));
            node.EnableButton();
        }
        coroutine = WaitForNodeChoice(callback);
        StartCoroutine(coroutine);
    }

    IEnumerator WaitForNodeChoice(Action<Node> callback) // pass in function for moving vs buying
    {

        while (SelectedNode is null)
        {
            yield return new WaitForSeconds(0.01f);
        }
        callback(SelectedNode.node);
        SelectedNode = null;
    }

    public void OnNodeClick(NodeButton node)
    {
        SelectedNode = node;
        foreach (NodeButton n in nodes)
        {
            n.DisableButton();
        }
    }

    public void StopAllPlayerCoroutines()
    {
        foreach (NodeButton n in nodes)
        {
            n.DisableButton();
        }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

    }

    /// <summary>
    /// Inventory handler
    /// </summary>


    public void OpenInventory()
    {
        //JSON_manager.SAVEGAME(game, "TESTFINDME");
        StopAllPlayerCoroutines();
        StartCoroutine(DisplayInventory());
    }


    IEnumerator DisplayInventory()
    {
        InventoryPopup inv = Instantiate(inventoryPopup).GetComponent<InventoryPopup>();
        foreach (KeyValuePair<Resource, int> entry in Interface.game.GetCurrentPlayer().getResources())
        {
            inv.Display(entry.Key.ToString(), entry.Value);
            yield return new WaitForSeconds(0.1f);
        }

    }


    /// <summary>
    /// Shopping handler
    /// </summary

    public void OpenShop()
    {
        StopAllPlayerCoroutines();

        StartCoroutine(DisplayShop());

    }


    public void AttemptPurchase(string name)
    {
        switch (name)
        {
            case "Road":
                game.tryPurchaseRoad();
                break;
            case "Wall":
                game.tryPurchaseWall();
                break;
            case "Village":
                game.tryPurchaseVillage();
                break;
            case "City":
                game.tryPurchaseCity();
                break;
            default:
                Debug.LogError("Invalid purchase name");
                break;
        }
    }

    IEnumerator DisplayShop()
    {
        ShopOverlay sv = Instantiate(shoppingPopup).GetComponent<ShopOverlay>();
        yield return new WaitForSeconds(0.1f);

    }


    void UI.HandleWinner(Player winner)
    {
        EndTurn();

        // Show winner screen



    }

}