using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SnakeGame
{
    class Game
    {
        static int width = 35, height = 20;
        short interval = 100;
        int score = -1;
        int[,] snake = new int[2, 2] {
                { width / 3, height / 3 },
                { width / 3+1, height / 3 }};
        bool grow = true;
        ConsoleKey command;
        ConsoleKeyInfo lastKey;
        int[] prey = new int[2];
        Random rnd = new Random();

        internal Timer tmr { get; private set; }

        public Game()
        {
            Console.CursorVisible = false;
            Console.Clear();
            DrawTheBorders();

            for (int i = 0; i < snake.GetLength(0); i++)
            {
                Console.SetCursorPosition(snake[i, 0] + 1, snake[i, 1] + 1);
                Console.Write("O");
            }

            prey[0] = snake[snake.GetLength(0) - 1, 0];
            prey[1] = snake[snake.GetLength(0) - 1, 1];

            tmr = new Timer();
            tmr.Interval = interval;
            tmr.Elapsed += Tick;
            tmr.AutoReset = false;
            tmr.Start();

            _ = Command();
        }

        void Tick(object sender, ElapsedEventArgs e)
        {
            //basılan tuşa göre array'in son elemanına yeni konum bilgisini gir
            switch (Command())
            {
                case ConsoleKey.NoName:
                    GoStraightAhead();
                    break;
                case ConsoleKey.LeftArrow:
                    snake[snake.GetLength(0) - 1, 0]--;
                    break;
                case ConsoleKey.UpArrow:
                    snake[snake.GetLength(0) - 1, 1]--;
                    break;
                case ConsoleKey.RightArrow:
                    snake[snake.GetLength(0) - 1, 0]++;
                    break;
                case ConsoleKey.DownArrow:
                    snake[snake.GetLength(0) - 1, 1]++;
                    break;
                case ConsoleKey.Spacebar:
                    tmr.Stop();
                    while (Command() != ConsoleKey.Spacebar)
                        Thread.Sleep(interval);
                    GoStraightAhead();
                    tmr.Start();
                    break;
            }

            if (UserTriesToReverse()) GoStraightAhead();

            for (int i = 0; i < snake.GetLength(0); i++)
            {
                snake[i, 0] = (snake[i, 0] + width) % width;
                snake[i, 1] = (snake[i, 1] + height) % height;
            }

            if (Clash())
            {
                tmr.Stop();
                Console.SetCursorPosition(width + 5, 3);
                Console.WriteLine("Yandın. Bi' daha? (E/H)");
                tmr.Dispose();

                do
                    lastKey = Console.ReadKey(true);
                while (lastKey.Key != ConsoleKey.E && lastKey.Key != ConsoleKey.H);

                if (lastKey.Key == ConsoleKey.E)
                    Program.Run();
                else
                    Environment.Exit(0);
            }

            Console.SetCursorPosition(snake[snake.GetLength(0) - 1, 0] + 1, snake[snake.GetLength(0) - 1, 1] + 1);
            Console.Write('O');

            grow = PreyIsEaten();

            if (grow)
            {
                score++;
                Console.SetCursorPosition(width + 5, 5);
                Console.Write("Skor: " + score);

                //make another prey appear at a random position
                do
                {
                    prey[0] = rnd.Next(width);
                    prey[1] = rnd.Next(height);
                }
                while (Clash(prey[0], prey[1]));

                Console.SetCursorPosition(prey[0] + 1, prey[1] + 1);
                Console.Write('+');
            }
            else
            {
                //erase the tail from screen
                Console.SetCursorPosition(snake[0, 0] + 1, snake[0, 1] + 1);
                Console.Write(' ');
            }
            snake = NewSnake(snake, grow);

            tmr.Start();
        }
        ConsoleKey Command()
        {
            command = ConsoleKey.NoName;
            while (Console.KeyAvailable)
            {
                lastKey = Console.ReadKey(true);
                if (lastKey.Key == ConsoleKey.Spacebar || lastKey.Key == ConsoleKey.RightArrow || lastKey.Key == ConsoleKey.UpArrow || lastKey.Key == ConsoleKey.LeftArrow || lastKey.Key == ConsoleKey.DownArrow)
                {
                    command = lastKey.Key;
                }
            }
            return command;
        }
        void GoStraightAhead()
        {
            try
            {
                for (int j = 0; j < snake.GetLength(1); j++)
                    snake[snake.GetLength(0) - 1, j] = 2 * snake[snake.GetLength(0) - 2, j] - snake[snake.GetLength(0) - 3, j];
            }
            catch { }
        }
        bool UserTriesToReverse()
        {
            try
            {
                for (int j = 0; j < snake.GetLength(1); j++)
                    if ((snake[snake.GetLength(0) - 1, j] + ((1 - j) * width + j * height)) % ((1 - j) * width + j * height) != (snake[snake.GetLength(0) - 3, j] + ((1 - j) * width + j * height)) % ((1 - j) * width + j * height))
                        return false;
            }
            catch { }

            return true;
        }
        int[,] NewSnake(int[,] arr1, bool eaten)
        {
            int[,] arr2 = new int[arr1.GetLength(0) + (eaten ? 1 : 0), arr1.GetLength(1)];
            for (int i = 0; i < arr2.GetLength(0); i++)
                for (int j = 0; j < arr2.GetLength(1); j++)
                    arr2[i, j] = i == arr2.GetLength(0) - 1 ? arr1[i - (eaten ? 1 : 0), j] : arr1[i + (eaten ? 0 : 1), j];

            return arr2;
        }
        bool PreyIsEaten()
        {
            for (int j = 0; j < snake.GetLength(1); j++)
                if (snake[snake.GetLength(0) - 1, j] != prey[j])
                    return false;
            return true;
        }
        bool Clash(int preyX, int preyY)
        {
            for (int i = 0; i < snake.GetLength(0); i++)
                if (snake[i, 0] == preyX && snake[i, 1] == preyY)
                    return true;
            return false;
        }
        bool Clash()
        {
            for (int i = 0; i < snake.GetLength(0) - 4; i++)
                if (snake[i, 0] == snake[snake.GetLength(0) - 1, 0] && snake[i, 1] == snake[snake.GetLength(0) - 1, 1])
                    return true;
            return false;
        }
        void DrawTheBorders()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write('┌');

            Console.SetCursorPosition(0, height + 1);
            Console.Write('└');

            Console.SetCursorPosition(width + 1, 0);
            Console.Write('┐');

            Console.SetCursorPosition(width + 1, height + 1);
            Console.Write('┘');

            for (int i = 1; i <= width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write('─');
                Console.SetCursorPosition(i, height + 1);
                Console.Write('─');
            }
            for (int i = 1; i <= height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write('│');
                Console.SetCursorPosition(width + 1, i);
                Console.Write('│');
            }
        }
    }
}
