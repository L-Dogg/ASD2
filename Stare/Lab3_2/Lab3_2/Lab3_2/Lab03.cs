
using System;
using System.Collections.Generic;

namespace ASD.Graphs
{

public static class BasicGraphExtender
    {
    /// <summary>Dodawanie wierzchołka do grafu</summary>
    /// <param name="g">Graf, do którego dodajemy wierzchołek</param>
    /// <returns>Graf z dodanym wierzchołkiem</returns>
    /// <remarks>
    /// 0.5 pkt.
    /// Metoda zwraca graf, będący kopią grafu wejściowego z dodanym wierzchołkiem.
    /// Graf wejściowy pozostaje niezmieniony.
    /// W utworzonym grafie są takie same krawędzie jak w wejściowym.
    /// Utworzony graf ma taką samą reprezentację jak graf wejściowy.
    /// Uwaga: W grafach nieskierowanych nie probować dodawawać po 2 razy tej samej krawędzi
    /// </remarks>
    public static Graph AddVertex(this Graph g)
        {
			Graph ret = g.IsolatedVerticesGraph(g.Directed, g.VerticesCount + 1);
			HashSet<Edge> elist = new HashSet<Edge>();
			for (int i = 0; i < g.VerticesCount; i++)
			{
				foreach(Edge e in g.OutEdges(i))
				{
					if (!elist.Contains(e))
					{
						ret.AddEdge(e);
						elist.Add(e);
					}

                }
			}

			return ret;
        }

    /// <summary>Usuwanie wierzchołka z grafu</summary>\
    /// <param name="g">Graf, z którego usuwamy wierzchołek</param>
    /// <param name="del">Usuwany wierzchołek</param>
    /// <returns>Graf z usunietym wierzchołkiem</returns>
    /// <remarks>
    /// 1.0 pkt.
    /// Metoda zwraca graf, będący kopią grafu wejściowego z usuniętym wierzchołkiem.
    /// Graf wejściowy pozostaje niezmieniony.
    /// W utworzonym grafie są takie same krawędzie jak w wejściowym
    /// (oczywiście z wyjątkiem incydentnych z usuniętym wierzchołkiem, numeracja wierzchołków zostaje zaktualizowana)
    /// Utworzony graf ma taką samą reprezentację jak graf wejściowy.
    /// Uwaga: W grafach nieskierowanych nie probować dodawawać po 2 razy tej samej krawędzi
    /// </remarks>
    public static Graph DeleteVertex(this Graph g, int del)
		{
			Graph a = g.Clone();

			// Usuniecie wszystkich krawedzi incydentnych z wierzcholkiem:
			foreach(Edge e in a.OutEdges(del))
			{
				a.DelEdge(e);
			}

			// Utworzenie grafu bez krawedzi na bazie wierzcholkow grafu g bez usuwanego:
			Graph ret = a.IsolatedVerticesGraph(a.Directed, a.VerticesCount - 1);

			HashSet<Edge> elist = new HashSet<Edge>();
			// Uzupelnienie wynikowego grafu o krawedzie z uwzglednieniem przesuniecia:
			for (int i = 0; i < a.VerticesCount; i++)
			{
				foreach(Edge e in a.OutEdges(i))
				{
					if (!elist.Contains(e))
					{
						if (i < del && e.To < del)
							ret.AddEdge(i, e.To, e.Weight);
						else if (i < del && e.To > del)
							ret.AddEdge(i, e.To - 1, e.Weight);
						else if (i > del && e.To < del)
							ret.AddEdge(i - 1, e.To, e.Weight);
						else
							ret.AddEdge(i - 1, e.To - 1, e.Weight);

						elist.Add(e);			
					}
				}
			}
			return ret; // zmienic !
        }

    /// <summary>Dopełnienie grafu</summary>
    /// <param name="g">Graf wejściowy</param>
    /// <returns>Graf będący dopełnieniem grafu wejściowego</returns>
    /// <remarks>
    /// 0.5 pkt.
    /// Dopełnienie grafu to graf o tym samym zbiorze wierzchołków i zbiorze krawędzi równym VxV-E-"pętle"
    /// Graf wejściowy pozostaje niezmieniony.
    /// Utworzony graf ma taką samą reprezentację jak graf wejściowy.
    /// Uwaga 1 : w przypadku stwierdzenia ze graf wejściowy jest grafem ważonym zgłosić wyjątek ArgumentException
    /// Uwaga 2 : W grafach nieskierowanych nie probować dodawawać po 2 razy tej samej krawędzi
    /// </remarks>
    public static Graph Complement(this Graph g)
        {
			Graph ret = g.IsolatedVerticesGraph();

			HashSet<Edge> elist = new HashSet<Edge>();
			for (int i = 0; i < g.VerticesCount; i++)
			{
				for(int j = 0; j < g.VerticesCount; j++)
				{
					if (i == j)
						continue;

					if (g.GetEdgeWeight(i, j) == null)
					{
						Edge e = new Edge(i, j, 1);
                        if (!elist.Contains(e))
						{
							Console.WriteLine("Adding edge: {0} - {1}", i, j);
							ret.AddEdge(i, j);
							elist.Add(e);
						}
					}
					else if (g.GetEdgeWeight(i, j) > 1)
						throw new ArgumentException();
				}
			}
			return ret; // zmienic !
        }

    /// <summary>Domknięcie grafu</summary>
    /// <param name="g">Graf wejściowy</param>
    /// <returns>Graf będący domknięciem grafu wejściowego</returns>
    /// <remarks>
    /// 1.5 pkt.
    /// Domknięcie grafu to graf, w którym krawędzią połączone są wierzchołki, 
    /// pomiędzy którymi istnieje ścieżka w wyjściowym grafie (pętle wykluczamy).
    /// Graf wejściowy pozostaje niezmieniony.
    /// Utworzony graf ma taką samą reprezentację jak graf wejściowy.
    /// Uwaga 1 : w przypadku stwierdzenia ze graf wejściowy jest grafem ważonym zgłosić wyjątek ArgumentException
    /// </remarks>
    public static Graph Closure(this Graph g)
        {
        return g.Clone(); // zmienic !
        }

    /// <summary>Badanie czy graf jest dwudzielny</summary>
    /// <param name="g">Graf wejściowy</param>
    /// <returns>Informacja czy graf wejściowy jest dwudzielny</returns>
    /// <remarks>
    /// 1.5 pkt.
    /// Graf wejściowy pozostaje niezmieniony.
    /// </remarks>
    public static bool IsBipartite(this Graph g)
        {
        return false; // zmienic !
        }

    }

}
