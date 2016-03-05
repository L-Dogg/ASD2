
using System;

namespace ASD
{
    class SpecialNumbers
    {
        const int mod = 10000;
		
        // funkcja rekurencyjna
        // n cyfr
        public static int SpecialNumbersRec(int n)
        {
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			int count = 0;
			
			// 9 mozliwosci na pierwszej od lewej pozycji
			for (int i = 9; i >= 1; i--)
				count = (count + recurence(n-1, i)) % mod;
			return count % mod;
        }
		//n - liczba cyfr, a - poprzednia cyfra
		private static int recurence(int n, int a)
		{
			if (n == 0)
				return 0;
			if (n == 1)
				return a/2 + 1;

			if (a == 1)	// po prawej moga byc juz tylko 1
				return 1;

			int count = 0;

			//zaczynamy od a - 1 zeby zachowac parzystosc, 
			//pozniej co dwie cyfry w dol, zeby zachowac parzystosc
			count = (count + recurence(n - 1, a)) % mod;
			for (int i = a - 1; i >= 1; i = i - 2)
			{
				if (n == 2)
					count += i / 2 + 1;
				else
					count = (count + recurence(n - 1, i)) % mod;
			}
			
			return count;

		}
        // programowanie dynamiczne
        // n cyfr
        public static int SpecialNumbersDP(int n)
        {
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			//int[] pop = new int[9 + 1];
			int[] tab = new int[9 + 1];

			for (int i = 1; i <= 9; i++)
				tab[i] = 1;

			for(int i = 2; i <= n; i++)
			{
				for(int j = 9; j >= 1; j--)
				{
					//tab[j] = pop[j];
					
					for (int k = j - 1; k >= 1; k = k - 2)
					{
						//tab[j] = (tab[j] + pop[k]) % mod;
						tab[j] = (tab[j] + tab[k]) % mod;
					}
				}
				//tab.CopyTo(pop, 0);
			}
			
			int count = 0;
			for (int i = 1; i <= 9; i++)
				//count = (count + pop[i]) % mod;
				count = (count + tab[i]) % mod;

			return count % mod;
        }
		private static void printer(int[] tab)
		{
			for (int i = 1; i < tab.Length; i++)
				Console.Write("{0,2} ", tab[i]);
			Console.WriteLine();
		}
        // programowanie dynamiczne
        // n cyfr
        // req - tablica z wymaganiami, jezeli req[i, j] == 0 to znaczy, ze  i + 1 nie moze stac PRZED j + 1
        public static int SpecialNumbersDP(int n, bool[,] req)
        {
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			int[] pop = new int[9 + 1];
			int[] tab = new int[9 + 1];

			for (int i = 1; i <= 9; i++)
				pop[i] = 1;

			for (int i = 2; i <= n; i++)
			{
				for (int j = 1; j <= 9; j++)
				{
					for (int k = 1; k <= 9; k++)
					{
						if(req[k-1, j-1])
							tab[j] = (tab[j] + pop[k]) % mod;
					}
				}
				tab.CopyTo(pop, 0);
			}

			int count = 0;
			for (int i = 1; i <= 9; i++)
				count = (count + tab[i]) % mod;

			return count % mod;
        }

    }//class SpecialNumbers

}//namespace ASD
				