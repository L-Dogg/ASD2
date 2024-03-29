﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

	class SweepLine
    {

		/// <summary>
		/// Struktura pomocnicza opisująca zdarzenie
		/// </summary>
		/// <remarks>
		/// Można jej użyć, przerobić, albo w ogóle nie używać i zrobić po swojemu
		/// </remarks>
		struct SweepEvent : IComparable
			{
			/// <summary>
			/// Współrzędna zdarzenia
			/// </summary>
			public double Coord;

			/// <summary>
			/// Czy zdarzenie oznacza początek odcinka/prostokąta
			/// </summary>
			public bool IsStartingPoint;

			/// <summary>
			/// Indeks odcinka/prodtokąta w odpowiedniej tablicy
			/// </summary>
			public int Idx;

			public SweepEvent(double c, bool sp, int i=-1 ) { Coord=c; IsStartingPoint=sp; Idx=i; }

			public int CompareTo(object obj)
			{
				SweepEvent se = (SweepEvent)obj;
				if (se.Coord < this.Coord && se.Idx != this.Idx ||
					se.Coord < this.Coord && se.IsStartingPoint)
					return 1;
				else if (se.Coord == this.Coord)
					return 0;
				else
					return -1;
			}
		}

		/// <summary>
		/// Funkcja obliczająca długość teoriomnogościowej sumy pionowych odcinków
		/// </summary>
		/// <returns>Długość teoriomnogościowej sumy pionowych odcinków</returns>
		/// <param name="segments">Tablica z odcinkami, których teoriomnogościowej sumy długość należy policzyć.
		/// Każdy odcinek opisany jest przez dwa punkty: początkowy i końcowy </param>
		public double VerticalSegmentsUnionLength(Geometry.Segment[] segments)
        {
			int open = 0;
			int closed = 0;
			double length = 0;
			
			var events = new List<SweepEvent>();
			for (int i = 0; i < segments.Length; i++)
			{
				events.Add(new SweepEvent(segments[i].ps.y, true, i));
				events.Add(new SweepEvent(segments[i].pe.y, false, i));
			}
			events.Sort();

			double start = 0;
			foreach(var e in events)
			{
				if (e.IsStartingPoint)
				{
					if (open == 0)
						start = e.Coord;
					open++;
				}
				else
				{
					closed++;
					if (!e.IsStartingPoint && closed == open)
					{
						closed = open = 0;
						length += Math.Abs(start - e.Coord);
					}
				}														
			}

			return length;
        }

		/// <summary>
		/// Funkcja obliczająca pole teoriomnogościowej sumy prostokątów
		/// </summary>
		/// <returns>Pole teoriomnogościowej sumy prostokątów</returns>
		/// <param name="rectangles">Tablica z prostokątami, których teoriomnogościowej sumy pole należy policzyć. 
		/// Każdy prostokąt opisany jest przez cztery wartości: minimalna współrzędna X, minimalna współrzędna Y, 
		/// maksymalna współrzędna X, maksymalna współrzędna Y.
		/// </param>
		public double RectanglesUnionArea(Geometry.Rectangle[] rectangles)
		{
			var events = new List<SweepEvent>();
			var lines = new Dictionary<int, Geometry.Segment>();
			double xCoord = 0;
			double area = 0;
			double D = 0;

			for(int i = 0; i < rectangles.Length; i++)
			{
				events.Add(new SweepEvent(rectangles[i].MinX, true, i));
				events.Add(new SweepEvent(rectangles[i].MaxX, false, i));
			}
			events.Sort();

			foreach(var e in events)
			{
				area += (e.Coord - xCoord) * D;				
				if (e.IsStartingPoint)
				{
					var seg = new Geometry.Segment
						(
						new Geometry.Point(e.Coord, rectangles[e.Idx].MinY),											
						new Geometry.Point(e.Coord, rectangles[e.Idx].MaxY)
						);
					lines.Add(e.Idx, seg);
                }
				else
				{
					lines.Remove(e.Idx);
				}
				xCoord = e.Coord;
				D = VerticalSegmentsUnionLength(lines.Values.ToArray());
			}
			
			return area;
		}

    }

}
