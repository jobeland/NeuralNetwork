using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicGame
{
    public class Game
    {
        public Dictionary<Tuple<int, int>, GameCell> Board { get; set; }
        public int MovesLeft;
        public Tuple<int, int> CurrentCoord { get; set; }
        public int Width;
        public int Length;
        private static int HIGHEST_VALUE = 999999;

        public Game(int width, int length, int movesAllowed)
        {
            Board = new Dictionary<Tuple<int, int>, GameCell>();
            MovesLeft = movesAllowed;
            Width = width;
            Length = length;
            CurrentCoord = new Tuple<int, int>(0, 0);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j != 0 && j != width - 1)
                    {
                        Board.Add(new Tuple<int, int>(j, i),
                                                new GameCell
                                                {
                                                    CanMoveDown = false,
                                                    CanMoveLeft = true,
                                                    CanMoveRight = true,
                                                    CanMoveUp = false,
                                                    HasDot = true
                                                });
                    }
                    else if (j == 0 && i % 2 == 0)
                    {
                        if (i == 0)
                        {
                            Board.Add(new Tuple<int, int>(j, i),
                                                                           new GameCell
                                                                           {
                                                                               CanMoveDown = false,
                                                                               CanMoveLeft = false,
                                                                               CanMoveRight = true,
                                                                               CanMoveUp = false,
                                                                               HasDot = true
                                                                           });
                        }
                        else
                        {
                            Board.Add(new Tuple<int, int>(j, i),
                                               new GameCell
                                               {
                                                   CanMoveDown = false,
                                                   CanMoveLeft = false,
                                                   CanMoveRight = true,
                                                   CanMoveUp = true,
                                                   HasDot = true
                                               });
                        }

                    }
                    else if (j == 0 && i % 2 != 0)
                    {
                        if (i == length - 1)
                        {
                            Board.Add(new Tuple<int, int>(j, i),
                                                                           new GameCell
                                                                           {
                                                                               CanMoveDown = false,
                                                                               CanMoveLeft = false,
                                                                               CanMoveRight = true,
                                                                               CanMoveUp = false,
                                                                               HasDot = true
                                                                           });
                        }
                        else
                        {
                            Board.Add(new Tuple<int, int>(j, i),
                                               new GameCell
                                               {
                                                   CanMoveDown = true,
                                                   CanMoveLeft = false,
                                                   CanMoveRight = true,
                                                   CanMoveUp = false,
                                                   HasDot = true
                                               });
                        }
                    }
                    else if (j == width - 1 && i % 2 == 0)
                    {
                        Board.Add(new Tuple<int, int>(j, i),
                                           new GameCell
                                           {
                                               CanMoveDown = true,
                                               CanMoveLeft = true,
                                               CanMoveRight = false,
                                               CanMoveUp = false,
                                               HasDot = true
                                           });
                    }
                    else if (j == width - 1 && i % 2 != 0)
                    {
                        Board.Add(new Tuple<int, int>(j, i),
                                           new GameCell
                                           {
                                               CanMoveDown = false,
                                               CanMoveLeft = true,
                                               CanMoveRight = false,
                                               CanMoveUp = true,
                                               HasDot = true
                                           });
                    }
                }
            }
        }

        public bool EatDot(int x, int y)
        {
            var coord = new Tuple<int, int>(x, y);
            if (!Board[coord].HasDot)
            {
                return false;
            }
            Board[coord].HasDot = false;
            return true;
        }

        public bool UseTurn(MoveDirection direction)
        {
            MovesLeft--;
            if(CanMove(CurrentCoord.Item1, CurrentCoord.Item2, direction))
            {
                if(direction == MoveDirection.DOWN)
                {
                    CurrentCoord = new Tuple<int, int>(CurrentCoord.Item1, CurrentCoord.Item2 + 1);
                }
                else if(direction == MoveDirection.LEFT)
                {
                    CurrentCoord = new Tuple<int, int>(CurrentCoord.Item1 - 1, CurrentCoord.Item2);
                }
                else if(direction == MoveDirection.RIGHT)
                {
                    CurrentCoord = new Tuple<int, int>(CurrentCoord.Item1 + 1, CurrentCoord.Item2);
                }
                else if(direction == MoveDirection.UP)
                {
                    CurrentCoord = new Tuple<int, int>(CurrentCoord.Item1, CurrentCoord.Item2 - 1);
                }
                return EatDot(CurrentCoord.Item1, CurrentCoord.Item2);
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder line = new StringBuilder();
            for (int i = 0; i < Width; i++)
            {
                line.Append("--");
            }
            sb.AppendLine(line.ToString());
            line.Clear();
            for (int i = 0; i < Length; i++)
            {
                line.Append("|");
                for (int j = 0; j < Width; j++)
                {
                    if (Board[new Tuple<int, int>(j, i)].HasDot)
                    {
                        line.Append(".");
                    }
                    else
                    {
                        line.Append(" ");
                    }
                }
                line.Append("|");
                sb.AppendLine(line.ToString());
                line.Clear();
                line.Append("|");
                for (int j = 0; j < Width; j++)
                {
                    if (Board[new Tuple<int, int>(j, i)].CanMoveDown)
                    {
                        line.Append(" ");
                    }
                    else
                    {
                        line.Append("--");
                    }
                }
                line.Append("|");
                sb.AppendLine(line.ToString());
                line.Clear();
            }
            for (int i = 0; i < Width; i++)
            {
                line.Append("--");
            }
            sb.AppendLine(line.ToString());
            line.Clear();
            return sb.ToString();
        }

        public bool CanMove(int x, int y, MoveDirection direction)
        {
            GameCell cell = Board[new Tuple<int, int>(x, y)];
            if (direction == MoveDirection.DOWN)
            {
                return cell.CanMoveDown;
            }
            if (direction == MoveDirection.LEFT)
            {
                return cell.CanMoveLeft;
            }
            if (direction == MoveDirection.RIGHT)
            {
                return cell.CanMoveRight;
            }
            if (direction == MoveDirection.UP)
            {
                return cell.CanMoveUp;
            }
            return false;
        }

        public bool IsGameWon()
        {
            return (MovesLeft >= 0 && AreAllDotsGone());
        }

        public bool IsGameLost()
        {
            return (MovesLeft <= 0 && !AreAllDotsGone());
        }

        public int GetDotsLeft()
        {
            int count = 0;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (Board[new Tuple<int, int>(j, i)].HasDot)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public bool AreAllDotsGone()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (Board[new Tuple<int, int>(j, i)].HasDot)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int GetDistanceToClosestDot(MoveDirection direction, Tuple<int, int> CoordinateToUse, List<Tuple<int, int>> visited){
            if(!CanMove(CoordinateToUse.Item1, CoordinateToUse.Item2, direction))
            {
                return HIGHEST_VALUE;
            }
            if(Board[CoordinateToUse].HasDot){
                return 1;
            }
            else{
                Tuple<int, int> newCoord = null; 
                if (direction == MoveDirection.DOWN)
                {
                    newCoord = new Tuple<int, int>(CoordinateToUse.Item1, CoordinateToUse.Item2 + 1);
                }
                else if (direction == MoveDirection.LEFT)
                {
                    newCoord = new Tuple<int, int>(CoordinateToUse.Item1 - 1, CoordinateToUse.Item2);
                }
                else if (direction == MoveDirection.RIGHT)
                {
                    newCoord = new Tuple<int, int>(CoordinateToUse.Item1 + 1, CoordinateToUse.Item2);
                }
                else if (direction == MoveDirection.UP)
                {
                    newCoord = new Tuple<int, int>(CoordinateToUse.Item1, CoordinateToUse.Item2 - 1);
                }
                if(visited.Any(p => p.Item1 == newCoord.Item1 && p.Item2 == newCoord.Item2)){
                    return HIGHEST_VALUE;
                }
                List<Tuple<int,int>> newVisited = new List<Tuple<int,int>>();
                newVisited.AddRange(visited);
                newVisited.Add(newCoord);
                int down = GetDistanceToClosestDot(MoveDirection.DOWN, newCoord, newVisited) + 1;
                int up = GetDistanceToClosestDot(MoveDirection.UP, newCoord, newVisited) + 1;
                int left = GetDistanceToClosestDot(MoveDirection.LEFT, newCoord, newVisited) + 1;
                int right = GetDistanceToClosestDot(MoveDirection.RIGHT, newCoord, newVisited) + 1;

                return Math.Min(Math.Min(up, down), Math.Min(left, right));
            }  
            }
    }

    public class GameCell
    {
        public bool HasDot { get; set; }
        public bool CanMoveUp { get; set; }
        public bool CanMoveDown { get; set; }
        public bool CanMoveLeft { get; set; }
        public bool CanMoveRight { get; set; }
    }

    public enum MoveDirection
    {
        UP, DOWN, LEFT, RIGHT
    }
}
