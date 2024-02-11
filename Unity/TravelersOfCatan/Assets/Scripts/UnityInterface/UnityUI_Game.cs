using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using NEAGame;

// Second half of the UnityUI partial class, containing the game-specific methods and variables.

[Serializable] // This is serializable so it can be viewed in the Unity inspector
public partial class UnityUI
{
    [Header("UI overlay prefabs and constants")]
    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;
    public GameObject PlayerPrefab;
    public GameObject PlayerUI;
    public GameObject inventoryPopup;
    public GameObject shoppingPopup;
    public GameObject TradePopup;
    public GameObject PlayerChoicePopup;
    public GameObject AlertPopup;
    public GameObject PauseSettings;
    public GameObject CardObj;
    public GameObject GameSavePopup;
    public Tile[] resources = new Tile[6];

    public GridLayout gridLayout;
    public Tilemap tilemap;
    public List<NodeButton> nodes;

    public float Timer = 0.0f;
    public bool TimerActive = false;


    private PlayerUIOverlay overlay;
    private NodeButton SelectedNode;
    private IEnumerator coroutine;

    // Update is called once per frame
    void Update()
    {
        if (TimerActive)
        {
            Timer = Mathf.Clamp(Timer - Time.deltaTime, 0, int.MaxValue);
        } // updates the in game timer if it is active
    }
    
    // interface method to begin the new user's turn
    void UI.BeginTurn(float time)
    {
        AudioManager.i.Play("Success");
        Timer = time;
        TimerActive = true;
        // setup the player's main overlay
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
            StartCoroutine(WaitForTurnToEnd());
        }
        overlay.PauseInput.onClick.AddListener(PauseButton);
        overlay.ColorMe.color = textToColor(game.GetCurrentPlayer().color);
        overlay.PlayerScore.text = game.GetCurrentPlayer().getVictoryPoints().ToString();
        overlay.PlayerName.text = game.GetCurrentPlayer().playerName;
        GetPlayerGameObject(game.GetCurrentPlayer().GetID()).isCurrentPlayer = true;

