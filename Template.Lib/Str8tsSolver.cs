//-----------------------------------------------------------------------
// <copyright file="Str8tsSolver.cs">
// Copyright (c) Benjamin Weirer. All rights reserved.
// </copyright>
// <author>Benjamin Weirer</author>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Template.Lib.Exceptions;

namespace Template.Lib;

public class Str8tsSolver
{

    public static readonly byte Color = 128;

    public static readonly char[] RowSeparators = { ';', '|' };
    public static readonly char[] ColSeparators = { ',', '.' };

    private byte[,] _grid;

    public byte[,] Grid
    {
        get => _grid;
        private set
        {
            _grid = value;

            _rows = (byte)_grid.GetLength(0);
            _cols = (byte)_grid.GetLength(1);
            _longest = _rows >= _cols ? _rows : _cols;
            _rowGroups = new (byte, byte)[_rows, _cols];
            _colGroups = new (byte, byte)[_rows, _cols];
            _rowNumbers = new bool[_rows, _longest];
            _colNumbers = new bool[_cols, _longest];
        }
    }

    private byte _rows;
    private byte _cols;
    private byte _longest;

    private (byte, byte)[,] _rowGroups;
    private (byte, byte)[,] _colGroups;
    private bool[,] _rowNumbers;
    private bool[,] _colNumbers;

    private ulong _counter;
    public List<(byte[,], long, ulong)> SolvedGrids { get; }

