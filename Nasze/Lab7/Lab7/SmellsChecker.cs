using System;
using static System.Console;

namespace Lab07
{

    public class SmellsChecker
    {

        private readonly int smellCount;
        private readonly int[][] customerPreferences;
        private readonly int satisfactionLevel;
		private readonly int _customerCount;
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
			_customerCount = customerPreferences.GetLength(0);
		}

		/// <summary>
		/// Implementacja etapu 1
		/// </summary>
		/// <returns><c>true</c>, jeśli przypisanie jest możliwe <c>false</c> w p.p.</returns>
		/// <param name="smells">Wyjściowa tablica rozpylonych zapachów realizująca rozwiązanie, jeśli się da. null w p.p. </param>

		private bool _success;
		private int[] _satLevel;
		private int[][] _onesAvailable;

		private void prepareHelpArray()
		{
			_onesAvailable = new int[_customerCount][];

			for (int i = 0; i < _customerCount; i++)
			{
				_onesAvailable[i] = new int[smellCount];
				for (int j = smellCount - 2; j >= 0; j--)
				{
					_onesAvailable[i][j] = _onesAvailable[i][j + 1];
					if (customerPreferences[i][j + 1] == 1)
					{
						_onesAvailable[i][j]++;
					}
				}
			}
		}

		public Boolean AssignSmells(out bool[] smells)
        {
			smells = new bool[smellCount];
			if (satisfactionLevel <= 0)
				return true;

			_success = false;
			_satLevel = new int[_customerCount];
			prepareHelpArray();
			backtrackingHelper(0, -1, ref smells);

			if (_success == false)
				smells = null;
			return _success;
        }

		public void backtrackingHelper(int iter, int last, ref bool[] smells)
		{
			if(iter > smellCount)
			{
				return;
			}

			int currentHappyCounter = 0;
			if (last != -1)
			{
				for (int i = 0; i < _customerCount; i++)
				{
					if (_satLevel[i] >= satisfactionLevel)
						currentHappyCounter++;
					else if (satisfactionLevel - _satLevel[i] > _onesAvailable[i][last])
					{
						return;
					} 
				}
			}

			if(currentHappyCounter == _customerCount)
			{
				_success = true;
				return;
			}
			for(int i = last + 1; i < smellCount; i++)
			{
				if(smells[i] == false)
				{
					for(int j = 0; j < _customerCount; j++)
					{
						_satLevel[j] += customerPreferences[j][i];
					}
					smells[i] = true;
					backtrackingHelper(iter + 1, i, ref smells);
					if(_success == true)
					{
						return;
					}
					smells[i] = false;
					for (int j = 0; j < _customerCount; j++)
					{
						_satLevel[j] -= customerPreferences[j][i];
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
		private bool _canFinish = true;
        public int AssignSmellsMaximizeHappyCustomers(out bool[] smells)
        {
			_satLevel = new int[_customerCount];
			bestSmell = new bool[smellCount];
            smells = new bool[smellCount];
			backtrackingHelperDwa(0, -1, ref smells);
			if (_success == false)
			{
				smells = bestSmell;
			}
            return _maxCust;
        }
		public void backtrackingHelperDwa(int iter, int last, ref bool[] smells)
		{
			_zadowoleni = 0;
			_success = true;
			for (int i = 0; i < _customerCount; i++)
			{
				if (_satLevel[i] < satisfactionLevel)
				{
					_success = false;
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

			if (_success == true)
			{
				_maxCust = _customerCount;
				Array.Copy(smells, bestSmell, smellCount);
				return;
			}

			if (iter == smellCount)
			{
				return;
			}

			for (int i = last + 1; i < smellCount; i++)
			{
				if (smells[i] == true)
					continue;
				if (smells[i] == false)
				{
					for (int j = 0; j < _customerCount; j++)
					{
						_satLevel[j] += customerPreferences[j][i];
					}
					smells[i] = true;
					backtrackingHelperDwa(iter + 1, i, ref smells);
					if (_success)
						return;
					if (_canFinish == false)
					{
						_success = false;
						return;
					}
					smells[i] = false;
					for (int j = 0; j < _customerCount; j++)
					{
						_satLevel[j] -= customerPreferences[j][i];
					}
				}
			}


		}
	}

}

