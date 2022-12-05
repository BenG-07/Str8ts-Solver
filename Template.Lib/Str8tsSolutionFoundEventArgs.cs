//-----------------------------------------------------------------------
// <copyright file="Str8tsSolutionFoundEventArgs.cs">
// Copyright (c) Benjamin Weirer. All rights reserved.
// </copyright>
// <author>Benjamin Weirer</author>
//-----------------------------------------------------------------------

namespace Template.Lib
{
    public class Str8tsSolutionFoundEventArgs : EventArgs
    {
        public byte[,] Grid { get; }

        public int SolutionsCount { get; }

        public Str8tsSolutionFoundEventArgs(byte[,] grid, int solutionsCount)
        {
            Grid = (byte[,])grid.Clone();
            SolutionsCount = solutionsCount;
        }
    }
}
