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
		private double[] koszty;
		
		public DaszekPlanner(double[] odleglosci, double[] wysokosci, double maxDl)
        {
			this.odleglosci = odleglosci;
			this.wysokosci = wysokosci;
			this.maxDl = maxDl;
			koszty = new double[wysokosci.Length];
        }
		//zwraca dlugosc fragmentu o2-o1 o wysokosciach w2-w1 (parametry to indeksy do odp. tablicy)
		private double licz_fragment(int o1, int o2, int w1, int w2)
		{
			return Math.Sqrt(Math.Pow((odleglosci[o2] - odleglosci[o1]), 2) + Math.Pow((wysokosci[w2] - wysokosci[w1]), 2));
		}

		//zwraca koszt daszku w tysiacach zlotych:
		//parametr wyjsciowy: slupki, na ktorych beda sie opieraly kolejne fragmenty daszku
        public int KosztDaszku(out int[] daszek)
        {
            daszek = null;
			List<int> wierzcholki = new List<int>();

			//dodajemy poczatek daszku:
			wierzcholki.Add(0); 
			int pocz = 0;

			int kon = 1;
			double frag = 0;
			int koszt = 0;
			bool last_is_short = false;

			for(int i = 1; i < odleglosci.Length; i++)
			{
				// Mozemy tym samym kawalkiem dotrzec do jeszcze jednego slupka:
				frag = licz_fragment(pocz, i, pocz, i);
				if(frag <= maxDl)
				{
					kon = i;
					last_is_short = true;
                }
				// Nastepny slupek jest za daleko:
				else
				{
					wierzcholki.Add(kon);   //konczymy odcinek
					pocz = i;
					kon = i;
					koszt++;
				}
			}
			if(last_is_short == true)
			{
				wierzcholki.Add(kon);
				koszt++;
			}
			daszek = wierzcholki.ToArray();
            return koszt;
        }

        public int KosztLadnegoDaszku(out int[] daszek)
        {
            daszek = null;
            return 0;
        }
    }
}
