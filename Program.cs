using System.Text;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

namespace GCExercises.Exercises.BlunderGame
{
    public class Blunder
    {
        public Blunder(GameArea board)
        {
            _board = board;
            location.xLocation = _board.blunder.xLocation;
            location.yLocation = _board.blunder.yLocation;
            location.cellProp = _board.blunder.cellProp;
            hasMoved = false;
            hasBeer = false;

        }

        static bool hasMoved = false;
        static bool hasBeer = false;
        public Cell location;
        private string[] directionPriorty = new string[] { "SOUTH", "EAST", "NORTH", "WEST" };
        private int directionIndex = 0;
        private GameArea _board;
        private readonly string[] pathModifiers = new string[] { "S", "E", "N", "W" };
        private int moveNumer = 1;
        private StringBuilder journey = new StringBuilder();

        public int Move()
        {
            if (_board.maxMoveTreshHold == moveNumer) //we are in a loop, exit (Game prematurely over)
            {
                Console.WriteLine("LOOP");
                return -1;
            }

            if (directionIndex == -1) //Journey Finished (Game Over)
            {
                return -1;
            }

            Cell tempLocation = location;

            if (directionPriorty[directionIndex] == "SOUTH")
            {
                tempLocation.yLocation++;

                return TryMove(tempLocation);
            }

            if (directionPriorty[directionIndex] == "EAST")
            {
                tempLocation.xLocation++;

                return TryMove(tempLocation);
            }


            if (directionPriorty[directionIndex] == "NORTH")
            {
                tempLocation.yLocation--;

                return TryMove(tempLocation);
            }

            if (directionPriorty[directionIndex] == "WEST")
            {
                tempLocation.xLocation--;

                return TryMove(tempLocation);
            }

            return -2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Direction index of Blunder</returns>
        private int TryMove(Cell tempLocation)
        {
            if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "#") //wall
            {
                if (hasMoved)
                {
                    directionIndex = 0;
                    hasMoved = false;
                }
                else
                {
                    directionIndex = (++directionIndex % 4); //directory index cannot be greater than 4
                }

                Move();
            }
            else if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "X") //destructiable
            {
                if (hasBeer)
                {
                    _board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp = " "; //if has beer destroy the cell
                    SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
                }
                else
                {
                    if (hasMoved)
                    {
                        directionIndex = 0;
                        hasMoved = false;
                    }
                    else
                    {
                        directionIndex = (++directionIndex % 4); //directory index cannot be greater than 4
                    }
                }
            }
            else if (pathModifiers.Any(p => p.Equals(_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp)) == true) //Path Modifier
            {
                SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
                directionIndex = Array.IndexOf(directionPriorty, directionPriorty.First(p => p.StartsWith(_board.board[location.xLocation, location.yLocation].cellProp)));
                Move();
            }
            else if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "I") //Inverter
            {
                ChangeDirectionPriorty();
                SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
            }
            else if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "B") //Beer
            {
                hasBeer ^= true;
                SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
            }
            else if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "T") //Teleporter 
            {
                Cell nextLocation = _board.teleporters.Except(_board.teleporters.Where(p => p.xLocation == tempLocation.xLocation && p.yLocation == tempLocation.yLocation)).First(); //Get the other teleport point
                SetBlunderLocation(nextLocation, directionPriorty[directionIndex]);
            }
            else if (_board.board[tempLocation.xLocation, tempLocation.yLocation].cellProp == "$") //Booth (Game Over)
            {
                SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
                Console.WriteLine(journey.ToString()); // journey finished, print journey
                directionIndex = -1;
                return -1;
            }
            else
            {
                SetBlunderLocation(tempLocation, directionPriorty[directionIndex]);
            }

            return directionIndex;
        }

        private void SetBlunderLocation(Cell tempLocation, string direction)
        {
            _board.blunder.yLocation = location.yLocation = tempLocation.yLocation;
            _board.blunder.xLocation = location.xLocation = tempLocation.xLocation;
            journey.AppendLine(direction);
            _board.ReDrowBoard();
            moveNumer++;
            hasMoved = true;
        }

        /// <summary>
        /// Reverses direction priority and current direction index accordingly
        /// </summary>
        private void ChangeDirectionPriorty()
        {
            string currentDirection = directionPriorty[directionIndex];
            directionPriorty = directionPriorty.Reverse().ToArray();
            directionIndex = Array.IndexOf(directionPriorty, currentDirection);
        }

    }

    public class GameArea
    {
        private int width = 0;
        private int height = 0;
        public Cell[,] board;
        public Cell blunder;
        public List<Cell> teleporters;
        public int maxMoveTreshHold = 5;
        public GameArea(int width, int height)
        {
            this.width = width;
            this.height = height;
            board = new Cell[this.width, this.height];
            teleporters = new List<Cell>();
            maxMoveTreshHold = width * height;
        }

        public void GenerateBoardLine(int rowNum, string row)
        {
            char[] hede = row.ToCharArray();
            for (int i = 0; i < hede.Length; i++)
            {
                board[i, rowNum].cellProp = hede[i].ToString();
                board[i, rowNum].xLocation = i;
                board[i, rowNum].yLocation = rowNum;

                if (board[i, rowNum].cellProp.Equals("@"))
                {
                    blunder = board[i, rowNum];
                    board[i, rowNum].cellProp = " ";
                }
                if (board[i, rowNum].cellProp.Equals("T"))
                    teleporters.Add(board[i, rowNum]);
            }
        }

        internal void ReDrowBoard()
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (blunder.xLocation == this.board[x, y].xLocation && blunder.yLocation == this.board[x, y].yLocation)
                        Console.Write("@");
                    else
                        Console.Write(this.board[x, y].cellProp);
                }
                Console.Write($"\n");
            }
            Thread.Sleep(500);
        }
    }

    public struct Cell
    {
        public int xLocation = 0;
        public int yLocation = 0;
        public string cellProp = string.Empty;
        public Cell()
        {
        }
    }

    class Solution
    {
        static void Main(string[] args)
        {
            /*
            string[] inputs = Console.ReadLine().Split(' ');
            int L = int.Parse(inputs[0]);
            int C = int.Parse(inputs[1]);
            */

            int L = 30;
            int C = 15;

            GameArea area = new(C, L);

            string[] gameArea = new string[] { "###############",
                                               "#  #@#I  T$#  #",
                                               "#  #    IB #  #",
                                               "#  #     W #  #",
                                               "#  #      ##  #",
                                               "#  #B XBN# #  #",
                                               "#  ##      #  #",
                                               "#  #       #  #",
                                               "#  #     W #  #",
                                               "#  #      ##  #",
                                               "#  #B XBN# #  #",
                                               "#  ##      #  #",
                                               "#  #       #  #",
                                               "#  #     W #  #",
                                               "#  #      ##  #",
                                               "#  #B XBN# #  #",
                                               "#  ##      #  #",
                                               "#  #       #  #",
                                               "#  #       #  #",
                                               "#  #      ##  #",
                                               "#  #  XBIT #  #",
                                               "#  #########  #",
                                               "#             #",
                                               "# ##### ##### #",
                                               "# #     #     #",
                                               "# #     #  ## #",
                                               "# #     #   # #",
                                               "# ##### ##### #",
                                               "#             #",
                                               "###############"
            };

            for (int i = 0; i < L; i++)
            {
                string row = gameArea[i];
                //string row = Console.ReadLine();
                area.GenerateBoardLine(i, row);
            }

            area.ReDrowBoard();

            Blunder player = new Blunder(area);

            int directionIndex = 0;

            //Game Loop
            while (directionIndex != -1)
            {
                directionIndex = player.Move();
            }
        }
    }
}