using System;
using System.Collections.Generic;

namespace AsdLab5
{
	public class InvalidExchangeException : Exception
	{
		public InvalidExchangeException()
		{
		}

		public InvalidExchangeException(string msg) : base(msg)
		{
		}

		public InvalidExchangeException(string msg, Exception ex) : base(msg, ex)
		{
		}
	}

	public struct ExchangePair
	{
		public readonly int From;
		public readonly int To;
		public readonly double Price;

		public ExchangePair(int from, int to, double price)
		{
			if (to < 0 || from < 0 || price <= 0.0)
				throw new InvalidExchangeException();
			From = from;
			To = to;
			Price = price;
		}
	}

	public class CurrencyGraph
	{
		private static double priceToWeight(double price)
		{
			return -Math.Log(price);
		}

		private static double weightToPrice(double weight)
		{
			return Math.Exp(-weight);
		}

		private double[,] weights;

		public CurrencyGraph(int n, ExchangePair[] exchanges)
		{
			weights = new double[n, n];
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
					weights[i, j] = double.MaxValue;
			if (exchanges == null)
				return;
			foreach (var ep in exchanges)
			{
				weights[ep.From, ep.To] = priceToWeight(ep.Price);
			}
		}

		// wynik: true jesli nie na cyklu ujemnego
		// currency: waluta "startowa"
		// bestPrices: najlepszy (najwyzszy) kurs wszystkich walut w stosunku do currency (byc mo¿e osiagalny za pomoca wielu wymian)
		// jesli wynik == false to bestPrices = null
		public bool findBestPrice(int currency, out double[] bestPrices)
		{
			//
			// wywolac odpowiednio FordBellmanShortestPaths
			// i na tej podstawie obliczyc bestPrices
			//
			bestPrices = null;
			double[] dist;
			int[] prev;
			if (!FordBellmanShortestPaths(currency, out dist, out prev))
				return false;
			int n = weights.GetLength(0);
			bestPrices = new double[n];

			for (int i = 0; i < n; i++)
			{
				if (i == currency)
				{
					bestPrices[i] = 1;
					continue;
				}
				if (dist[i] != double.MaxValue)
				{
					bestPrices[i] = weightToPrice(dist[i]);
				}
			}
			return true;
		}

		// wynik: true jesli jest mozliwosc arbitrazu, false jesli nie ma (nie rzucamy wyjatkow!)
		// currency: waluta "startowa"
		// exchangeCycle: a cycle of currencies starting from 'currency' and ending with 'currency'
		//  jesli wynik == false to exchangeCycle = null
		public bool findArbitrage(int currency, out int[] exchangeCycle)
		{
			//
			// Czêœæ 1: wywolac odpowiednio FordBellmanShortestPaths
			// Czêœæ 2: dodatkowo wywolac odpowiednio FindNegativeCostCycle
			//
			double[] dist;
			int[] prev;
			exchangeCycle = null;
			if (FordBellmanShortestPaths(currency, out dist, out prev))
				return false;
			FindNegativeCostCycle(dist, prev, out exchangeCycle);
			return true;
		}

		// wynik: true jesli nie na cyklu ujemnego
		// s: wierzcho³ek startowy
		// dist: obliczone odleglosci
		// prev: tablica "poprzednich"
		private bool FordBellmanShortestPaths(int s, out double[] dist, out int[] prev)
		{
			dist = null;
			prev = null;
			int n = weights.GetLength(0);   // liczba wierzcho³ków
			dist = new double[n];

			prev = new int[n];
			for (int i = 0; i < n; i++)
			{
				dist[i] = double.MaxValue;
				prev[i] = -1;
			}
			dist[s] = 0;

			int k = 0;
			bool zmiana = true;
			while (zmiana)
			{
				zmiana = false;
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < n; j++)
					{
						if (weights[i, j] == double.MaxValue) // nie ma tej krawedzi
							continue;
						if (dist[j] > dist[i] + weights[i, j])
						{
							zmiana = true;
							dist[j] = dist[i] + weights[i, j];
							prev[j] = i;
						}
					}
				}

				if (k > n - 1)
					return false;
				k++;
			}
			return true;
		}

		// wynik: true jesli JEST cykl ujemny
		// dist: tablica odleglosci
		// prev: tablica "poprzednich"
		// cycle: wyznaczony cykl (kolejne elementy to kolejne wierzcholki w cyklu, pierwszy i ostatni element musza byc takie same - zamkniêcie cyklu)
		private bool FindNegativeCostCycle(double[] dist, int[] prev, out int[] cycle)
		{
			cycle = null;
			//
			// wyznaczanie cyklu ujemnego
			// przykladowy pomysl na algorytm
			// 1) znajdowanie wierzcholka, którego odleglosc zostalaby poprawiona w kolejnej iteracji algorytmu Forda-Bellmana
			// 2) cofanie sie po lancuchu poprzednich (prev) - gdy zaczna sie powtarzac to znaleŸlismy wierzcholek nale¿acy do cyklu o ujemnej dlugosci
			// 3) konstruowanie odpowiedzi zgodnie z wymogami zadania
			//
			int n = weights.GetLength(0);
			int v = -1;
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (weights[i, j] == double.MaxValue) // nie ma tej krawedzi
						continue;
					if (dist[j] > dist[i] + weights[i, j])
					{
						// Jezeli mozna poprawic, to znaczy ze moze byc ujemny cykl
						// zaczynajacy sie w wierzcholku j: cofajac sie wrzucamy
						// wierzcholki na stos:
						v = j;
						Stack<int> myCycle = new Stack<int>();
						myCycle.Push(v);
						int u = prev[v];
						int k = 0;
						while (u != v && k < n)
						{
							myCycle.Push(u);
							u = prev[u];
							k++;
						}
						if (u != v)
							continue;
						myCycle.Push(v);
						cycle = myCycle.ToArray();
						return true;
					}
				}
			}

			return false;
		}
	}
}