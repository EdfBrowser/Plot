using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    internal interface ITimeUnit
    {
        IReadOnlyList<int> Divisors { get; }
        TimeSpan MinSize { get; }

        //https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tostring
        string GetFormatString();
        DateTime Snap(DateTime dateTime);
        DateTime Next(DateTime dateTime, int increment = 1);
        int GetTickCount(DateTime minDT, DateTime maxDT, int inc);
        DateTime GetTick(DateTime minDT, int index, int inc);
    }
}
