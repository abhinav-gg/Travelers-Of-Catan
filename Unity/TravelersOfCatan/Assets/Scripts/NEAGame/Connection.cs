using System;

namespace NEAGame
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A <c>Connection</c> represents the status of a connection between two nodes on the game board.<br/>
    /// Skill A: Complex User-Defined OOP - Inheritance
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Connection : Settlement
    {   
        // Constructor to create a connection from an id
        public Connection( int i = 0, string status = "", int occupant = -1)
        {
            statuses = new string[] { "Empty", "Road", "Wall" };
            id = i;
            if (status != "")
            {
                id = Array.IndexOf(statuses, status);
            }
            occupantID = occupant;
        }

        // Constructor to create a connection from a settlement wrapper
        public Connection(SettlementWrapper settlementWrapper)
        {
            statuses = new string[] { "Empty", "Road", "Wall" };
            
            occupantID = settlementWrapper.occupantID;
            SetStatus(settlementWrapper.status);
        }

        // Method to get the occupant of the connection
        public int GetOccupant()
        {
            return occupantID;
        }

        // Method to set the occupant of the connection
        public void SetOccupant(Player p)
        {
            occupantID = p.GetID();
        }

        // Method to get the status of the connection
        public string GetStatus()
        {
            return statuses[id];
        }

        // Method to set the status of the connection
        public void SetStatus(string status)
        {
            id = Array.IndexOf(statuses, status);
        }

        // Method to get the cost of walking on this connection
        public int GetWalkingCost(Player otherPlayer)
        {
            if (id == 0)
            {
                return 1;
            }
            if (otherPlayer.GetID() == occupantID)
            {
                if (id == 1)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (otherPlayer.GetID() != occupantID)
            {
                return int.MaxValue;
            }
            throw new Exception("Invalid Player Number");
        }

        // override the ToString method to return a string representation of the connection
        public override string ToString()
        {
            return $"{statuses[id]} Owned by {occupantID}";
        }
    }
}
