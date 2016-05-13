using ASD.Graphs;
using System.Collections.Generic;

public static class BottleNeckExtender
{

    /// <summary>
    /// Wyszukiwanie "wąskich gardeł" w sieci przesyłowej
    /// </summary>
    /// <param name="g">Graf przepustowości krawędzi</param>
    /// <param name="c">Graf kosztów rozbudowy sieci (kosztów zwiększenia przepustowości)</param>
    /// <param name="p">Tablica mocy produkcyjnych/zapotrzebowania w poszczególnych węzłach</param>
    /// <param name="flowValue">Maksymalna osiągalna produkcja (parametr wyjściowy)</param>
    /// <param name="cost">Koszt rozbudowy sieci, aby możliwe było osiągnięcie produkcji flowValue (parametr wyjściowy)</param>
    /// <param name="flow">Graf przepływu dla produkcji flowValue (parametr wyjściowy)</param>
    /// <param name="ext">Tablica rozbudowywanych krawędzi (parametr wyjściowy)</param>
    /// <returns>
    /// 0 - zapotrzebowanie można zaspokoić bez konieczności zwiększania przepustowości krawędzi<br/>
    /// 1 - zapotrzebowanie można zaspokoić, ale trzeba zwiększyć przepustowość (niektórych) krawędzi<br/>
    /// 2 - zapotrzebowania nie można zaspokoić (zbyt małe moce produkcyjne lub nieodpowiednia struktura sieci
    ///     - można jedynie zwiększać przepustowości istniejących krawędzi, nie wolno dodawać nowych)
    /// </returns>
    /// <remarks>
    /// Każdy element tablicy p opisuje odpowiadający mu wierzchołek<br/>
    ///    wartość dodatnia oznacza moce produkcyjne (wierzchołek jest źródłem)<br/>
    ///    wartość ujemna oznacza zapotrzebowanie (wierzchołek jest ujściem),
    ///       oczywiście "możliwości pochłaniające" ujścia to moduł wartości elementu<br/>
    ///    "zwykłym" wierzchołkom odpowiada wartość 0 w tablicy p<br/>
    /// <br/>
    /// Jeśli funkcja zwraca 0, to<br/>
    ///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
    ///    parametr cost jest równy 0<br/>
    ///    parametr ext jest pustą (zeroelementową) tablicą<br/>
    /// Jeśli funkcja zwraca 1, to<br/>
    ///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
    ///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
    ///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
    /// Jeśli funkcja zwraca 2, to<br/>
    ///    parametr flowValue jest równy maksymalnej możliwej do osiągnięcia produkcji
    ///      (z uwzględnieniem zwiększenia przepustowości)<br/>
    ///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
    ///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
    /// Uwaga: parametr ext zawiera informacje jedynie o krawędziach, których przepustowości trzeba zwiększyć
    //     (każdy element tablicy to opis jednej takiej krawędzi)
    /// </remarks>
    public static int BottleNeck(this Graph g, Graph c, int[] p, out int flowValue, out int cost, out Graph flow, out Edge[] ext)
    {
		int n = g.VerticesCount;
		Graph helper = g.IsolatedVerticesGraph(true, 2 * n + 2);
		Graph helperCosts = g.IsolatedVerticesGraph(true, 2 * n + 2);
		int s = helper.VerticesCount - 1;
		int t = helper.VerticesCount - 2;
		int need = 0;

		for (int i = 0; i < n; i++)
		{
			if (p[i] > 0)
			{
				helper.AddEdge(s, i, p[i]);
				helperCosts.AddEdge(s, i, 0);
			}
			else if (p[i] < 0)
			{
				helper.AddEdge(i, t, -p[i]);
				helperCosts.AddEdge(i, t, 0);
				need -= p[i];
			}
		}

		for(int i = 0; i < n; i++)
		{
			if (g.OutDegree(i) == 0)
				continue;
			helper.AddEdge(i, i + n, int.MaxValue);
			helperCosts.AddEdge(i, i + n, 0);
			foreach (Edge e in g.OutEdges(i))
			{
				helper.AddEdge(e);
				helperCosts.AddEdge(e.From, e.To, 0);

				helper.AddEdge(i + n, e.To, int.MaxValue);
				helperCosts.AddEdge(i + n, e.To, c.GetEdgeWeight(e.From, e.To).Value);
			}
		}

		Graph helperFlow;
		int myCost;
		int myFlowValue = helper.MinCostFlow(helperCosts, s, t, out myCost, out helperFlow);


		flowValue = myFlowValue;
		cost = myCost;
		flow = g.IsolatedVerticesGraph(true, n);

		if (myCost == 0)
		{
			ext = new Edge[0];
			for(int i = 0; i < n; i++)
			{
				foreach(Edge e in helperFlow.OutEdges(i))
				{
					if (e.To == i + n || e.To == t)
						continue;
					flow.AddEdge(e);
				}
			}

			return 0;
		}

		int retVal = 1;
		List<Edge> myExt = new List<Edge>();
		GraphExport ge = new GraphExport();
	
		for (int i = 0; i < n; i++)
		{
			foreach (Edge e in helperFlow.OutEdges(i))
			{
				if (e.To == i + n || e.To == t)
					continue;

				int pathFlow = helperFlow.GetEdgeWeight(i + n, e.To).Value;
				if (pathFlow > 0)
				{
					myExt.Add(new Edge(e.From, e.To, pathFlow));
					flow.AddEdge(e.From, e.To, e.Weight + pathFlow);
				}
				else
				{
					flow.AddEdge(e);
				}
			}
		}
		ext = myExt.ToArray();
		if (flowValue >= need)
			return 1;
		return 2;
    }
}
