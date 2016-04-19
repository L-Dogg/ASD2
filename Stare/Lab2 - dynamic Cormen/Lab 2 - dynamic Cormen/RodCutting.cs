using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2___dynamic_Cormen
{
	//Mamy zdefiniowana dlugosc najdluzszego fragmentu i cene za fragment o danej dlugosci:
	//price[i] to cena fragmentu dlugosci i
	//Nalezy zwrocic najwiekszy przychod jaki mozna uzyskac tnac dany szajz (np. drzewo).
	class RodCutting
	{
		private int maxLen = 10;
		private int[] price = { 0, 1, 5, 8, 9, 10, 17, 17, 20, 24, 30 };

		public int cutRod(int len)
		{
			int[] income = new int[len + 1];
			int[] frags = new int[len + 1];
			income[0] = 0;

			int q;
			for(int j = 1; j <= len; j++)
			{
				q = int.MinValue;
				for(int i = 1; i <= j; i++)
				{
					// Bierzemy całą kłodę długości i + resztę długości j-i
					// (przychód dla j-i obliczony)
					if(q < price[i] + income[j - i])
					{
						q = price[i] + income[j - i];
						frags[j] = i;
                    }
				}
				income[j] = q;
			}
			printSolution(frags, len);
			return income[len];
		}

		private void printSolution(int[] sol, int len)
		{
			int n = len;
			Console.Write("{0} = ", len);
			while(n > 0)
			{
				Console.Write(" + {0}", sol[n]);
				n = n - sol[n];
			}
			Console.WriteLine();
        }
	}
}
