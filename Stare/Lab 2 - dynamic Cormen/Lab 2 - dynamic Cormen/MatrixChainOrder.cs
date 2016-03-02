using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2___dynamic_Cormen
{
	class MatrixChainOrder
	{
		//matrix Ai (i=1...n): dim[i-1] x dim[i]
		private int n;
		private int[,] m, s;

		//Koszt wymnozenia macierzy
		public int MatrixChain(int[] dim)
		{
			n = dim.Length - 1;
			m = new int[n+1, n+1];
			s = new int[n+1, n+1];
			for (int i = 1; i <= n; i++)
				m[i, i] = 0;

			int q, j;
			for(int l = 2; l <= n; l++)	//liczba macierzy w ciagu
			{
				for(int i = 1; i <= n - l + 1; i++)
				{
					j = i + l - 1;
					m[i, j] = int.MaxValue;
					for(int k = i; k <= j - 1; k++)
					{
						q = m[i, k] + m[k + 1, j] + dim[i - 1] * dim[k] * dim[j];
						if (q < m[i, j])
						{
							m[i, j] = q;
							s[i, j] = k;
						}
					}
				}
			}
			return m[1, n];
		}
		//Wydruk nawiasowania:
		public void printParens(int i, int j)
		{
			if (i == j)
				Console.Write(" A_{0} ", i);
			else
			{
				Console.Write("(");
				printParens(i, s[i, j]);
				printParens(s[i, j] + 1, j);
				Console.Write(")");
			}
		}
	}
}
