namespace ConsoleGame2;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Level> levels = new();

        using (var sr = new StreamReader("C:\\Users\\vmate\\RiderProjects\\ConsoleGame2\\ConsoleGame2\\Levels.txt"))
        {
            Console.WriteLine("Reading levels...");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var levelLines = new List<string>();
                while (!string.IsNullOrEmpty(line))
                {
                    levelLines.Add(line);
                    line = sr.ReadLine();
                }

                var map = new char[levelLines.Count, levelLines[0].Length];
                for (var i = 0; i < levelLines.Count; i++)
                for (var j = 0; j < levelLines[i].Length; j++)
                    map[i, j] = levelLines[i][j];

                levels.Add(new Level(map));
            }
        }

        var mapIndex = 0;
        var currentLevel = levels[mapIndex];
        var correctBoxCount = 0;

        bool running;
        do
        {
            running = RunGame();
        } while (running);

        bool RunGame()
        {
            var map = currentLevel.Map;
            var playerRow = currentLevel.PlayerPosition.Row;
            var playerCol = currentLevel.PlayerPosition.Col;
            Console.Clear();
            Console.WriteLine($" Level {mapIndex + 1}\n");
            for (var row = 0; row < map.GetLength(0); row++)
            {
                for (var col = 0; col < map.GetLength(1); col++)
                    switch (currentLevel.GetTile(row, col))
                    {
                        case 'P':
                            Console.Write(".@.");
                            break;
                        case 'B':
                            Console.Write("[-]");
                            break;
                        case '#':
                            Console.Write(" # ");
                            break;
                        case '0':
                            Console.Write("...");
                            break;
                        case 'S':
                            Console.Write("///");
                            break;
                        case ' ':
                            Console.Write("   ");
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
                case ConsoleKey.R:
                    correctBoxCount = 0;
                    currentLevel.Reset();
                    return true;
                case ConsoleKey.L:
                    if (mapIndex == levels.Count - 1) return true;
                    mapIndex++;
                    currentLevel = levels[mapIndex];
                    return true;
                case ConsoleKey.K:
                    if (mapIndex == 0) return true;
                    mapIndex--;
                    currentLevel = levels[mapIndex];
                    return true;
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    return false;
            }

            if (IllegalMove(toRow, toCol)) return true;

            if (currentLevel.GetTile(toRow, toCol) == 'B')
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
                if (map[toRow, toCol] != 'S' && map[boxToRow, boxToCol] == 'S')
                {
                    correctBoxCount++;
                    if (correctBoxCount == levels[mapIndex].BoxCount)
                    {
                        correctBoxCount = 0;
                        mapIndex++;
                        if (mapIndex == levels.Count)
                        {
                            Console.Clear();
                            Console.WriteLine(" You Win!");
                            return false;
                        }

                        currentLevel = levels[mapIndex];
                        return true;
                    }
                }

                if (map[toRow, toCol] == 'S' && map[boxToRow, boxToCol] != 'S') correctBoxCount--;

                currentLevel.BoxPositions[currentLevel.FindBox(toRow, toCol)] = new Position(boxToRow, boxToCol);
            }

            currentLevel.PlayerPosition = new Position(toRow, toCol);
            return true;

            bool IllegalBoxMove(int row, int col)
            {
                if (row < 0 || row >= map.GetLength(0) || col < 0 || col >= map.GetLength(1) ||
                    currentLevel.GetTile(row, col) == '#' || currentLevel.GetTile(row, col) == 'B') return true;
                return false;
            }

            bool IllegalMove(int row, int col)
            {
                if (row < 0 || row >= map.GetLength(0) || col < 0 || col >= map.GetLength(1) ||
                    currentLevel.GetTile(row, col) == '#') return true;

                return false;
            }
        }
    }

    private class Level
    {
        public Level(char[,] map)
        {
            OriginalMap = map.Clone() as char[,];
            Map = map.Clone() as char[,];
            PlayerPosition = new Position(0, 0);
            BoxPositions = new List<Position>();
            for (var row = 0; row < map.GetLength(0); row++)
            for (var col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] == 'P')
                {
                    PlayerPosition = new Position(row, col);
                    Map[row, col] = '0';
                }

                if (map[row, col] == 'B')
                {
                    BoxPositions.Add(new Position(row, col));
                    Map[row, col] = '0';
                }
            }
        }

        public char[,] Map { get; private set; }
        private char[,] OriginalMap { get; }
        public Position PlayerPosition { get; set; }
        public List<Position> BoxPositions { get; set; }
        public int BoxCount => BoxPositions.Count;

        public char GetTile(int row, int col)
        {
            if (BoxPositions.Exists(box => box.Row == row && box.Col == col)) return 'B';
            if (PlayerPosition.Col == col && PlayerPosition.Row == row) return 'P';
            return Map[row, col];
        }

        public int FindBox(int row, int col)
        {
            var ind = 0;
            while (ind < BoxCount)
            {
                if (BoxPositions[ind].Row == row && BoxPositions[ind].Col == col) return ind;
                ind++;
            }

            return -1;
        }

        public void Reset()
        {
            Map = OriginalMap.Clone() as char[,];
            BoxPositions = new List<Position>();
            for (var row = 0; row < OriginalMap.GetLength(0); row++)
            for (var col = 0; col < OriginalMap.GetLength(1); col++)
            {
                if (OriginalMap[row, col] == 'P')
                {
                    PlayerPosition = new Position(row, col);
                    Map[row, col] = '0';
                }

                if (OriginalMap[row, col] == 'B')
                {
                    BoxPositions.Add(new Position(row, col));
                    Map[row, col] = '0';
                }
            }
        }
    }

    private record Position(int Row, int Col);

    private enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }
}