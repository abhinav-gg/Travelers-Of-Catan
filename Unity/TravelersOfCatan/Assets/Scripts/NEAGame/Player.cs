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
        protected bool isAI = false; // gets changed by child AI class

        public Player(int playerNumber, string playerName, Vector3 origin)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            this.origin = origin;
            victoryPoints = 0;
            position = origin;
        }


        public Player(PlayerWrapper player)
        {
            this.playerNumber = player.playerNumber;
            this.playerName = player.playerName;
            this.origin = new Vector3(player.origin.x, player.origin.y, player.origin.z);
            this.moves = player.moves;
            this.position = new Vector3(player.position.x, player.position.y, player.position.z);
            this.resources = new Dictionary<Resource, int>();
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

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public int getNumber()
        {
            return playerNumber;
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
            float wealth = 0;
            foreach (KeyValuePair<Resource, int> resource in resources)
            {
                wealth += resource.Value / 5;
            }
            wealth += victoryPoints * 2;
            
            // int cast wealth 
            return (int)wealth;
        }

        public void upgradeCillage(Node node)
        {
            foreach (Node n in buildings)
            {
                if (n == node)
                {
                    n.status.UpgradeVillage();
                    addVictoryPoints(TravelersOfCatan.victoryPointConvertor["City"]);
                }
            }
        }
    }


}