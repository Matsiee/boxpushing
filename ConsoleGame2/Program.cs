namespace ConsoleGame2;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Level> levels = new();

        var level1 = new[,]
        {
            { '#', '#', '#', '#', '#', '#' },
            { '#', '0', '0', '0', '0', '#' },
            { '#', '0', '0', '0', '0', '#' },
            { '#', '0', 'B', '0', '0', '#' },
            { '#', '0', '0', '0', '0', '#' },
            { '#', '#', '#', '#', '#', '#' }
        };
        levels.Add(new Level(level1));
        var map = level1;

        var playerRow = 1;
        var playerCol = 1;
        var boxes = new List<(int, int)>();
        bool running;

        var debug = "nothing to see here";
        do
        {
            running = RunGame();
        } while (running);

        bool RunGame()
        {
            Console.Clear();
            Console.WriteLine(debug);
            for (var row = 0; row < map.GetLength(0); row++)
            {
                for (var col = 0; col < map.GetLength(1); col++)
                    if (playerRow == row && playerCol == col)
                        Console.Write(".@");
                    else
                        switch (map[row, col])
                        {
                            case '#':
                                Console.Write(" #");
                                break;
                            case '0':
                                Console.Write("..");
                                break;
                            case 'B':
                                Console.Write("[]");
                                break;
                            case ' ':
                                Console.Write("  ");
                                break;
                        }

                Console.WriteLine();
            }

            var key = Console.ReadKey(true);
            var toRow = playerRow;
            var toCol = playerCol;
            Direction direction = new();
            switch (key.Key)
            {
                case ConsoleKey.W:
                    toRow = playerRow - 1;
                    direction = Direction.Up;
                    break;
                case ConsoleKey.A:
                    toCol = playerCol - 1;
                    direction = Direction.Left;
                    break;
                case ConsoleKey.S:
                    toRow = playerRow + 1;
                    direction = Direction.Down;
                    break;
                case ConsoleKey.D:
                    toCol = playerCol + 1;
                    direction = Direction.Right;
                    break;
                case ConsoleKey.Escape:
                    return false;
            }

            if (IllegalMove(toRow, toCol)) return true;

            if (map[toRow, toCol] == 'B')
            {
                var boxToRow = playerRow;
                var boxToCol = playerCol;
                switch (direction)
                {
                    case Direction.Up:
                        boxToRow = toRow - 1;
                        break;
                    case Direction.Left:
                        boxToCol = toCol - 1;
                        break;
                    case Direction.Down:
                        boxToRow = toRow + 1;
                        break;
                    case Direction.Right:
                        boxToCol = toCol + 1;
                        break;
                }

                if (IllegalBoxMove(boxToRow, boxToCol)) return true;
                map[boxToRow, boxToCol] = 'B';
                map[toRow, toCol] = '0';
            }

            playerRow = toRow;
            playerCol = toCol;

            return true;
        }

        bool IllegalBoxMove(int toRow, int toCol)
        {
            if (toRow < 0 || toRow >= map.GetLength(0) || toCol < 0 || toCol >= map.GetLength(1) ||
                map[toRow, toCol] != '0') return true;

            return false;
        }

        bool IllegalMove(int toRow, int toCol)
        {
            if (toRow < 0 || toRow >= map.GetLength(0) || toCol < 0 || toCol >= map.GetLength(1) ||
                map[toRow, toCol] == '#') return true;

            return false;
        }
    }

    private class Level
    {
        public Level(char[,] map)
        {
            Map = map;
        }

        public char[,] Map { get; set; }
    }

    private enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }
}