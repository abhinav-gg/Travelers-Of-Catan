using System;

namespace NEAGame
{

    [Serializable]
    public class Connection : Settlement
    {   

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

        public Connection(SettlementWrapper settlementWrapper)
        {
            statuses = new string[] { "Empty", "Road", "Wall" };
            
            occupantID = settlementWrapper.occupantID;
            SetStatus(settlementWrapper.status);
        }

        public int GetOccupant()
        {
            return occupantID;
        }

        public void SetOccupant(Player p)
        {
            occupantID = p.GetID();
        }

        public string GetStatus()
        {
            return statuses[id];
        }

        public void SetStatus(string status)
        {
            id = Array.IndexOf(statuses, status);
        }

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


        public override string ToString()
        {
            return $"{statuses[id]} Owned by {occupantID}";
        }



    }

}