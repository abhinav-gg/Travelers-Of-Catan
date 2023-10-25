using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAGame;

public class GameUI : UnityUI
{


    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;

    public Sprite[] resources = new Sprite[6];

    public GameObject[] hexes = new GameObject[19];
    public NodeButton[] nodes = new NodeButton[54];
    public ConnectionButton[] connections;

    bool hasInitializedBoard = false;

    // Start is called before the first frame update
    void Start()
    {
        //game = new TravelersOfCatan(new UnityUIManager(), 2, 2);
        //game.startGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void UpdateBoard(Board board)
    {
        if (!hasInitializedBoard)
        {
            hasInitializedBoard = true;

            foreach (KeyValuePair<System.Numerics.Vector3, Resource> entry in board.GetResourcesOnBoard()) 
            {
                // Resource.resources.IndexOf(entry.Value.ToString()); is the index of the resource in the list of resources
            }


            foreach (Node n in board.GetAllNodes())
            {
                //node.transform.position = //  this is a pronblem;
                NodeButton nodeui = Instantiate(NodePrefab, UnityUI.ConvertVector(n.position), Quaternion.identity).GetComponent<NodeButton>();
                nodeui.node = n;
                nodeui.NodePos = UnityUI.ConvertVector(n.position);
            }
        }
    }





}
