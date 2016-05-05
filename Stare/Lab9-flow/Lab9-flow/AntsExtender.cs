using System;
using ASD.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace lab9
{
	public static class AntsExtender
	{

        /// <summary>
        /// Sprawdza czy istnieje kraw�d�, kt�rej dodanie/poszerzenie poprawi przep�yw zapas�w
        /// </summary>
        /// <param name="baseGraph">graf</param>
        /// <param name="sources">numery wierzcho�k�w - wej�� do mrowiska</param>
        /// <param name="destinations">numery wierzcho�k�w - magazyn�w</param>
        /// <param name="flowValue">warto�� przep�ywu przed rozbudow� mrowiska</param>
        /// <returns>kraw�d� o wadze 1, kt�r� nale�y doda� lub poszerzy� (zwracamy te� kraw�d�
        /// o wadze 1)</returns>
		public static Edge? ImprovementChecker (this Graph baseGraph, int[] sources, int[] destinations, out int flowValue)
		{
			flowValue=0;
			Graph flowGraph = baseGraph.IsolatedVerticesGraph(true, baseGraph.VerticesCount + 2);
			int s = baseGraph.VerticesCount;
			int t = baseGraph.VerticesCount + 1;
			for(int i = 0; i < baseGraph.VerticesCount; i++)
			{
				foreach(Edge e in baseGraph.OutEdges(i))
				{
					flowGraph.AddEdge(e);
				}

				for(int j = 0; j < sources.Length; j++)
				{
					if(sources[j] == i)
					{
						flowGraph.AddEdge(s, i, int.MaxValue);
					}
				}

				for (int j = 0; j < destinations.Length; j++)
				{
					if (destinations[j] == i)
					{
						flowGraph.AddEdge(i, t, int.MaxValue);
					}
				}
			}

			Graph flow;
			flowValue = flowGraph.FordFulkersonMaxFlow(s, t, out flow);

			return null;
		}

	}
}

