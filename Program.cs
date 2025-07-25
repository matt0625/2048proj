using System.Globalization;
using System.Net.NetworkInformation;

namespace _2048
{
    internal class Program
    {
        const int GRIDSIZE = 4;
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            int[,] Grid = new int[4,4];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = 0;
                }
            }

            PlayGame(Grid);
        }

        static void DisplayState(int[,] Grid)
        {
            string tmp = "|";
            for (int i = 0; i < Grid.GetLength(0);i++)
            {
                for (int j = 0 ; j < Grid.GetLength(1); j++)
                {
                    tmp += "   " + Grid[i, j] + "   |";
                }
                Console.WriteLine(tmp);
                tmp = "|";
            }
        }

        static void PlayGame(int[,] Grid)
        {
            bool EndCondition = false;
            var available = GetAvailablePos(Grid);
            int Score = 0;
            InsertNextNum(ref Grid, available);

            while (!EndCondition)
            {
                InsertNextNum(ref Grid, available);
                DisplayState(Grid);
                Console.WriteLine();
                Console.WriteLine($"Your score: {Score}");
                Console.WriteLine();
                Console.Write("Next move (WASD): ");
                string move = Console.ReadLine();
                while (!"wasdWASD".Contains(move))
                {
                    Console.WriteLine("Invalid move");
                    Console.Write("Next move (WASD): ");
                    move = Console.ReadLine();
                }

                Score += Execute(ref Grid, move);
                
                available = GetAvailablePos(Grid);
                EndCondition = CheckEndCondition(Grid, available);
            }

            Console.WriteLine($"Final Score: {Score}");
            Console.WriteLine();
            Console.WriteLine("Game Over!");
        }

        static bool CheckEndCondition(int[,] Grid, int[] available)
        {
            bool EndCondition = true;
            if (available.Length != GRIDSIZE*GRIDSIZE)
            {
                return false;
            }
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0;  j < Grid.GetLength(1); j++)
                {
                    if (i + 1 > Grid.GetLength(0) - 1 || j + 1 > Grid.GetLength(1) - 1)
                    {
                        Console.WriteLine("here");
                        continue;
                    }
                    else
                    {
                        var current = Grid[i, j];
                        Console.WriteLine($"{current}, {Grid[i+1, j]}, {Grid[i, j+1]}");
                        if (current == 0)
                        {
                            continue;
                        }
                        if (current == Grid[i + 1, j] || current == Grid[i, j + 1])
                        {
                            EndCondition = false;
                        }
                    }
                }
            }
            return EndCondition;
        }
        static int Execute(ref int[,] Grid, string move)
        {
            List<List<int>> rows = new List<List<int>>();
            List<List<int>> cols = new List<List<int>>();

            GetRowCol(Grid, ref rows, ref cols);
            bool rev = true;
            int x = 0;
            if (move == "w" || move == "W")
            {
                x = ExecuteMove(ref Grid, cols, move);
            }
            else if (move == "s" || move == "S")
            {
                x = ExecuteMove(ref Grid, cols, move, rev);
            }
            else if (move == "a" || move == "A")
            {
                x = ExecuteMove(ref Grid, rows, move);
            }
            else if (move == "d" || move == "D")
            {
                x = ExecuteMove(ref Grid, rows, move, rev);
            }

            return x;
        }

        
        static List<int> RemoveZero(List<int> list, bool rev)
        {
            var result = new List<int>();
            if (rev)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] != 0)
                    {
                        result.Add(list[i]);
                    }
                }
            }
            else
            {
                foreach (int x in list)
                {
                    if (x != 0)
                    {
                        result.Add(x);
                    }
                }
            }

            return result;
        }
        

        static int ExecuteMove(ref int[,] Grid,  List<List<int>> rowcol, string move, bool rev=false)
        {
            int res = 0;
            for (int i = 0; i < rowcol.Count; i++)
            {
                List<int> tmp = new List<int>();
                var current = RemoveZero(rowcol[i], rev);
                bool Skip = false;
                if (current.Count > 2)
                {
                    for (int j = 0; j < current.Count-1; j++)
                    {
                        if (Skip)
                        {
                            Skip = false;
                            continue;
                        }
                        if (current[j] == current[j + 1])
                        {
                            res += 2 * current[j];
                            tmp.Add(2 * current[j]);
                            Skip = true;
                        }
                        else
                        {
                            tmp.Add(current[j]);
                        }
                    }
                    if (!Skip)
                    {
                        tmp.Add(current[current.Count-1]);
                    }
                }
                else
                {
                    if (current.Count == 2 && current[0] == current[1])
                    {
                        res += 2 * current[0];
                        tmp.Add(2 * current[0]);
                    }
                    else if (current.Count == 2)
                    {
                        tmp.Add(current[0]);
                        tmp.Add(current[1]);
                    }
                    else if (current.Count == 1)
                    {
                        tmp.Add(current[0]);
                    }
                }
                while (tmp.Count != GRIDSIZE)
                {
                    tmp.Add(0);
                }

                if (rev)
                {
                    tmp.Reverse();
                }

                if (move.ToUpper() == "W" || move.ToUpper() == "S")
                {
                    for (int gridi = 0; gridi < Grid.GetLength(0); gridi++)
                    {
                        Grid[gridi, i] = tmp[gridi];
                    }
                }
                else
                {
                    for (int gridj = 0; gridj < Grid.GetLength(0); gridj++)
                    {
                        Grid[i, gridj] = tmp[gridj];
                    }
                }
                
            }
            return res;
        }

        static void GetRowCol(int[,] Grid, ref List<List<int>> rows, ref List<List<int>> cols)
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                List<int> rtmp = new List<int>();
                List<int> ctmp = new List<int>();
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    rtmp.Add(Grid[i, j]);
                    ctmp.Add(Grid[j, i]);
                }
                rows.Add(rtmp);
                cols.Add(ctmp);
            }
        }

        static int[] GetAvailablePos(int[,] Grid)
        {
            List<int> available = new List<int>();
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (Grid[i,j] == 0)
                    {
                        available.Add(i * 4 + j);
                    }
                }
            }

            return available.ToArray();
        }

        // updates the grid by inserting a new number in an available position 
        static void InsertNextNum(ref int[,] Grid, int[] available)
        {
            if (available.Length == 0)
            {
                return;
            }
            // 75% chance for a 2, 25% for a 4
            int[] options = [2, 2, 2, 4];
            int index = rnd.Next(options.Length);
            // take the first item in the 1-item array generated by get items

            int pos = rnd.GetItems(available, 1)[0];

            int row = pos / 4;
            int col = pos % 4;

            Grid[row, col] = options[index];
        }

    }
}
