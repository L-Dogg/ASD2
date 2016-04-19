using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2___dynamic_Cormen
{
	class Cashier
	{
		//dobrane nominaly uniemozliwiaja znalezienie
		//optymalnego rozwiazania algorytmem zachlannym
		private int[] nominals = { 1, 5, 6 };
		private List<int>[] coins;

		//zwraca ile monet uzyjemy do wydania change szekli
		public int getChange(int change)
		{
			int[] tab = new int[change + 1];
			coins = new List<int>[change + 1];

			for (int i = 1; i <= change; i++)
			{
				coins[i] = new List<int>();
				tab[i] = int.MaxValue;
			}

			return countChange(tab, change);
		}

		private int countChange(int[] tab, int change)
		{
			int q;
			// juz obliczylismy te wartosc:
			if (tab[change] < int.MaxValue)
				return tab[change];

			//wystarczy jedna moneta:
			for (int i = 0; i < nominals.Length; i++)
			{
				if (change == nominals[i])
				{
					coins[change].Add(change);
					tab[change] = 1;
					return 1;
				}
			}
			
			//rekurencyjnie tak jak Pan Kotowski powiedzial
			q = int.MaxValue;
			for(int i = 1; i < change; i++)
			{
				//q = Math.Min(q, countChange(tab, i) + countChange(tab,change - i));
				int k = countChange(tab, i) + countChange(tab, change - i);
                if (q > k)
				{
					q = k;
					coins[change].Clear();
					coins[change].AddRange(coins[i]);
					coins[change].AddRange(coins[change - i]);
				}

			}
			tab[change] = q;
			Console.Write("\n{0} = ", change);
			foreach (int x in coins[change])
				Console.Write(" + {0}", x);
			Console.WriteLine();
			return q;
		}
	}
}
