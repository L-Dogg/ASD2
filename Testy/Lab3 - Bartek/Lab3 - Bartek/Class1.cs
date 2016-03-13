using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Lab03
{
	class Lab03_Test
	{
		const int size = 500;
		const int testRnds = 1;
		static int reverseOk, bipartiteOk, kruskalOk, testNo;
		static Type[] types =
		{
			typeof(AdjacencyMatrixGraph),
			typeof(AdjacencyListsGraph<SimplyAdjacencyList>),
			typeof(AdjacencyListsGraph<HashTableAdjacencyList>),
			typeof(AdjacencyListsGraph<AVLAdjacencyList>)
		};
		static int totalTestCount = types.Length * testRnds * 4;

		public static void Main()
		{
			Random rng = new Random();
			RandomGraphGenerator rgg = new RandomGraphGenerator();
			for (int i = 1; i <= testRnds; i++)
			{
				foreach (Type type in types)
				{
					TestGraph(rgg.UndirectedGraph(type, size, rng.NextDouble()), "Random undirected graph");
					TestGraph(rgg.DirectedGraph(type, size, rng.NextDouble()), "Random directed graph");
					TestGraph(rgg.UndirectedGraph(type, size, rng.NextDouble() * 2 / size, 0, 100), "Sparse undirected graph");
					int k = rng.Next() % (size - 1) + 1;
					TestGraph(rgg.BipariteGraph(type, k, size - k, rng.NextDouble()), "Bipartite graph");
				}
			}
			Console.WriteLine("Test summary:\n Reverse graphs:\t{0}/{3}, \n Bipartite graphs:\t{1}/{3},\n Kruskal's algorithm:\t{2}/{3}",
				reverseOk,
				bipartiteOk,
				kruskalOk,
				totalTestCount);
		}

		public static void TestGraph(Graph graph, string type)
		{
			Console.WriteLine("Test {0} of {1}\nGraph type: {2}\nVertex count: {3}\nEdge count: {4}\nTest type: {5}",
				++testNo,
				totalTestCount,
				graph.GetType().FullName,
				graph.VerticesCount,
				graph.EdgesCount,
				type);

			try
			{
				TestGraphReverse(graph);
			}
			catch (Exception ex)
			{
				Console.WriteLine(" ! {0}", ex.Message);
			}
			try
			{
				TestGraphBipartite(graph);
			}
			catch (Exception ex)
			{
				Console.WriteLine(" ! {0}", ex.Message);
			}
			try
			{
				TestGraphKruskal(graph);
			}
			catch (Exception ex)
			{
				Console.WriteLine(" ! {0}", ex.Message);
			}
			Console.WriteLine();
		}

		private static void TestGraphReverse(Graph graph)
		{
			Graph expected = null, actual = null, clone = graph.Clone();
			Exception expectedEx = null, actualEx = null;
			ulong initial = Graph.Counter, counter;
			Console.WriteLine("(1) Testing graph reverse...");
			Console.Write(" * Getting expected inverse... ");
			try
			{
				expected = graph.Reverse();
			}
			catch (Exception ex)
			{
				expectedEx = ex;
			}
			counter = Graph.Counter - initial;
			initial = Graph.Counter;
			Console.WriteLine("access count was {0}", counter);
			Console.Write(" * Getting actual graph... ");
			try
			{
				actual = graph.Lab03Reverse();
			}
			catch (Exception ex)
			{
				actualEx = ex;
			}
			counter = Graph.Counter - initial;
			Console.WriteLine("access count was {0}", counter);
			try
			{
				TestExceptions(expectedEx, actualEx);
				TestDirectedness(expected, actual);
				TestGraphType(expected, actual);
				TestGraphResult(expected, actual);
				TestGraphPreserved(clone, actual);
			}
			catch (Exception tEx)
			{
				throw tEx;
			}
			reverseOk++;
			Console.WriteLine(" ! Test completed successfully.");
		}

		private static void TestGraphKruskal(Graph graph)
		{
			Graph expectedTree = null, actualTree = null, clone = graph.Clone();
			int expected = 0, actual = 0;
			Exception expectedEx = null, actualEx = null;
			ulong initial = Graph.Counter, counter;
			Console.WriteLine("(3) Testing Kruskal's algorithm...");
			Console.Write(" * Getting expected minimal spanning tree... ");
			try
			{
				expected = graph.Kruskal(out expectedTree);
			}
			catch (Exception ex)
			{
				expectedEx = ex;
			}
			counter = Graph.Counter - initial;
			initial = Graph.Counter;
			Console.WriteLine("access count was {0}", counter);
			Console.Write(" * Getting actual graph... ");
			try
			{
				actualTree = graph.Lab03Kruskal(out actual);
			}
			catch (Exception ex)
			{
				actualEx = ex;
			}
			counter = Graph.Counter - initial;
			Console.WriteLine("access count was {0}", counter);
			try
			{
				TestExceptions(expectedEx, actualEx);
				TestDirectedness(expectedTree, actualTree);
				TestGraphType(expectedTree, actualTree);
				TestResult(expected, actual);
				TestMinTree(clone, actualTree);
				TestGraphPreserved(clone, actualTree);
			}
			catch (Exception tEx)
			{
				throw tEx;
			}
			kruskalOk++;
			Console.WriteLine(" ! Test completed successfully.");
		}

		private static void TestGraphBipartite(Graph graph)
		{
			bool expected = false;
			Exception expectedEx = null;
			int[] partition = null;
			Graph clone = graph.Clone();
			ulong initial = Graph.Counter, counter;
			Console.WriteLine("(2) Testing if graph is bipartite...");
			Console.WriteLine(" * Getting partition... ");
			try
			{
				expected = graph.Lab03IsBipartite(out partition);
			}
			catch (Exception ex)
			{
				expectedEx = ex;
			}
			counter = Graph.Counter - initial;
			try
			{
				TestExceptions(expectedEx, graph.Directed);
				TestResult(graph, partition);
				TestGraphPreserved(clone, graph);
			}
			catch (Exception tEx)
			{
				throw tEx;
			}
			bipartiteOk++;
			Console.WriteLine(" ! Test completed successfully.");
		}

		private static void TestExceptions(Exception exception, bool expected)
		{
			Console.WriteLine(" * Testing exceptions...");
			if ((exception == null && expected) ||
				(exception != null && !expected))
				throw new Exception(String.Format("Exception discrepancy: expected {0} exception, got {1}",
					expected ? "an" : "no",
					exception?.Message ?? "no exception"));
		}

		private static void TestExceptions(Exception expectedEx, Exception actualEx)
		{
			Console.WriteLine(" * Testing exceptions...");
			if ((expectedEx == null && actualEx != null) ||
				(expectedEx != null && actualEx == null))
				throw new Exception(String.Format("Exception discrepancy: expected {0}, got {1}",
					expectedEx?.Message ?? "no exception",
					actualEx?.Message ?? "no exception"));
		}

		private static void TestResult<T>(T expected, T actual)
			where T : struct
		{
			Console.WriteLine(" * Testing result...");
			if (!expected.Equals(actual))
				throw new Exception("Wrong result");
		}

		private static void TestResult(Graph graph, int[] result)
		{
			Console.Write(" * Testing partition... ");
			if (result == null)
			{
				Console.WriteLine("result is null, skipping check");
				return;
			}
			else
				Console.WriteLine();
			for (int i = 0; i < graph.VerticesCount; i++)
			{
				if (graph.OutDegree(i) == 0)
					if (result[i] != 1 && result[i] != 2)
						throw new Exception("Found isolated vertex which does not belong to any set");
				foreach (Edge e in graph.OutEdges(i))
					if (result[e.From] == result[e.To])
						throw new Exception("Invalid partition");
			}
		}

		private static void TestGraphResult(Graph expected, Graph actual)
		{
			Console.WriteLine(" * Testing graph result...");
			if (expected == null && actual == null)
				return;
			if (!expected.IsEqual(actual))
				throw new Exception("Wrong result");
		}

		private static void TestGraphPreserved(Graph expected, Graph actual)
		{
			Console.WriteLine(" * Testing graph preservation...");
			if (actual == null)
				return;
			if (expected.Equals(actual))
				throw new Exception("Graph was not preserved");
		}

		private static void TestGraphType(Graph expected, Graph actual)
		{
			Console.WriteLine(" * Testing graph type...");
			if (expected == null && actual == null)
				return;
			if (expected.GetType() != actual.GetType())
				throw new Exception(String.Format("Invalid graph type: expected {0}, got {1}",
					expected.GetType().FullName,
					actual.GetType().FullName));
		}

		private static void TestDirectedness(Graph expected, Graph actual)
		{
			Console.WriteLine(" * Testing directedness...");
			if (expected == null && actual == null)
				return;
			if (expected.Directed != actual.Directed)
				throw new Exception(String.Format("Directedness discrepancy: expected {0}, got {1}",
					expected.Directed,
					actual.Directed));
		}

		private static void TestMinTree(Graph expected, Graph actual)
		{
			Console.WriteLine(" * Testing if result is a spanning tree...");
			if (actual == null)
				return;
			if (expected.VerticesCount != actual.VerticesCount)
				throw new Exception(String.Format("Vertex count mismatch: expected {0}, got {1}",
					expected.VerticesCount,
					actual.VerticesCount));
			for (int v = 0; v < actual.VerticesCount; ++v)
				foreach (Edge e in actual.OutEdges(v))
					if (e.Weight != expected.GetEdgeWeight(e.From, e.To))
						throw new Exception(String.Format("Edge weight mismatch: expected {0}, got {1}",
							expected.GetEdgeWeight(e.From, e.To),
							e.Weight));
			int expectedCc, actualCc;
			expected.GeneralSearchAll<EdgesStack>(null, null, out expectedCc);
			actual.GeneralSearchAll<EdgesStack>(null, null, out actualCc);
			if (expectedCc != actualCc)
				throw new Exception(String.Format("Connected component count mismatch: expected {0}, got {1}",
					expectedCc,
					actualCc));
		}
	}
}