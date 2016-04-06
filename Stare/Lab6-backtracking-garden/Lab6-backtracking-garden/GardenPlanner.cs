using ASD.Graphs;
using System.Collections.Generic;
using static System.Console;

namespace lab7
{
    public static class GardenPlannerExtender
    {

        /// <summary>
        /// Znajduje wszystkie optymalne ustawienia sza�as�w w ogrodzie opisanym przez graf.
        /// </summary>
        /// <param name="graph">Graf reprezentuj�cy ogr�d</param>
        /// <returns>Lista rozwi�za�. Ka�de rozwi�zanie to tablica zawieraj�ca numery
        /// wierzcho�k�w, w kt�rych stoj� sza�azy. Ka�dy numej mo�e wyst�pi� co najwy�ej raz.</returns>
        public static List<int[]> GardenPlanner(this Graph graph)
        {
            var gph = new GardenPlannerHelper(graph);
			// wywo�aj metod� Plan dla obiektu gph
			// zwr�� wynik
			gph.Plan(0);
			List<int[]> ret = new List<int[]>();
			foreach(var l in gph.rozwiazania)
			{
				ret.Add(l.ToArray());
			}
            return ret;


        }
		public static string Indent(int count)
		{
			return "".PadLeft(count);
		}

		private sealed class GardenPlannerHelper
        {
            private Graph graph;
			private int n;
			//
			// Tu mo�esz dopisa� deklaracje potrzebnych sk�adowych
			//
			private List<int> szalasy;
			private int minimum;
			public List<List<int>> rozwiazania;
			private List<int> pokryte;
            internal GardenPlannerHelper(Graph _graph)
            {
                graph = _graph;
				//
				// Tu mo�esz dopisa� potrzebne inicjalizacje
				//
				n = graph.VerticesCount;
				szalasy = new List<int>();
				pokryte = new List<int>();
				rozwiazania = new List<List<int>>();
                minimum = int.MaxValue;
			}

            internal void Plan(int k)
            {
				if (szalasy.Count > minimum)
				{
					//WriteLine(Indent(k * 3) + "Too big");
					return;
				}

                if(pokryte.Count == n)
				{
					if(szalasy.Count < minimum)
					{
						//WriteLine(Indent(k * 3) + "New best");
						rozwiazania.Clear();
						minimum = szalasy.Count;
						rozwiazania.Add(new List<int>(szalasy));
					}
					if(szalasy.Count == minimum)
					{
						//WriteLine(Indent(k * 3) + "Another solution");
						rozwiazania.Add(new List<int>(szalasy));
					}
					return;
				}
				if (k == n)
				{
					//WriteLine(Indent(k * 3) + "k==n and no solution");
					return;
				}

				for(int i = 0; i < n; i++)
				{
					if(canAdd(i))
					{
						//WriteLine(Indent(k * 3) + "Adding vertex: " + i.ToString());
						int j = add(i);
						Plan(k+1);
						// rekurencja wraca - usuwamy szalas i pokryte przez niego place
						backtrack(i, j);
					}
				}
            }

            //
            //  Tu mo�esz dopisa� potrzebne metody pomocnicze
            //
			private void backtrack(int i, int j)
			{
				szalasy.Remove(i);
				for (int u = 0; u < j; u++)
				{
					pokryte.RemoveAt(pokryte.Count - 1);
				}
			}
			private bool canAdd(int i)
			{
				return !(szalasy.Contains(i) && pokryte.Contains(i));
			}
			private int add(int i)
			{
				int added = 0;
				szalasy.Add(i);
				pokryte.Add(i);
				for(int j = 0; j < n; j++)
				{
					if(graph.GetEdgeWeight(i, j) != null && !pokryte.Contains(j))
					{
						added++;
						pokryte.Add(i);
					}
				}
				return added;
			}
            }

        }

    }



