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


    class GamePlayLoop
    {
        int turn = 0;
        int MAXplayer = 0;
        public int[] victoryPoints;

        public GamePlayLoop(int MAXplayer)
        {
            this.MAXplayer = MAXplayer;
            victoryPoints = new int[MAXplayer];
        }

        public void startGame()
        {

            Console.WriteLine("Game Started");
            while (true)
            {
                
                takeTurn();
                
            }
        }

        public void takeTurn()
        {
            turn++;
            turn = turn % MAXplayer;
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

        public string getResource()
        {
            return resources[i];
        }

    }

    class Node
    {
        public List<Node> neighbours;
        private BuildingStatus status;

        public Node(BuildingStatus status)
        {
            this.status = status;
        }
    }

    class HexagonUnit
    {
        private Resource m_resource;
        private Node[] nodes = new Node[6];


        public HexagonUnit(Resource R)
        {
            m_resource = R;


            for (int i = 0; i < 6; i++)
            {
                nodes[i] = new Node(new BuildingStatus(0));
                nodes[i].neighbours.Add(nodes[(i + 1) % 6]);
                nodes[i].neighbours.Add(nodes[(i + 5) % 6]);
            }

        }

        

        public string getResource()
        {
            return m_resource.getResource();
        }

    }

    class Board
    {

        private HexagonUnit[] board;

        public Board()
        {
            for (int i = 0; i < 25; i++)
            {
                board[i] = new HexagonUnit(new Resource(0));
            }
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
