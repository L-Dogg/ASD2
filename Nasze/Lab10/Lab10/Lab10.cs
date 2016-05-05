
using System.Collections.Generic;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
    {
	/// <summary>
	/// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
	/// </summary>
	/// <param name="g">Badany graf</param>
	/// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
	/// <returns>Rozmiar największej kliki</returns>
	/// <remarks>
	/// 1) Uwzględniamy grafy sierowane i nieskierowane.
	/// 2) Nie wolno modyfikować badanego grafu.
	/// </remarks>

	private static int n;
	private static List<int> klika;
	private static List<int> maxKlika;
	private static bool[] vertices;
	public static int MaxClique(this Graph g, out int[] clique)
	{
		n = g.VerticesCount;
		klika = new List<int>();
		maxKlika = new List<int>();
		vertices = new bool[g.VerticesCount];

		for(int i = 0; i < g.VerticesCount; i++)
		{
			klika.Add(i);
			//vertices[i] = true;
			Clique(0, g);
			klika.Remove(i);
			//vertices[i] = false;
		}

		clique = maxKlika.ToArray();
		return maxKlika.Count;
	}

	public static void Clique(int k, Graph g) // k - iteracja, g - graf
	{
		if (k == n)
			return;

		for(int i = klika[klika.Count - 1] + 1; i < n; i++)
		{
			//if(vertices[i] == false && g.OutDegree(i) >= klika.Count)
			//{
				bool ok = true;
				foreach(var j in klika)
				{
					if(g.GetEdgeWeight(i, j) == null || g.GetEdgeWeight(j, i) == null)
					{
						ok = false;
						break;
					}
				}
				if(ok)
				{
					klika.Add(i);
					//vertices[i] = true;
					if(klika.Count > maxKlika.Count)
					{
						maxKlika = new List<int>(klika);
					}
					Clique(k + 1, g);
					klika.Remove(i);
					//vertices[i] = false;
				}
			//}
		}
	}

	/// <summary>
	/// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
	/// </summary>
	/// <param name="g">Pierwszy badany graf</param>
	/// <param name="h">Drugi badany graf</param>
	/// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
	/// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
	/// <remarks>
	/// 1) Nie wolno korzystać z bibliotecznych metod do badania izomorfizmu
	/// 2) Uwzględniamy wagi krawędzi i "skierowalność grafu"
	/// 3) Nie wolno modyfikować badanych grafów.
	/// </remarks>
	public static bool IsomorpchismTest(this Graph g, Graph h, out int[] map)
    {
        map=null;
		if (g.VerticesCount != h.VerticesCount || g.EdgesCount != h.EdgesCount || g.Directed != h.Directed)
			return false;

		int[] myMap = new int[g.VerticesCount];
		bool[] used = new bool[g.VerticesCount];
		bool ret = Backtracking(g, h, 0, ref myMap, ref used);
		if (ret == true)
		{
			map = new int[g.VerticesCount];
			myMap.CopyTo(map, 0);
		}
		return ret;
    }
	public static bool Backtracking(Graph g, Graph h, int vh, ref int[] map, ref bool[] used) // szukamy mapowania dla wierzchołka vh
	{
		if(vh == g.VerticesCount)
		{
			return true;
		}

		// Szukamy wsrod wszystkich wierzcholkow dotad niezmapowanych:
		for(int i = 0; i < h.VerticesCount; i++)
		{
			if(used[i] == false && h.OutDegree(i) == g.OutDegree(vh))
			{
				bool ok = true;
				// Gdyby vh zmapowac na i, to musza zgadzac sie mapowania krawedzi wychodzacych
				foreach (Edge e in g.OutEdges(vh))
				{
					if (used[e.To] == true)
						if (h.GetEdgeWeight(i, map[e.To]).HasValue == false || h.GetEdgeWeight(i, map[e.To]).Value != g.GetEdgeWeight(vh, e.To).Value)
						{
							ok = false;
							break;
						}
				}
				if(ok)
				{
					used[i] = true;
					map[vh] = i;
					if (Backtracking(g, h, vh + 1, ref map, ref used))
						return true;
					used[i] = false;
		
				}
            }
		}
		return false;
	}

    }

