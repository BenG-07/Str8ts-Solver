//-----------------------------------------------------------------------
// <copyright file="Program.cs">
// Copyright (c) Benjamin Weirer. All rights reserved.
// </copyright>
// <author>Benjamin Weirer</author>
//-----------------------------------------------------------------------

using Template.Exe;
using Template.Lib;

Console.CursorVisible = false;
int delay = 10;

Str8tsSolver solver;
Str8tsVisualizer visualizer;

try
{
    solver = new(args);
}
catch (Exception e)
{
    Console.WriteLine(e);
    return;
}
visualizer = new(solver.Grid.GetLength(1), solver.Grid.GetLength(0));

solver.Str8tsSolutionFound += visualizer.SolutionFoundCallBack;

var solveTask = new Task<long>(() => solver.Solve());
solveTask.Start();

Console.Clear();
while (!solveTask.IsCompleted)
{
    visualizer.Visualize(solver.Grid);
    Thread.Sleep(delay);
}
visualizer.WriteBelowStr8ts($"finished after {solveTask.Result}ms");

if (solver.SolvedGrids.Count == 0)
    return;

var solvedGrid = solver.SolvedGrids[0].Item1;
visualizer.Visualize(solvedGrid);

ConsoleKey input;
int index = 0;
while ((input = Console.ReadKey(true).Key) != ConsoleKey.Enter)
{
    switch (input)
    {
        case ConsoleKey.RightArrow:
        case ConsoleKey.DownArrow:
            if (index < solver.SolvedGrids.Count - 1)
                index++;
            break;

        case ConsoleKey.LeftArrow:
        case ConsoleKey.UpArrow:
            if (index > 0)
                index--;
            break;
    }

    solvedGrid = solver.SolvedGrids[index].Item1;
    visualizer.Visualize(solvedGrid, index + 1);
}
