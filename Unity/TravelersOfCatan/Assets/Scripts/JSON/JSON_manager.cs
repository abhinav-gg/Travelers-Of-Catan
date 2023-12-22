﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace NEAGame
{


    public abstract class JSONWrapper<T> where T : JSONWrapper<T>, new()
    {

        public static string Dump(T t)
        {
            return JsonUtility.ToJson(t);
        }

        public static T Load(string jsonString)
        {
            T t = new T();
            JsonUtility.FromJsonOverwrite(jsonString, t);
            return t;
        }

    }

    class JSON_manager
    {

        public static void SAVEGAME(TravelersOfCatan game, string SAVE)
        {
            GameWrapper gameWrapper = new GameWrapper(game);
            // save the game to a file in unity persistent data path with the name SAVE
            string json = JSONWrapper<GameWrapper>.Dump(gameWrapper);
            string fullpath = Application.persistentDataPath + "/" + SAVE + ".json";

            FileHandler fileHandler = new FileHandler(fullpath, false);
            fileHandler.Save(json);

        }

        public static GameWrapper LOADGAME(string SAVE)
        {
            string fullpath = Application.persistentDataPath + "/" + SAVE + ".json";
            FileHandler fileHandler = new FileHandler(fullpath, false);
            string json = fileHandler.Load();
            GameWrapper gameWrapper = JSONWrapper<GameWrapper>.Load(json);
            Debug.Log(json);
           
            return gameWrapper;
        
        }


    }


    [Serializable]
    public class ResourceWrapper : JSONWrapper<ResourceWrapper>
    {

        public int id;

        public ResourceWrapper() { }
        public ResourceWrapper(Resource resource)
        {
            id = resource.GetHashCode();
        }


    }

    [Serializable]
    public class HexagonUnitWrapper : JSONWrapper<HexagonUnitWrapper>
    {

        public Vector3 position;
        public ResourceWrapper resource;

        public HexagonUnitWrapper() { }
        public HexagonUnitWrapper(HexagonUnit unit)
        {
            position = UnityUI.ConvertVector(unit.position);
            resource = new ResourceWrapper(unit.resource);
        }


    }

    [Serializable]
    public class SettlementWrapper : JSONWrapper<SettlementWrapper>
    {

        public string status;
        public int occupantID;

        public SettlementWrapper() { }
        public SettlementWrapper(Connection connection)
        {
            status = connection.GetStatus();
            occupantID = connection.GetOccupant();
        }

        public SettlementWrapper(Building node)
        {
            status = node.GetStatus();
            occupantID = node.GetOccupant();
        }

    }


    [Serializable]
    public class AdjMatrixWrapper : JSONWrapper<AdjMatrixWrapper>
    {

        public List<Vector3> _Keys = new List<Vector3>();
        public List<AdjMatrixBottomWrapper> _Values = new List<AdjMatrixBottomWrapper>();

        public AdjMatrixWrapper() { }
        public AdjMatrixWrapper(Dictionary<System.Numerics.Vector3, Dictionary<System.Numerics.Vector3, Connection>> connections)
        {

            foreach (var entry in connections)
            {
                _Keys.Add(UnityUI.ConvertVector(entry.Key));
                _Values.Add(new AdjMatrixBottomWrapper(entry.Value));
            }
        }

    }

    [Serializable]
    public class AdjMatrixBottomWrapper : JSONWrapper<AdjMatrixBottomWrapper>
    {

        public List<Vector3> _Keys = new List<Vector3>();
        public List<SettlementWrapper> _Values = new List<SettlementWrapper>();

        public AdjMatrixBottomWrapper() { }
        public AdjMatrixBottomWrapper(Dictionary<System.Numerics.Vector3, Connection> connections)
        {
            foreach (var entry in connections)
            {
                if (entry.Value.GetStatus() != "Empty")
                {
                    _Keys.Add(UnityUI.ConvertVector(entry.Key));
                    _Values.Add(new SettlementWrapper(entry.Value));
                }
            }
        }

    }

    [Serializable]
    public class PlayerWrapper : JSONWrapper<PlayerWrapper>
    {
        public Vector3 position;
        public int playerNumber;
        public string playerName;
        public int victoryPoints;
        public int moves;
        public bool isAI;
        public Vector3 origin;
        public InventoryWrapper resources;

        //public Dictionary<int, int> resources = new Dictionary<int, int>();

        public PlayerWrapper() { }
        public PlayerWrapper(Player player)
        {
            position = UnityUI.ConvertVector(player.position);
            playerNumber = player.getNumber();
            playerName = player.playerName;
            victoryPoints = player.getVictoryPoints();
            moves = player.moves;
            isAI = player.isPlayerAI();
            origin = UnityUI.ConvertVector(player.origin);
            resources = new InventoryWrapper(player.getResources());
        }
        // player can find their own buildings and connections if needed using the board

    }

    [Serializable]
    public class InventoryWrapper : JSONWrapper<InventoryWrapper>
    {
        public List<ResourceWrapper> _Keys = new List<ResourceWrapper>();
        public List<int> _Values = new List<int>();

        public InventoryWrapper() { }
        public InventoryWrapper(Dictionary<Resource, int> resources)
        {
            foreach (var entry in resources)
            {
                _Keys.Add(new ResourceWrapper(entry.Key));
                _Values.Add(entry.Value);
            }
        }

    }

    [Serializable]
    public class AllNodesWrapper : JSONWrapper<AllNodesWrapper>
    {
        public List<Vector3> _Keys = new List<Vector3>();
        public List<NodeWrapper> _Values = new List<NodeWrapper>();

        public AllNodesWrapper() { }
        public AllNodesWrapper(Dictionary<System.Numerics.Vector3, Node> nodes)
        {
            foreach (var entry in nodes)
            {
                if (entry.Value.status.GetStatus() == "Empty") continue;
                _Keys.Add(UnityUI.ConvertVector(entry.Key));
                _Values.Add(new NodeWrapper(entry.Value));
            }
        }
    }

    [Serializable]
    public class NodeWrapper : JSONWrapper<NodeWrapper>
    {
        public Vector3 position;
        public SettlementWrapper status;

        public NodeWrapper() { }
        public NodeWrapper(Node node)
        {
            position = UnityUI.ConvertVector(node.position);
            status = new SettlementWrapper(node.status);
        }
    }


    [Serializable]
    public class BoardWrapper : JSONWrapper<BoardWrapper>
    {

        public List<HexagonUnitWrapper> board = new List<HexagonUnitWrapper>();
        public AdjMatrixWrapper connections;
        public AllNodesWrapper nodes;

        public BoardWrapper() { }
    
    }

    [Serializable]
    public class GameWrapper : JSONWrapper<GameWrapper> 
    {
        
        public int winVictoryPoints;
        public float timePerMove;
        public float timer; // controlled by UI
        public int turn;
        public List<PlayerWrapper> allPlayers = new List<PlayerWrapper>();
        public BoardWrapper board;


        public GameWrapper() { }

        public GameWrapper(TravelersOfCatan game)
        {
            winVictoryPoints = game.WinningVictoryPoints;
            timePerMove = game.TimePerMove;
            timer = game.UserInterface.GetTimer();
            // get index of current player in game.gamePlayers
            turn = game.gamePlayers.IndexOf(game.GetCurrentPlayer());
            foreach (Player player in game.gamePlayers)
            {
                allPlayers.Add(new PlayerWrapper(player));
            }
            board = game.board.SoftSerialize();

        }

    }

}