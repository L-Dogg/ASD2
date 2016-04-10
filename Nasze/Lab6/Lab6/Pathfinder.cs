using System;
using System.Linq;
using ASD.Graphs;

namespace lab06
{
    class Pathfinder
    {
        Graph RoadsGraph;
        int[] CityCosts;

        public Pathfinder(Graph roads, int[] cityCosts)
        {
            RoadsGraph = roads;
            CityCosts = cityCosts;
        }

        //uwagi do wszystkich części (graf najkrótszych ścieżek)
        //   Jeżeli nie ma ścieżki pomiędzy miastami A i B to tworzymy sztuczną krawędź od A do B o karnym koszcie 10 000.

        // return: tablica kosztów organizacji ŚDM dla poszczególnym miast gdzie
        // za koszt organizacji ŚDM uznajemy sumę kosztów dostania się ze wszystkim miast do danego miasta, bez uwzględnienia kosztów przechodzenia przez miasta.
        // minCost: najmniejszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocationWithoutCityCosts(out int minCost, out Graph paths)
        {
			const int noEdge = 10000;
            minCost = -1;
			paths = RoadsGraph.IsolatedVerticesGraph(true, RoadsGraph.VerticesCount);
			int[] ret = new int[RoadsGraph.VerticesCount];

			PathsInfo[] bestPaths = null;
			PathsInfo[] curPaths;
			int curCost = 0;
            int bestCost = int.MaxValue;
			int bestCity = 0;

			for(int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				curCost = 0;
				
				RoadsGraph.DijkstraShortestPaths(i, out curPaths);
				
				foreach (var p in curPaths)
				{
					if (p.Dist == null)
						curCost += noEdge;
					else
						curCost += p.Dist.Value;
				}

				if(curCost < bestCost)
				{
					bestCity = i;
					bestCost = curCost;
					bestPaths = curPaths;
				}
				ret[i] = curCost;
				
			}

			// konstrukcja grafu:
			
			for(int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				if (i == bestCity)
					continue;

				if (bestPaths[i].Dist == null)
					paths.AddEdge(i, bestCity, noEdge);
				else
				{
					int j = i;
					while (j != bestCity)
					{
						paths.AddEdge(bestPaths[j].Last.Value.To, bestPaths[j].Last.Value.From, bestPaths[j].Last.Value.Weight);
						j = bestPaths[j].Last.Value.From;
					}
				}
					
			}

			minCost = bestCost;
			return ret;
        }

        // return: tak jak w punkcie poprzednim, ale tym razem
        // za koszt organizacji ŚDM uznajemy sumę kosztów dostania się ze wszystkim miast do wskazanego miasta z uwzględnieniem kosztów przechodzenia przez miasta (cityCosts[]).
        // Nie uwzględniamy kosztu przejścia przez miasto które organizuje ŚDM.
        // minCost: najlepszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocation(out int minCost, out Graph paths)
        {
			const int noEdge = 10000;
			minCost = -1;
			paths = RoadsGraph.IsolatedVerticesGraph(true, RoadsGraph.VerticesCount);
			int[] ret = new int[RoadsGraph.VerticesCount];

			PathsInfo[] bestPaths = null;
			PathsInfo[] curPaths;
			int curCost = 0;
			int bestCost = int.MaxValue;
			int bestCity = 0;

			var tmp = RoadsGraph.IsolatedVerticesGraph(true, RoadsGraph.VerticesCount);
			
			for(int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				foreach(Edge e in RoadsGraph.OutEdges(i))
				{
					tmp.AddEdge(e.From, e.To, e.Weight + CityCosts[e.To]);
				}
			}

			for (int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				curCost = 0;

				tmp.DijkstraShortestPaths(i, out curPaths);

				foreach (var p in curPaths)
				{
					if (p.Dist == null)
						curCost += noEdge;
					else
						curCost += p.Dist.Value;
				}

				if (curCost < bestCost)
				{
					bestCity = i;
					bestCost = curCost;
					bestPaths = curPaths;
				}
				ret[i] = curCost;

			}

			// konstrukcja grafu:

			for (int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				if (i == bestCity)
					continue;

				if (bestPaths[i].Dist == null)
					paths.AddEdge(i, bestCity, noEdge);
				else
				{
					int j = i;
					while (j != bestCity)
					{
						paths.AddEdge(bestPaths[j].Last.Value.To, bestPaths[j].Last.Value.From, 
							RoadsGraph.GetEdgeWeight(bestPaths[j].Last.Value.To, bestPaths[j].Last.Value.From).Value);
						j = bestPaths[j].Last.Value.From;
					}
				}

			}
			
			minCost = bestCost;
			return ret;
		}

