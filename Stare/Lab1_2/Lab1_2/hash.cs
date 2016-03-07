using System;

namespace ASD
{

    class HashTable
    {
        // klucze w tablicy sa unikalne i nieujemne

        private const int empty = -1;
        private const int deleted = -2;

        private double alfa; // wspolczynnik wypelnienia przy ktorym tablica jest dwukrotnie powiekszana
        private int elemCount;
        private int accessCount;
        private int[] table;
        private Func<int, int, int> hash;   // podstawowa funkcja haszujaca
        private Func<int, int, int> shift;  // funkcja przesuniecia wywolywana w razie konfliktu, drugi parametr - numer iteracji

        // domyslny rozmiar poczatkowy 8
        // domyslny wspolczynnik zapelnienia powodujacy podwojenie rozmiaru 7/8
        public HashTable(Func<int, int, int> _hash, Func<int, int, int> _shift, int _size = 8, double _alfa = 7.0/8.0)
        {
            hash = _hash;
            shift = _shift;
            alfa = _alfa;
            elemCount = accessCount = 0;
            table = new int[_size];
            for (int i = 0; i < _size; i++)
                table[i] = empty;
        }

        public int Size
        {
            get
            {
                return table.Length;
            }
        }

        public int AccessCount
        {
            get
            {
                return accessCount;
            }
        }

        public int ElemCount  // liczba elementow w tablicy (z uwzglednieniem skasowanych)
        {
            get
            {
                return elemCount;
            }
        }

        // wyszukiwanie w tablicy elementu v - zwraca true jesli element v jest w tablicy
        public bool Search(int v)
		{
			//Console.WriteLine("Search");

			int i = hash(v, table.Length);
			int k = 1;
			while (table[i] != empty)
			{
				accessCount++;
				if (table[i] == v)
				{
					accessCount++;
					return true;
				}
				else
				{
					i = (i + shift(v, k++) ) % table.Length;
				}
			}
            return false;
        }

        // wstawianie do tablicy elementu v - zwraca false jesli v juz jest w tablicy
        // jesli zapelnienie tablicy >= alfa rozmiar tablicy jest podwajany (elementy są przenoszone)
        public bool Insert(int v)
        {
			//Console.WriteLine("Insert");

			int i = hash(v, table.Length);
			int k = 1;
			int firstDel = empty;
			while(table[i] != v && table[i] != empty)
			{
				accessCount += 2;
				if(table[i] == deleted && firstDel == empty)
				{
					accessCount++;
					firstDel = i;
				}
				i = (i + shift(v, k++)) % table.Length;
			}
			if (table[i] == v)
			{
				accessCount++;
				return false;
			}
			if (firstDel != empty)
			{
				table[firstDel] = v;
				return true;
			}
			else
			{
				elemCount++;
				table[i] = v;
			}
			if (((double)elemCount / table.Length) >= alfa)
			{
				int[] pom = new int[table.Length];
				for (int j = 0; j < table.Length; j++)
					pom[j] = table[j];

				table = new int[2 * table.Length];
				for (int j = 0; j < table.Length; j++)
					table[j] = empty;

				elemCount = 0;
				for (int j = 0; j < pom.Length; j++)
				{
					if (pom[j] != empty && pom[j] != deleted)
						Insert(pom[j]);
				}
			}
            return true;
        }

        // usuwanie elementu v z tablicy - zwraca false jesli elementu v nie ma w tablicy
        public bool Remove(int v)
		{
			//Console.WriteLine("Remove");

			if (Search(v) == false)
				return false;

			//Console.WriteLine("Remove po searchu");

			int i = hash(v, table.Length);
			int k = 1;
			while (table[i] != v)
			{
				accessCount++;
				i = (i + shift(v, k++)) % table.Length;
			}
			table[i] = deleted;
			return true;
        }

        // Funkcje haszujace - dla wszystkich
        // v     - klucz
        // size  - rozmiar tablicy
        // wynik - indeks elementu v w tablicy rozmiaru size

        // Najprostsze haszowanie modulo
        public static int ModHash(int v, int size)
        {
            return v % size;
        }

        // haszowanie multiplikatywne
        public static int MultiplyHash(int v, int size)
        {
			double a = (Math.Sqrt(5) - 1) / 2;
			double ulamkowa = v * a - (int)(v * a);
			int wynik = (int)(size * ulamkowa);
			// wynik = czesc_calkowita(size*(czesc_ulamkowa(v*a)))
			return wynik % size; 
        }

        // Funkcje rozwiazywania kolizji - dla wszystkich
        // v     - klucz
        // k     - numer kroku ( przy probie dostepu do klucza v wystapilo juz k kolizji )
        // wynik - przesuniecie wzgledem aktualnego indeksu

        // sekwencyjne rozwiazywanie kolizji
        // - szukamy pierwszego wolnego miejsca (z krokiem 1)
        public static int Shift1(int v, int k) { return 1; }

        // liniowe rozwiazywanie kolizji
        // - szukamy pierwszego wolnego miejsca z zadanym krokiem > 1 (np. 5)
        //   (krok musi byc wzglednie pierwszy z rozmiarem tablicy)
        public static int Shift5(int v, int k) { return 5; }

        // rozwiazywanie kolizji z rosnacym krokiem
        // - w k-tej probie przesuwamy sie o k
        public static int Shiftk(int v, int k) { return k; }

        // rozwiazywanie kolizji przy pomocy haszowania dwukrotnego
        // - rozny krok dla roznych kluczy !
        // do wyznaczenia kroku zastosowac jakies proste haszowanie modulo
        // (dla kazdego klucza krok musi byc wzglednie pierwszy z rozmiarem tablicy)
        public static int Shifth(int v, int k)
        {
			return 2 * (v % 1337) + 1;
        }

    } // class HashTable

} // namespace ASD 