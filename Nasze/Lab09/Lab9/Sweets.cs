using System;

using ASD.Graphs;
using System.Linq;

namespace Lab09
{
    class Sweets
    {
        /// <summary>
        /// Implementacja zadania 1
        /// </summary>
        /// <param name="childrenCount">Liczba dzieci</param>
        /// <param name="sweetsCount">Liczba batoników</param>
        /// <param name="childrenLikes">
        /// Tablica tablic upodobań dzieci. Tablica childrenLikes[i] zawiera indeksy batoników
        /// które lubi i-te dziecko. Dzieci i batoniki są indeksowane od 0.
        /// </param>
        /// <param name="assignment">
        /// Wynikowy parametr. assigment[i] zawiera indeks batonika które dostało i-te dziecko.
        /// Jeśli dziecko nie dostało żadnego batonika to -1.
        /// </param>
        /// <returns>Liczba dzieci, które dostały batonik.</returns>
        public static int Task1( int childrenCount, int sweetsCount, int[][] childrenLikes, out int[] assignment )
        {
            assignment = null;
			Graph network = new AdjacencyMatrixGraph(true, sweetsCount + childrenCount + 2);
			int s = sweetsCount + childrenCount;
			int t = sweetsCount + childrenCount + 1;
		
			// Krawędzie dzieci-ujście
			for (int i = 0; i < childrenCount; i++)
			{
				network.AddEdge(i, t);
			}

			// Krawędzie źródło-batoniki
			for (int i = childrenCount; i < sweetsCount + childrenCount; i++)
			{
				network.AddEdge(s, i);
			}

			// Krawędzie dziecko-batonik który dziecko lubi
			for(int i = 0; i < childrenLikes.Length; i++)
			{
				for(int j = 0; j < childrenLikes[i].Length; j++)
				{
					network.AddEdge(childrenCount + childrenLikes[i][j], i);
				}
			}


			Graph flow;
			int size = network.FordFulkersonMaxFlow(s, t, out flow);
			assignment = Enumerable.Repeat(-1, childrenCount).ToArray();

			for (int i = 0; i < flow.VerticesCount; i++)
			{
				foreach(Edge e in flow.OutEdges(i))
				{
					if(e.Weight == 0 || e.From == s || e.To == t)
					{
						continue;
					}
					assignment[e.To] = i - childrenCount;
				}
			}
			
			return size;
		}

        /// <summary>
        /// Implementacja zadania 2
        /// </summary>
        /// <param name="childrenCount">Liczba dzieci</param>
        /// <param name="sweetsCount">Liczba batoników</param>
        /// <param name="childrenLikes">
        /// Tablica tablic upodobań dzieci. Tablica childrenLikes[i] zawiera indeksy batoników
        /// które lubi i-te dziecko. Dzieci i batoniki są indeksowane od 0.
        /// </param>
        /// <param name="childrenLimits">Tablica ograniczeń dla dzieci. childtrenLimits[i] to maksymalna liczba batoników jakie może zjeść i-te dziecko.</param>
        /// <param name="sweetsLimits">Tablica ograniczeń batoników. sweetsLimits[i] to dostępna liczba i-tego batonika.</param>
        /// <param name="happyChildren">Wynikowy parametr zadania 2a. happyChildren[i] powinien zawierać true jeśli dziecko jest zadowolone i false wpp.</param>
        /// <param name="shoppingList">Wynikowy parametr zadania 2b. shoppingList[i] poiwnno zawierać liczbę batoników i-tego rodzaju, które trzeba dokupić.</param>
        /// <returns>Maksymalna liczba rozdanych batoników.</returns>
        public static int Task2( int childrenCount, int sweetsCount, int[][] childrenLikes, int[] childrenLimits, int[] sweetsLimits, out bool[] happyChildren, out int[] shoppingList )
        {
            happyChildren = null;
            shoppingList = null;

			Graph network = new AdjacencyMatrixGraph(true, childrenCount * 2 + sweetsCount + 2);

			int s = childrenCount * 2 + sweetsCount;
			int t = childrenCount * 2 + sweetsCount + 1;

			// Krawędzie dziecko -> dziecko_dubel-> ujście
			for (int i = 0; i < childrenCount; i++)
			{
				network.AddEdge(i, i + sweetsCount + childrenCount, childrenLimits[i]);
				network.AddEdge(i + sweetsCount + childrenCount, t, childrenLimits[i]);
			}

			// Krawędzie źródło -> batonik
			for(int i = childrenCount; i < childrenCount + sweetsCount; i++)
			{
				network.AddEdge(s, i, sweetsLimits[i - childrenCount]);		 
			}

			// Krawędzie batonik -> dziecko które lubi batonik
			for (int i = 0; i < childrenLikes.Length; i++)
			{
				for (int j = 0; j < childrenLikes[i].Length; j++)
				{
					network.AddEdge(childrenCount + childrenLikes[i][j], i, sweetsLimits[childrenLikes[i][j]]);
				}
			}

			Graph flow;
			int size = network.FordFulkersonMaxFlow(s, t, out flow);
			happyChildren = Enumerable.Repeat(false, childrenCount).ToArray();
			shoppingList = new int[sweetsCount];

			// Sprawdzenie czy dziecko jest zadowolone
			for (int i = 0; i < childrenCount; i++)
			{
				if (flow.OutEdges(i).First().Weight == childrenLimits[i])
					happyChildren[i] = true;
				// ile batoników trzeba dokupić
				else
				{
					shoppingList[childrenLikes[i][0]] += childrenLimits[i] - flow.OutEdges(i).First().Weight;
				}
			}

			// Sprawdzenie ile batoników wyszło ze źródła
			int num = 0;
			foreach (Edge e in flow.OutEdges(s))
				num += e.Weight;

			return num;
        }

    }
}
