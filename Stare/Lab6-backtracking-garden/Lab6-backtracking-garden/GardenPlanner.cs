using ASD.Graphs;
using System.Collections.Generic;
using static System.Console;

namespace lab7
{
	public static class GardenPlannerExtender
	{

		/// <summary>
		/// Znajduje wszystkie optymalne ustawienia sza³asów w ogrodzie opisanym przez graf.
		/// </summary>
		/// <param name="graph">Graf reprezentuj¹cy ogród</param>
		/// <returns>Lista rozwi¹zañ. Ka¿de rozwi¹zanie to tablica zawieraj¹ca numery
		/// wierzcho³ków, w których stoj¹ sza³azy. Ka¿dy numej mo¿e wyst¹piæ co najwy¿ej raz.</returns>
		public static List<int[]> GardenPlanner(this Graph graph)
		{
			var gph = new GardenPlannerHelper(graph);
			return gph.Plan();
		}

		public static string Indent(int count)
		{
			return "".PadLeft(count);
		}

		private sealed class GardenPlannerHelper
		{
			private Graph graph;

			internal GardenPlannerHelper(Graph _graph)
			{
				graph = _graph;
			}

			internal List<int[]> Plan()
			{
				int n = graph.VerticesCount;
				int maxNoTestCount = 0;

				int[] tents = new int[n];
				bool[] noTent = new bool[n];
				List<int[]> tentsList = new List<int[]>();

				PlanRecurse(noTent, -1, 0, tentsList, ref maxNoTestCount);
				return tentsList;
			}

			internal void PlanRecurse(bool[] noTent, int lastNoTent, int noTentsCount, List<int[]> tentsList, ref int maxNoTentsCount)
			{
				int n = graph.VerticesCount;
				if (noTentsCount == n)
					return;

				for (int v = lastNoTent + 1; v < n; ++v)
				{
					if (TestTentSetting(noTent, v))
					{
						if (noTentsCount + 1 > maxNoTentsCount)
						{
							tentsList.Clear();
							maxNoTentsCount = noTentsCount + 1;
						}
						if (noTentsCount + 1 == maxNoTentsCount)
						{
							int j = 0;
							int[] arr = new int[n - noTentsCount - 1];
							for (int i = 0; i < n; ++i)
								if (!noTent[i])
									arr[j++] = i;
							tentsList.Add(arr);
						}
						PlanRecurse(noTent, v, noTentsCount + 1, tentsList, ref maxNoTentsCount);
						noTent[v] = false;
					}
				}
			}

			internal bool TestTentSetting(bool[] noTents, int del)
			{
				noTents[del] = true;
				foreach (Edge e in graph.OutEdges(del))
				{
					int u = e.To;
					if (!noTents[u])
						continue;

					foreach (Edge eu in graph.OutEdges(u))
						if (noTents[eu.To])
						{
							noTents[del] = false;
							return false;
						}
				}
				return true;
			}

		}
	}
	}



