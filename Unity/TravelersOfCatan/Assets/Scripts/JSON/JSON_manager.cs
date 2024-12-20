﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace NEAGame
{
    /// <summary>
    /// The <c>JSON_manager</c> class is used to manage the serialization and deserialization of the game to and from JSON.
    /// </summary>
    class JSON_manager
    {
        public readonly string[] SAVE_FILES = { "Save1", "Save2", "Save3", "Save4" };
        private FileHandler fileHandler;

        // Constructor for the JSON manager that takes in the save file number
        public JSON_manager(int Save)
        {
            string fullpath = Application.persistentDataPath + "/" + SAVE_FILES[Save];
            fileHandler = new FileHandler(fullpath);
        }

        // Checking if the save file exists
        public bool DoesGameExist()
        {
            return fileHandler.IsMade;
        }

        // Clearing the save file
        public void ClearSave()
        {
            fileHandler.Delete();
        }

        // Saving the game to a file
        public void SaveGame(TravelersOfCatan game)
        {
            GameWrapper gameWrapper = new GameWrapper(game);
            // save the game to a file in unity persistent data path
            string json = JSONWrapper<GameWrapper>.Dump(gameWrapper);
            fileHandler.Save(json);
        }

        // Loading the game from a file
        public GameWrapper LoadGame()
        {
            
            string json = fileHandler.Load();
            GameWrapper gameWrapper = JSONWrapper<GameWrapper>.Load(json);          
            return gameWrapper;
        }
    }


    /// <summary>
    /// <c>JSONWrapper</c> is an abstract class that defines the methods to serialize and deserialize an object to and from JSON.
    /// </summary>
    public abstract class JSONWrapper<T> where T : JSONWrapper<T>, new()
    {
        // Method to serialize the game to JSON 
        public static string Dump(T t)
        {
            return JsonUtility.ToJson(t);
        }

        // Method to deserialize the game from JSON
        public static T Load(string jsonString)
        {
            T t = new T();
            JsonUtility.FromJsonOverwrite(jsonString, t);
            return t;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////
    // Serializable Class Wrappers to be used for JSON Serialization and Deserialization
    
    
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

        public List<Vector3> _Keys = new List<Vector3>();
        public List<ResourceWrapper> _Values = new List<ResourceWrapper>();

        public HexagonUnitWrapper() { }
        public HexagonUnitWrapper(Dictionary<System.Numerics.Vector3, Resource> board)
        {
            foreach (var entry in board)
            {
                _Keys.Add(UnityUI.ConvertVector(entry.Key));
                _Values.Add(new ResourceWrapper(entry.Value));
            }
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
        public string color;
        public Vector3 origin;
        public InventoryWrapper resources;

        public PlayerWrapper() { }
        public PlayerWrapper(Player player)
        {
            position = UnityUI.ConvertVector(player.position);
            playerNumber = player.GetID();
            playerName = player.playerName;
            victoryPoints = player.getVictoryPoints();
            moves = player.moves;
            isAI = player.isPlayerAI();
            color = player.color;
            origin = UnityUI.ConvertVector(player.origin);
            resources = new InventoryWrapper(player.getResources());
        }
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

        public HexagonUnitWrapper board;
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
            timer = game.GetGameTime();
            turn = game.gamePlayers.IndexOf(game.GetCurrentPlayer());
            foreach (Player player in game.gamePlayers)
            {
                allPlayers.Add(new PlayerWrapper(player));
            }
            board = game.board.SoftSerialize();
        }
    }
}
