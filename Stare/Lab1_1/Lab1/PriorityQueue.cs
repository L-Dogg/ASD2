
using System;
using System.Collections;
using System.Collections.Generic;

namespace ASD
{

interface IPriorityQueue
    {
    void Put(int p);     // wstawia element do kolejki
    int GetMax();        // pobiera maksymalny element z kolejki (element jest usuwany z kolejki)
    int ShowMax();       // pokazuje maksymalny element kolejki (element pozostaje w kolejce)
    int Count { get; }   // liczba elementów kolejki
    }


class LazyPriorityQueue : IPriorityQueue
    {
		LinkedList<int> list;
    public LazyPriorityQueue()
        {
			list = new LinkedList<int>();
        }

    public void Put(int p)
        {
			list.AddFirst(p);
        }

    public int GetMax()
        {
			if (list.Count == 0)
				throw new InvalidOperationException("Access to empty queue");
            int max = ShowMax();
            list.Remove(max);

			return max;
        }

    public int ShowMax()
        {
			if (list.Count == 0)
				throw new InvalidOperationException("Access to empty queue");

			int max = list.First.Value;

			foreach (int el in list)
				if (el > max)
					max = el;

			return max;
		}

    public int Count
        {
        get {
            return list.Count;
            }
        }

    } // LazyPriorityQueue


class EagerPriorityQueue : IPriorityQueue
    {
		LinkedList<int> list;
		public EagerPriorityQueue()
        {
			list = new LinkedList<int>();
        }

    public void Put(int p)
        {
			if (list.Count == 0)
			{
				list.AddFirst(p);
				return;
			}
			foreach(int el in list)
			{
				if (el <= p)
				{
					list.AddBefore(list.Find(el), p);
					return;
				}
			}
			list.AddLast(p);
        }

    public int GetMax()
        {
			if (list.Count == 0)
				throw new InvalidOperationException("Access to empty queue");
			int max = list.First.Value;
			list.RemoveFirst();
			return max;
        }

    public int ShowMax()
		{
			if (list.Count == 0)
				throw new InvalidOperationException("Access to empty queue");
			return list.First.Value;
		}

    public int Count
        {
        get {
            return list.Count;
            }
        }

    } // EagerPriorityQueue


class HeapPriorityQueue : IPriorityQueue
    {
		private const int SIZE = 101;
		private	int n = 0; //indeks ostatniego elementu
		private int[] tab = new int[SIZE];
    public HeapPriorityQueue()
        {
			tab[0] = int.MaxValue; //wartownik
        }

    public void Put(int p)
        {
			tab[++n] = p;
			UpHeap(n);
        }

    public int GetMax()
        {
			if (n == 0)
				throw new InvalidOperationException("Access to empty queue");
			if (n == 1)
				return tab[n--];

			int m = tab[1];
			tab[1] = tab[n--];
			DownHeap(1);

			return m;
		}

    public int ShowMax()
        {
			if (n == 0)
				throw new InvalidOperationException("Access to empty queue");

			return tab[1];
		}

	private void DownHeap(int num)
		{
			int k = 2 * num;
            int v = tab[num];
			while(k <= n)
			{
				if (k + 1 <= n)
					if (tab[k + 1] > tab[k])
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
			while(tab[num/2] < v)
			{
				tab[num] = tab[num / 2];
				num = num / 2;
			}
			tab[num] = v;
		
		}
    public int Count
        {
        get {
            return 0;
            }
        }

    } // HeapPriorityQueue

	class SkewHeap : IPriorityQueue
	{
		private int count = 0;
		private class Node
		{
			public int Val { get; set; }
			public int Npl { get; set; }
			public Node left, right;
		};
		private Node root = null;

		public int Count
		{
			get
			{
				return count;
			}
		}

		public int GetMax()
		{
			if(root == null)
				throw new InvalidOperationException("Access to empty queue");
			
			int v = root.Val;
			root = Union(root.left, root.right);
			return v;
		}

		public  void Put(int p)
		{
			Node elem = new Node();
			elem.left = elem.right = null;
			elem.Val = p;
			elem.Npl = 0;
			root = Union(root, elem);
		}

		public int ShowMax()
		{
			if (root == null)
				throw new InvalidOperationException("Access to empty queue");

			return root.Val;
		}
		private Node Union(Node left, Node right)
		{
			if (left == null)
				return right;
			else if (right == null)
				return left;
			Node ret;
			if (left.Val > right.Val)
			{
				ret = left;
				ret.right = Union(ret.right, right);
			}
			else
			{
				ret = right;
				ret.right = Union(ret.right, left);
			}
			if (ret.left == null || ret.right.Npl > ret.left.Npl)
			{
				Node tmp = ret.right;
				ret.right = ret.left;
				ret.left = tmp;
			}
			if (ret.right == null)
				ret.Npl = 0;
			else
				ret.Npl = ret.right.Npl + 1;

			return ret;
		}
	}
}
