
using System;

namespace ASD
{
    class SpecialNumbers
    {
        const int mod = 10000;
		// count jest statycznym licznikiem, poniewaz rekurencyjna metoda
		// typu void, operujaca na tym liczniku, dziala szybciej niz
		// podobna metoda typu int dzialajaca na zmiennej lokalnej tej metody
		static int count = 0;

        // funkcja rekurencyjna
        // n cyfr
        public static int SpecialNumbersRec(int n)
        {
			// Pierwszy warunek dla "szczegolnego" przypadku
			if (n < 0)
				return 0;

			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			// Musi byc wyzerowane przed kazdym uruchomieniem
			count = 0;

			// 9 mozliwosci na pierwszej od lewej pozycji
			for (int i = 9; i >= 1; i--)
				recurence(n - 1, i);
            return count % mod;
        }
		//n - liczba cyfr, a - poprzednia cyfra
		//private static int recurence(int n, int a)
		private static void recurence(int n, int a)
		{
			if (n == 0)
				return;
			if (n == 1)
			{
				count += a / 2 + 1;
				if (count >= mod)
					count = count % mod;
				return;
			}
			if (a == 1) // po prawej moga byc juz tylko 1
			{
				++count;
				if (count >= mod)
					count = count % mod;
				return;
			}

			//zaczynamy od a - 1 zeby zachowac parzystosc, 
			//pozniej co dwie cyfry w dol, zeby zachowac parzystosc
			recurence(n - 1, a);
            for (int i = a - 1; i >= 1; i = i - 2)
			{
				if (n == 2)
				{
					count += i / 2 + 1;
					if (count >= mod)
						count = count % mod;
				}
				else
					recurence(n - 1, i);
            }

		}
        // programowanie dynamiczne
        // n cyfr
        public static int SpecialNumbersDP(int n)
        {
			// Pierwszy warunek dla "szczegolnego" przypadku
			if (n < 0)
				return 0;

			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			int[] tab = new int[9 + 1];

			for (int i = 1; i <= 9; i++)
				tab[i] = 1;

			for(int i = 2; i <= n; i++)
			{
				for(int j = 9; j >= 1; j--)
				{					
					for (int k = j - 1; k >= 1; k = k - 2)
					{
						tab[j] += tab[k];
						if (tab[j] >= mod)
							tab[j] = tab[j] % mod;
					}
				}
			}
			
			int count = 0;
			for (int i = 1; i <= 9; i++)
				count = (count + tab[i]) % mod;

			return count % mod;
        }

        // programowanie dynamiczne
        // n cyfr
        // req - tablica z wymaganiami, jezeli req[i, j] == 0 to znaczy, ze  i + 1 nie moze stac PRZED j + 1
        public static int SpecialNumbersDP(int n, bool[,] req)
        {
			// Pierwszy warunek dla "szczegolnego" przypadku
			if (n < 0)
				return 0;

			if (n == 0)
				return 0;
			if (n == 1)
				return 9;

			// Jezeli w tab przechowujemy dane dla liczb n-cyfrowych, 
			// to wektor pop zawiera dane dla liczb (n-1)-cyfrowych
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
						if (req[j - 1, k - 1])
						{
							tab[j] += pop[k];
							if (tab[j] >= mod)
								tab[j] = tab[j] % mod;
						}
					}
				}
				//Przepisanie i wyzerowanie:
				for (int j = 1; j <= 9; j++)
					pop[j] = tab[j];
				for (int j = 1; j <= 9; j++)
					tab[j] = 0;
            }

			int count = 0;
			for (int i = 1; i <= 9; i++)
			{
				count += pop[i];
				if (count >= mod)
					count = count % mod;
            }

			return count % mod;
        }

    }//class SpecialNumbers

}//namespace ASD
				