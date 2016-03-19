﻿using System;

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
			{
				crossoutsNumber = 0;
				return true;
			}
			if (sequence.Length == 1)
			{
				if (comparePattern(patterns, sequence[0]))
				{
					crossoutsNumber = 1;
					return true;
				}
			}
			if (sequence.Length == 2)
			{
				if (comparePattern(patterns, sequence[0], sequence[1]))
				{
					crossoutsNumber = 1;
					return true;
				}
				else if (comparePattern(patterns, sequence[0]) && comparePattern(patterns, sequence[1]))
				{
					crossoutsNumber = 2;
					return true;
				}
			}
			int len = sequence.Length;
			int[,] b = new int[len, len];       // b[i,j] - czy fragment spojny [i,j] jest wymazywalny

			for (int i = 0; i < len; i++)
				for (int k = 0; k < len; k++)
					b[i, k] = int.MaxValue;

			for (int i = 0; i < len; i++)		// dla ciągów jednoelementowych
				if (comparePattern(patterns, sequence[i]))
					b[i, i] = 1;

			for (int i = 0; i < len - 1; i++)	// dla ciągów dwuelementowych
			{
				if (comparePattern(patterns, sequence[i], sequence[i + 1]))
					b[i, i + 1] = 1;
				else if (comparePattern(patterns, sequence[i]) && comparePattern(patterns, sequence[i + 1]))
					b[i, i + 1] = 2;
					
			}
			int j;
			for (int l = 3; l <= len; l++)  
			{
				for(int i = 0; i < len - l + 1; i++)
				{
					j = i + l - 1;
					for(int k = i; k < j; k++)
					{
						if (b[i, k] != int.MaxValue && b[k + 1, j] != int.MaxValue)
						{
							if(b[i, k] + b[k + 1, j] < b[i,j])
								b[i, j] = b[i, k] + b[k + 1, j];
						}
					}
					if (comparePattern(patterns, sequence[i], sequence[j]))
					{
						if (b[i + 1, j - 1] != int.MaxValue)
							b[i, j] = b[i + 1, j - 1] + 1;
					}
					else if (comparePattern(patterns, sequence[i]) && comparePattern(patterns, sequence[j]))
					{
						if (b[i + 1, j - 1] != int.MaxValue)
							b[i, j] = b[i + 1, j - 1] + 2;
					}
				}
			}
			crossoutsNumber = b[0, len - 1];
            return b[0, len - 1] != int.MaxValue;
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
			int crossoutsNumber = -1;
			if (sequence.Length == 0)
			{
				return 0;
			}
			if (sequence.Length == 1)
			{
				if (comparePattern(patterns, sequence[0]))
				{
					return 0;
				}
				return 1;
			}
			if (sequence.Length == 2)
			{
				if (comparePattern(patterns, sequence[0], sequence[1]))
				{
					return 0;
				}
				else if (comparePattern(patterns, sequence[0]) && comparePattern(patterns, sequence[1]))
				{
					return 0;
				}
				else if (comparePattern(patterns, sequence[0]) || comparePattern(patterns, sequence[1]))
				{
					return 1;
				}
				return 2;
			}
			int len = sequence.Length;
			int[,] b = new int[len, len];	// najdluzsza dlugosc podciagu wymazywalnego w [i,j]
			bool[,] p = new bool[len, len]; // b[i,j] - czy fragment spojny [i,j] jest wymazywalny
			/*for (int i = 0; i < len; i++)
				for (int k = 0; k < len; k++)
					b[i, k] = 0;
			*/
			for (int i = 0; i < len; i++)       // dla ciągów jednoelementowych
				if (comparePattern(patterns, sequence[i]))
				{
					p[i, i] = true;
					b[i, i] = 1;
				}

			for (int i = 0; i < len - 1; i++)   // dla ciągów dwuelementowych
			{
				if (comparePattern(patterns, sequence[i], sequence[i + 1]) ||
					(comparePattern(patterns, sequence[i]) && comparePattern(patterns, sequence[i + 1])))
				{
					b[i, i + 1] = 2;
					p[i, i + 1] = true;
				}

			}
			int j;
			for (int l = 3; l <= len; l++)
			{
				for (int i = 0; i < len - l + 1; i++)
				{
					j = i + l - 1;
					for (int k = i; k < j; k++)
					{
						if (p[i, k]  && p[k + 1, j])
						{
							p[i, j] = true;
						}

						if (b[i, k] + b[k + 1, j] > b[i, j])
							b[i, j] = b[i, k] + b[k + 1, j];
					}
					if (comparePattern(patterns, sequence[i], sequence[j]))
					{
						if (p[i + 1, j - 1])
						{
							p[i, j] = true;
						}

						if (b[i, j] < b[i + 1, j - 1] + 2)
							b[i, j] = b[i + 1, j - 1] + 2;
					}
					else if (comparePattern(patterns, sequence[i]) && comparePattern(patterns, sequence[j]))
					{
						if (p[i + 1, j - 1])
						{
							p[i, j] = true;
						}

						if (b[i, j] < b[i + 1, j - 1] + 2)
							b[i, j] = b[i + 1, j - 1] + 2;
					}
				}
			}
			if (p[0, len - 1])
				return 0;
			return len - b[0, len-1];
		}

    // można dopisać metody pomocnicze

    }
}
