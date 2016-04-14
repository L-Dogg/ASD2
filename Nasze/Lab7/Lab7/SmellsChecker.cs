using System;
using static System.Console;

namespace Lab07
{

    public class SmellsChecker
    {

        private readonly int smellCount;
        private readonly int[][] customerPreferences;
        private readonly int satisfactionLevel;
		private readonly int _ileKlientow;
        /// <summary>
        ///   
        /// </summary>
        /// <param name="smellCount">Liczba zapachów, którymi dysponuje sklep</param>
        /// <param name="customerPreferences">Preferencje klientów
        /// Każda tablica -- element tablicy tablic -- to preferencje jednego klienta.
        /// Preferencje klienta mają długość smellCount, na i-tej pozycji jest
        ///  1 -- klient preferuje zapach
        ///  0 -- zapach neutralny
        /// -1 -- klient nie lubi zapachu
        /// 
        /// Zapachy numerujemy od 0
        /// </param>
        /// <param name="satisfactionLevel">Oczekiwany poziom satysfakcji</param>
        public SmellsChecker(int smellCount, int[][] customerPreferences, int satisfactionLevel)
        {
            this.smellCount = smellCount;
            this.customerPreferences = customerPreferences;
            this.satisfactionLevel = satisfactionLevel;
			_ileKlientow = customerPreferences.GetLength(0);
		}

		/// <summary>
		/// Implementacja etapu 1
		/// </summary>
		/// <returns><c>true</c>, jeśli przypisanie jest możliwe <c>false</c> w p.p.</returns>
		/// <param name="smells">Wyjściowa tablica rozpylonych zapachów realizująca rozwiązanie, jeśli się da. null w p.p. </param>

		private bool _sukces = true;
		private int[] _poziomUsatysf;
		private bool _canFinish = true;

		public Boolean AssignSmells(out bool[] smells)
        {
			_canFinish = true;
			_sukces = true;
            smells = new bool[smellCount];
			_poziomUsatysf = new int[_ileKlientow];

			backtrackingHelper(0, -1, ref smells);

			if (_sukces == false)
				smells = null;
			return _sukces;
        }

		public void backtrackingHelper(int iter, int last, ref bool[] smells)
		{
			if(iter == smellCount)
			{
				return;
			}

			for (int i = last+1; i < smellCount; i++)
			{
				if (smells[i] == true)
					continue;
				
				if (smells[i] == false)
				{
					_sukces = true;
					for (int j = 0; j < _ileKlientow; j++)
					{
						_poziomUsatysf[j] += customerPreferences[j][i];
						if (_sukces)
						{
							if (_poziomUsatysf[j] < satisfactionLevel)
							{
								_sukces = false;
								if (satisfactionLevel - _poziomUsatysf[j] > smellCount - iter)
								{
									_canFinish = false;
									break;
								}
							}
						}

					}

					if (_canFinish == false)
					{
						_sukces = false;
						return;
					}
					
					smells[i] = true;
					if (_sukces)
					{
						return;
					}
					backtrackingHelper(iter + 1, i, ref smells);

					if (_sukces)
						return;
					if(_canFinish == false)
					{
						_sukces = false;
						return;
					}
					smells[i] = false;
					for (int j = 0; j < _ileKlientow; j++)
					{
						_poziomUsatysf[j] -= customerPreferences[j][i];
					}
				}
			}

			
		}

		/// <summary>
		/// Implementacja etapu 2
		/// </summary>
		/// <returns>Maksymalna liczba klientów, których można usatysfakcjonować</returns>
		/// <param name="smells">Wyjściowa tablica rozpylonych zapachów, realizująca ten poziom satysfakcji</param>

		private int _maxCust = 0;
		private bool[] bestSmell;
		private int _zadowoleni = 0;
        public int AssignSmellsMaximizeHappyCustomers(out bool[] smells)
        {
			_poziomUsatysf = new int[_ileKlientow];
			bestSmell = new bool[smellCount];
            smells = new bool[smellCount];
			backtrackingHelperDwa(0, -1, ref smells);
			if (_sukces == false)
			{
				smells = bestSmell;
			}
            return _maxCust;
        }
		public void backtrackingHelperDwa(int iter, int last, ref bool[] smells)
		{
			_zadowoleni = 0;
			_sukces = true;
			for (int i = 0; i < _ileKlientow; i++)
			{
				if (_poziomUsatysf[i] < satisfactionLevel)
				{
					_sukces = false;
				}
				else
				{
					_zadowoleni++;
				}
			}
			if (_zadowoleni > _maxCust)
			{
				Array.Copy(smells, bestSmell, smellCount);
				_maxCust = _zadowoleni;
			}

			if (_sukces == true)
			{
				_maxCust = _ileKlientow;
				Array.Copy(smells, bestSmell, smellCount);
				return;
			}

			if (iter == smellCount)
			{
				return;
			}

			for (int i = 0; i < smellCount; i++)
			{
				if (smells[i] == true)
					continue;
				if (smells[i] == false && i > last)
				{
					for (int j = 0; j < _ileKlientow; j++)
					{
						_poziomUsatysf[j] += customerPreferences[j][i];
					}
					smells[i] = true;
					backtrackingHelperDwa(iter + 1, i, ref smells);
					if (_sukces)
						return;
					if (_canFinish == false)
					{
						_sukces = false;
						return;
					}
					smells[i] = false;
					for (int j = 0; j < _ileKlientow; j++)
					{
						_poziomUsatysf[j] -= customerPreferences[j][i];
					}
				}
			}


		}
	}

}

