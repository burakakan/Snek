using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame
{
    class Initializer
    {
        static int width = 35, height = 20;
        int[,] snake = new int[2, 2] {
                { width / 3, height / 3 },
                { width / 3+1, height / 3 }};
        bool grow = true;
        ConsoleKey command;
        ConsoleKeyInfo lastKey;
        int[] prey = new int[2] { 16, 8 };
        Random rnd = new Random();

        public Initializer()
        {
            Console.CursorVisible = false;
            DrawTheBorders();

            for (int i = 0; i < snake.GetLength(0); i++)
            {
                Console.SetCursorPosition(snake[i, 0] + 1, snake[i, 1] + 1);
                Console.Write("O");
            }

            prey[0] = snake[snake.GetLength(0) - 1, 0];
            prey[1] = snake[snake.GetLength(0) - 1, 1];
            //Console.SetCursorPosition(prey[0] + 1, prey[1] + 1);
            //Console.Write('+');

            //Thread.Sleep(500);

            TimerCallback callback = new TimerCallback(Tick);
            Timer tmr = new Timer(callback, 5, 1000, 100);

            _ = Command();
        }
        void Tick(Object del)
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
            }

            if (UserTriesToReverse()) GoStraightAhead();

            for (int i = 0; i < snake.GetLength(0); i++)
            {
                snake[i, 0] = (snake[i, 0] + width) % width;
                snake[i, 1] = (snake[i, 1] + height) % height;
            }

            //if(Clash())

            Console.SetCursorPosition(snake[snake.GetLength(0) - 1, 0] + 1, snake[snake.GetLength(0) - 1, 1] + 1);
            Console.Write('O');

            grow = PreyIsEaten();

            if (grow)
            {
                //skor++;

                //make another prey appear at a random position
                do
                {
                    prey[0] = rnd.Next(width);
                    prey[1] = rnd.Next(height);
                }
                while (OverSnake());

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

            //Thread.Sleep(100);
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
        bool OverSnake()
        {
            for (int i = 0; i < snake.GetLength(0); i++)
                if (snake[i, 0] == prey[0] && snake[i, 1] == prey[1])
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
