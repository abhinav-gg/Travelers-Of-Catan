using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using System.Collections;


namespace NEAGame
{

    [System.Serializable]
    public class Connection
    {   


        // Merged with ConnectionStatus class
        public static readonly string[] statuses = { "Empty", "Road", "Wall" };
        public Node start;
        public Node end;
        private int i = 0;
        private int occupant;


        public Connection(Node Start, Node End, int i = 0, string status = "", int occupant = -1)
        {
            this.i = i;
            if (status != "")
            {
                this.i = Array.IndexOf(statuses, status);
            }
            this.occupant = occupant;
            this.start = Start;
            this.end = End;
        }

        public static bool operator ==(Connection c1, Connection c2)
        {
            if (c1 is null)
            {
                if (c2 is null)
                {
                    return true;
                }
                return false;
            }
            return c1.Equals(c2);
        }

        public static bool operator !=(Connection c1, Connection c2)
        {
            if (c1 is null)
            {
                if (c2 is null)
                {
                    return false;
                }
                return true;
            }
            return c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Connection))
            {
                return false;
            }
            Connection otherConnection = obj as Connection;
            if (otherConnection.start == start && otherConnection.end == end)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public int GetOccupant()
        {
            return occupant;
        }

        public void SetOccupant(Player p)
        {
            occupant = p.getNumber();
        }

        public string GetStatus()
        {
            return statuses[i];
        }

        public void SetStatus(string status)
        {
            i = Array.IndexOf(statuses, status);
        }

        public int GetWalkingCost(Player otherPlayer)
        {
            if (this.i == 0)
            {
                return 1;
            }
            if (otherPlayer.getNumber() == occupant)
            {
                if (this.i == 1)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (otherPlayer.getNumber() != occupant)
            {
                return int.MaxValue;
            }
            return int.MinValue; // should never happen
        }


        public override string ToString()
        {
            return $"{statuses[i]} Owned by {occupant} Which connects {start} to {end}";
        }

    }

}