using System;

namespace ASD
{
    class CrossoutChecker
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
            crossoutsNumber = -1;
			if (sequence.Length == 0)
				return true;
			if (sequence.Length == 1)
				if (comparePattern(patterns, sequence[0]))
					return true;
			if (sequence.Length == 2)
				if (comparePattern(patterns, sequence[0], sequence[1]) || 
					(comparePattern(patterns, sequence[0]) && comparePattern(patterns, sequence[1])))
					return true;

			int len = sequence.Length;
			bool[,] b = new bool[len, len];		// b[i,j] - czy fragment spojny [i,j] jest wymazywalny

			for (int i = 0; i < len; i++)		// dla ciągów jednoelementowych
				if (comparePattern(patterns, sequence[i]))
					b[i, i] = true;

			for (int i = 0; i < len - 1; i++)	// dla ciągów dwuelementowych
			{
				if (comparePattern(patterns, sequence[i], sequence[i + 1]) ||
					(comparePattern(patterns, sequence[i]) && comparePattern(patterns, sequence[i + 1])))	// czy fragment jest wzorcem lub konkatenacją 2 wzorców
					b[i, i + 1] = true;
			}
//			printb(b);
			int j;
			for (int l = 3; l < len; l++)   // właściwe sprawdzenie: od fragmentow dlugosci 3
			{
				for(int i = 0; i < len - l + 1; i++)
				{
					j = i + l - 1;			// ustawiamy koniec podciągu długości l o poczatku i
					for(int k = i; k < j; k++)
					{
						if (k + 1 < j)      // żeby nie wyjść za tablicę
						{
							if (b[i, k] && b[k + 1, j])
							{
								b[i, j] = true;
								break;
							}
						}
						// przypadek typu AxB z przykladu: wycieramy x a potem AB:
						
					}
				}
			}
            return b[0, len - 1];
        }
		public static void printb(bool[,] tab)
		{
			for(int i = 0; i < tab.GetLength(0); i++)
			{
				for (int j = 0; j < tab.GetLength(1); j++)
					Console.Write("{0, 2} ", tab[i, j] ? 1 : 0);
				Console.WriteLine();
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
            return -1;
        }

    // można dopisać metody pomocnicze

    }
}
