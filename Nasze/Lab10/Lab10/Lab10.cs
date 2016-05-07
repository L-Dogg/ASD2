
using System.Collections.Generic;
using ASD.Graphs;
using System;
using System.Linq;

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
	private static bool[] used;
	public static int MaxClique(this Graph g, out int[] clique)
	{
		n = g.VerticesCount;
		// Graf pełny
		if((g.EdgesCount == (n-1) * n / 2 && !g.Directed) || (g.EdgesCount == (n-1)*n && g.Directed))
		{
			clique = Enumerable.Range(0, n).ToArray();
			return n;
		}

		klika = new List<int>();
		maxKlika = new List<int>();
		used = new bool[n];
		for(int i = 0; i < g.VerticesCount; i++)
		{
			klika.Add(i);
			used[i] = true;
			foreach (Edge e in g.OutEdges(i))
			{
				if(e.To > i)
					Clique(e.To, g);
			}
			used[i] = false;
			klika.Remove(i);
		}

		clique = maxKlika.ToArray();
		return maxKlika.Count;
	}

	public static void Clique(int k, Graph g)
	{
		foreach (int v in klika)
		{
			if (g.GetEdgeWeight(k, v) == null || g.GetEdgeWeight(v, k) == null)
				return;
		}

		used[k] = true;
		klika.Add(k);

		if (klika.Count > maxKlika.Count)
		{
			maxKlika = new List<int>(klika);
		}

		foreach (Edge e in g.OutEdges(k))
		{
			if(!used[e.To] && e.To > k)
				Clique(e.To, g);
		}

		klika.Remove(k);
		used[k] = false;
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
		bool[] hUsed = new bool[g.VerticesCount];
		bool ret = Backtracking(g, h, 0, ref myMap, ref used, ref hUsed);
		if (ret == true)
		{
			map = new int[g.VerticesCount];
			myMap.CopyTo(map, 0);
		}
		return ret;
    }
	public static bool Backtracking(Graph g, Graph h, int vh, ref int[] map, ref bool[] used, ref bool[] hUsed) // szukamy mapowania dla wierzchołka vh
	{
		if(vh == g.VerticesCount)
		{
			return true;
		}
		
		for(int i = 0; i < g.VerticesCount; i++)
		{
			if(used[i] == false && g.OutDegree(i) == h.OutDegree(vh))
			{
				bool ok = true;
				for(int k = 0; k < vh; k++)
				{
					if (hUsed[k] == true)
					{
						if (h.GetEdgeWeight(vh, k) != g.GetEdgeWeight(i, map[k]) || h.GetEdgeWeight(k, vh) != g.GetEdgeWeight(map[k], i))
						{
							ok = false;
							break;
						}
					}
                }
				if(ok)
				{
					map[vh] = i;
					used[i] = true;
					hUsed[vh] = true;
					if (Backtracking(g, h, vh + 1, ref map, ref used, ref hUsed))
						return true;
					used[i] = false;
					hUsed[vh] = false;

				}
            }
		}
		return false;
	}

    }

