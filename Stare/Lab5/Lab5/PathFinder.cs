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
				_hasShortcut = new bool[height, width];
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
			int k = 0;
			AdjacencyMatrixGraph g = new AdjacencyMatrixGraph(false, _width * _height);
			for(int u = 0; u < _height - 1; u++)
			{
				for(int w = 0; w < _width - 1; w++)
				{
					g.AddEdge(k, k + 1, _sideLength);
					g.AddEdge(k, k + _width, _sideLength);
                    if (_hasShortcut[u, w])
						g.AddEdge(k, k + _width + 1, _shortcutLength);
					++k;
				}
			}
			k = _width - 1;
			for(int i = 0; i < _height - 1; i++)
			{
				g.AddEdge(k, k + _width, _sideLength);
				k += _width;
			}
			k = (_height - 1) * _width;
			for(int i = 0; i < _width - 1; i++)
			{
				g.AddEdge(k, k + 1, _sideLength);
				k++;
			}
			if (_hasShortcut[_height - 2, _width - 1])
				g.AddEdge(g.VerticesCount - 1 - _width, g.VerticesCount - 2, _shortcutLength);
			PathsInfo[] pf;
			g.DijkstraShortestPaths(0, out pf);
            shortestPaths = new Location[0][];
            return (int)pf[g.VerticesCount - 1].Dist;
        }

        /// <summary>
        /// Pomocnicza rekurencyjna metoda do zliczania ścieżek. (nie ma obowiązku korzystania z niej)
        /// </summary>
        private void AllPaths(/* odpowiednie parametry */)
        {
            
        }
    }
}
