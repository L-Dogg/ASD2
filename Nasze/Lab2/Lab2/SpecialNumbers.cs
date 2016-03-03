
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
			//Console.WriteLine("n = {0}, a = {1}", n, a);
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
				count = (count + recurence(n - 1, i)) % mod;
			}
			//Console.WriteLine("Count = {0}", count);
			return count;

		}
        // programowanie dynamiczne
        // n cyfr
        public static int SpecialNumbersDP(int n)
        {
            // ZMIEN
            return 0;
        }

        // programowanie dynamiczne
        // n cyfr
        // req - tablica z wymaganiami, jezeli req[i, j] == 0 to znaczy, ze  i + 1 nie moze stac PRZED j + 1
        public static int SpecialNumbersDP(int n, bool[,] req)
        {
            // ZMIEN
            return 0;
        }

    }//class SpecialNumbers

}//namespace ASD