using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Class1
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }

}


// Path: Class1.cs
namespace TerminalBasedGame
{

    class Resource
    {
        private int i;
        private string[] resources = { "Wood", "Brick", "Wheat", "Sheep", "Ore" };

        public Resource(int i)
        {
            this.i = i;
        }
    }

    class HexagonUnit
    {
        private Resource r;
    }

    class Board
    {


        public void Mn()
        {
        }
    }

    class BuildingStatus
    {
        private int i;
        private string[] statuses = { "Empty", "City", "Village", "Highway Man" };

        public BuildingStatus(int i)
        {
            this.i = i;
        }

    }

    class ConnectionStatus
    {
        private int i;
        private string[] statuses = { "Empty", "Road", "Wall" };

        public ConnectionStatus(int i)
        {
            this.i = i;
        }

    }

}
