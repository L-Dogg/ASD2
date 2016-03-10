
using System;
using ASD.Graphs;
using System.Collections.Generic;

namespace ASD.Lab03
{

	public static class Lab03GraphExtender
	{

		// Część 1
		// Wyznaczanie odwrotności grafu
		//   0.5 pkt
		// Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym
		// Parametry:
		//   g - graf wejściowy
		// Wynik:
		//   odwrotność grafu
		// Uwagi:
		//   1) Metoda uruchomiona dla grafu nieskierowanego powinna zgłaszać wyjątek Lab03Exception
		//   2) Graf wejściowy pozostaje niezmieniony
		//   3) Graf wynikowy musi być w takiej samej reprezentacji jak wejściowy
		public static Graph Lab03Reverse(this Graph g)
		{
			if (g.Directed == false)
				throw new Lab03Exception();

			Graph ret = g.IsolatedVerticesGraph(g.Directed, g.VerticesCount);

			for (int i = 0; i < g.VerticesCount; i++)
			{
				foreach (Edge e in g.OutEdges(i))
				{
					ret.AddEdge(e.To, e.From, e.Weight);
				}
			}
			return ret;  // zmienić
		}

		// Część 2
		// Badanie czy graf jest dwudzielny
		//   1.5 pkt
		// Graf dwudzielny to graf nieskierowany, którego wierzchołki można podzielić na dwa rozłączne zbiory
		// takie, że dla każdej krawędzi jej końce należą do róźnych zbiorów
		// Parametry:
		//   g - badany graf
		//   vert - tablica opisująca podział zbioru wierzchołków na podzbiory w następujący sposób
		//          vert[i] == 1 oznacza, że wierzchołek i należy do pierwszego podzbioru
		//          vert[i] == 2 oznacza, że wierzchołek i należy do drugiego podzbioru
		// Wynik:
		//   true jeśli graf jest dwudzielny, false jeśli graf nie jest dwudzielny (w tym przypadku parametr vert ma mieć wartość null)
		// Uwagi:
		//   1) Metoda uruchomiona dla grafu skierowanego powinna zgłaszać wyjątek Lab03Exception
		//   2) Graf wejściowy pozostaje niezmieniony
		//   3) Podział wierzchołków może nie być jednoznaczny - znaleźć dowolny
		//   4) Pamiętać, że każdy z wierzchołków musi być przyporządkowany do któregoś ze zbiorów
		//   5) Metoda ma mieć taki sam rząd złożoności jak zwykłe przeszukiwanie (za większą będą kary!)

		public static bool Lab03IsBipartite(this Graph g, out int[] ver)
		{
			ver = new int[g.VerticesCount];
			for (int i = 0; i < g.VerticesCount; i++)
				ver[i] = 0;
			for (int i = 0; i < g.VerticesCount; i++)
			{
				if(ver[i] == 0)
				{
					ver[i] = 1;
					foreach (Edge e in g.OutEdges(i))
					{
						if (ver[e.To] == 1)
							return false;
						ver[e.To] = 2;
					}
				}
			}
			return true;
		}
		}
		// Część 3
		// Wyznaczanie minimalnego drzewa rozpinającego algorytmem Kruskala
		//   1.5 pkt
		// Schemat algorytmu Kruskala
		//   1) wrzucić wszystkie krawędzie do "wspólnego worka"
		//   2) wyciągać z "worka" krawędzie w kolejności wzrastających wag
		//      - jeśli krawędź można dodać do drzewa to dodawać, jeśli nie można to ignorować
		//      - punkt 2 powtarzać aż do skonstruowania drzewa (lub wyczerpania krawędzi)
		// Parametry:
		//   g - graf wejściowy
		//   mstw - waga skonstruowanego drzewa (lasu)
		// Wynik:
		//   skonstruowane minimalne drzewo rozpinające (albo las)
		// Uwagi:
		//   1) Metoda uruchomiona dla grafu skierowanego powinna zgłaszać wyjątek Lab03Exception
		//   2) Graf wejściowy pozostaje niezmieniony
		//   3) Wykorzystać klasę UnionFind z biblioteki Graph
		//   4) Jeśli graf g jest niespójny to metoda wyznacza las rozpinający
		//   5) Graf wynikowy (drzewo) musi być w takiej samej reprezentacji jak wejściowy
		public static Graph Lab03Kruskal(this Graph g, out int mstw)
		{
			mstw = 0;       // zmienić
			if (g.Directed == true)
				throw new Lab03Exception();

			Graph ret = g.IsolatedVerticesGraph();
			EdgesMinPriorityQueue pq = new EdgesMinPriorityQueue();
			// hashset, zeby nie dodawac dwa razy tej samej krawedzi:
			//HashSet<Edge> hs = new HashSet<Edge>();
			for (int i = 0; i < g.VerticesCount; i++)
			{
				foreach (Edge e in g.OutEdges(i))
				{
					/*if (!hs.Contains(e))
					{
						//Console.WriteLine("Edge added");
						pq.Put(e);
						hs.Add(new Edge(e.To, e.From, e.Weight));
					}*/
					if(e.To < i)
					{
						pq.Put(e);
					}
				}
			}
			UnionFind vs = new UnionFind(g.VerticesCount);
			int vsCount = g.VerticesCount;
			Edge ed;
			int s1, s2;
            while (pq.Count > 1 && vsCount > 1)
			{
				//Console.WriteLine("PEEK: {0}", pq.Peek());
				ed = pq.Get();
				s1 = vs.Find(ed.From);
				s2 = vs.Find(ed.To);
				if(s1 != s2)
				{
					vsCount--;
					vs.Union(s1, s2);
					ret.AddEdge(ed);
					mstw += ed.Weight;
					//Console.WriteLine("Weigth: {0}", mstw);
				}
			}
			return ret;  // zmienić
        }

    // Część 4
    // Badanie czy graf nieskierowany jest acykliczny
    //   0.5 pkt
    // Parametry:
    //   g - badany graf
    // Wynik:
    //   true jeśli graf jest acykliczny, false jeśli graf nie jest acykliczny
    // Uwagi:
    //   1) Metoda uruchomiona dla grafu skierowanego powinna zgłaszać wyjątek Lab03Exception
    //   2) Graf wejściowy pozostaje niezmieniony
    //   3) Najpierw pomysleć jaki, prosty do sprawdzenia, warunek spełnia acykliczny graf nieskierowany
    //      Zakodowanie tefo sprawdzenia nie powinno zająć więcej niż kilka linii!
    //      Zadanie jest bardzo łatwe (jeśli wydaje się trudne - poszukać prostszego sposobu, a nie walczyć z trudnym!)
    public static bool Lab03IsUndirectedAcyclic(this Graph g)
        {
			if (g.Directed == true)
				throw new Lab03Exception();

			bool[] visited = new bool[g.VerticesCount];
			int[] from = new int[g.VerticesCount];
			Predicate<Edge> IsTree = delegate (Edge e)
			{
				if (!visited[e.From])
				{
					visited[e.From] = true;
					from[e.From] = e.From;
				}
				if (visited[e.To] && e.To != from[e.From])
					return false;
				visited[e.To] = true;
				from[e.To] = e.From;
				return true;
			};

			int cc;
			return g.GeneralSearchAll<EdgesQueue>(null, IsTree, out cc);
        }

    }

}
