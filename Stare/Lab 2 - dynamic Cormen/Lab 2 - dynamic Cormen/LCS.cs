using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2___dynamic_Cormen
{
	class LCS
	{
		public enum next {UpLeft, Up, Left};
		private int[,] c;
		private next[,] b;

		//Dlugosc najdluzszego wspolnego podciagu:
		public int LCS_Lenght(string x, string y)
		{
			int m = x.Length;
			int n = y.Length;

			string u = prepareString(x);
			string w = prepareString(y);

			c = new int[m+1, n+1];
			b = new next[m+1, n+1];

			for (int i = 1; i <= m; i++)
				c[i, 0] = 0;
			for (int j = 0; j <= n; j++)
				c[0, j] = 0;

			for(int i = 1; i <= m; i++)
			{
				for(int j = 1; j <= n; j++)
				{
					if(u[i] == w[j])
					{
						c[i, j] = c[i - 1, j - 1] + 1;
						b[i, j] = next.UpLeft;
					}
					else if (c[i - 1, j] >= c[i, j-1])
					{
						c[i, j] = c[i - 1, j];
						b[i, j] = next.Up;
					}
					else
					{
						c[i, j] = c[i, j - 1];
						b[i, j] = next.Left;
					}
				}
			}

			return c[m - 1, n - 1];
		}

		public void printLCS(string x, string y)
		{
			int m = x.Length;
			int n = y.Length;
			string u = prepareString(x);
			string w = prepareString(y);

			print(u, b, m, n);
		}

		private void print(string x, next[,] b, int i, int j)
		{
			if (i == 0 || j == 0)
			{
				return;
			}
			if (b[i, j] == next.UpLeft)
			{
				print(x, b, i - 1, j - 1);
				Console.Write(x[i]);
			}
			else if (b[i, j] == next.Up)
			{
				print(x, b, i - 1, j);
			}
			else
			{
				print(x, b, i, j - 1);
			}
		}

		private string prepareString(string s)
		{
			return "A" + s;
		}
	}
}
