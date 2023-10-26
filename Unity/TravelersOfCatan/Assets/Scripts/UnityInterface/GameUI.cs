using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NEAGame;
using System;

[Serializable]
public class GameUI : UnityUI
{


    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;

    public Tile[] resources = new Tile[6];
    public GridLayout gridLayout;

    public GameObject[] hexes = new GameObject[19];
    public NodeButton[] nodes = new NodeButton[54];
    public ConnectionButton[] connections;

    bool hasInitializedBoard = false;

    // Start is called before the first frame update
    internal void StartGame()
    {
        game = new TravelersOfCatan(UnityUI.Interface, 2, 2);
        game.startGame();
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
                Debug.Log(entry);
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


    public Vector3 GetNodeGlocalPos(Node node)
    {
        Vector3 center;
        float totalX = 0f;
        float totalY = 0f;
        float Z = 0f;
        foreach (System.Numerics.Vector3 vec in node.GetHexNeighbours())
        {
            center = HexLocalToGlobal(UnityUI.ConvertVector(vec));
            totalX += center.x;
            totalY += center.y;
            Z = center.z;
        }
        float avgX = 3;
        float avgY = 3;
        return new Vector3(avgX, avgY, Z);
    }

    public Vector3 HexLocalToGlobal(Vector3 pos)
    {
        Vector3Int cellPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        return gridLayout.CellToWorld(cellPos);
    }

    internal Node GetUserNodeChoice(Node[] options)
    {

        // Add LeanTween Animation to the selected nodes here!



        throw new NotImplementedException();
    }
}
