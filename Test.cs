namespace AdventOfCode;
public class ConsoleGame : IRunnable
{
    private int gridSize = 10;
    private string[,] grid;
    private ConsoleColor[,] gridColors;
    private Random random = new Random();

    private int playerX = 0;
    private int playerY = 0;
    private const string playerChar = "██";

    private void InitializeGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = playerChar;
                gridColors[i, j] = ConsoleColor.White;
            }
        }
    }

    public ConsoleGame()
    {
        grid = new string[gridSize, gridSize];
        gridColors = new ConsoleColor[gridSize, gridSize];
        InitializeGrid();
        PlacePlayer();

        Console.CursorVisible = false;
        Console.Clear();
    }

    private void PlacePlayer()
    {
        grid[playerX, playerY] = playerChar;
        gridColors[playerX, playerY] = ConsoleColor.Green;
    }
    public void Run()
    {
        while (true)
        {
            Update();
            Render();
        }
    }

    private void Update()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            MovePlayer(key.Key);
            RefreshGrid();
            //Thread.Sleep(50);
        }
    }

    private void MovePlayer(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                if (playerY > 0) playerY--;
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                if (playerX > 0) playerX--;
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                if (playerY < gridSize - 1) playerY++;
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                if (playerX < gridSize - 1) playerX++;
                break;
        }
    }

    private void RefreshGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (i == playerY && j == playerX)
                {
                    grid[i, j] = playerChar;
                    gridColors[i, j] = ConsoleColor.Green;
                }
                else
                {
                    grid[i, j] = playerChar;
                    gridColors[i, j] = ConsoleColor.White;
                }
            }
        }
    }

    private void Render()
    {
        Console.SetCursorPosition(0, 0);

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Console.ForegroundColor = gridColors[i, j];
                if (i == playerY && j == playerX)
                {

                }
                Console.Write(grid[i, j]); // Added an extra space
            }
            Console.WriteLine();
        }
    }


}
