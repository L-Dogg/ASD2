using System;
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
        // ToDo - algorytm "kruskalopodobny"
			cycle=null;
			
			return g.Lab03Kruskal(out cycle);
        }  // TSP_Kruskal

		public static int? Lab03Kruskal(this Graph g, out Edge[] cycle)
		{
			int mstw = 0;
			cycle = null;

			// Dodanie krawedzi do kolejki:
			Graph ret = g.IsolatedVerticesGraph();
			EdgesMinPriorityQueue pq = new EdgesMinPriorityQueue();
			for (int i = 0; i < g.VerticesCount; i++)
			{
				foreach (Edge e in g.OutEdges(i))
				{
					if (e.To < i && g.Directed == false)
					{
						pq.Put(e);
					}
				}
			}

			UnionFind vs = new UnionFind(g.VerticesCount);
			int vsCount = g.VerticesCount;
			Edge ed;
			int s1, s2;
			
			while (pq.Count > 0 && vsCount > 0)
			{
				ed = pq.Get();
				s1 = vs.Find(ed.From);
				s2 = vs.Find(ed.To);
				if (s1 != s2 && ret.OutDegree(ed.To) < 2 && ret.OutDegree(ed.From) < 2)
				{
					vsCount--;
					vs.Union(s1, s2);
					ret.AddEdge(ed);
					mstw += ed.Weight;
					//Console.WriteLine("Adding Edge {0}", ed.ToString());
				}
			}
			int to = -1, from = -1;
			for(int i = 0; i < ret.VerticesCount; i++)
			{
				if((ret.OutDegree(i) != 2 && g.Directed == false) || (ret.InDegree(i) + ret.OutDegree(i) != 2 && g.Directed == true))
				{
					if (from == -1)
						from = i;
					else if (to == -1)
						to = i;
				}
			}
			if(g.GetEdgeWeight(from, to).HasValue)
			{

				ed = new Edge(from, to, g.GetEdgeWeight(from, to).Value);
				ret.AddEdge(ed);
				//Console.WriteLine("Adding Edge {0}", ed.ToString());
			}
			int prev = 0;
			int cur = ret.OutEdges(prev).First().To;
			cycle = new Edge[g.VerticesCount];
			cycle[0] = ret.OutEdges(prev).First();
			for (int i = 1; i < ret.VerticesCount; i++)
			{
				foreach(Edge e in ret.OutEdges(cur))
				{
					if(e.To != prev)
					{
						prev = cur;
						cur = e.To;
						cycle[i] = e;
						break;
					}
				}
			}
			return mstw;
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
