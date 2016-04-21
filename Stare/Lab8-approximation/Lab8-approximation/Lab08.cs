using System;
using System.Collections.Generic;
using System.Linq;  // potrzebne dla metody First rozszerzającej interfejs IEnumerable

namespace ASD.Graphs
{

	/// <summary>
	/// Rozszerzenie interfejsu <see cref="Graph"/> o rozwiązywanie problemu komiwojażera metodami przybliżonymi
	/// </summary>
	public static class Lab08Extender
    {

		/// <summary>
		/// Znajduje rozwiązanie przybliżone problemu komiwojażera algorytmem zachłannym "kruskalopodobnym"
		/// </summary>
		/// <param name="g">Badany graf</param>
		/// <param name="cycle">Znaleziony cykl (parametr wyjściowy)</param>
		/// <returns>Długość znalezionego cyklu (suma wag krawędzi)</returns>
		/// <remarks>
		/// Elementy (krawędzie) umieszczone są w tablicy <i>cycle</i> w kolejności swojego następstwa w znalezionym cyklu Hamiltona.<br/>
		/// <br/>
		/// Jeśli algorytm "kruskalopodobny" nie znajdzie w badanym grafie cyklu Hamiltona
		/// (co oczywiście nie znaczy, że taki cykl nie istnieje) to metoda zwraca <b>null</b>,
		/// parametr wyjściowy <i>cycle</i> również ma wówczas wartość <b>null</b>.<br/>
		/// <br/>
		/// Metodę można stosować dla grafów skierowanych i nieskierowanych.<br/>
		/// <br/>
		/// Metodę można stosować dla dla grafów z dowolnymi (również ujemnymi) wagami krawędzi.
		/// </remarks>
		public static int? TSP_Kruskal(this Graph g, out Edge[] cycle)
		{
			var tree = GetTree(g);
			CloseCycle(g, ref tree);
			return ConstructCycle(tree, out cycle);
		}  // TSP_Kruskal

		private static Graph GetTree(Graph g)
		{
			Graph helper = g.IsolatedVerticesGraph(true, g.VerticesCount);
			var queue = new EdgesMinPriorityQueue();
			for (int i = 0; i < g.VerticesCount; i++)
				foreach (var e in g.OutEdges(i))
				{
					helper.AddEdge(e);
					queue.Put(e);
				}

			Graph tree = g.IsolatedVerticesGraph(true, g.VerticesCount);
			UnionFind uf = new UnionFind(g.VerticesCount);
			while (tree.EdgesCount != tree.VerticesCount - 1)
			{
				if (queue.Empty)
					return null;
				Edge e = queue.Get();
				if (uf.Find(e.To) != uf.Find(e.From) &&
					tree.OutDegree(e.From) < 1 &&
					tree.InDegree(e.To) < 1)
				{
					tree.AddEdge(e);
					uf.Union(e.From, e.To);
				}
			}
			return tree;
		}

		private static void CloseCycle(Graph g, ref Graph tree)
		{
			if (tree == null)
				return;
			int first = -1, last = -1;
			for (int i = 0; i < tree.VerticesCount; i++)
			{
				if (tree.InDegree(i) == 0)
					first = i;
				if (tree.OutDegree(i) == 0)
					last = i;
			}
			if (first == -1 || last == -1)
			{
				tree = null;
				return;
			}
			int? weight = g.GetEdgeWeight(last, first);
			if (weight.HasValue)
				tree.AddEdge(last, first, weight.Value);
			else
				tree = null;
		}

		private static int? ConstructCycle(Graph g, out Edge[] edges)
		{
			if (g == null)
			{
				edges = null;
				return null;
			}
			List<Edge> cycle = new List<Edge>();
			bool[] visited = new bool[g.VerticesCount];
			int last = 0;
			int weight = 0;
			while (cycle.Count != g.EdgesCount)
			{
				foreach (Edge e in g.OutEdges(last))
					if (!visited[e.To])
					{
						cycle.Add(e);
						weight += e.Weight;
						visited[e.To] = true;
						last = e.To;
						break;
					}
			}
			edges = cycle.ToArray();
			return weight;
		}

		/// <summary>
		/// Znajduje rozwiązanie przybliżone problemu komiwojażera tworząc cykl Hamiltona na podstawie drzewa rozpinającego
		/// </summary>
		/// <param name="g">Badany graf</param>
		/// <param name="cycle">Znaleziony cykl (parametr wyjściowy)</param>
		/// <returns>Długość znalezionego cyklu (suma wag krawędzi)</returns>
		/// <remarks>
		/// Elementy (krawędzie) umieszczone są w tablicy <i>cycle</i> w kolejności swojego następstwa w znalezionym cyklu Hamiltona.<br/>
		/// <br/>
		/// Jeśli algorytm bazujący na drzewie rozpinającym nie znajdzie w badanym grafie cyklu Hamiltona
		/// (co oczywiście nie znaczy, że taki cykl nie istnieje) to metoda zwraca <b>null</b>,
		/// parametr wyjściowy <i>cycle</i> również ma wówczas wartość <b>null</b>.<br/>
		/// <br/>
		/// Metodę można stosować dla grafów nieskierowanych.<br/>
		/// Zastosowana do grafu skierowanego zgłasza wyjątek <see cref="System.ArgumentException"/>.<br/>
		/// <br/>
		/// Metodę można stosować dla dla grafów z dowolnymi (również ujemnymi) wagami krawędzi.<br/>
		/// <br/>
		/// Dla grafu nieskierowanego spełniajacego nierówność trójkąta metoda realizuje algorytm 2-aproksymacyjny.
		/// </remarks>
		public static int? TSP_TreeBased(this Graph g, out Edge[] cycle)
        {
			// TODO - algorytm oparty na minimalnym drzewie rozpinajacym
			cycle = null;
			if (g.Directed == true)
			{
				throw new System.ArgumentException();
			}

			// Niespójny: 
			int cc;
			g.DFSearchAll(null, null, out cc);
			if (cc != 1)
				return null;

			int? sum = buildCycle(g, out cycle);
			return sum;
		}  // TSP_TreeBased

		public static int? buildCycle(Graph g, out Edge[] cycle)
		{
			cycle = null;
			int[] verticesOrder = new int[g.VerticesCount];
			int orderCounter = 0;
			Graph tree;
			g.Kruskal(out tree);

			Predicate<int> preVisit = delegate (int i)
			{
				verticesOrder[orderCounter++] = i;
				return true;
			};

			tree.DFSearchFrom(0, preVisit, null);

			cycle = new Edge[g.VerticesCount];
			int sum = 0;
			int weight;
			for(int i = 0; i < g.VerticesCount - 1; i++)
			{
				if(!g.GetEdgeWeight(verticesOrder[i], verticesOrder[i + 1]).HasValue)
				{
					cycle = null;
					return null;
				}
				weight = g.GetEdgeWeight(verticesOrder[i], verticesOrder[i+1]).Value;
				sum += weight;
				cycle[i] = new Edge(verticesOrder[i], verticesOrder[i + 1], weight);
			}
			weight = g.GetEdgeWeight(verticesOrder[tree.VerticesCount - 1], verticesOrder[0]).Value;
			sum += weight;
			cycle[tree.VerticesCount - 1] = new Edge(verticesOrder[tree.VerticesCount - 1], verticesOrder[0], weight);

			return sum;
        }
    }  // class Lab08Extender

}  // namespace ASD.Graph
