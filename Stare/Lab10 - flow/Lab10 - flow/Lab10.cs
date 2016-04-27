using ASD.Graphs;
using System;
using System.Linq;

namespace ASD2
{

public static class FlowGraphExtender
    {

    /// <summary>
    /// Metoda zwraca graf reprezentujący najliczniejsze skojarzenie w grafie dwudzielnym.
    /// Skojarzenie znajdowane jest przy pomocy algorytmu znajdowania maksymalnego przepływu w sieci.
    /// Dla grafu dwudzielnego (nieskierowanego) G = (X,Y,E) tworzymy sieć N:
    /// V(N) = X u Y u {s,t}
    /// E(N) = { (x,y) : x należy do X i y należy do Y i {x,y} należy do E(G) } 
    ///        u {(s,x) : x należy do X } 
    ///        u {(y,t) : y należy do Y }
    /// c(e) = 1 dla każdego e należącego do E(N)
    /// 
    /// Krawędzie realizujące maksymalny przepływ w sieci N (poza krawędziami zawierającymi źródło i ujście) 
    /// odpowiadają najliczniejszemu skojarzeniu w G.
    /// </summary>
    /// <param name="g">Graf dwudzielny</param>
    /// <param name="matching">Znalezione skojarzenie - 
    /// graf nieskierowany będący kopią g z usuniętymi krawędziami spoza znalezionego skojarzenia</param>
    /// <returns>Liczność znalezionego skojarzenia</returns>
    /// <remarks>
    /// Uwaga 1: metoda nie modyfikuje zadanego grafu
    /// Uwaga 2: jeśli dany graf nie jest dwudzielny, metoda zgłasza wyjątek ArgumentException
    /// </remarks>
    public static int GetMaxMatching(this Graph g, out Graph matching)
        {
			//
			// TODO (2 pkt.)
			//
			int[] vertices;
			if (!g.Lab03IsBipartite(out vertices))
				throw new ArgumentException();

			Graph flowGraph = g.IsolatedVerticesGraph(true, g.VerticesCount + 2);
			int s = g.VerticesCount;
			int t = g.VerticesCount + 1;
			for (int i = 0; i < g.VerticesCount; i++)
			{
				foreach(var edge in g.OutEdges(i))
				{
					if(vertices[i] == 1 && vertices[edge.To] == 2)
						flowGraph.AddEdge(edge.From, edge.To);
				}
				if (vertices[i] == 1)
					flowGraph.AddEdge(s, i);
				else
					flowGraph.AddEdge(i, t);
			}
			var ge = new GraphExport();
			//ge.Export(g);
			//ge.Export(flowGraph);

			Graph flow;
			flowGraph.FordFulkersonMaxFlow(s, t, out flow);

			//ge.Export(flow);

			//Jeśli po danej krawędzi nie płynie żaden przepływ, to nadal jest ona w wynikowym grafie flow (oczywiście z wagą 0).
			matching = g.IsolatedVerticesGraph(false, g.VerticesCount);
			for(int i = 0; i < g.VerticesCount; i++)				// źródło i ujście pomijamy
			{
				foreach (Edge e in flow.OutEdges(i))
					if (e.To == s || e.To == t || e.Weight == 0)    // źródło i ujście pomijamy 
						continue;
					else
						matching.AddEdge(e.From, e.To);
			}
			//ge.Export(matching);
			return matching.EdgesCount;
        }
		public static bool Lab03IsBipartite(this Graph g, out int[] ver)
		{
			if (g.Directed == true)
				throw new ArgumentException();

			int[] tmp = new int[g.VerticesCount];

			// Graf zlozony tylko z wierzcholkow izolowanych:
			if (g.EdgesCount == 0)
			{
				ver = new int[g.VerticesCount];
				for (int i = 0; i < g.VerticesCount; i++)
					ver[i] = 1;
				return true;
			}

			// Predykat dla wierzcholka izolowanego:
			Predicate<int> predVert = delegate (int i)
			{
				if (g.OutDegree(i) == 0)
					tmp[i] = 1;
				return true;
			};

			// Predykat do kolorowania krawedzi:
			Predicate<Edge> predEdge = delegate (Edge e)
			{
				if (tmp[e.From] == 0)
					tmp[e.From] = 1;
				if (tmp[e.From] == tmp[e.To])
					return false;

				if (tmp[e.From] == 1)
					tmp[e.To] = 2;
				else
					tmp[e.To] = 1;

				return true;
			};

			// Wszystkie wierzcholki nieodwiedzone:
			for (int i = 0; i < g.VerticesCount; i++)
			{
				tmp[i] = 0;
			}

			int cc;
			if (!g.GeneralSearchAll<EdgesQueue>(predVert, predEdge, out cc, null))
			{
				ver = null;
				return false;
			}
			ver = tmp;

			return true;
		}
		/// <summary>
		/// Znajduje przepływ w sieci N z ograniczonymi przepustowościami wierzchołków (c(v)).
		/// Do ograniczeń wynikających ze klasycznego problemu maksymalnego przepływu
		/// w sieci dokładamy dodatkowe:
		/// dla każdego wierzchołka v, niebędącego źródłem lub ujściem przepływ przez
		/// dany wierzchołek nie może przekraczać jego przepustowości.
		/// Przepływ taki możemy znaleźć konstruując pomocniczą sieć N':
		/// V(N') = { v_in, v_out dla każdego v należącego do V(N) \ {s,t} } u {s,t}
		/// Dla każdego v należącego do V(N) \ {s,t} wierzchołki v_in i v_out łączymy krawędzią
		/// o przepustowości c(v). Każda krawędź (u,v) w E(N) jest reprezentowana przez krawędź
		/// (u_out, v_in) w N'. (przyjmujemy, że w N' s=s_in=s_out i t=t_in=t_out) - przepustowości pozostają bez zmian.
		/// Maksymalny przepływ w sieci N' odpowiada maksymalnemu przepływowi z ograniczeniami w sieci N.
		/// 
		/// </summary>
		/// <param name="network">sieć wejściowa</param>
		/// <param name="s">źródło sieci</param>
		/// <param name="t">ujście sieci</param>
		/// <param name="capacity">przepustowości wierzchołków, przepustowości źródła i ujścia to int.MaxValue</param>
		/// <param name="flowGraph">Znaleziony graf przepływu w sieci wejściowej</param>
		/// <returns>Wartość maksymalnego przepływu</returns>
		/// <remarks>
		/// Wskazówka: Można przyjąć, że przepustowości źródła i ujścia są nieskończone
		/// i traktować je jak wszystkie inne wierzchołki.
		/// </remarks>
		public static int ConstrainedMaxFlow(this Graph network, int s, int t, int[] capacity, out Graph flowGraph)
        {
			//
			// TODO (1 pkt.)
			//
			int vc = network.VerticesCount;
			Graph helpNetwork = network.IsolatedVerticesGraph(true, vc * 2);		

			for(int i = 0; i < vc; i++)
			{
				if (i != s && i != t)
					helpNetwork.AddEdge(i + vc, i, capacity[i]);
				foreach(Edge e in network.OutEdges(i))
				{
					if (e.To != t)
						helpNetwork.AddEdge(i, e.To + vc, e.Weight);
					else
						helpNetwork.AddEdge(e);
				}
			}

			Graph flow;
			//int flowValue = helpNetwork.FordFulkersonMaxFlow(s, t, out flow);
			int flowValue = helpNetwork.PushRelabelMaxFlow(s, t, out flow);

			Graph realFlow = network.IsolatedVerticesGraph(true, vc);
			for(int i = 0; i < vc; i++)
			{
				foreach(Edge e in flow.OutEdges(i))
				{
					if (e.To != t)
						realFlow.AddEdge(i, e.To - vc, e.Weight);
					else
						realFlow.AddEdge(e);
				}
			}
			flowGraph = realFlow;

			return flowValue;			
        }

