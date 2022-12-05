//-----------------------------------------------------------------------
// <copyright file="DeepThoughtTests.cs">
// Copyright (c) Benjamin Weirer. All rights reserved.
// </copyright>
// <author>Benjamin Weirer</author>
//-----------------------------------------------------------------------

using NUnit.Framework;
using System.Linq;
using System;
using Template.Lib;
using Template.Lib.Exceptions;
using System.Collections.Generic;

namespace Template.Test;

public class DeepThoughtTests
{

    [Test]
    public void Str8tsSolver_ReadArgs_Throw_Exception_On_Too_High_Grid()
    {
        var args = new[] { string.Concat(Enumerable.Repeat("w0,", 127)).Remove(127 * 3 - 1) + ';' };

        Assert.Throws<ArgumentException>(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_ReadArgs_Throw_Exception_On_Too_Long_Grid()
    {
        var args = Enumerable.Repeat("w0;", 127).ToArray();

        Assert.Throws<ArgumentException>(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_ReadArgs_Max_High_Grid()
    {
        var args = new[] { string.Concat(Enumerable.Repeat("w0,", 126)).Remove(126 * 3 - 1) + ';' };

        Assert.DoesNotThrow(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_ReadArgs_Max_Long_Grid()
    {
        var args = Enumerable.Repeat("w0;", 126).ToArray();

        Assert.DoesNotThrow(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_Init_Throw_ValidationException()
    {
        // Wrong separator in first row
        var args = new string[]
        {
            "w0;w0;",
            "w1,w0;"
        };

        Assert.That(() => { new Str8tsSolver(args); }, Throws.ArgumentException);
    }

    [Test]
    public void Str8tsSolver_Init_Throw_ValidationException_On_Small_Grid()
    {
        // First column digit 1 twice
        var args = new string[]
        {
            "w1,w0;",
            "w1,w0;"
        };

        Assert.Throws<Str8tsValidationException>(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_Init_Throw_ValidationException_On_Large_Grid()
    {
        // Last row digit 3 twice
        var args = new string[]
        {
            "w0.w0.b0,b5,w0.w0,b0,w0,w0;",
            "w0,w0,w0.w0,w0,w0,w0,w0,w0;",
            "b1,w9,w0,w0,b0,w0,w0,w0.w0|",
            "w0,w0,b0,w0,w0.b8,w3,w0,b0|",
            "w0.w0,w0,b0,w0,b0,w0,w0,w0|",
            "b0,w0.w0,b0,w0,w0,b2,w0,w0;",
            "w0,w0,w0,w0,b9,w0.w0.w0,b0;",
            "w0,w0,w0,w0,w0.w0,w0.w2,w0;",
            "w0,w0,b3.w0,w0,b0,b0.w0,w3|"
        };

        Assert.Throws<Str8tsValidationException>(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_Init_Invalid_Puzzles()
    {
        var multipleNumberInRowsArgs = new string[]
        {
            "w0,w1,b1;",
            "w0,b0,b2;",
            "b0,w0,w0;"
        };
        var multipleNumberInColsArgs = new string[]
        {
            "w0,w1,b2;",
            "w0,b0,b2;",
            "b0,w0,w0;"
        };
        var tooLargeNumberInArgs = new string[]
        {
            "w0,w1,b4;",
            "w0,b0,b2;",
            "b0,w0,w0;"
        };

        Assert.Throws<Str8tsValidationException>(() => { new Str8tsSolver(multipleNumberInRowsArgs); });
        Assert.Throws<Str8tsValidationException>(() => { new Str8tsSolver(multipleNumberInColsArgs); });
        Assert.Throws<Str8tsValidationException>(() => { new Str8tsSolver(tooLargeNumberInArgs); });
    }

    [Test]
    public void Str8tsSolver_Init_Valid_Puzzle()
    {
        var args = new string[]
        {
            "w0.w0.b0,b5,w0.w0,b0,w0,w0;",
            "w0,w0,w0.w0,w0,w0,w0,w0,w0;",
            "b1,w9,w0,w0,b0,w0,w0,w0.w0|",
            "w0,w0,b0,w0,w0.b8,w3,w0,b0|",
            "w0.w0,w0,b0,w0,b0,w0,w0,w0|",
            "b0,w0.w0,b0,w0,w0,b2,w0,w0;",
            "w0,w0,w0,w0,b9,w0.w0.w0,b0;",
            "w0,w0,w0,w0,w0.w0,w0.w2,w0;",
            "w0,w0,b3.w0,w0,b0,b0.w0,w0|"
        };

        Assert.DoesNotThrow(() => { new Str8tsSolver(args); });
    }

    [Test]
    public void Str8tsSolver_Init_Empty_Args()
    {
        var solver = new Str8tsSolver(Array.Empty<string>());
        var grid = new byte[9, 9];

        Assert.That(solver.Grid, Is.EqualTo(grid));
    }

    [Test]
    public void Str8tsSolver_Grid_creation_and_IsValid()
    {
        var args = new string[]
        {
            "w0,w0,b0,b5,w0,w0,b0,w0,w0;",
            "w0,w0,w0,w0,w0,w0,w0,w0,w0;",
            "b1,w9,w0,w0,b0,w0,w0,w0,w0;",
            "w0,w0,b0,w0,w0,b8,w3,w0,b0;",
            "w0,w0,w0,b0,w0,b0,w0,w0,w0;",
            "b0,w0,w0,b0,w0,w0,b2,w0,w0;",
            "w0,w0,w0,w0,b9,w0,w0,w0,b0;",
            "w0,w0,w0,w0,w0,w0,w0,w2,w0;",
            "w0,w0,b3,w0,w0,b0,b0,w0,w0;"
        };

        var grid = new byte[,]
        {
            {0, 0, 128 + 0, 128 + 5, 0, 0, 128, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {128 + 1, 9, 0, 0, 128, 0, 0, 0, 0},
            {0, 0, 128, 0, 0, 128 + 8, 3, 0, 128},
            {0, 0, 0, 128, 0, 128, 0, 0, 0},
            {128, 0, 0, 128, 0, 0, 128 + 2, 0, 0},
            {0, 0, 0, 0, 128 + 9, 0, 0, 0, 128},
            {0, 0, 0, 0, 0, 0, 0, 2, 0},
            {0, 0, 128 + 3, 0, 0, 128, 128, 0, 0},
        };

        var solver = new Str8tsSolver(args);

        Assert.That(solver.Grid, Is.EqualTo(grid));

        // Valid test
        Assert.That(solver.IsValid(solver.Grid, 2, (0, 0)), Is.EqualTo(true));
        Assert.That(solver.IsValid(solver.Grid, 4, (3, 7)), Is.EqualTo(true));
        Assert.That(solver.IsValid(solver.Grid, 1, (4, 4)), Is.EqualTo(true));
        Assert.That(solver.IsValid(solver.Grid, 2, (4, 4)), Is.EqualTo(true));

        //Assert.That(solver.IsValid(solver.Grid, 1, (0, 2)), Is.EqualTo(false)); // Insert to black field should never happen (caught outside IsValid())
        //Assert.That(solver.IsValid(solver.Grid, 0, (0, 0)), Is.EqualTo(false)); // Insert wrong value should never happen (caught outside IsValid())
        //Assert.That(solver.IsValid(solver.Grid, 10, (0, 0)), Is.EqualTo(false)); // Insert wrong value should never happen (caught outside IsValid())

        // Line violation
        Assert.That(solver.IsValid(solver.Grid, 1, (0, 0)), Is.EqualTo(false));
        Assert.That(solver.IsValid(solver.Grid, 2, (3, 7)), Is.EqualTo(false));
        Assert.That(solver.IsValid(solver.Grid, 9, (4, 4)), Is.EqualTo(false));
        Assert.That(solver.IsValid(solver.Grid, 5, (6, 3)), Is.EqualTo(false));
        Assert.That(solver.IsValid(solver.Grid, 3, (8, 1)), Is.EqualTo(false));

        // Strait violation
        Assert.That(solver.IsValid(solver.Grid, 5, (3, 7)), Is.EqualTo(false));
        Assert.That(solver.IsValid(solver.Grid, 1, (3, 7)), Is.EqualTo(false));
    }

    [Test]
    public void Str8tsSolver_Solve()
    {
        // Negative digits should be converted to positive
        var args = new string[]
        {
            "w0,w0,b0,w0,w0,b6;",
            "w0,w-4,w0,w0,w0,w1;",
            "b0,w0,w0,b0,w0,w0;",
            "b0,w0,w0,w0,w3,b0;",
            "w0,w0,b0,w4,w0,b0;",
            "w0,w2,b-3,b0,w0,w0;"
        };

        var expectedNumberOfSolutions = 1;
        var expectedResult = new byte[,]
        {
            {4, 3, 128 + 0, 1, 2, 128 + 6},
            {3, 4, 5, 2, 6, 1},
            {128 + 0, 5, 6, 128 + 0, 1, 2},
            {128 + 0, 6, 4, 5, 3, 128 + 0},
            {2, 1, 128 + 0, 4, 5, 128 + 0},
            {1, 2, 128 + 3, 128 + 0, 4, 5},
        };

        var solver = new Str8tsSolver(args);
        solver.Solve();

        var resultNumberOfSolutions = solver.SolvedGrids.Count;
        var resultSolution = solver.SolvedGrids[0].Item1;

        Assert.That(resultNumberOfSolutions, Is.EqualTo(expectedNumberOfSolutions));
        Assert.That(resultSolution, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Str8tsSolver_Solve_With_Real_Data()
    {
        // Standard str8ts puzzle
        var args = new string[]
        {
            "w0,w0,b0,b0,w0,w0,w4,w0,w1;",
            "w9,w0,w8,w0,w0,w0,w0,w0,b0;",
            "w0,w0,w0,w6,w0,w0,w0,w0,b0;",
            "w0,w0,w0,w0,w0,w8,b9,w0,w0;",
            "b0,w0,w0,w0,b0,w0,w0,w0,w0;",
            "w0,w5,w0,w0,w0,w0,w0,w0,w0;",
            "w2,w0,b0,w0,w0,w0,b0,w0,w0;",
            "b0,w1,w0,w3,w0,w0,w0,w2,b8;",
            "w0,w0,w0,b1,w0,w0,w0,w0,b0;"
        };

        var expectedNumberOfSolutions = 1;
        var expectedResult = new byte[,]
        {
            {8, 9, 128 + 0, 128 + 0, 2, 3, 4, 5, 1},
            {9, 6, 8, 5, 4, 7, 2, 3, 128 + 0},
            {7, 2, 9, 6, 5, 4, 3, 8, 128 + 0},
            {6, 7, 5, 4, 3, 8, 128 + 9, 1, 2},
            {128 + 0, 8, 6, 7, 128 + 0, 2, 5, 4, 3},
            {3, 5, 7, 2, 8, 1, 6, 9, 4},
            {2, 3, 128 + 0, 8, 7, 9, 128 + 0, 6, 5},
            {128 + 0, 1, 4, 3, 6, 5, 7, 2, 128 + 8},
            {5, 4, 3, 128 + 1, 9, 6, 8, 7, 128 + 0},
        };

        var solver = new Str8tsSolver(args);
        solver.Solve();

        var resultNumberOfSolutions = solver.SolvedGrids.Count;
        var resultSolution = solver.SolvedGrids[0].Item1;

        Assert.That(resultNumberOfSolutions, Is.EqualTo(expectedNumberOfSolutions));
        Assert.That(resultSolution, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Str8tsSolver_Solution_Found_Event()
    {
        // 3x2 str8ts puzzle => should have 4 solutions
        var args = new string[]
        {
            "w1,w0,w0;",
            "b0,w0,w0;"
        };

        List<Str8tsSolutionFoundEventArgs> callBacks = new List<Str8tsSolutionFoundEventArgs>();

        var solver = new Str8tsSolver(args);
        solver.Str8tsSolutionFound += (sender, args) => { callBacks.Add(args); };
        solver.Solve();

        Assert.That(callBacks.Count, Is.EqualTo(4));
        var first = callBacks[0];
        var second = callBacks[1];
        var third = callBacks[2];
        var fourth = callBacks[3];

        Assert.That(first.Grid.LongLength, Is.EqualTo(6));
        Assert.That(second.Grid.LongLength, Is.EqualTo(6));
        Assert.That(third.Grid.LongLength, Is.EqualTo(6));
        Assert.That(fourth.Grid.LongLength, Is.EqualTo(6));

        Assert.That(first.SolutionsCount, Is.EqualTo(1));
        Assert.That(second.SolutionsCount, Is.EqualTo(2));
        Assert.That(third.SolutionsCount, Is.EqualTo(3));
        Assert.That(fourth.SolutionsCount, Is.EqualTo(4));
    }
}
