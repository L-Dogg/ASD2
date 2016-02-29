using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    class DaszekPlanner
    {
		private double [] odleglosci, wysokosci;
		private double maxDl;
		private int[] koszty;
		private int len;

		public DaszekPlanner(double[] odleglosci, double[] wysokosci, double maxDl)
        {
			this.odleglosci = odleglosci;
			this.wysokosci = wysokosci;
			this.maxDl = maxDl;
			len = wysokosci.Length;
            koszty = new int[len];
        }
		//zwraca dlugosc fragmentu o2-o1 o wysokosciach w2-w1 (parametry to indeksy do odp. tablicy)
		private double licz_fragment(int i, int j)
		{
			return Math.Sqrt(Math.Pow((odleglosci[j] - odleglosci[i]), 2) + Math.Pow((wysokosci[j] - wysokosci[i]), 2));
		}

		private double licz_tangens(int i, int j)
		{
			return Math.Abs(wysokosci[j] - wysokosci[i]) / Math.Abs(odleglosci[j] - odleglosci[i]);
        }
		//zwraca koszt daszku w tysiacach zlotych:
		//parametr wyjsciowy: slupki, na ktorych beda sie opieraly kolejne fragmenty daszku
        public int KosztDaszku(out int[] daszek)
        {
            daszek = null;
			int[] poprzedni = new int[len];
			

			for(int i = 1; i < len; i++)
			{
				koszty[i] = i;
				poprzedni[i] = i - 1;

				for (int j = i - 1; j >= 0; j--)
				{
					if (licz_fragment(i, j) <= maxDl)
					{
						koszty[i] = koszty[j] + 1;
						poprzedni[i] = j;
					}
				}
			}

			daszek = new int[koszty[len - 1] + 1];
			int p = len - 1;
			for(int i = koszty[len-1]; i >= 0; i--)
			{
				daszek[i] = p;
				p = poprzedni[p];
			}
            return koszty[len - 1];
        }

        public int KosztLadnegoDaszku(out int[] daszek)
        {
			daszek = null;
			int[] poprzedni = new int[len];
			double max_tan = double.MinValue;

			for (int i = 1; i < len; i++)
			{
				koszty[i] = i;
				poprzedni[i] = i - 1;
				max_tan = 0;

				for (int j = i - 1; j >= 0; j--)
				{
					double cur_tan = licz_tangens(i, j);
					if (cur_tan >= max_tan && licz_fragment(i, j) <= maxDl)
					{
						koszty[i] = koszty[j] + 1;
						poprzedni[i] = j;
						max_tan = cur_tan;
					}
				}
			}

			daszek = new int[koszty[len - 1] + 1];
			int p = len - 1;
			for (int i = koszty[len - 1]; i >= 0; i--)
			{
				daszek[i] = p;
				p = poprzedni[p];
			}
			return koszty[len - 1];

		}
    }
}
