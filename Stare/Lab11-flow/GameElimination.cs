namespace GameElimination
{
    using System;

    using ASD.Graphs;

    public partial class Program
    {
        /// <summary>
        /// Procedura określająca czy drużyna jest wyeliminowana z rozgrywek
        /// </summary>
        /// <param name="teamId">indeks drużyny do sprawdzenia</param>
        /// <param name="teams">lista zespołów</param>
        /// <param name="predictedResults">wyniki gwarantujące zwycięstwo sprawdzanej drużyny</param>
        /// <returns></returns>
        public static bool IsTeamEliminated(int teamId, Team[] teams, out int[,] predictedResults)
        {
            predictedResults = null;
			int n = teams.Length;

			Graph g = new AdjacencyListsGraph<SimplyAdjacencyList>(true, (n * (n-1))/2 + n + 2);
			int s = g.VerticesCount - 2;
			int t = g.VerticesCount - 1;

			// krawędzie drużyna -> ujście
			for (int i = 0; i < n; i++)
				g.AddEdge(i, t, teams[teamId].NumberOfWins + teams[teamId].NumberOfGamesToPlay - teams[i].NumberOfWins);

			int v = n;
			for(int i = 0; i < n; i++)
			{
				for (int j = i; j < n; j++)
				{
					g.AddEdge(s, v, teams[i].NumberOfGamesToPlayByTeam[j]);
					g.AddEdge(v, i, int.MaxValue);
					g.AddEdge(v, j, int.MaxValue);
                    v++;
                }
			}
			Graph flow;
			g.FordFulkersonMaxFlow(s, t, out flow);

			foreach(Edge e in flow.OutEdges(s))
			{
				if (e.Weight < g.GetEdgeWeight(s, e.To).Value)
					return false;
			}

            return true;
        }
    }
}