    public EventHandler<Str8tsSolutionFoundEventArgs> Str8tsSolutionFound;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args">A valid str8ts puzzle.</param>
    /// <exception cref="ArgumentException">Is thrown when the arguments are not in specified format or the puzzle itself is invalid.</exception>
    /// <exception cref="Str8tsValidationException">Is thrown if the str8ts puzzle is not valid.</exception>
    public Str8tsSolver(string[] args)
    {
        try
        {
            ReadArgs(args);
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        Init(_grid);

        SolvedGrids = new();
    }

    public long Solve()
    {
        var sw = Stopwatch.StartNew();
        foreach (var solvedGrid in Solve(_grid, (0, 0)))
        {
            SolvedGrids.Add(((byte[,])_grid.Clone(), sw.ElapsedMilliseconds, _counter));
            FireSolutionFound(solvedGrid, SolvedGrids.Count);
        }

        sw.Stop();

        return sw.ElapsedMilliseconds;
    }

    /// <summary>
    /// Initializes all fields to solve the str8ts puzzle faster.
    /// </summary>
    /// <param name="grid">The str8ts puzzle.</param>
    /// <exception cref="Str8tsValidationException">Is thrown if the str8ts puzzle is not valid.</exception>
    internal void Init(byte[,] grid)
    {
        byte start;

        // Inspect row by row
        for (byte row = 0; row < _rows; row++)
        {
            // reset values
            start = 0;

            for (byte col = 0; col < _cols; col++)
            {
                var num = (byte)(grid[row, col] % Color);
                var color = (byte)(grid[row, col] & Color);

                if (num > _longest)
                    throw new Str8tsValidationException($"Error in row {row} - number {num} is larger than the longest dimension {_longest}!");

                // If black cell
                if (color != 0)
                {
                    // If previous cell(s) were a white group  => add group start and end to RowGroups
                    if (col != start)
                        for (int groupCol = start; groupCol < col; groupCol++)
                            _rowGroups[row, groupCol] = (start, (byte)(col - 1));

                    start = (byte)(col + 1);

                    if (num != 0)
                    {
                        if (_rowNumbers[row, num - 1])
                            throw new Str8tsValidationException($"Error in row {row} - number {num} is contained multiple times!");
                        _rowNumbers[row, num - 1] = true;
                    }

                    continue;
                }

                if (num == 0)
                    continue;

                if (_rowNumbers[row, num - 1])
                    throw new Str8tsValidationException($"Error in row {row} - number {num} is contained multiple times!");

                _rowNumbers[row, num - 1] = true;
            }

            // If last cell(s) were a white group  => add group start and end to RowGroups
            if (_cols != start)
                for (int groupCol = start; groupCol < _cols; groupCol++)
                    _rowGroups[row, groupCol] = (start, (byte)(_cols - 1));
        }

        // Inspect col by col
        for (byte col = 0; col < _cols; col++)
        {
            // reset values
            start = 0;

            for (byte row = 0; row < _rows; row++)
            {
                var num = (byte)(grid[row, col] % Color);
                var color = (byte)(grid[row, col] & Color);

                // If black cell
                if (color != 0)
                {
                    // If previous cell(s) were a white group  => add group start and end to ColGroups
                    if (row != start)
                        for (int groupRow = start; groupRow < row; groupRow++)
                            _colGroups[groupRow, col] = (start, (byte)(row - 1));

                    start = (byte)(row + 1);

                    if (num != 0)
                    {
                        if (_colNumbers[col, num - 1])
                            throw new Str8tsValidationException($"Error in col {col} - number {num} is contained multiple times!");
                        _colNumbers[col, num - 1] = true;
                    }

                    continue;
                }

                if (num == 0)
                    continue;

                if (_colNumbers[col, num - 1])
                    throw new Str8tsValidationException($"Error in col {col} - number {num} is contained multiple times!");
                _colNumbers[col, num - 1] = true;

                _colNumbers[col, num - 1] = true;
            }

            // If last cell(s) were a white group  => add group start and end to ColGroups
            if (_rows != start)
                for (int groupRow = start; groupRow < _rows; groupRow++)
                    _colGroups[groupRow, col] = (start, (byte)(_rows - 1));
        }
    }

    internal bool IsValid(byte[,] grid, byte value, (byte, byte) pos)
    {
        _counter++;
        // Check row
        if (_rowNumbers[pos.Item1, value - 1])
            return false;

        // Check column
        if (_colNumbers[pos.Item2, value - 1])
            return false;

        // Check row-group
        var rowGroup = _rowGroups[pos.Item1, pos.Item2];
        var length = (byte)(rowGroup.Item2 - rowGroup.Item1);
        var min = value;
        var max = value;

        if (length != _longest - 1)
        {
            for (var col = rowGroup.Item1; col <= rowGroup.Item2; col++)
            {
                var num = (byte)(grid[pos.Item1, col] % Color);

                if (num == 0)
                    continue;

                if (num < min)
                    min = num;
                if (num > max)
                    max = num;
            }

            if (length < max - min)
                return false;
        }

        // Check col-group
        var colGroup = _colGroups[pos.Item1, pos.Item2];
        length = (byte)(colGroup.Item2 - colGroup.Item1);
        min = value;
        max = value;

        if (length != _longest - 1)
        {
            for (var row = colGroup.Item1; row <= colGroup.Item2; row++)
            {
                var num = (byte)(grid[row, pos.Item2] % Color);

                if (num == 0)
                    continue;

                if (num < min)
                    min = num;
                if (num > max)
                    max = num;
            }

            if (length < max - min)
                return false;
        }

        return true;
    }

    internal IEnumerable<byte[,]> Solve(byte[,] grid, (byte, byte) pos)
    {
        byte row = pos.Item1, col = pos.Item2;

        for (; row < _rows; row++)
        {
            for (; col < _cols; col++)
            {
                var num = grid[row, col];

                // If cell is black or number is predetermined
                if ((num & Color) != 0 ||
                    (num % Color) != 0)
                    continue;

                for (byte i = 1; i <= _longest; i++)
                {
                    if (IsValid(grid, i, (row, col)))
                    {
                        grid[row, col] = i;
                        _rowNumbers[row, i - 1] = true;
                        _colNumbers[col, i - 1] = true;

                        foreach (var solution in Solve(grid, (row, col)))
                            yield return solution;

                        grid[row, col] = 0;
                        _rowNumbers[row, i - 1] = false;
                        _colNumbers[col, i - 1] = false;
                    }
                }

                yield break;
            }

            col = 0;
        }

        yield return grid;
    }

    internal void ReadArgs(string[] args)
    {
        switch (args.Length)
        {
            // Default grid
            case 0:
                Grid = new byte[9, 9];
                return;

            // Split args into rows
            case 1:
                args = args[0].Split(RowSeparators, StringSplitOptions.RemoveEmptyEntries);
                break;
        }

        var height = args.Length;
        var length = args[0].Split(ColSeparators).Length;

        if (height > 126 || length > 126)
            throw new ArgumentException($"The maximum dimensions are 127x127, current are {length}x{height}");

        // Create grid and fill with args
        Grid = new byte[height, length];
        for (var row = 0; row < args.Length; row++)
        {
            var values = args[row].Split(ColSeparators);
            for (var col = 0; col < values.Length; col++)
            {
                var value = values[col].Trim(RowSeparators.Union(new char[] { ' ', '\n', '\r' }).ToArray()).ToLower();
                byte num = (byte)Math.Abs(int.Parse(value[1..]));

                if (value[0] == 'b')
                    num += Color;

                Grid[row, col] = num;
            }
        }
    }

    internal void FireSolutionFound(byte[,] grid, int solutions)
    {
        this.Str8tsSolutionFound?.Invoke(this, new Str8tsSolutionFoundEventArgs(grid, solutions));
    }
}
