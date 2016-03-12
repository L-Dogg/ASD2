
using System;

namespace lab4a
{
	public class Competition
	{
		//Wariant A (k-ty element)
		/// <summary>
		/// Znajduje zwycięzcę konkursu na najlepszą trzcinę
		/// </summary>
		/// <returns>
		/// Wysokość zwycięskiej trzciny
		/// </returns>
		/// <param name='arrays'>
		/// Tablice (skrzynki) biorące udział w konkursie. 
		/// </param>
		/// <param name='k'>
		/// k - parametr poszukiwania zwycięzcy z treści zadania
		/// </param>
		/// <param name='winnerArray'>
		/// Numer tablicy (skrzynki) w której znajduje się zwycięzca
		/// </param>
		/// <param name='winnerArrayPos'>
		/// Pozycja zwycięzcy w jego tablicy.
		/// </param>
		/// 
		public static int FindWinner (int[][] arrays, int k, out int winnerArray, out int winnerArrayPos)
		{
			int[] start = new int[arrays.GetLength(0)];			// Od którego elementu rozważamy i-tą tablicę
			int[] arrSize = new int[arrays.GetLength(0)];		// Ile elementów rozważamy w i-tej tablicy
			bool[] arrPresent = new bool[arrays.GetLength(0)];	// Czy i-ta tablica jest rozwazana
			double c = 0;										// Ile mamy elementów w sumie
			int arrCount = arrays.GetLength(0);					// Ile mamy tablic

			//Początkowe wypełnienie tablic:
            for (int i = 0; i < arrays.GetLength(0); i++)
			{
				arrSize[i] = arrays[i].Length;
				c += arrSize[i];
				start[i] = 0;
				arrPresent[i] = true;
			}
			/*for (int i = 0; i < arrays.GetLength(0); i++)
			{
				foreach (var x in arrays[i])
					Console.Write("{0} ", x);
				Console.WriteLine();
			}*/
				while (arrCount > 1)
			{
				//Console.WriteLine("k = {0}, c/2 = {1}", k, c / 2);

				if (k == 0)
				{
					int min = 0, minVal = int.MaxValue;
					for (int i = 0; i < arrays.GetLength(0); i++)
					{
						if (arrPresent[i])  
						{
							if(arrays[i][start[i]] < minVal)
							{
								min = i;
								minVal = arrays[i][start[i]];
                            }
						}
					}
					winnerArray = min;
					winnerArrayPos = start[winnerArray];
					return arrays[winnerArray][winnerArrayPos];
				}
				// Jeśli k-ta liczba jest mniejsza od mediany całego ciągu, to na pewno
				// jest mniejsza od największej z median górnych pojedynczych tablic
				if ((double)k < c / 2)
				{
					//Console.WriteLine("Mediany gorne");
					int max = 0;			// Indeks max mediany gornej
					int j = 0;              // Tablica w ktorej jest ta mediana
					// Znajdowanie max mediany:
					for (int i = 0; i < arrays.GetLength(0); i++)
					{
						if(arrPresent[i])   // Tablica nie zostala usunieta
						{
							int gorMed = gornaMediana(start[i], arrSize[i]);
							if (arrays[i][gorMed] > arrays[j][max])
							{
								j = i;
								max = gorMed;
							}
						}
					} // for
					//Console.WriteLine("Mediana = {0} i jest w tablicy nr {1}", arrays[j][max], j);
					if (arrSize[j] == 1)
					{
						arrCount--;
						arrPresent[j] = false;
						c--;
						continue;
					}
					c -= (arrSize[j] - (max - start[j]));
					arrSize[j] = max - start[j];
					//Console.WriteLine("Modyfikujemy {0}-ta tablice, c = {1}, k = {2}, arrSize[{0}] = {3}, start[{0}] = {4}", j, c, k, arrSize[j], start[j]);
				} // k < c

				// Jeśli k-ta liczba jest większa od mediany całego ciągu, to na pewno
				// jest większa od najmniejszej z median dolnych pojedynczych tablic
				else
				{
					//Console.WriteLine("Mediany dolne");
					int valMin = int.MaxValue;
					int min = start[0] + arrSize[0] - 1; // Indeks max mediany gornej
					int j = 0;							 // Tablica w ktorej jest ta mediana
					// Znajdowanie max mediany:
					for (int i = 0; i < arrays.GetLength(0); i++)
					{
						if (arrPresent[i])	// Tablica nie zostala usunieta
						{
							int dolMed = dolnaMediana(start[i], arrSize[i]);
							if (arrays[i][dolMed] <= valMin)
							{
								j = i;
								min = dolMed;
								valMin = arrays[j][min];
                            }
						}
					} // for
					//Console.WriteLine("Mediana = {0} i jest w tablicy nr {1}", arrays[j][min], j);
					// Jeżeli znaleziona tablica ma jeden element, to ją usuwamy
					if (arrSize[j] == 1)
					{
						arrCount--;
						arrPresent[j] = false;
						c--;
						k--;
						continue;
					}
					c -= (min - start[j] + 1);
					k -= (min - start[j] + 1);
					arrSize[j] -= (min - start[j] + 1);
					start[j] = min + 1;
					//Console.WriteLine("Modyfikujemy {0}-ta tablice, c = {1}, k = {2}, arrSize[{0}] = {3}, start[{0}] = {4}", j, c, k, arrSize[j], start[j]);
				} // k >= c
			}//while

			// Została jedna tablica:
			int u = 0;
			for(int i = 0; i < arrays.GetLength(0); i++)
				if(arrPresent[i])
				{
					u = i;
					break;
				}

			winnerArray = u;
			winnerArrayPos = arrSize[u] / 2 + start[u];
			//Console.WriteLine();
            return arrays[winnerArray][winnerArrayPos];
		}
		//można dopisać własne metody
		public static int dolnaMediana(int start, int arrSize)
		{
			if (arrSize == 1)
				return start;

			if (arrSize % 2 == 0)
				return (arrSize - 1)/ 2 + start;
			return arrSize / 2 + start;
		}
		public static int gornaMediana(int start, int arrSize)
		{
			if (arrSize == 1)
				return start;

			if (arrSize % 2 == 0)
				return arrSize / 2 + start;
			return arrSize / 2 + 1 + start;
		}
		
	}
}

