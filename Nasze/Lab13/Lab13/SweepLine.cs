
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// <param name="segments">Tablica z odcinkami, których teoriomnogościowej sumy długość należy policzyć</param>
		/// Każdy odcinek opisany jest przez dwa punkty: początkowy i końcowy
		/// </param>
		public double VerticalSegmentsUnionLength(Geometry.Segment[] segments)
        {
			int open = 0;
			int closed = 0;
			double length = 0;
			
			List<SweepEvent> events = new List<SweepEvent>();
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
		/// <param name="rectangles">Tablica z prostokątami, których teoriomnogościowej sumy pole należy policzyć</param>
		/// Każdy prostokąt opisany jest przez cztery wartości: minimalna współrzędna X, minimalna współrzędna Y, 
		/// maksymalna współrzędna X, maksymalna współrzędna Y.
		/// </param>
		public double RectanglesUnionArea(Geometry.Rectangle[] rectangles)
		{
			List<SweepEvent> events = new List<SweepEvent>();
			List<Geometry.Segment> lines = new List<Geometry.Segment>();

			for(int i = 0; i < rectangles.Length; i++)
			{
				events.Add(new SweepEvent(rectangles[i].MinX, true, i));
				events.Add(new SweepEvent(rectangles[i].MaxX, false, i));
			}
			events.Sort();

			double xCoord = 0;
			double area = 0;
			double D = 0;
			foreach(var e in events)
			{
				Console.WriteLine("ITER");
				if (e.IsStartingPoint)
				{
					if(lines.Count == 0)
						xCoord = e.Coord;
					Console.WriteLine("Adding starting line: {0}", e.Coord);
					var seg = new Geometry.Segment
						(
						new Geometry.Point(e.Coord, rectangles[e.Idx].MinY),											
						new Geometry.Point(e.Coord, rectangles[e.Idx].MaxY)
						);
					lines.Add(seg);
                }
				else
				{
					Console.WriteLine("Calculated xCoord: {0}", xCoord);
					area += Math.Abs(xCoord - e.Coord) * D;
					Console.WriteLine("Area += |{0} - {1}| * {2} = {3}", xCoord, e.Coord, D, Math.Abs(xCoord - e.Coord) * D);
					
					lines.Remove(new Geometry.Segment(
						new Geometry.Point(rectangles[e.Idx].MinX, rectangles[e.Idx].MinY),
						new Geometry.Point(rectangles[e.Idx].MinX, rectangles[e.Idx].MaxY)));
					Console.WriteLine("Removing line: {0}", rectangles[e.Idx].MinX);
					xCoord = e.Coord;
				}
				D = VerticalSegmentsUnionLength(lines.ToArray());
				Console.WriteLine("Calculated D: {0}", D);
			}
			
			return area;
		}

    }

}
