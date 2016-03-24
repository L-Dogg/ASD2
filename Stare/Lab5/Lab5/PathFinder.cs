using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiSD_Lab5
{
    public class PathFinder
    {
        int _width;
        int _height;
        int _sideLength;
        int _shortcutLength;
        Location[] _shortcuts;
        bool[,] _hasShortcut;  // tablica skrótów
							   // można dodać pola jeśli będą potrzebne
		int colsize, rowsize;
		/// <summary>
		/// Konstruktor klasy PathFinder
		/// </summary>
		/// <param name="width">Szerokość miasta</param>
		/// <param name="height">Wysokość miasta</param>
		/// <param name="sideLength">Długość normalnej drogi</param>
		/// <param name="shortcutLength">Długość drogi na skróty</param>
		/// <param name="shortcuts">Lista skrótów (wystąpienie punktu [x,y] oznacza, że z [x,y] można przejść do [x+1,y+1] skrótem)</param>
		public PathFinder(int width, int height, int sideLength, int shortcutLength, Location[] shortcuts)
        {
            _width = width;
            _height = height;
            _sideLength = sideLength;
            _shortcutLength = shortcutLength;
            _shortcuts = shortcuts;
			rowsize = _width / _sideLength + 1;
			colsize = _height / _sideLength + 1;
			if(shortcuts != null)
			{
				_hasShortcut = new bool[width, height];
				for (int i = 0; i < shortcuts.Length; i++)
					_hasShortcut[shortcuts[i].X, shortcuts[i].Y] = true;
			}
		}

        /// <summary>
        /// Metoda znajduje najkrótszą szcieżkę w mieście
        /// </summary>
        /// <param name="shortestPaths">
        /// Parametr wyjściowy - znaleziona tablica ze wszystkimi najkrótszymi scieżkami (zaczynając od punktu [0,0] kończąc na celu).
        /// W przypadku realizacji zadania zwracającego jedną ścieżkę należy zwrócić tablicę zawierającą dokładnie jedną tablicę z najkrótszą ścieżka.
        /// </param>
        /// <returns></returns>
        public int FindShortestPath(out Location[][] shortestPaths)
        {
            shortestPaths = new Location[0][];
			return AllPaths();
        }

        /// <summary>
        /// Pomocnicza rekurencyjna metoda do zliczania ścieżek. (nie ma obowiązku korzystania z niej)
        /// </summary>
        private int AllPaths(/* odpowiednie parametry */)
        {
			int vertices = _height * _width;
			int[] dist = new int[vertices];
			int[] prev = new int[vertices];
            for(int i = 0; i < vertices; i++)
			{
				dist[i] = int.MaxValue;
				prev[i] = -1;
			}
			dist[0] = 0;
			
			bool zmiana = true;
			
			while (zmiana)
			{
				zmiana = false;
				for(int i = 0; i < vertices; i++)
				{
					int col = i % _width;
					int row = i / _width;
					int[] dirs = { -1,-1,-1,-1};
					if (row > 0)
						dirs[0] = i - _width;
					if (row < _height - 1)
						dirs[1] = i + _width;
					if (col > 0)
						dirs[2] = i - 1;
					if (col < _width - 1)
						dirs[3] = i + 1;
					for(int j = 0; j < 4; j++)
					{
						int v = dirs[j];
                        if (v != -1)
							if(dist[v] > dist[i] + _sideLength)
							{
								zmiana = true;
								dist[v] = dist[i] + _sideLength;
								prev[v] = i;
							}
					}
					if (_hasShortcut[col, row] && row < _height - 1 && col < _width - 1)
						if (dist[i + _width + 1] > dist[i] + _shortcutLength)
						{
							zmiana = true;
							dist[i + _width + 1] = dist[i] + _shortcutLength;
						}
					if (col > 0 && row > 0)
						if (_hasShortcut[col - 1, row - 1])
							if (dist[i - _width - 1] > dist[i] + _shortcutLength)
							{
								zmiana = true;
								dist[i - _width - 1] = dist[i] + _shortcutLength;
							}
				}
			}
			return dist[vertices - 1];
        }
		
    }
}
