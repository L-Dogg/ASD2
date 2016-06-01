using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point = ASD.Geometry.Point;
using Triangle = ASD.Geometry.Triangle;
using Segment = ASD.Geometry.Segment;

namespace ASD
{
    partial class lab13
    {
        /// <summary>
        /// 
        /// (1p.)
        /// 
        /// Wielokąt monotoniczny w kierunku poziomym to wielokąt, dla którego każda pionowa prosta 
        /// przecina go w maksymalnie dwóch punktach.
        /// Zbiór krawędzi takiego wielokąta można podzielić na dwa łańcuchy: 
        /// - górny (tworzony przez krawędzie, które stanowią górne punkty przecięcia prostych pionowych),
        /// - dolny (tworzony przez krawędzie, które stanowią dolne punkty przecięcia prostych pionowych).
        /// 
        /// Zadanie: sprawdź czy podany wielokąt jest monotoniczny i jeśli jest, to zwróć 
        /// listę jego wierzchołków w kolejności od wierzchołka o najmniejszej wartości x,
        /// w p.p. zwróć pustą tablicę.
        /// Dodatkowo zakłada się, że przekazywany jako argument wielokąt nie ma przecinających się 
        /// wzajemnie krawędzi ani dwóch wierzchołków w tym samym punkcie.
        /// </summary> 
        /// 
        /// <param name="polygon">Tablica wierzchołków wielokąta począwszy od nieznanego wierzchołka </param>
        /// <param name="sortedPolygon"> Tablica wierzchołków wielokąta począwszy od wierzchołka o najmniejszej 
        /// wartości x </param>
        /// 
        /// <returns>True jeśli wielokąt jest monotoniczny, False - w p.p.</returns>  
        /// 
        public static bool isMonotone(Point[] polygon, out Point[] sortedPolygon)
        {
			sortedPolygon = null;

			int leftMostInd = 0, rightMostInd = 0;
			for(int i = 1; i < polygon.Length; i++)
			{
				if (polygon[i].x < polygon[leftMostInd].x)
					leftMostInd = i;
				if (polygon[i].x > polygon[rightMostInd].x)
					rightMostInd = i;
			}

			List<Point> upper = new List<Point>();
			List<Point> lower = new List<Point>();
			upper.Add(polygon[leftMostInd]);
			lower.Add(polygon[rightMostInd]);
			
			int k = (leftMostInd + 1) % polygon.Length;
			while(k != rightMostInd)
			{
				if (polygon[k].x <= upper[upper.Count - 1].x)
					return false;
				upper.Add(polygon[k]);		
				k = (k + 1) % polygon.Length;
			}
			k = (rightMostInd + 1) % polygon.Length;
			while(k != leftMostInd)
			{
				if (polygon[k].x >= lower[lower.Count - 1].x)
					return false;
				lower.Add(polygon[k]);
				k = (k + 1) % polygon.Length;
			}
			
			var tmp = new List<Point>();
			int v = 1;
			tmp.Add(polygon[leftMostInd]);
			while(v != polygon.Length)
			{
				leftMostInd = (leftMostInd + 1) % polygon.Length;
				v++;
				tmp.Add(polygon[leftMostInd]);
			}
			sortedPolygon = tmp.ToArray();
            return true;
        }



