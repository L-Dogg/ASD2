using static System.Console;
using System.Collections.Generic;

namespace ASD.Graphs
{

/// <summary>
/// Klasa rozszerzająca interfejs Graph o rozwiązania problemów największej kliki i izomorfizmu podgrafu metodą pełnego przeglądu (backtracking)
/// </summary>
public static class CliqueGraphExtender
    {

		/// <summary>
		/// Wyznacza liczbę klikową grafu
		/// </summary>
		/// <param name="g">Badany graf</param>
		/// <param name="clique">Wierzchołki znalezionej kliki (parametr wyjściowy)</param>
		/// <returns>Rozmiar największej kliki</returns>
		/// <remarks>
		/// Liczba klikowa grafu G to rozmiar największego grafu pełnego będącego podgrafem G
		/// </remarks>
		private static List<int> klika;
		private static List<int> maxKlika;
    public static int CliqueNumber(this Graph g, out int[] clique)
        {

			// Wskazówki
			// 1) w sposób systematyczny sprawdzać wszystkie podzbiory wierzchołków grafu, czy tworzą podgraf pełny
			// 2) unikać wielokrotnego sprawdzania tego samego podzbioru
			// 3) zastosować algorytm z powrotami (backtracking)
			// 4) zdefiniować pomocniczą funkcję rekurencyjną
			// 5) do badania krawędzi pomiędzy wierzchołkami i oraz j użyć metody GetEdgeWeight(i,j)
			klika = new List<int>();
			maxKlika = new List<int>();
			for(int i = 0; i < g.VerticesCount; i++)
			{
				//WriteLine("Dodaje wierzcholek: {0}", i);
				klika.Add(i);
				//WriteLine("Rekure - rodzic");
				Clique(0, g);
				klika.Remove(i);
				//WriteLine("Usuwa wierzcholek: {0}", i);
			}
			clique = maxKlika.ToArray();
			return maxKlika.Count;
        }
		
	public static void Clique(int k, Graph g) // k - iteracja, g - graf
		{
			int n = g.VerticesCount;
			if(k == n)
			{
				return;
			}
			for (int i = 0; i < n; i++)
			{
				if(!klika.Contains(i))
				{
					bool ok = true;
					foreach(var j in klika)
					{
						if (g.GetEdgeWeight(i, j) == null || g.GetEdgeWeight(j, i) == null)
						{
							ok = false;
							break;
						}
					}
					if (ok && (i > klika[klika.Count - 1]))
					{
						//WriteLine("Dodaje wierzcholek: {0}", i);
						klika.Add(i);
						if (klika.Count > maxKlika.Count)
						{
							//WriteLine("Nowy max");
							maxKlika = new List<int>(klika);
						}
						//WriteLine("Rekure");
						Clique(k + 1, g);
						//WriteLine("Usuwa wierzcholek: {0}", i);
						klika.Remove(i);
					}
				}
			}
		}
    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    public static bool IsIzomorpchic(this Graph g, Graph h, out int[] map)
        {
        map=null;
        if ( g.VerticesCount!=h.VerticesCount || g.EdgesCount!=h.EdgesCount || g.Directed!=h.Directed )
            return false;
        var helper = new IzomorpchismHelper(g,h);
        map = new int[g.VerticesCount];
        return helper.FindMapping(0,map);
        }

    /// <summary>
    /// Klasa pomocnicza dla badania izomorfizmu grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <remarks>
    /// Dzięki wprowadzeniu tej klasy wygodniej implementuje się rekurencyjne badanie izomorfizmu.
    /// </remarks>
    private sealed class IzomorpchismHelper
        {
        
        /// <summary>
        /// Pierwszy badany graf
        /// </summary>
        private Graph g;

        /// <summary>
        /// Drugi badany graf
        /// </summary>
        private Graph h;

        /// <summary>
        /// Informacja czy dany wierzchołek grafu g już jest wykorzystany w mapowaniu
        /// </summary>
        private bool[] used;

        /// <summary>
        /// Tworzy obiekt klasy pomocniczej dla badania izomorfizmu grafów metodą pełnego przeglądu
        /// </summary>
        /// <param name="g">Pierwszy badany graf</param>
        /// <param name="h">Drugi badany graf</param>
        internal IzomorpchismHelper(Graph g, Graph h)
            {
            this.g=g;
            this.h=h;
            used = new bool[g.VerticesCount];
            }

        /// <summary>
        /// Bada izomorfizm grafów metodą pełnego przeglądu (rekurencyjnie)
        /// </summary>
        /// <param name="vh">Aktualnie rozważany wierzchołek</param>
        /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g</param>
        /// <returns>Informacja czy znaleziono mapowanie definiujące izomotfizm</returns>
        internal bool FindMapping(int vh, int[] map)
            {

            // Wskazówki
            // 1) w sposób systematyczny sprawdzać wszystkie potencjalne mapowania
            // 2) unikać wielokrotnego sprawdzania tego samego mapowania
            // 3) zastosować algorytm z powrotami (backtracking)
            // 4) do badania krawędzi pomiędzy wierzchołkami i oraz j użyć metody GetEdgeWeight(i,j)

            return false;
            }

        }  // class IzomorpchismHelper

    }

}