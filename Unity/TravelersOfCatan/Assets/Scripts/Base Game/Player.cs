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
        public string playerName;

        public int moves;
        public Vector3 position;
        protected bool isAI = false; // gets changed by child AI class

        public Player(int playerNumber, string playerName, Vector3 origin)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            victoryPoints = 0;
            position = origin;
        }

        private void addVictoryPoints(int points) // required for future features
        {
            victoryPoints += points;
        }


        public void addBuilding(Node building)
        {
            buildings.Add(building);
            //addVictoryPoints(TravelersOfCatan.victoryPointConvertor[building.status.GetStatus()]);

        }

        public Node GetCapital()
        {
            return buildings[0];
        }

        public void addConnection(Connection connection)
        {
            connections.Add(connection);
            //addVictoryPoints(TravelersOfCatan.victoryPointConvertor[connection.GetStatus()]);
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public int getNumber()
        {
            return playerNumber;
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

    }


}