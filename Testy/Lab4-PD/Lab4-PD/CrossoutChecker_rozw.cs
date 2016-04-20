using System;

namespace ASD
{
    class CrossoutChecker_rozw
    {
        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec x
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Jedyny znak szukanego wzorca</param>
        /// <returns></returns>
        static bool comparePattern(char[][] patterns, char x)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.Length == 1 && pat[0] == x)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec xy
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Pierwszy znak szukanego wzorca</param>
        /// <param name="y">Drugi znak szukanego wzorca</param>
        /// <returns></returns>
        static bool comparePattern(char[][] patterns, char x, char y)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.GetLength(0) == 2 && pat[0] == x && pat[1] == y)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda sprawdza, czy podany ciąg znaków można sprowadzić do ciągu pustego przez skreślanie zadanych wzorców.
        /// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
        /// </summary>
        /// <param name="sequence">Ciąg znaków</param>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="crossoutsNumber">Minimalna liczba skreśleń gwarantująca sukces lub int.MaxValue, jeżeli się nie da</param>
        /// <returns></returns>
        public static bool Erasable(char[] sequence, char[][] patterns, out int crossoutsNumber)
        {
            int n = sequence.Length;
            if (n == 0)
            {
                crossoutsNumber = 0;
                return true;
            }
            //reducible[i,j] to liczba skreśleń potrzebna, żeby skreślić fragment [i,i+1,...,j-1]
            int[,] reducible;
            makeDynamicTable(sequence, patterns, out reducible);
            crossoutsNumber = reducible[0, n];
            return reducible[0, n] < int.MaxValue;
        }

        static void makeDynamicTable(char[] sequence, char[][] patterns, out int[,] reducible)
        {
            int n = sequence.Length;
            reducible = new int[n + 1, n + 1];
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                    reducible[i, j] = int.MaxValue;
                reducible[i, i] = 0;
            }
            for (int d = 1; d <= n; d++)
            {
                for (int start = 0; start <= n - d; start++)
                {
                    int end = start + d;
                    if (d == 1 && comparePattern(patterns, sequence[start]))
                        reducible[start, end] = 1;
                    if (d == 2 && comparePattern(patterns, sequence[start], sequence[start + 1]))
                        reducible[start, end] = 1;
                    for (int k = start; k <= end; k++)
                        if (reducible[start, k] < int.MaxValue && reducible[k, end] < int.MaxValue)
                            reducible[start, end] = Math.Min(reducible[start, end], reducible[start, k] + reducible[k, end]);
                    if (start + 1 < end && reducible[start + 1, end - 1] < int.MaxValue)
                        if (reducible[start + 1, end - 1] + 1 < reducible[start, end] && comparePattern(patterns, sequence[start], sequence[end - 1]))
                            reducible[start, end] = reducible[start + 1, end - 1] + 1;
                }
            }
        }

        /// <summary>
        /// Metoda sprawdza, jaka jest minimalna długość ciągu, który można uzyskać z podanego poprzez skreślanie zadanych wzorców.
        /// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
        /// </summary>
        /// <param name="sequence">Ciąg znaków</param>
        /// <param name="patterns">Lista wzorców</param>
        /// <returns></returns>
        public static int MinimumRemainder(char[] sequence, char[][] patterns)
        {
            int n = sequence.GetLength(0);
            if (n == 0)
                return 0;
            //reducible[i,j] to liczba skreśleń potrzebna, żeby skreślić fragment [i,i+1,...,j-1]
            int[,] reducible;
            makeDynamicTable(sequence, patterns, out reducible);

            //maxReducibleSymbols[i,j] to maksymalna liczba symboli, które można skreślić z fragmentu [i,i+1,...,j-1]
            int[,] maxReducibleSymbols = new int[n + 1, n + 1];
            for (int d = 1; d <= n; d++)
                for (int i = 0; i <= n - d; i++)
                {
                    int j = i + d;
                    if (reducible[i, j] < int.MaxValue)
                        maxReducibleSymbols[i, j] = j - i;
                    for (int k = i; k <= j; k++)
                        maxReducibleSymbols[i, j] = Math.Max(maxReducibleSymbols[i, j], maxReducibleSymbols[i, k] + maxReducibleSymbols[k, j]);
                }

            return n - maxReducibleSymbols[0, n];
        }
    }
}
