using System.Collections.Generic;

namespace Plot.Skia
{
    internal static class StandardDivisors
    {
        internal static readonly IReadOnlyList<int> m_decimal = new int[] { 1, 5, 10 };
        internal static readonly IReadOnlyList<int> m_sexagesimal = new int[] { 1, 5, 10, 15, 20, 30, 60 };
        internal static readonly IReadOnlyList<int> m_dozenal = new int[] { 1, 2, 3, 4, 6, 12 };
        internal static readonly IReadOnlyList<int> m_hexadecimal = new int[] { 1, 2, 3, 4, 6, 8, 16 };
        internal static readonly IReadOnlyList<int> m_days = new int[] { 1, 3, 7, 14, 28 };
        internal static readonly IReadOnlyList<int> m_months = new int[] { 1, 3, 6 };
        internal static readonly IReadOnlyList<int> m_years = new int[] { 1, 2, 3, 4, 5, 10 };
    }
}