    /// <summary>
    /// Znajduje największą liczbę rozłącznych ścieżek pomiędzy wierzchołkami s i f w grafe G.
    /// Ścieżki są rozłączne, jeśli poza końcami nie mają wierzchołków wspólnych.
    /// Problem rozwiązujemy sprowadzając go do problemu maksymalnego przepływu z przepustowościami wierzchołków.
    /// Konstruujemy sieć N.
    /// V(N) = V(G)
    /// (u,v) należy do E(N) <=> (u,v) należy do E(G)
    /// c(e) = 1 dla każdego e należącego do E(N)
    /// c(v) = 1 dla każdego v różnego od s i f.
    /// Wierzchołki s i f są odpowiednio źródłem i ujściem sieci.
    /// </summary>
    /// <param name="g">Graf wejściowy</param>
    /// <param name="start">Wierzchołek startowy ścieżek</param>
    /// <param name="finish">Wierzchołek końcowy ścieżek</param>
    /// <param name="paths">Graf ścieżek</param>
    /// <returns>Liczba znalezionych ścieżek</returns>
    /// <remarks>
    /// Uwaga: Metoda działa zarówno dla grafów skierowanych, jak i nieskierowanych.
    /// </remarks>
    public static int FindMaxIndependentPaths(this Graph g, int start, int finish, out Graph paths)
        {
			//
			// TODO (1 pkt.)
			//
			Graph helper = g.IsolatedVerticesGraph(true, g.VerticesCount);
			
			for(int i = 0; i < g.VerticesCount; i++)
			{
				foreach (Edge e in g.OutEdges(i))
					helper.AddEdge(e.From, e.To);
			}
			int[] capacity = Enumerable.Repeat(1, g.VerticesCount).ToArray();
			Graph ret;
			int count = helper.ConstrainedMaxFlow(start, finish, capacity, out ret);
			paths = ret;
			return count;
        }

    }

}