﻿using System;
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

			return null;
        }

    }
}