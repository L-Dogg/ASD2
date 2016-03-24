
using System;
using System.Collections.Generic;

namespace ASD.Graphs
{

public static class Lab05GraphExtender
    {

    /// <summary>
    /// Wyznacza silnie spójne składowe
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="scc">Silnie spójne składowe (parametr wyjściowy)</param>
    /// <returns>Liczba silnie spójnych składowych</returns>
    /// <remarks>
    /// scc[v] = numer silnie spójnej składowej do której należy wierzchołek v<br/>
    /// (numerujemy od 0)<br/>
    /// <br/>
    /// Metoda uruchomiona dla grafu nieskierowanego zgłasza wyjątek <see cref="System.ArgumentException"/>.
    /// <br/>
    /// Graf wejściowy pozostaje niezmieniony.
    /// </remarks>
    public static int StronglyConnectedComponents(this Graph g, out int[] scc)
		{
			scc=new int[g.VerticesCount];
			int[] myscc = new int[g.VerticesCount];
			if (g.Directed == false)
				throw new System.ArgumentException("Graph should be directed");
			Graph tmp = Reverse(g);
			Stack<int> S = new Stack<int>(g.VerticesCount);
			Predicate<int> postFix = delegate (int n)
			{
				S.Push(n);
				return true;
			};
			bool[] added = new bool[g.VerticesCount];
			int cc = 0;
			g.DFSearchAll(null, postFix, out cc, null);
			cc = 0;
			Predicate<int> AddToComponent = delegate (int n)
			{
				if (added[n])
					return true;
				myscc[n] = cc;
				added[n] = true;
				return true;
			};
			while (S.Count > 0)
			{
				int v = S.Pop();
				if (added[v])
					continue;
				tmp.DFSearchFrom(v, null, AddToComponent, null);
				cc++;
			}
			scc = myscc;
			return cc;
        }

    /// <summary>
    /// Wyznacza odwrotność grafu
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <returns>Odwrotność grafu</returns>
    /// <remarks>
    /// Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym.<br/>
    /// <br/>
    /// Metoda uruchomiona dla grafu nieskierowanego zgłasza wyjątek <see cref="System.ArgumentException"/>.
    /// <br/>
    /// Graf wejściowy pozostaje niezmieniony.
    /// </remarks>
    public static Graph Reverse(this Graph g)
        {
			if (g.Directed == false)
				throw new System.ArgumentException("Graph should be directed");
			Graph ret = g.IsolatedVerticesGraph();
			for(int i = 0; i < g.VerticesCount; i++)
			{
				foreach(Edge e in g.OutEdges(i))
				{
					ret.AddEdge(e.To, e.From, e.Weight);
				}
			}
			return ret;
        }

    /// <summary>
    /// Wyznacza jądro grafu
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <returns>Jądro grafu</returns>
    /// <remarks>
    /// Jądro grafu to graf skierowany, którego wierzchołkami są silnie spójne składowe pierwotnego grafu.<br/>
    /// Cała silnie spójna składowa jest "ściśnięta" do jednego wierzchiłka.<br/>
    /// Wierzchołki jądra są połączone krawędzią gdy w grafie pierwotnym połączone są krawędzią dowolne
    /// z wierzchołków odpowiednich składowych (ale nie wprowadzamy pętli !). Wagi krawędzi przyjmujemy równe 1.<br/>
    /// <br/>
    /// Metoda uruchomiona dla grafu nieskierowanego zgłasza wyjątek <see cref="System.ArgumentException"/>.
    /// <br/>
    /// Graf wejściowy pozostaje niezmieniony.
    /// </remarks>
    public static Graph Kernel(this Graph g)
        {
			if (g.Directed == false)
				throw new System.ArgumentException("Graph should be directed");

			int[] scc;
			int n = g.StronglyConnectedComponents(out scc);
			Graph ret = g.IsolatedVerticesGraph(true, n);
			for(int i = 0; i < scc.Length; i++)
			{
				foreach(Edge e in g.OutEdges(i))
				{
					if (scc[e.To] != scc[i])
						ret.AddEdge(scc[i], scc[e.To]);
				}
			}
			return g;
        }

    /// <summary>
    /// Wyznacza ścieżki o maksymalnej przepustowości
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="s">Wierzchołek źródłowy</param>
    /// <param name="d">Znalezione ścieżki (parametr wyjściowy)</param>
    /// <returns><b>true</b></returns>
    /// <remarks>
    /// Metoda przydaje się w algorytmach wyznaczania maksymalnego przepływu, wagi krawędzi oznaczają tu przepustowość krawędzi.<br/>
    /// Przepustowość ścieżki to najmniejsza z przepustowości krawędzi wchodzących w jej skład.<br/>
    /// <br/>
    /// Elementy tablicy <i>d</i> zawierają przepustowości ścieżek od źródła do wierzchołka określonego przez indeks elementu.<br/>
    /// Jeśli ścieżka od źródła do danego wierzchołka nie istnieje, to przepustowość ma wartość <b>null</b>.
    /// <br/>
    /// Metoda zawsze zwraca <b>true</b> (można ją stosować do każdego grafu).
    /// </remarks>
    public static bool MaxFlowPathsLab05(this Graph g, int s, out PathsInfo[] d)
        {
			PathsInfo[] paths = new PathsInfo[g.VerticesCount];

			Predicate<Edge> visitEdge = (Edge e) =>
			{
				int? last = paths[e.From].Dist;
				if (last == null)
				{
					paths[e.To].Last = e;
					paths[e.To].Dist = e.Weight;
					return true;
				}
				if (paths[e.To].Dist == null || paths[e.To].Dist.Value <= Math.Min(e.Weight, last.Value))
				{
					paths[e.To].Last = e;
					paths[e.To].Dist = Math.Min(e.Weight, last.Value);
				}
				return true;
			};

			g.GeneralSearchFrom<EdgesMaxPriorityQueue>(s, null, visitEdge);
			d = paths;
			return true;
	}

    /// <summary>
    /// Bada czy graf nieskierowany jest acykliczny
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <returns>Informacja czy graf jest acykliczny</returns>
    /// <remarks>
    /// Metoda uruchomiona dla grafu skierowanego zgłasza wyjątek <see cref="System.ArgumentException"/>.
    /// <br/>
    /// Graf wejściowy pozostaje niezmieniony.
    /// </remarks>
    public static bool IsUndirectedAcyclic(this Graph g)
        {
			if (g.Directed == true)
				throw new System.ArgumentException("Should be undirected");

			int cc;
			g.GeneralSearchAll<EdgesStack>(null, null, out cc, null);
			return cc == (g.VerticesCount - g.EdgesCount);
        }

    }  // class Lab05GraphExtender

}  // namespace ASD.Graph