        // set camera position to player position on board!
        LeanTween.cancel(Camera.main.gameObject);
        LeanTween.move(Camera.main.gameObject, GetPlayerGameObject(game.GetCurrentPlayer().GetID()).transform.position + new Vector3(0f, 0f, -10f), 0.3f).setEase(LeanTweenType.easeInSine);
    }

    // method to begin the AI's turn
    IEnumerator BeginAI()
    {
        // call the AI BRS in a thread and wait for it to finish
        Thread t = new Thread(() => ((AI)game.GetCurrentPlayer()).BRS());
        t.Start();
        while (t.IsAlive)
        {
            if (Timer <= 5f)
            {
                break; // if the timer is less than 5 seconds, break out of the loop and end the search. The best recorded move found so far will instead be used.
            }
            yield return null;
        }
        FindAnyObjectByType<GamePauseOverlay>()?.CloseGUI(); // fixes a sepcific bug where the player can get the AI two turns by exiting while the ai is making its moves
        t.Abort();
        game.DisplayAIMoves(); 
        yield return new WaitForSeconds(2f);
        EndTurn();
    }

    // method to setup the game scene
    void SetupGameScene()
    {
        gridLayout = FindObjectOfType<GridLayout>();
        tilemap = FindObjectOfType<Tilemap>();
        nodes = new List<NodeButton>();
    }

    // async method to count down until the player is out of time for their turn
    IEnumerator WaitForTurnToEnd()
    {
        bool startedCD = false;
        while (Timer > 0.0f)
        {
            yield return null;
            if (Timer < 5f && !startedCD)
            {
                AudioManager.i.Play("Countdown");
                startedCD = true;
            }
        }
        EndTurn();
    }

    // onclick method to pause the game
    public void PauseButton()
    {
        TimerActive = false;
        StartCoroutine(GoToPause());
    }

    // method to load the pause overlay
    IEnumerator GoToPause()
    {
        SceneTransition.i.PlayAnimation();
        yield return new WaitForSeconds(0.5f);
        Instantiate(PauseSettings); 
    }

    // function called when the player wishes to move 
    public void OnPlayerMove()
    {
        StopAllPlayerCoroutines();
        game.attemptPlayerMove();
    }

    // method to get the time in a nice string format
    public string GetTime()
    {
        // take float time of seconds and convert to string of minutes and seconds
        int minutes = Mathf.FloorToInt(Timer / 60F);
        int seconds = Mathf.FloorToInt(Timer - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        return niceTime;
    }

    // interface method to get the time in seconds
    float UI.GetTimer()
    {
        return Timer;
    }

    // method for ending the player's turn
    public void EndTurn()
    {
        AudioManager.i.StopAll();
        if (!overlay.isZoomed)
        {
            overlay.ZoomButton();
        }
        GetPlayerGameObject(Interface.game.GetCurrentPlayer().GetID()).isCurrentPlayer = false;

        StopAllPlayerCoroutines();

        // Close all GUIS
        CloseAllGameUIs();
        Interface.game.EndTurn();
    }

    // method to close all game UIs
    public void CloseAllGameUIs()
    {
        FindAnyObjectByType<InventoryPopup>()?.CloseGUI();
        FindAnyObjectByType<ShopOverlay>()?.CloseGUI();
        FindAnyObjectByType<TradingInterface>()?.CloseGUI();
        FindAnyObjectByType<PlayerChoice>()?.CloseGUI();
        FindAnyObjectByType<PopupController>()?.CloseGUI();
        if (FindAnyObjectByType<PlayerUIOverlay>())
            Destroy(FindObjectOfType<PlayerUIOverlay>().gameObject);
    }

    // method to find the connection gameobject between two nodes
    public ConnectionAnimator FindConnectionGameObject(Node v1, Node v2)
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

    // interface method to begin the game
    void UI.BeginGame(float timeleft)
    {
        StartCoroutine(WaitThenLoad(timeleft));
    }

    // method to wait for the game to load before starting the first player's turn
    IEnumerator WaitThenLoad(float timeleft)
    {
        while (game == null)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        DisplayPlayers();
        DisplayBoard();
        yield return 0;
        game.StartTurn(timeleft); 
    }

    // method to initialise the game board on the screen
    public void DisplayBoard()
    {
        int resourceID;
        Vector3Int gridPos;
        foreach (var entry in game.board.GetResourcesOnBoard())
        {

            resourceID = Array.IndexOf(Resource.RESOURCES, entry.Value.ToString());// is the index of the resource in the list of resources
            gridPos = CubicToOddRow(entry.Key);
            tilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), resources[resourceID]);
        }

        foreach (Node n in game.board.GetAllNodes())
        {
            
            NodeButton nodeui = Instantiate(NodePrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("NodeParent").transform).GetComponent<NodeButton>();
            nodeui.node = n;
            nodeui.NodePos = ConvertVector(n.position);
            nodeui.transform.position = GetNodeGlobalPos(n);
            nodeui.UpdateSettlement();

            nodes.Add(nodeui);
        }
    }

    // method to initialise the players on the screen
    public void DisplayPlayers()
    {
        foreach (Player pl in game.gamePlayers)
        {
            PlayerAnimator playUI = Instantiate(PlayerPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("PlayerParent").transform).GetComponent<PlayerAnimator>();
            playUI.player = pl;
            playUI.gameObject.name = pl.playerName;
            playUI.GetComponent<SpriteRenderer>().color = textToColor(pl.color);
            playUI.transform.position = GetNodeGlobalPos(game.board.GetNode(pl.position));

        }
    }

    // interface method to animate the player along a path of nodes
    void UI.UpdatePlayer(Stack<Node> path)
    {
        StartCoroutine(MovePlayerAlongPath(path));
    }
    IEnumerator MovePlayerAlongPath(Stack<Node> path)
    {
        overlay.FinishMove();
        while (path.Count > 0)
        {
            Node otherNode = path.Pop();
            Vector3 pos = GetNodeGlobalPos(otherNode);
            LeanTween.move(GetPlayerGameObject(Interface.game.GetCurrentPlayer().GetID()).gameObject, pos, 0.5f).setEase(LeanTweenType.easeInOutElastic);
            LeanTween.move(Camera.main.gameObject, pos + new Vector3(0f, 0f, -10f), 0.3f).setEase(LeanTweenType.easeInSine).setDelay(0.5f);
            AudioManager.i.Play("UIEffect");
            yield return new WaitForSeconds(0.55f);
        }
        yield return null;
    }

    // interface method to update the connection between two nodes
    void UI.UpdateConnection(Node otherNode, Node current, Connection con)
    {
        var x = current.position;
        var y = otherNode.position;
        ConnectionAnimator conui = FindConnectionGameObject(current, otherNode);
        if (conui == null)
        {
            conui = Instantiate(ConnectionPrefab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("ConnectionParent").transform).GetComponent<ConnectionAnimator>();
            conui.UpdateConnection(ConvertVector(x), ConvertVector(y));
            conui.transform.position = GetConnectionGlobalPos(current, otherNode);
        }
        conui.connection = con;
        conui.UpdateDisplay();
    }

    // method to get the color of the specified player
    public Color GetPlayerColor(int playerID)
    {
        foreach (Player pdl in game.gamePlayers)
        {
            if (pdl.GetID() == playerID)
            {
                return textToColor(pdl.color);
            }
        }
        Debug.LogError("Failed to find color");
        return Color.clear;
    }

    // interface method to update the settlement on a node
    void UI.UpdateSettlement(Node otherNode)
    {
        var x = otherNode.position;
        NodeButton nodeui = FindNodeGameObject(ConvertVector(x)); // no need to instantiate as the nodes were made on startup
        nodeui.UpdateSettlement();
    }

    // method to get the player gameobject for the specified player
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Used to represent and convert hexagonal grid of nodes and connection between cubic coordinates, odd row coordinates and global positions
    // Skill A: Use of Complex Mathematical Model
    
    public Vector3 GetConnectionGlobalPos(Node v1, Node v2)
    {
        HashSet<System.Numerics.Vector3> starthexes = new HashSet<System.Numerics.Vector3>();
        HashSet<System.Numerics.Vector3> endhexes = new HashSet<System.Numerics.Vector3>();
        float totalX = 0f;
        float totalY = 0f;
        float Z = 0f;

        foreach (var vec in v1.GetHexNeighbours())
        {
            starthexes.Add(vec);
        }
        foreach (var vec in v2.GetHexNeighbours())
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

    // get the global position of a node
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
        // to get the center of the node, we average the x and y of the 3 hexes around it
        return new Vector3(avgX, avgY, Z);
    }

    // Get the global position of a hexagonal grid cell
    public Vector3 GetHexGlobalPos(Vector3 cellPos)
    {
        Vector3Int Pos = new Vector3Int((int)cellPos.x, (int)cellPos.y, 0);
        return gridLayout.CellToWorld(Pos);
    }

    // convert a System.Numerics.Vector3 to a Unity Vector3
    public static Vector3 ConvertVector(System.Numerics.Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }

    // convert the cubic coordinates of the hexagonal grid used in game calculations to odd row coordinates used in Unity
    public static Vector3Int CubicToOddRow(System.Numerics.Vector3 vec)
    {
        int col = (int)(vec.Z + ((vec.X - ((int)vec.X & 1)) / 2));
        int row = (int)vec.X;
        return new Vector3Int(col, row, 0);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // convert a string to a color
    public static Color textToColor(string color)
    {
        switch (color)
        {
            case "blue":
                return Color.blue;
            case "cyan":
                return Color.cyan;
            case "green":
                return Color.green;
            case "grey":
                return Color.grey;
            case "magenta":
                return Color.magenta;
            case "red":
                return Color.red;
            case "white":
                return Color.white;
            case "yellow":
                return Color.yellow;
            default:
                return Color.clear;
        }
    }

    // method to get the node gameobject at the specified position
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

    // interface method to show a resource being given to the player
    void UI.ShowResource(System.Numerics.Vector3 u, NEAGame.Resource resource, System.Numerics.Vector3 endDest)
    {
        StartCoroutine(AnimateResource(u, resource, endDest));
    }
    IEnumerator AnimateResource(System.Numerics.Vector3 u, NEAGame.Resource resource, System.Numerics.Vector3 Dest)
    {
        Vector3 spawnpos;
        yield return new WaitForSeconds(0.5f);
        Vector3 destination = ConvertVector(Dest);
        if (destination == new Vector3(0, 0, 0))
        {
            spawnpos = GetHexGlobalPos(CubicToOddRow(u));
            destination = overlay.InventoryInput.transform.position;
        }
        else
        {
            destination = GetNodeGlobalPos(game.board.GetNode(Dest));
            spawnpos = GetNodeGlobalPos(game.board.GetNode(u));
        }
        CardCollection card = Instantiate(CardObj, spawnpos, Quaternion.identity).GetComponent<CardCollection>();
        card.SetCard(resource.GetHashCode(), destination);
    }

    // interface method to get the user's choice of node
    void UI.GetUserNodeChoice(Node[] options, Action<Node> callback)
    {
        SelectedNode = null;
        foreach (Node choice in options)
        {
            NodeButton node = FindNodeGameObject(ConvertVector(choice.position));
            node.EnableButton();
        }
        coroutine = WaitForNodeChoice(callback);
        StartCoroutine(coroutine);
    }

    // method to wait for the user to choose a node
    IEnumerator WaitForNodeChoice(Action<Node> callback) // pass in function for moving vs buying
    {
        while (SelectedNode is null)
        {
            yield return new WaitForSeconds(0.01f);
        }
        callback(SelectedNode.node);
        SelectedNode = null;
    }

    // method to handle the user clicking on a node option
    public void OnNodeClick(NodeButton node)
    {
        SelectedNode = node;
        foreach (NodeButton n in nodes)
        {
            n.DisableButton();
        }
    }

    // method to stop all player coroutines
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

    // method to display the inventory of the current player
    public void OpenInventory()
    {
        StopAllPlayerCoroutines();
        InventoryPopup inv = Instantiate(inventoryPopup).GetComponent<InventoryPopup>();
        foreach (KeyValuePair<Resource, int> entry in Interface.game.GetCurrentPlayer().getResources())
        {
            inv.Display(entry.Key.ToString(), entry.Value);
        }

    } 
    
    // method to open the player selection overlay for trading
    public IEnumerator OpenTrade()
    {
        StopAllPlayerCoroutines();
        SceneTransition.i.PlayAnimation();
        yield return new WaitForSeconds(0.25f);
        PlayerChoice overlay = Instantiate(PlayerChoicePopup).GetComponent<PlayerChoice>();
        List<Player> options = new List<Player>();
        foreach (Player pl in Interface.game.gamePlayers)
        {
            if (pl != Interface.game.GetCurrentPlayer() && !pl.isPlayerAI())
            {
                options.Add(pl);
            }
        }
        overlay.Setup(options);
        yield return null;
    }

    // method called when the player selects a partner to trade with
    public void SelectPartner(Player pl)
    {
        StartCoroutine(DisplayTrade(pl));
    }

    // function to display the trade overlay between players
    IEnumerator DisplayTrade(Player pl)
    {
        SceneTransition.i.PlayAnimation();
        yield return new WaitForSeconds(0.1f);
        TradingInterface inv = Instantiate(TradePopup).GetComponent<TradingInterface>();
        inv.Setup(Interface.game.GetCurrentPlayer(), pl);
        yield return null;
    }

    // function to register a trade between players
    public IEnumerator RegisterTrade(Dictionary<int, int> trades, Player other)
    {
        yield return new WaitForSeconds(1.25f);
        Dictionary<Resource, int> GameDict = new Dictionary<Resource, int>();
        foreach (var entry in trades)
        {
            GameDict.Add(new Resource(entry.Key + 1), entry.Value);
        }
        game.CompleteTrade(other, GameDict);
    }



    // methods to handle the shop event
    public void OpenShop()
    {
        StopAllPlayerCoroutines();
        StartCoroutine(DisplayShop());
    }
    IEnumerator DisplayShop()
    {
        ShopOverlay sv = Instantiate(shoppingPopup).GetComponent<ShopOverlay>();
        yield return new WaitForSeconds(0.1f);

    }

    // method to attempt a purchase from the shop
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

    // interface method to handle the end of the game
    void UI.HandleWinner(Player winner)
    {
        AudioManager.i.StopAll();
        AudioManager.i.Play("Victory");
        CloseAllGameUIs();
        SceneTransition.i.SendToScene("Victory");
    }
}
