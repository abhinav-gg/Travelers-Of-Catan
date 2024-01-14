using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NEAGame
{
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
    // Unity static color options

    public class Player
    {
        private int victoryPoints;
        private Dictionary<Resource, int> resources = new Dictionary<Resource, int>() {
            { new Resource(1), 0 },
            { new Resource(2), 0 },
            { new Resource(3), 0 },
            { new Resource(4), 0 },
            { new Resource(5), 0 }
        };

        /// <summary>
        private List<Node> buildings = new List<Node>(); //
        private List<Connection> connections = new List<Connection>(); //
        /// Unused for now however may be useful for future features
        /// </summary>

        protected int playerNumber; // useful as a UID for the player allowing the same name in testing
        public readonly string playerName;
        public readonly Vector3 origin;
        public int moves;
        public Vector3 position;
        public string color;
        protected bool isAI = false; // gets changed by child AI class

        public Player(int playerNumber, string playerName, Vector3 origin, string playerColor)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            this.origin = origin;
            this.color = playerColor;
            victoryPoints = 0;
            position = origin;
        }


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
                this.resources.Add(new Resource(entry.k), entry.v);
            }
        }


        public bool isPlayerAI()
        {
            return isAI;
        }

        private void addVictoryPoints(int points) // required for future features
        {
            victoryPoints += points;
        }

        public int GetID()
        {
            return playerNumber;
        }

        public void addBuilding(Node building)
        {
            buildings.Add(building);
            addVictoryPoints(TravelersOfCatan.victoryPointConvertor[building.status.GetStatus()]);

        }
        public void removeBuilding(Node building)
        {
            if (buildings.Contains(building))
            {
                buildings.Remove(building);
                addVictoryPoints(-TravelersOfCatan.victoryPointConvertor[building.status.GetStatus()]);
            }
            else
            {
                throw new Exception("Player does not own this building");
            }
        }

        public List<Node> GetBuildings()
        {
            return buildings;
        }

        public Vector3 GetCapital()
        {
            return origin;
        }

        public void addConnection(Connection connection)
        {
            connections.Add(connection);
            addVictoryPoints(TravelersOfCatan.victoryPointConvertor[connection.GetStatus()]);
        }

        public void removeConnection(Connection con)
        {
            if (connections.Contains(con))
            {
                connections.Remove(con);
                addVictoryPoints(-TravelersOfCatan.victoryPointConvertor[con.GetStatus()]);
            }
            else
            {
                throw new Exception("Player does not own this connection");
            }
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public int getMovesLeft()
        {
            return moves;
        }
        public void addResource(Resource resource, int amount = 1)
        {
            resources[resource] += amount;
        }


        public void removeResource(Resource resource, int amount = 1)
        {
            resources[resource] -= amount;
        }

        public Dictionary<Resource, int> getResources()
        {
            return resources;
        }

        public override string ToString()
        {
            return playerName;
        }

        public void Trade()
        {

        }

        /// <summary>
        /// Used to calculate the wealth of a player for the AI static state evaluation function
        /// </summary>
        public int GetWealth()
        {
            int wealth = 0;
            foreach (KeyValuePair<Resource, int> resource in resources)
            {
                wealth += resource.Value;
            }
            wealth += victoryPoints * 5;
            
            // int cast wealth 
            return wealth;
        }

        public void upgradeVillage(Node node)
        {
            addVictoryPoints(TravelersOfCatan.victoryPointConvertor["City"]);
        }

        public void undoUpgradeVillage(Node node)
        {
            addVictoryPoints(-TravelersOfCatan.victoryPointConvertor["City"]);
        }
    }


}