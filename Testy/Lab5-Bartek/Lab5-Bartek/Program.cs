#if DEBUG
#define VERBOSE
#undef VERBOSE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AsdLab5
{
	class ArbitrageTestMain
	{
		private static ArbitrageTest[] tests =
		{
			new EmptyArbitrageTest(),
			new SinglePathArbitrageTest(5),
			new SinglePathArbitrageTest(10),
			new SinglePathArbitrageTest(20),
			new SinglePathArbitrageTest(50),
			new SinglePathArbitrageTest(100),
			new TwoPathsArbitrageTest(5),
			new TwoPathsArbitrageTest(10),
			new TwoPathsArbitrageTest(20),
			new TwoPathsArbitrageTest(50),
			new TwoPathsArbitrageTest(100),
			new PositiveCycleArbitrageTest(2),
			new PositiveCycleArbitrageTest(5),
			new PositiveCycleArbitrageTest(10),
			new PositiveCycleArbitrageTest(20),
			new PositiveCycleArbitrageTest(50),
			new PositiveCycleArbitrageTest(100),
			new NegativeCycleArbitrageTest(2),
			new NegativeCycleArbitrageTest(5),
			new NegativeCycleArbitrageTest(10),
			new NegativeCycleArbitrageTest(20),
			new NegativeCycleArbitrageTest(50),
			new NegativeCycleArbitrageTest(100),
			new PathToNegativeCycleArbitrageTest(5),
			new PathToNegativeCycleArbitrageTest(10),
			new PathToNegativeCycleArbitrageTest(20),
			new PathToNegativeCycleArbitrageTest(50),
			new PathToNegativeCycleArbitrageTest(100),
			new TwoCyclesArbitrageTest(5),
			new TwoCyclesArbitrageTest(10),
			new TwoCyclesArbitrageTest(20),
			new TwoCyclesArbitrageTest(50),
			new TwoCyclesArbitrageTest(100)
		};

		public static void Main()
		{
			foreach (ArbitrageTest test in tests)
				test.RunTest();
			Console.WriteLine("Result: {0} out of {1} tests successful; completed in {2}",
				ArbitrageTest.testSuccess,
				ArbitrageTest.testCount,
				ArbitrageTest.allTests);
		}
	}

	abstract class Test
	{
		protected abstract string testName { get; }
		public static int testCount;
		public static int testNumber;
		public static int testSuccess;
		public TimeSpan timeTaken;
		public static TimeSpan allTests;

		public abstract void RunTest();

		protected void PrintTestInfo()
		{
			Console.WriteLine("Test {0}: {1}", testNumber, testName);
		}

		protected void PrintMessage(string format, params object[] arg)
		{
			Console.WriteLine(String.Format("* {0}", format), arg);
		}

		protected void PrintImportant(string format, params object[] arg)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(String.Format("! {0}", format), arg);
			Console.ForegroundColor = ConsoleColor.White;
		}

		protected void PrintSuccess(string format, params object[] arg)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(String.Format("+ {0}", format), arg);
			Console.ForegroundColor = ConsoleColor.White;
		}

		protected void PrintFailure(string format, params object[] arg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(String.Format("X {0}", format), arg);
			Console.ForegroundColor = ConsoleColor.White;
		}

		protected void CompareResult<T>(T expected, T actual, string comparedName)
			where T : IEquatable<T>
		{
			if (!expected.Equals(actual))
				throw new Exception(String.Format("{0} mismatch: expected {1}, got {2}",
					comparedName, expected, actual));
#if VERBOSE
            else
                PrintMessage("{0} result: expected {1}, got {2}",
                    comparedName, expected, actual);
#endif
		}

		protected void CompareResult(double expected, double actual, string comparedName)
		{
			if (Math.Abs(expected - actual) > 1e-5)
				throw new Exception(String.Format("{0} mismatch: expected {1}, got {2}",
					comparedName, expected, actual));
#if VERBOSE
            else
                PrintMessage("{0} result: expected {1}, got {2}",
                    comparedName, expected, actual);
#endif
		}

		protected void CompareResult<T>(T[] expected, T[] actual, string comparedName)
			where T : IEquatable<T>
		{
			if (expected == null && actual == null)
			{
#if VERBOSE
                PrintMessage("{0} result: expected null, got null", comparedName);
#endif
				return;
			}
			if ((expected == null && actual != null) ||
				(expected != null && actual == null))
				throw new Exception(String.Format("Unexpected null in {0} array", expected == null ? "expected" : "actual"));
			if (expected.Length != actual.Length)
				throw new Exception(String.Format("Length mismatch: expected {1}, got {2}",
					comparedName, expected.Length, actual.Length));
			for (int i = 0; i < expected.Length; i++)
				CompareResult(expected[i], actual[i], String.Format("{0} [{1}]", comparedName, i));
#if VERBOSE
            PrintMessage("{0} comparison successful", comparedName);
#endif
		}

		protected void CompareResult(double[] expected, double[] actual, string comparedName)
		{
			if (expected == null && actual == null)
				return;
			if ((expected == null && actual != null) ||
				(expected != null && actual == null))
				throw new Exception(String.Format("Unexpected null in {0} array", expected == null ? "expected" : "actual"));
			if (expected.Length != actual.Length)
				throw new Exception(String.Format("Length mismatch: expected {1}, got {2}",
					comparedName, expected.Length, actual.Length));
			for (int i = 0; i < expected.Length; i++)
				CompareResult(expected[i], actual[i], String.Format("{0} [{1}]", comparedName, i));
#if VERBOSE
            PrintMessage("{0} comparison successful", comparedName);
#endif
		}

		protected T GetResult<T>(Func<T> function, out Exception exception)
		{
			T result = default(T);
			TimeSpan elapsedTime;
			DateTime start = DateTime.Now;
			exception = null;
			try
			{
				result = function();
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			elapsedTime = DateTime.Now - start;
			timeTaken += elapsedTime;
#if DEBUG
			PrintMessage(String.Format("Elapsed time: {0}", elapsedTime));
#endif
			return result;
		}
	}

	abstract class ArbitrageTest : Test
	{
		protected int testSize;
		protected ExchangePair[] exchanges;
		private CurrencyGraph graph;
		protected double[][] expectedBestPrices;
		protected bool[] expectedArbitrages;
		protected int[][] expectedArbitrageCycles;

		public ArbitrageTest()
		{
			testCount++;
		}

		public ArbitrageTest(int size) : this()
		{
			if (size <= 0)
				throw new ArgumentException("Number of vertices must be positive");
			testSize = size;
		}

		public override void RunTest()
		{
			testNumber++;
			PrepareTestData();
			PrintTestInfo();
			try
			{
				ConstructCurrencyGraph();
				CheckConstruction();
				CheckBestPrices();
				CheckArbitrage();
			}
			catch (Exception ex)
			{
				PrintFailure("Test failed: {0}\n", ex.Message);
#if DEBUG
				Console.ReadKey();
#endif
				return;
			}
			PrintSuccess("Test passed");
#if DEBUG
			Console.ReadKey();
#endif
			PrintMessage("Total time: {0}\n", timeTaken);
			allTests += timeTaken;
			testSuccess++;
		}

		protected virtual void PrepareTestData()
		{
			expectedArbitrageCycles = new int[testSize][];
			expectedArbitrages = new bool[testSize];
			expectedBestPrices = new double[testSize][];
			for (int i = 0; i < testSize; i++)
			{
				expectedArbitrageCycles[i] = null;
				expectedArbitrages[i] = false;
				expectedBestPrices[i] = new double[testSize];
				expectedBestPrices[i][i] = 1.0;
			}
		}

		private void ConstructCurrencyGraph()
		{
			PrintImportant("Constructing graph...");
			Exception ex;
			graph = GetResult(() =>
			{
				return new CurrencyGraph(testSize, exchanges);
			}, out ex);
			if (ex != null)
				PrintMessage("Unexpected exception: {0}", ex.Message);
		}

		private double[,] GetWeights(CurrencyGraph graph)
		{
			return typeof(CurrencyGraph)
				.GetField("weights", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(graph) as double[,];
		}

		protected virtual void CheckConstruction()
		{
			PrintImportant("Checking graph construction...");
			double[,] weights = GetWeights(graph);
			if (weights == null)
				throw new Exception("Weights array is empty or invalid");
			if (exchanges != null)
				foreach (var pair in exchanges)
					if (Math.Abs(weights[pair.From, pair.To] + Math.Log(pair.Price)) > 1e-8 &&
						Math.Abs(weights[pair.To, pair.From] + Math.Log(pair.Price)) > 1e-8)
						throw new Exception(String.Format("Pair {0}-{1} with price {2} could not be found",
							pair.From, pair.To, pair.Price));
		}

		protected virtual void CheckBestPrices()
		{
			PrintImportant("Testing best prices...");
			double[] actualBestPrices = null;
			bool actualBestPriceFound;
			for (int i = 0; i < testSize; i++)
			{
#if DEBUG
				PrintMessage("Testing vertex {0}", i);
#endif
				Exception exception;
				actualBestPriceFound = GetResult(() =>
				{
					return graph.findBestPrice(i, out actualBestPrices);
				}, out exception);
				if (exception != null)
					throw exception;
				try
				{
					CompareResult(expectedBestPrices[i], actualBestPrices, "Best prices");
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		protected virtual void CheckArbitrage()
		{
			PrintImportant("Testing arbitrage...");
			int[] actualExchangeCycle = null;
			bool actualArbitrageFound;
			for (int i = 0; i < testSize; i++)
			{
#if DEBUG
				PrintMessage("Testing vertex {0}", i);
#endif
				Exception exception;
				actualArbitrageFound = GetResult(() =>
				{
					return graph.findArbitrage(i, out actualExchangeCycle);
				}, out exception);
				if (exception != null)
					throw exception;
				try
				{
					CompareResult(expectedArbitrages[i], actualArbitrageFound, "Arbitrage");
					CheckArbitrageCycle(expectedArbitrageCycles[i], actualExchangeCycle);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		private void CheckArbitrageCycle(int[] expected, int[] actual)
		{
#if DEBUG
			PrintMessage("Checking cycle...");
#endif
			if (expected == null && actual == null)
			{
#if VERBOSE
                PrintMessage("Expected null, got null");
#endif
				return;
			}
			if ((expected == null && actual != null) ||
				(expected != null && actual == null))
				throw new Exception(String.Format("Unexpected null in {0} array", expected == null ? "expected" : "actual"));
			if (actual.First() != actual.Last())
				throw new Exception(String.Format("Cycle is not closed: first element is {0}, last is {1}",
					actual.First(), actual.Last()));
			double[,] weights = GetWeights(graph);
			double overall = 0.0;

			for (int i = 0; i < actual.Length - 1; i++)
			{
				if (weights[actual[i], actual[i + 1]] == double.PositiveInfinity ||
					weights[actual[i], actual[i + 1]] == double.MaxValue)
					throw new Exception(String.Format("Nonexistent edge {0}-{1} found in cycle",
						actual[i], actual[i + 1]));
				overall += weights[actual[i], actual[i + 1]];
			}
			overall = Math.Exp(-overall);
			if (overall < 1.0)
				throw new Exception(String.Format("Arbitrage cycle is not profitable: overall price is {0}", overall));
#if VERBOSE
            PrintMessage("Cycle verified successfully");
#endif
		}
	}

	class EmptyArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Empty arbitrage";
		protected override void PrepareTestData()
		{
			exchanges = null;
			testSize = 1;
			expectedArbitrageCycles = new int[][] { null };
			expectedArbitrages = new bool[] { false };
			expectedBestPrices = new double[][] { new double[] { 1 } };
		}
	}

	class SinglePathArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph is a single path; no arbitrage";

		public SinglePathArbitrageTest(int size) : base(size) { }

		protected override void PrepareTestData()
		{
			base.PrepareTestData();
			exchanges = CurrencyGraphHelper.ConstructPath(1013 * testNumber,
				Enumerable.Range(0, testSize).Reverse().ToArray(),
				expectedBestPrices).ToArray();
		}
	}

	class TwoPathsArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph contains two directed paths; no arbitrage";

		public TwoPathsArbitrageTest(int size) : base(size)
		{
			if (size < 4)
				throw new ArgumentException("This test can be only performed on graphs with more than four vertices");
		}

		protected override void PrepareTestData()
		{
			base.PrepareTestData();

			// start = 0
			// end = 1
			int firstPathLength = (testSize - 2) / 2;
			int secondPathLength = testSize - 2 - firstPathLength;

			List<ExchangePair> edges = new List<ExchangePair>();
			List<int> firstPath, secondPath;

			firstPath = new List<int>(new int[] { 0, 1 });
			secondPath = new List<int>(new int[] { 0, 1 });
			firstPath.InsertRange(1, Enumerable.Range(2, firstPathLength));
			secondPath.InsertRange(1, Enumerable.Range(firstPathLength + 2, secondPathLength));

			edges.AddRange(CurrencyGraphHelper.ConstructPath(2057 * testNumber,
				firstPath.ToArray(),
				expectedBestPrices));
			edges.AddRange(CurrencyGraphHelper.ConstructPath(3043 * testNumber,
				secondPath.ToArray(),
				expectedBestPrices));
			exchanges = edges.ToArray();
		}
	}

	class PositiveCycleArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph is a positive cycle; no arbitrage";

		public PositiveCycleArbitrageTest(int size) : base(size)
		{
			if (size < 2)
				throw new ArgumentException("Directed cycle has to have two or more vertices");
		}

		protected override void PrepareTestData()
		{
			base.PrepareTestData();

			exchanges = CurrencyGraphHelper.ConstructCycle(4577 * testNumber,
				Enumerable.Range(0, testSize).ToArray(),
				expectedBestPrices,
				false).ToArray();
		}
	}

	class NegativeCycleArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph is a negative cycle; arbitrages possible for all currencies";

		public NegativeCycleArbitrageTest(int size) : base(size)
		{
			if (size < 2)
				throw new ArgumentException("Directed cycle has to have two or more vertices");
		}

		protected override void PrepareTestData()
		{
			base.PrepareTestData();

			List<int> cycle = new List<int>(Enumerable.Range(0, testSize));

			exchanges = CurrencyGraphHelper.ConstructCycle(5797 * testNumber,
				cycle.ToArray(),
				expectedBestPrices,
				true).ToArray();

			cycle.Add(0);
			for (int i = 0; i < testSize; i++)
			{
				expectedBestPrices[i] = null;
				expectedArbitrages[i] = true;
				expectedArbitrageCycles[i] = cycle.ToArray();
			}
		}
	}

	class PathToNegativeCycleArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph consists of path connected to negative cycle";

		public PathToNegativeCycleArbitrageTest(int size) : base(size)
		{
			if (size < 4)
				throw new ArgumentException("Graph has to have four or more vertices");
		}

		protected override void PrepareTestData()
		{
			base.PrepareTestData();

			int cycleLength = testSize / 2;
			int pathLength = testSize - cycleLength + 1;
			List<int> cycle = new List<int>(Enumerable.Range(0, cycleLength));
			List<ExchangePair> edges = new List<ExchangePair>();

			edges.AddRange(CurrencyGraphHelper.ConstructCycle(6171 * testNumber,
				cycle.ToArray(),
				expectedBestPrices,
				true));
			edges.AddRange(CurrencyGraphHelper.ConstructPath(7291 * testNumber,
				Enumerable.Range(cycle.Last(), pathLength).Reverse().ToArray(),
				null));

			cycle.Add(0);
			for (int i = 0; i < testSize; i++)
			{
				expectedBestPrices[i] = null;
				expectedArbitrages[i] = true;
				expectedArbitrageCycles[i] = cycle.ToArray();
			}
			exchanges = edges.ToArray();
		}
	}

	class TwoCyclesArbitrageTest : ArbitrageTest
	{
		protected override string testName => "Graph consists of a negative and a positive cycle, with one common vertex";

		public TwoCyclesArbitrageTest(int size) : base(size)
		{
			if (size < 4)
				throw new ArgumentException("Graph has to have four or more vertices");
		}

		protected override void PrepareTestData()
		{
			base.PrepareTestData();

			int negativeCycleLength = testSize / 2;
			int positiveCycleLength = testSize - negativeCycleLength + 1;
			List<int> negativeCycle = new List<int>(Enumerable.Range(0, negativeCycleLength));
			List<int> positiveCycle = new List<int>(Enumerable.Range(negativeCycle.Last(), positiveCycleLength));
			List<ExchangePair> edges = new List<ExchangePair>();

			Random rng = new Random(10113);
			if (rng.NextDouble() > 0.5)
				positiveCycle.Reverse();

			edges.AddRange(CurrencyGraphHelper.ConstructCycle(12609 * testNumber,
				negativeCycle.ToArray(),
				expectedBestPrices,
				true));
			edges.AddRange(CurrencyGraphHelper.ConstructCycle(13413 * testNumber,
				positiveCycle.ToArray(),
				expectedBestPrices,
				false));

			negativeCycle.Add(0);
			for (int i = 0; i < testSize; i++)
			{
				expectedBestPrices[i] = null;
				expectedArbitrages[i] = true;
				expectedArbitrageCycles[i] = negativeCycle.ToArray();
			}
			exchanges = edges.ToArray();
		}
	}

	static class CurrencyGraphHelper
	{
		public static List<ExchangePair> ConstructPath(int seed, int[] vert, double[][] bestPrices)
		{
			List<ExchangePair> edges = new List<ExchangePair>();
			Random rng = new Random(seed);
			for (int i = 0; i < vert.Length - 1; i++)
			{
				double newWeight = rng.NextDouble();
				newWeight = rng.NextDouble() > 0.5 ? newWeight : 1 / newWeight;
				edges.Add(new ExchangePair(vert[i], vert[i + 1], newWeight));
				if (bestPrices != null)
					for (int j = 0; j <= i; j++)
						bestPrices[vert[j]][vert[i + 1]] = Math.Max(bestPrices[vert[j]][vert[i + 1]],
							bestPrices[vert[j]][vert[i]] * newWeight);
			}
			return edges;
		}

		public static List<ExchangePair> ConstructCycle(int seed, int[] vert, double[][] bestPrices, bool isNegative)
		{
			List<ExchangePair> edges = new List<ExchangePair>();
			Random rng = new Random(seed);
			// adding edges
			for (int i = 0; i < vert.Length; i++)
			{
				double newWeight = rng.NextDouble();
				newWeight = isNegative ? 1 / newWeight : newWeight;
				edges.Add(new ExchangePair(vert[i], vert[(i + 1) % vert.Length], newWeight));
			}
			if (!isNegative)
				for (int i = 0; i < vert.Length; i++)
					for (int j = 1; j < vert.Length; j++)
						bestPrices[vert[i]][vert[(j + i) % vert.Length]] = Math.Max(
							bestPrices[vert[i]][vert[(j + i) % vert.Length]],
							bestPrices[vert[i]][vert[(j + i - 1) % vert.Length]] *
								edges[(j + i - 1) % vert.Length].Price);
			return edges;
		}
	}
}