        // return: tak jak w punkcie poprzednim, ale tym razem uznajemy zarówno koszt przechodzenia przez miasta, jak i wielkość miasta startowego z którego wyruszają pielgrzymi.
        // Szczegółowo opisane jest to w treści zadania "Częśc 2". 
        // minCost: najlepszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocationSecondMetric(out int minCost, out Graph paths)
        {
			const int noEdge = 10000;
			minCost = -1;
			paths = RoadsGraph.IsolatedVerticesGraph(true, RoadsGraph.VerticesCount);
			int[] ret = new int[RoadsGraph.VerticesCount];

			//new GraphExport().Export(RoadsGraph);

			PathsInfo[] bestPaths = null;
			PathsInfo[] curPaths;
			PathsInfo[][] allPaths = new PathsInfo[RoadsGraph.VerticesCount][];

			int[] costSum = new int[RoadsGraph.VerticesCount];
			int[] costCur = new int[RoadsGraph.VerticesCount];

			// Dla każdego wierzchołka tworzymy nowy graf, liczymy koszty dojścia
			// z tego wierzchołka do innych i sumujemy wyniki z resztą w costSum
			for (int v = 0; v < RoadsGraph.VerticesCount; v++)
			{
				// Utworzenie grafu ze zmodyfikowanymi wagami:
				var tmp = RoadsGraph.IsolatedVerticesGraph(true, RoadsGraph.VerticesCount);
				for (int i = 0; i < RoadsGraph.VerticesCount; i++)
				{
					foreach (Edge e in RoadsGraph.OutEdges(i))
					{
						tmp.AddEdge(e.From, e.To, Math.Min(e.Weight, CityCosts[v]) + Math.Min(CityCosts[e.From], CityCosts[v]));
					}
				}

				
				for (int j = 0; j < RoadsGraph.VerticesCount; j++)
				{
					costCur[j] = 0;
				}

				// Obliczenie kosztów w nowym grafie:
				tmp.DijkstraShortestPaths(v, out curPaths);

				allPaths[v] = curPaths;

				for (int j = 0; j < RoadsGraph.VerticesCount; j++)
				{
					if (j == v)
						continue;
					if (curPaths[j].Dist == null)
						costCur[j] += noEdge;
					else
						costCur[j] += curPaths[j].Dist.Value;
				}


				// Dodanie do reszty kosztów:
				for (int i = 0; i < RoadsGraph.VerticesCount; i++)
				{
					if (i == v)
						continue;
					costSum[i] += costCur[i];
				}
			}

			int bestCost = int.MaxValue;
			int bestCity = 0;

			for(int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				if(costSum[i] < bestCost)
				{
					bestCost = costSum[i];
					bestCity = i;
				}
			}
			bestPaths = allPaths[bestCity];
			
			// konstrukcja grafu:

			for (int i = 0; i < RoadsGraph.VerticesCount; i++)
			{
				if (i == bestCity)
					continue;

				if (bestPaths[i].Dist == null)
					paths.AddEdge(i, bestCity, noEdge);
				else
				{
					int j = i;
					while (j != bestCity)
					{
						paths.AddEdge(bestPaths[j].Last.Value.To, bestPaths[j].Last.Value.From,
							RoadsGraph.GetEdgeWeight(bestPaths[j].Last.Value.To, bestPaths[j].Last.Value.From).Value);
						j = bestPaths[j].Last.Value.From;
					}
				}

			}

			minCost = bestCost;
			return costSum;
        }

    }
}