        /// <summary>
        /// 
        /// (3p.)
        /// 
        /// Zadanie: dokonaj traingulacji czyli podziału wielokąta będącego monotonicznym w kierunku poziomym
        /// na trójkąty na podstawie wskazówek:
        /// - dodawaj do listy kolejne wierzchołki wg ich wartości współrzednej x rozpocznając 
        ///   od skrajnie lewego wierzchołka,
        /// - sprawdź czy z już dodanych wierzchołków można utworzyć trójkąt, a może mieć to miejsce 
        ///   w dwóch przypadkach:
        ///    a) wierzchołki należą do tego samego łańcucha i tworzą trójkąt leżący wewnątrz wielokąta, 
        ///    b) wierzchołki należą do różnych łańcuchów (tutaj zakładamy, że nie trzeba sprawdzać czy 
        ///       cały trójkąt leży wewnątrz wielokąta),
        /// - jeśli zachodzi warunek a) należy utworzyć trójkąt lub trójkąty z wierzchołków z końca listy 
        ///   do momentu gdy tworzą one trójkąt leżący wewnątrz wielokąta usuwając dla każdego nowo 
        ///   utworzonego trójkąta po jednym wierzchołku z listy,
        /// - jeśli zachodzi warunek b) należy utworzyć trójkąt lub trójkąty począwszy od początku listy 
        ///   usuwając dla każdego nowo utworzonego trójkąta po jednym wierzchołku z listy,
        /// - aby określić czy dany trójkąt tworzony z wierzchołków tego samego łańcucha leży wewnątrz 
        ///   wielokąta wystarczy określić po której stronie prostej zawierającej dwa wierzchołki 
        ///   o mniejszej wartości x leży trzeci wierzchołek.  
        /// </summary> 
        /// 
        /// <param name="polygon">Tablica wierzchołków wielokąta począwszy od wierzchołka o najmniejszej 
        /// wartości x </param>
        /// <param name="triangulation"> Tablica trójkątów </param>
        /// 
        /// <returns>Liczba trójkątów</returns>  
        /// 
        public static int triangulateMonotone(Point[] polygon, out Triangle[] triangulation)
        {
			int leftMostInd = 0, rightMostInd = polygon.Length - 1;
			
			for(int i = 0; i < polygon.Length; i++)
			{
				int minInd = i;
				for (int j = i + 1; j < polygon.Length; j++)
					if (polygon[j].x < polygon[minInd].x)
						minInd = j;

				var p = polygon[i];
				polygon[i] = polygon[minInd];
				polygon[minInd] = p;
			}

			List<Point> upper = new List<Point>();
			List<Point> lower = new List<Point>();
			upper.Add(polygon[leftMostInd]);
			lower.Add(polygon[rightMostInd]);
			int k = (leftMostInd + 1) % polygon.Length;
			while (k != rightMostInd)
			{
				upper.Add(polygon[k]);
				k = (k + 1) % polygon.Length;
			}
			k = (rightMostInd + 1) % polygon.Length;
			while (k != leftMostInd)
			{
				lower.Add(polygon[k]);
				k = (k + 1) % polygon.Length;
			}
			var tmp = new List<Triangle>();
			var pts = new List<Point>();

			pts.Add(polygon[0]);
			pts.Add(polygon[1]);
			int v = 2;

			while(v < polygon.Length)
			{
				Point p = polygon[v++];

				if(upper.Contains(p) && lower.Contains(pts[0]) || lower.Contains(p) && upper.Contains(pts[0]))
				{
					while(pts.Count > 1)
					{
						tmp.Add(new Triangle(pts[0], pts[1], p));
						pts.RemoveAt(0);
					}
					pts.Add(p);
				}
				else
				{
					// while do poprawy
					while(pts.Count > 1 && new Segment(pts.Last(), pts[pts.Count - 2]).Direction(p) > 0)
					{
						tmp.Add(new Triangle(pts.Last(), pts[pts.Count - 2], p));
						pts.RemoveAt(pts.Count - 1);
					}
					pts.Add(p);
				}
			}

			triangulation = tmp.ToArray();
			
            return 0;            
        }



        /// <summary>
        /// 
        /// (1p.)
        /// 
        /// Zadanie: wyznacz pole wielokata na podstawie jego triangulacji
        /// </summary> 
        /// 
        /// <param name="triangulation"> Tablica trójkątów będących wynikiem triangulacji wielokąta
        /// </param>
        /// 
        /// <returns>Pole wielokąta</returns>
        /// 
        public static double polygonArea(Triangle[] triangulation)
        {
			double p, a, b, c;
			double area = 0;
			foreach (Triangle t in triangulation)
			{
				a = Point.Distance(t.a, t.b);
				b = Point.Distance(t.a, t.c);
				c = Point.Distance(t.b, t.c);
				p = 0.5 * (a + b + c);
				area += Math.Sqrt(p * (p - a) * (p - b) * (p - c));			 
			}

			return area;
        }
    }
}
