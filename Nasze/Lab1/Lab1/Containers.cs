
using System;

namespace ASD
{

public interface IContainer
    {
    void Put(int x);      //  dodaje element do kontenera

    int  Get();           //  zwraca pierwszy element kontenera i usuwa go z kontenera
                          //  w przypadku pustego kontenera zg³asza wyj¹tek typu EmptyException (zdefiniowany w Lab01_Main.cs)

    int  Peek();          //  zwraca pierwszy element kontenera (ten, który bêdzie pobrany jako pierwszy),
                          //  ale pozostawia go w kontenerze (czyli nie zmienia zawartoœci kontenera)
                          //  w przypadku pustego kontenera zg³asza wyj¹tek typu EmptyException (zdefiniowany w Lab01_Main.cs)

    int  Count { get; }   //  zwraca liczbê elementów w kontenerze

    int  Size  { get; }   //  zwraca rozmiar kontenera (rozmiar wewnêtznej tablicy)
    }

public class Stack : IContainer
    {
    private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
    private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
    // nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

    public Stack(int n=2)
        {
        tab = new int[n>2?n:2];
        }

    public void Put(int x)
        {
			if(count == tab.Length) //podwajamy rozmiar tablicy
			{
				int[] tmp = new int[2 * tab.Length];
				for (int i = 0; i < tab.Length; i++)
					tmp[i] = tab[i];
				tab = tmp;
			}
			tab[count++] = x;
        }

    public int Get()
        {
			// zmieniæ
			if (count == 0)
				throw new EmptyException();
			else
				return tab[--count];       
        }

    public int Peek()
        {
			if (count == 0)
				throw new EmptyException();
			else
				return tab[count-1];
		}

    public int Count => count;

    public int Size => tab.Length;

    } // class Stack


public class Queue : IContainer
    {
    private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
    private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
							
	int first = 0;          // mo¿na dodaæ jedno pole (wiêcej nie potrzeba)

		public Queue(int n=2)
        {
        tab = new int[n>2?n:2];
        }

    public void Put(int x)
        {
			if (count == tab.Length) //podwajamy rozmiar tablicy
			{
				int[] tmp = new int[2 * tab.Length];
				for (int i = 0; i < count; i++)
				{
					tmp[i] = tab[(first + i) % tab.Length];
				}
				tab = tmp;
				first = 0;
			}
			//wstawiamy zawsze po prawej o ile jest miejsce
			tab[(first + count) % tab.Length] = x;
			count++;
			
	}

    public int Get()
        {
			if (count == 0)
				throw new EmptyException();

			int v = tab[first];
			count--;
			//zmiana firsta:
			first = (first + 1) % tab.Length;
			return v; 
        }

    public int Peek()
        {
			if (count == 0)
				throw new EmptyException();

			return tab[first]; // zmienic
        }

    public int Count => count;

    public int Size => tab.Length;

    } // class Queue


public class LazyPriorityQueue : IContainer
    {
    private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
    private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
    // nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

    public LazyPriorityQueue(int n=2)
        {
        tab = new int[n>2?n:2];
        }

    public void Put(int x)
        {
			if (count == tab.Length) //podwajamy rozmiar tablicy
			{
				int[] tmp = new int[2 * tab.Length];
				for (int i = 0; i < tab.Length; i++)
					tmp[i] = tab[i];
				tab = tmp;
			}
			tab[count++] = x;
		}

    public int Get()
        {
			if (count == 0)
				throw new EmptyException();

			int ind = FindMax();
			int v = tab[ind];

			//Zeby nie bylo dziury, 
			//wstawiamy ostatni element w miejsce usuwanego:
			tab[ind] = tab[--count];

			return v;
        }

    public int Peek()
        {
			// zmieniæ
			if (count == 0)
				throw new EmptyException();

			return tab[FindMax()]; 
        }
	private int FindMax()
		{ 
			int max_ind = 0;
			for(int i = 1; i < count; i++)
			{
				if (tab[max_ind] < tab[i])
					max_ind = i;
			}
			return max_ind;
		}
    public int Count => count;

    public int Size => tab.Length;

    } // class LazyPriorityQueue


public class HeapPriorityQueue : IContainer
    {
    private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
    private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
    // nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

    public HeapPriorityQueue(int n=2)
        {
			tab = new int[n>2?n:2];
        }

    public void Put(int x)
        {
        // uzupe³niæ
			if(count == tab.Length)
			{
				int[] tmp = new int[2 * tab.Length];
				for (int i = 0; i < tab.Length; i++)
					tmp[i] = tab[i];
				tab = tmp;
			}
			// wstawianie na pierwsze wolne miejsce i poprawa w gore:
			tab[count++] = x;
			UpHeap(count - 1);
        }

    public int Get()
        {
			if (count == 0)
				throw new EmptyException();

			int v = tab[0];
			//wpisanie ostatniego w korzen i poprawa w dol:
			tab[0] = tab[--count];	
			DownHeap(0);
			return v; 
        }
    public int Peek()
        {
			if (count == 0)
				throw new EmptyException();

			return tab[0]; 
        }

	private void DownHeap(int num)
		{
			int k = 2 * num + 1; // lewe dziecko: 2*n + 1
			int v = tab[num];
			while (k < count) 
			{
				if (k + 1 <= count)	// prawe dziecko: 2*n + 2 istnieje
					if (tab[k + 1] > tab[k]) //prawe dziecko jest wiêksze od lewego
						k = k + 1;
				if (tab[k] > v)
				{
					tab[num] = tab[k];
					num = k;
					k = 2 * num;
				}
				else
					break;
			}
			tab[num] = v;
		}

	private void UpHeap(int num)
		{
			int v = tab[num];
			int k = (num % 2 == 0) ? (num / 2 - 1) : (num / 2); 
			while (num != 0 && tab[k] < v)
			{
				tab[num] = tab[k];
				num = k;
				k = (num % 2 == 0) ? (num / 2 - 1) : (num / 2);
			}
			tab[num] = v;

		}

		public int Count => count;

    public int Size => tab.Length;

    } // class HeapPriorityQueue

} // namespace ASD
