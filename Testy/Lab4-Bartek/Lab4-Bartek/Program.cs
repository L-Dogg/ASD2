using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
	abstract class CrossoutTest
	{
		protected char[] testSequence;
		protected char[][] testPatterns;
		protected abstract bool expectedErasable { get; }
		protected abstract string testName { get; }
		protected int expectedCrossoutsNumber;
		protected int expectedRemainderNumber;
		public static int testCount;
		public static int testNumber;
		public static int testSuccess;
		public static TimeSpan totalTime;

		public CrossoutTest(int minLength)
		{
			testCount++;
			GeneratePatterns();
			testSequence = GenerateTestString(minLength, out expectedCrossoutsNumber, out expectedRemainderNumber);
		}
		public void RunTest()
		{
			testNumber++;
			bool actualErasable;
			int actualCrossoutsNumber, actualRemainderNumber;
			DateTime begin;
			TimeSpan elapsed;

			PrintTestInfo();
#if DEBUG
			Console.WriteLine(new string(testSequence));
#endif
			PrintMessage("Checking if sequence is erasable and getting crossout number...");
			begin = DateTime.Now;
			try
			{
				actualErasable = CrossoutChecker.Erasable(testSequence, testPatterns, out actualCrossoutsNumber);
			}
			catch (Exception ex)
			{
				PrintImportant(String.Format("Unexpected exception: {0}", ex.Message));
				return;
			}
			elapsed = DateTime.Now - begin;
			PrintMessage(String.Format("Elapsed time: {0}", elapsed));

			PrintMessage("Getting minimum remainder number...");
			begin = DateTime.Now;
			try
			{
				actualRemainderNumber = CrossoutChecker.MinimumRemainder(testSequence, testPatterns);
			}
			catch (Exception ex)
			{
				PrintImportant(String.Format("Unexpected exception: {0}", ex.Message));
				return;
			}
			elapsed = DateTime.Now - begin;
			totalTime += elapsed;
			PrintMessage(String.Format("Elapsed time: {0}", elapsed));

			try
			{
				CompareResult(expectedErasable, actualErasable, "Erasable");
				CompareResult(expectedCrossoutsNumber, actualCrossoutsNumber, "Crossout number");
				CompareResult(expectedRemainderNumber, actualRemainderNumber, "Remainder number");
			}
			catch (Exception ex)
			{
				PrintImportant(ex.Message);
				Console.WriteLine();
				return;
			}
			PrintImportant("Test passed successfully");
			testSuccess++;
			Console.WriteLine();
		}
		private void PrintTestInfo()
		{
			Console.WriteLine("Test {0}: {1}", testNumber, testName);
		}
		private void PrintMessage(string message)
		{
			Console.WriteLine("* {0}", message);
		}
		private void PrintImportant(string message)
		{
			Console.WriteLine("! {0}", message);
		}
		private void CompareResult<T>(T expected, T actual, string comparedName)
			where T : IEquatable<T>
		{
			if (!expected.Equals(actual))
				throw new Exception(String.Format("{0} mismatch: expected {1}, got {2}",
					comparedName, expected, actual));
			else
				PrintMessage(String.Format("{0} result: expected {1}, got {2}",
					comparedName, expected, actual));
		}

		protected abstract char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber);
		protected void GeneratePatterns()
		{
			List<char[]> patternsList = new List<char[]>();
			List<char> patternBuilder = new List<char>();

			for (char i = 'a'; i <= 'z'; i++)
			{
				patternBuilder.Add(i);
				patternsList.Add(patternBuilder.ToArray());
				patternBuilder.Clear();
			}
			for (char i = 'A'; i <= 'Z'; i++)
			{
				patternBuilder.Add(i);
				patternBuilder.Add(i);
				patternsList.Add(patternBuilder.ToArray());
				patternBuilder.Clear();
			}
			testPatterns = patternsList.ToArray();
		}
	}

	class CrossoutTestMain
	{
		static CrossoutTest[] tests =
		{
			new ZeroLengthCrossoutTest(0),
			new CrossoutAllConcatenateTest(5),
			new CrossoutAllConcatenateTest(10),
			new CrossoutAllConcatenateTest(25),
			new CrossoutAllConcatenateTest(50),
			new CrossoutAllConcatenateTest(100),
			new CrossoutAllConcatenateTest(250),
			new CrossoutAllConcatenateTest(500),
			new CrossoutAllConcatenateTest(1000),
			new CrossoutAllInsertTest(5),
			new CrossoutAllInsertTest(10),
			new CrossoutAllInsertTest(25),
			new CrossoutAllInsertTest(50),
			new CrossoutAllInsertTest(100),
			new CrossoutAllInsertTest(250),
			new CrossoutAllInsertTest(500),
			new CrossoutAllInsertTest(1000),
			new CrossoutAllAmbiguousTest(50),
			new CrossoutAllAmbiguousTest(100),
			new CrossoutAllAmbiguousTest(250),
			new CrossoutAllAmbiguousTest(500),
			new CrossoutAllAmbiguousTest(1000),
			new CrossoutPartConcatenateTest(5),
			new CrossoutPartConcatenateTest(10),
			new CrossoutPartConcatenateTest(25),
			new CrossoutPartConcatenateTest(50),
			new CrossoutPartConcatenateTest(100),
			new CrossoutPartConcatenateTest(250),
			new CrossoutPartConcatenateTest(500),
			new CrossoutPartConcatenateTest(1000),
			new CrossoutPartWrapTest(50),
			new CrossoutPartWrapTest(100),
			new CrossoutPartWrapTest(100),
			new CrossoutPartWrapTest(500),
			new CrossoutPartWrapTest(1000)
		};

		public static void Main()
		{
			foreach (CrossoutTest test in tests)
				test.RunTest();
			Console.WriteLine("Result: {0} out of {1} tests successful; time taken: {2}",
				CrossoutTest.testSuccess,
				CrossoutTest.testCount,
				CrossoutTest.totalTime);
		}
	}

	class ZeroLengthCrossoutTest : CrossoutTest
	{
		protected override bool expectedErasable => true;
		protected override string testName => "Zero length string";

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			testPatterns = null;
			crossoutsNumber = 0;
			remainderNumber = 0;
			return null;
		}

		public ZeroLengthCrossoutTest(int minLength) : base(minLength)
		{
			if (minLength != 0)
				throw new NotSupportedException("This test does not support lengths other than zero");
		}
	}

	class CrossoutAllConcatenateTest : CrossoutTest
	{
		protected override bool expectedErasable => true;
		protected override string testName => "Whole sequence can be crossed out, concatenation only";

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			Random rng = new Random(1087 * testCount);
			List<char> sequenceList = new List<char>();
			crossoutsNumber = 0;
			remainderNumber = 0;
			while (sequenceList.Count < minLength)
			{
				int next = rng.Next() % testPatterns.Length;
				if (rng.NextDouble() > 0.5)
					sequenceList.AddRange(testPatterns[next]);
				else
					sequenceList.InsertRange(0, testPatterns[next]);
				crossoutsNumber++;
			}
			return sequenceList.ToArray();
		}

		public CrossoutAllConcatenateTest(int minLength) : base(minLength) { }
	}

	class CrossoutAllInsertTest : CrossoutTest
	{
		protected override bool expectedErasable => true;
		protected override string testName => "Whole sequence can be crossed out, random insertion";

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			Random rng = new Random(2029 * testCount);
			List<char> sequenceList = new List<char>();
			crossoutsNumber = 0;
			remainderNumber = 0;
			while (sequenceList.Count < minLength)
			{
				int next = rng.Next() % testPatterns.Length;
				int insertAt = sequenceList.Count == 0 ? 0 : rng.Next() % sequenceList.Count;
				sequenceList.InsertRange(insertAt, testPatterns[next]);
				crossoutsNumber++;
			}
			return sequenceList.ToArray();
		}

		public CrossoutAllInsertTest(int minLength) : base(minLength) { }
	}

	class CrossoutAllAmbiguousTest : CrossoutTest
	{
		protected override bool expectedErasable => true;
		protected override string testName => "Whole sequence can be crossed out, ambiguous pattern sequences may occur (such as A and AA)";

		protected List<char> ambiguousChars;

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			PrepareAmbiguousPatterns();
			Random rng = new Random(3167 * testCount);
			List<char> sequenceList = new List<char>();
			List<char> ambiguousLeft = new List<char>(ambiguousChars);
			crossoutsNumber = 0;
			remainderNumber = 0;
			while (sequenceList.Count < minLength)
			{
				if (rng.NextDouble() > 0.2 || ambiguousLeft.Count == 0)
				{
					int next = rng.Next() % testPatterns.Length;
					int insertAt = sequenceList.Count == 0 ? 0 : rng.Next() % sequenceList.Count;
					sequenceList.InsertRange(insertAt, testPatterns[next]);
					crossoutsNumber++;
				}
				else
				{
					int next = rng.Next() % ambiguousLeft.Count;
					int count = rng.Next() % 10;
					int insertAt = sequenceList.Count == 0 ? 0 : rng.Next() % sequenceList.Count;
					for (int i = 0; i < count; i++)
						sequenceList.Insert(insertAt, ambiguousLeft[next]);
					crossoutsNumber = crossoutsNumber + (int)Math.Ceiling(count / 2.0);
					ambiguousLeft.RemoveAt(next);
				}
			}
			AddAmbiguousPatterns();
			return sequenceList.ToArray();
		}

		public CrossoutAllAmbiguousTest(int minLength) : base(minLength) { }

		protected void PrepareAmbiguousPatterns()
		{
			ambiguousChars = new List<char>();
			for (char i = ' '; i <= '@'; i++)
				ambiguousChars.Add(i);
		}

		protected void AddAmbiguousPatterns()
		{
			List<char[]> patternTemp = new List<char[]>(testPatterns);
			List<char> patternBuilder = new List<char>();
			foreach (char p in ambiguousChars)
			{
				patternBuilder.Add(p);
				patternTemp.Add(patternBuilder.ToArray());
				patternBuilder.Add(p);
				patternTemp.Add(patternBuilder.ToArray());
				patternBuilder.Clear();
			}
			testPatterns = patternTemp.ToArray();
		}
	}

	class CrossoutPartConcatenateTest : CrossoutTest
	{
		protected List<char> forbiddenChars;

		protected override bool expectedErasable => false;
		protected override string testName => "Whole sequence cannot be crossed out, concatenation only";

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			GenerateForbiddenPatterns();
			Random rng = new Random(4051 * testCount);
			List<char> sequenceList = new List<char>();
			crossoutsNumber = 0;
			remainderNumber = 0;
			bool firstForbiddenIn = false;
			while (sequenceList.Count < minLength)
			{
				if (firstForbiddenIn && rng.NextDouble() > 0.5)
				{
					// pattern
					int next = rng.Next() % testPatterns.Length;
					if (rng.NextDouble() > 0.5)
						sequenceList.AddRange(testPatterns[next]);
					else
						sequenceList.InsertRange(0, testPatterns[next]);
				}
				else
				{
					// non-pattern
					int next = rng.Next() % forbiddenChars.Count;
					if (rng.NextDouble() > 0.5)
						sequenceList.Add(forbiddenChars[next]);
					else
						sequenceList.Insert(0, forbiddenChars[next]);
					remainderNumber++;
					firstForbiddenIn = true;
				}
			}
			return sequenceList.ToArray();
		}

		public CrossoutPartConcatenateTest(int minLength) : base(minLength)
		{
			expectedCrossoutsNumber = int.MaxValue;
		}

		protected void GenerateForbiddenPatterns()
		{
			forbiddenChars = new List<char>();
			for (char i = ' '; i <= '@'; i++)
				forbiddenChars.Add(i);
		}
	}

	class CrossoutPartWrapTest : CrossoutPartConcatenateTest
	{
		protected override string testName => "Whole sequence cannot be crossed out, some two-character patterns may wrap around parts that cannot be crossed out";

		protected override char[] GenerateTestString(int minLength, out int crossoutsNumber, out int remainderNumber)
		{
			GenerateForbiddenPatterns();
			Random rng = new Random(5227 * testCount);
			List<char> sequenceList = new List<char>();
			List<char[]> unusedPatterns = new List<char[]>(testPatterns);
			crossoutsNumber = 0;
			remainderNumber = 0;
			bool firstForbiddenIn = false;
			while (sequenceList.Count < minLength)
			{
				if (firstForbiddenIn && rng.NextDouble() > 0.5)
				{
					// pattern
					int next = rng.Next() % unusedPatterns.Count;
					if (unusedPatterns[next].Length == 2)
					{
						if (sequenceList.Intersect(forbiddenChars).Count() != 0)
							remainderNumber += 2;
						sequenceList.Insert(0, unusedPatterns[next][0]);
						sequenceList.Add(unusedPatterns[next][1]);
						unusedPatterns.RemoveAt(next);
						continue;
					}
					if (rng.NextDouble() > 0.5)
						sequenceList.AddRange(unusedPatterns[next]);
					else
						sequenceList.InsertRange(0, unusedPatterns[next]);
				}
				else
				{
					// non-pattern
					int next = rng.Next() % forbiddenChars.Count;
					if (rng.NextDouble() > 0.5)
						sequenceList.Add(forbiddenChars[next]);
					else
						sequenceList.Insert(0, forbiddenChars[next]);
					remainderNumber++;
					firstForbiddenIn = true;
				}
			}
			return sequenceList.ToArray();
		}

		public CrossoutPartWrapTest(int minLength) : base(minLength) { }
	}
}