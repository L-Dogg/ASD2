
using System;
using System.Collections;
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
				foreach (Edge e in g.OutEdges(i))
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
			// Czy w ogole istnieje taki wierzcholek:
			if (del > g.VerticesCount)
				return g;

			// Utworzenie grafu bez krawedzi na bazie wierzcholkow grafu g bez usuwanego:
			Graph ret = g.IsolatedVerticesGraph(g.Directed, g.VerticesCount - 1);

			HashSet<Edge> elist = new HashSet<Edge>();
			// Uzupelnienie wynikowego grafu o krawedzie z uwzglednieniem przesuniecia:
			for (int i = 0; i < g.VerticesCount; i++)
			{
				// Omijamy krawedzie z usuwanego wierzcholka:
				if (i == del)
					continue;
				foreach (Edge e in g.OutEdges(i))
				{
					// Omijamy krawedzie z usuwanego wierzcholka:
					if (e.To == del)
						continue;
					if (!elist.Contains(new Edge(e.To, e.From, e.Weight)))
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
				for (int j = 0; j < g.VerticesCount; j++)
				{
					if (i == j)
						continue;

					if (g.GetEdgeWeight(i, j) == null)
					{
						Edge e = new Edge(i, j, 1);
						if (!elist.Contains(new Edge(j, i, 1)) && !elist.Contains(e))
						{
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
			Graph ret = g.IsolatedVerticesGraph();
			PathsInfo[] tab = new PathsInfo[g.VerticesCount];
			HashSet<Edge> elist = new HashSet<Edge>();

			// Dla każdego wierzchołka i znajdź ścieżki do pozostałych wierzchołków:
			for (int i = 0; i < g.VerticesCount; i++)
			{
				if (g.DijkstraShortestPaths(i, out tab))
				{
					for (int j = 0; j < g.VerticesCount; j++)
					{
						// Pomiń pętle:
						if (i == j)
							continue;

						if (g.GetEdgeWeight(i, j) > 1)
							throw new ArgumentException();

						Edge e = new Edge(i, j, 1);
						if (tab[j].Dist != null && elist.Contains(new Edge(j, i, 1)) == false && elist.Contains(e) == false)
						{
							//Console.WriteLine("Adding edge {0} - {1}", i, j);
							elist.Add(e);
							ret.AddEdge(e);
						}
					}
				}

			}
			return ret; // zmienic !
		}

		/// <summary>Badanie czy graf jest dwudzielny</summary>
		/// <param name="g">Graf wejściowy</param>
		/// <returns>Informacja czy graf wejściowy jest dwudzielny</returns>
		/// <remarks>
		/// 1.5 pkt.
		/// Graf wejściowy pozostaje niezmieniony.
		/// </remarks>
		 
		public enum COLORS { NONE, RED, BLUE };
		public static bool IsBipartite(this Graph g)
		{
			COLORS[] col = new COLORS[g.VerticesCount];

            for (int i = 1; i < g.VerticesCount; i++)
				col[i] = COLORS.NONE;

			for(int i = 0; i < g.VerticesCount; i++)
			{
				if (col[i] == COLORS.NONE)
				{
					if (!BFSColoring(g, i, ref col))
						return false;
				}
			}

			return true; // zmienic !
        }
		public static bool BFSColoring(Graph g, int v, ref COLORS []col)
		{
			Queue<int> q = new Queue<int>();
			int vert;
			int partition_class = 0;
			int i;
            q.Enqueue(v);

			while(q.Count != 0)
			{
				i = 0;
				vert = q.Dequeue();
				if (col[vert] == COLORS.NONE || col[vert] == ((partition_class % 2 == 0) ? COLORS.RED : COLORS.BLUE))
				{
					if (col[vert] == COLORS.NONE)
						col[vert] = (partition_class++ % 2 == 0) ? COLORS.RED : COLORS.BLUE;
					//Console.WriteLine("Partition: {0}", partition_class % 2);
					//Console.WriteLine("DEQ: Vertex {0} has color {1}", vert, col[vert]);
					foreach (Edge e in g.OutEdges(vert))
					{
						if (col[e.To] == COLORS.NONE)
						{
							if (col[vert] == COLORS.RED)
								col[e.To] = COLORS.BLUE;
							else
								col[e.To] = COLORS.RED;
							//Console.WriteLine("ENQ: Vertex {0} has color {1}", e.To, col[e.To]);
							q.Enqueue(e.To);
						}
						else if (col[e.To] == col[vert])
						{
							Console.WriteLine("Vertex {0} (Color {1}) is connected with Vertex {2} (Color {3})", e.To, col[e.To], vert, col[vert]);
							return false;
						}
						else
							i++;
					}
					if (i == g.OutDegree(v))
						partition_class--;
				}
				else
				{
					//Console.WriteLine("NIE WESZŁO! Vertex {0} has color {1}", vert, col[vert]);
				}
				foreach (Edge e in g.OutEdges(vert))
					if (col[e.To] == col[vert])
						return false;
				partition_class++;
			}
			return true;
		}
	}

}
