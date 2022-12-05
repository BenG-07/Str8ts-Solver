//-----------------------------------------------------------------------
// <copyright file="Str8tsVisualizer.cs">
// Copyright (c) Benjamin Weirer. All rights reserved.
// </copyright>
// <author>Benjamin Weirer</author>
//-----------------------------------------------------------------------

using Template.Lib;

namespace Template.Exe
{
    public class Str8tsVisualizer
    {
        public static readonly byte Color = 128;

        private byte[,]? _lastGrid;

        private readonly object _locker;

        private readonly int _rows;

        private readonly int _cols;

        private int _solutionsCount;

        public Str8tsVisualizer(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
            _locker = new object();
        }

        public void Visualize(byte[,] origGrid)
        {
            var grid = (byte[,])origGrid.Clone();

            lock (_locker)
            {
                Console.SetCursorPosition(0, 0);

                for (var row = 0; row < _rows; row++)
                {
                    for (var col = 0; col < _cols; col++)
                    {
                        var num = grid[row, col];
                        var lastNum = _lastGrid == null ? -1 : _lastGrid[row, col];
                        if (num == lastNum)
                            continue;

                        Console.SetCursorPosition(col * 2, row);

                        if ((num & Color) == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else if (num % Color == 0)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                        }

                        Console.Write(num % 128 == 0 ? " " : (num % 128 % 10).ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                        Console.WriteLine();
                    }
                }
            }

            _lastGrid = grid;
        }

        public void Visualize(byte[,] origGrid, int index)
        {
            Visualize(origGrid);
            WriteBelowStr8ts($"Currently showing solution: {index}/{_solutionsCount}");
        }

        public void WriteBelowStr8ts(string message, int indent = 0)
        {
            lock (_locker)
            {
                Console.SetCursorPosition(0, _rows + indent);
                Console.WriteLine(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, _rows + indent);
                Console.WriteLine(message);
            }
        }

        public void SolutionFoundCallBack(object? sender, Str8tsSolutionFoundEventArgs args)
        {
            _solutionsCount = args.SolutionsCount;
            WriteBelowStr8ts($"Solutions found: {_solutionsCount}", 2);
        }
    }
}
