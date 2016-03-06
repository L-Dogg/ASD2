using System;
using ASD.Graphs;

namespace Lab3_Graph
{
	class Class1
	{
		public static void Main()
		{
			int Verticles = 6;
			RandomGraphGenerator rgg = new RandomGraphGenerator(12345);
			Graph g = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), Verticles, 0.70);
			Graph tree = null;

			int cc;
			int orda = 0;
			int []ord = new int[g.VerticesCount];
			bool[] visited = new bool[g.VerticesCount];
			int[] from = new int[g.VerticesCount];

			Predicate<Edge> createTree = delegate (Edge e)
			{
				if (visited[e.From] == false)
					visited[e.From] = true;
				if(visited[e.To] == false)
				{
					visited[e.To] = true;
					tree.AddEdge(e);
				}
				return true;
			};
			Predicate<int> vertVisit = delegate (int v)
			{
				Console.WriteLine("Vertex: {0}", v);
				if (ord[v] == 0)
					ord[v] = ++orda;
				return true;
			};
			Predicate<Edge> edgeVisit = delegate (Edge e)
			{
				Console.WriteLine("Visiting edge: {0} - {1}", e.From, e.To);
				if (visited[e.From] == false)
					visited[e.From] = true;
				if (visited[e.To] && e.To != from[e.From])
				{
					Console.WriteLine("Detected cycle: {0} - {1} - {2}", e.To, e.From, from[e.From]);
					return false;
				}
				visited[e.To] = true;
				from[e.To] = e.From;
				return true;
			};

			if (g.GeneralSearchAll<EdgesQueue>(vertVisit, edgeVisit, out cc))
				Console.WriteLine("Acyclic");
			else
				Console.WriteLine("Has cycles");

			Console.WriteLine("V - N");
			for (int i = 0; i < g.VerticesCount; ++i)
			{
				Console.WriteLine("{0} - {1}", i, ord[i]);
				ord[i] = 0;
				visited[i] = false;
				from[i] = 0;
			}

			tree = g.IsolatedVerticesGraph();
			g.GeneralSearchAll<EdgesQueue>(null, createTree, out cc);

			new GraphExport().Export(g);
			new GraphExport().Export(tree);
		}
    }
}
