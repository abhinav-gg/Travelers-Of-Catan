using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NEAGame;
using System;
using Unity.VisualScripting;

[Serializable]
public class GameUI : UnityUI
{


    public GameObject NodePrefab;
    public GameObject ConnectionPrefab;

    public Tile[] resources = new Tile[6];
    public GridLayout gridLayout;
    public Tilemap tilemap;

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



    internal void UpdateBoard(Board board)
    {

        if (!hasInitializedBoard)
        {
            int resourceID;
            Vector3Int gridPos;
            hasInitializedBoard = true;

            foreach (KeyValuePair<System.Numerics.Vector3, Resource> entry in board.GetResourcesOnBoard()) 
            {
                
                resourceID = Array.IndexOf(Resource.resources, entry.Value.ToString());// is the index of the resource in the list of resources
                gridPos = CubicToOddRow(entry.Key);
                tilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), resources[resourceID]);


            }
            
            
            foreach (Node n in board.GetAllNodes())
            {
                //node.transform.position = //  this is a pronblem;
                NodeButton nodeui = Instantiate(NodePrefab, ConvertVector(n.position), Quaternion.identity, GameObject.FindGameObjectWithTag("NodeParent").transform).GetComponent<NodeButton>();
                nodeui.node = n;
                nodeui.NodePos = ConvertVector(n.position);
                nodeui.transform.position = GetNodeGlobalPos(n);
            }
        }
        Debug.Break();
    }


    public Vector3 GetNodeGlobalPos(Node node)
    {
        Vector3 center;
        float totalX = 0f;
        float totalY = 0f;
        float Z = 0f;
        foreach (System.Numerics.Vector3 vec in node.GetHexNeighbours())
        {
            center = HexLocalToGlobal(CubicToOddRow(vec));
            totalX += center.x;
            totalY += center.y;
            Z = center.z;
        }
        float avgX = totalX / 3;
        float avgY = totalY / 3;
        return new Vector3(avgX, avgY, Z);
    }

    public Vector3 HexLocalToGlobal(Vector3 cellPos)
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


    public NodeButton FindGameObjectAtNode(Vector3 pos)
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

    internal void DisplayPlayers(Player[] players)
    {
        foreach (Player player in players)
        {
            Debug.Log(player.playerName);

        }
    }



}
