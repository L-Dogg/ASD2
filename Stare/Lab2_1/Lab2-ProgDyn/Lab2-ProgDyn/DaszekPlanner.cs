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
		
		public DaszekPlanner(double[] odleglosci, double[] wysokosci, double maxDl)
        {
			this.odleglosci = odleglosci;
			this.wysokosci = wysokosci;
			this.maxDl = maxDl;
			koszty = new int[wysokosci.Length];
        }
		//zwraca dlugosc fragmentu o2-o1 o wysokosciach w2-w1 (parametry to indeksy do odp. tablicy)
		private double licz_fragment(int i, int j)
		{
			return Math.Sqrt(Math.Pow((odleglosci[j] - odleglosci[i]), 2) + Math.Pow((wysokosci[j] - wysokosci[i]), 2));
		}

		//zwraca koszt daszku w tysiacach zlotych:
		//parametr wyjsciowy: slupki, na ktorych beda sie opieraly kolejne fragmenty daszku
        public int KosztDaszku(out int[] daszek)
        {
            daszek = null;
			for (int k = 0; k < koszty.Length; k++)
				koszty[k] = int.MaxValue;
			koszty[0] = 0;
			koszty[1] = 1;
			
			for(int i = 2; i < odleglosci.Length - 1; i++)
			{
				for(int j = i + 1; j < odleglosci.Length; j++)
				{
					if(licz_fragment(i, j) <= maxDl)
					{
						koszty[i] = koszty[i - 1];
					}
					else
					{
						koszty[i] = Math.Min
					}
				}
			}

            return koszty[0];
        }

        public int KosztLadnegoDaszku(out int[] daszek)
        {
            daszek = null;
            return 0;
        }
    }
}
