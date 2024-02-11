using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace NEAGame
{
    /// <summary>
    /// A <c>Player</c> object represents a single player in the game.
    /// </summary>
    public class Player
    {
        // Unity static color options
        [System.Serializable]
        public enum PlayerColors
        {
            blue,
            cyan,
            green,
            grey,
            magenta,
            red,
            white,
            yellow
        }
        
        private int victoryPoints;
        private Dictionary<Resource, int> resources = new Dictionary<Resource, int>() {
            { new Resource(1), 0 },
            { new Resource(2), 0 },
            { new Resource(3), 0 },
            { new Resource(4), 0 },
            { new Resource(5), 0 }
        };

        private List<Node> buildings = new List<Node>(); 
        private List<Connection> connections = new List<Connection>();

        protected int playerNumber; // used as a UID for the player allowing the same name
        public readonly string playerName;
        public readonly Vector3 origin;
        public int moves;
        public Vector3 position;
        public string color;
        protected bool isAI = false; // gets changed by child AI class

        // constructor for player
        public Player(int playerNumber, string playerName, Vector3 origin, string color)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            this.origin = origin;
            this.color = color;
            victoryPoints = 0;
            position = origin;
        }

        // constructor for player that loads from a save file
        public Player(PlayerWrapper player)
        {
            playerNumber = player.playerNumber;
            playerName = player.playerName;
            origin = new Vector3(player.origin.x, player.origin.y, player.origin.z);
            moves = player.moves;
            color = player.color;
            position = new Vector3(player.position.x, player.position.y, player.position.z);
            resources = new Dictionary<Resource, int>();
            foreach (var entry in player.resources._Keys.Zip(player.resources._Values, (k, v) => new { k, v }))
            {
                resources.Add(new Resource(entry.k), entry.v);
            }
            victoryPoints = player.victoryPoints;
        }


        // checks if the player is an AI
        public bool isPlayerAI()
        {
            return isAI;
        }

        // adds victory points to the player
        public void addVictoryPoints(int points)
        {
            victoryPoints += points;
        }

        // gets the player's ID
        public int GetID()
        {
            return playerNumber;
        }

        // adds a building to the player's list of buildings
        public void addBuilding(Node building)
        {
            buildings.Add(building);
        }

        // removes a building from the player's list of buildings
        public void removeBuilding(Node building)
        {
            if (buildings.Contains(building))
            {
                buildings.Remove(building);
            }
            else
            {
                throw new Exception("Player does not own this building");
            }
        }

        // gets the player's list of buildings
        public List<Node> GetBuildings()
        {
            return buildings;
        }

        // gets the player's original position
        public Vector3 GetCapital()
        {
            return origin;
        }

        // adds a connection to the player's list of connections
        public void addConnection(Connection connection)
        {
            connections.Add(connection);
        }

        // removes a connection from the player's list of connections
        public void removeConnection(Connection con)
        {
            if (connections.Contains(con))
            {
                connections.Remove(con);
            }
            else
            {
                throw new Exception("Player does not own this connection");
            }
        }

        // gets the player's list of connections
        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        // get the number of moves the player has left
        public int getMovesLeft()
        {
            return moves;
        }

        // adds a resource to the player's list of resources
        public void addResource(Resource resource, int amount = 1)
        {
            resources[resource] += amount;
        }

        // removes a resource from the player's list of resources
        public void removeResource(Resource resource, int amount = 1)
        {
            resources[resource] -= amount;
        }

        // gets the player's inventory
        public Dictionary<Resource, int> getResources()
        {
            return resources;
        }

        // overrides the ToString method to return the player's name
        public override string ToString()
        {
            return playerName;
        }

        // Used to calculate the wealth of a player for the AI static state evaluation function
        public int GetWealth()
        {
            int wealth = 0;
            foreach (KeyValuePair<Resource, int> resource in resources)
            {
                wealth += resource.Value;
            }
            // victory points are weighted as they indicate resource have been spent
            wealth += victoryPoints * 5;
            
            // int cast wealth 
            return wealth;
        }
    }
}
