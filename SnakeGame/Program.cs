using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame
{
    class Program
    {
        static Game game;
        static void Main(string[] args)
        {
            Run();
        }
        internal static void Run()
        {
            //Console.SetCursorPosition(2, 2);
            //Console.WriteLine("Specify the width: ");
            //Console.ReadLine(width);
            //Console.SetCursorPosition(2, 2);
            //Console.WriteLine("Specify the height: ");
            //Console.ReadLine(height);

            game = new Game(35,20);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
