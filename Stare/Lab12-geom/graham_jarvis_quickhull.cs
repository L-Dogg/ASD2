//#define Graham
//#define Jarvis
#define QuickHull

using System;
using System.Collections.Generic;
using System.Linq;
using Point = ASD.Geometry.Point;
using Segment = ASD.Geometry.Segment;

namespace ASD
{
    class Lab12
    {
        /// <summary>
        /// Znajduje otoczkę wypukłą dla punktów.
        /// Można zastosować dowolny algorytm liczenia otoczki omówiony na wykłądzie
        /// </summary>
        /// <param name="points">Tablica punktów</param>
        /// <returns>Tablica kolejnych punktów należących do otoczki</returns>
        public static Point[] ConvexHull(Point[] p)
        {
#if Graham
            #region Graham

            int minIdx = 0;
            for (int i = 1; i < p.Length; i++)
                if (p[i].y < p[minIdx].y || p[i].y == p[minIdx].y && p[i].x < p[minIdx].x)
                    minIdx = i;

            Point tmp = p[0];
            p[0] = p[minIdx];
            p[minIdx] = tmp;

            p = Geometry.AngleSort(p[0], p);
            //Func<double, double> Sqr = ((a) => a * a);
            //Func<Point, Point, double> QuadraticDist = (a, b) => (Sqr(a.x - b.x) + Sqr(a.y - b.y));
            //p = Geometry.AngleSort(p[0], p, (a, b) =>
            //{
            //    double da = QuadraticDist(p[0], a);
            //    double db = QuadraticDist(p[0], b);
            //    if (da != db)
            //        return (da < db ? -1 : 1);
            //    else
            //        return 0;
            //});

            Segment line = new Segment(p[0], p[1]);
            var stack = new Stack<Point>();
            stack.Push(p[0]);
            stack.Push(p[1]);
            for (int i = 2; i <= p.Length; ++i) // dodatkowa iteracja
            {
                int j = (i % p.Length);

                while (stack.Count > 2 && line.Direction(p[j]) != Geometry.AntiClockWise)
                {
                    stack.Pop();
                    line.pe = stack.Pop();  // top
                    line.ps = stack.Peek(); // below
                    stack.Push(line.pe);
                }

                if (stack.Count == 2 && line.Direction(p[j]) != Geometry.AntiClockWise) // trzy punkty wspolliniowe
                    stack.Pop();

                stack.Push(p[j]);
                line.ps = line.pe;  // below
                line.pe = p[j];     // top
            }

            stack.Pop(); // wyrzucamy powtorzony pierwszy punkt
            return stack.ToArray();

            #endregion
#elif Jarvis
            #region Jarvis

            int minIdx = 0;
            for (int i = 1; i < p.Length; i++)
                if (p[i].y < p[minIdx].y || p[i].y == p[minIdx].y && p[i].x < p[minIdx].x)
                    minIdx = i;

            Point tmp = p[0];
            p[0] = p[minIdx];
            p[minIdx] = tmp;

            Segment line = new Segment(new Point(int.MinValue, 0), new Point(0, int.MaxValue)); // cokolwiek
            Stack<Point> stack = new Stack<Point>();
            stack.Push(p[0]);

            while (true)
            {
                int best = -1;
                for (int k = 0; k < p.Length; k++)
                {
                    if (best == -1
                        || line.Direction(p[k]) == Geometry.ClockWise
                        || line.Direction(p[k]) == Geometry.Collinear
                        && Point.Distance(stack.Peek(), p[best]) < Point.Distance(stack.Peek(), p[k]))
                        best = k;
                    line.ps = stack.Peek();
                    line.pe = p[best];
                }
                if (best == 0)
                    return stack.ToArray();
                stack.Push(p[best]);
            }

            #endregion
#elif QuickHull
            #region QuickHull

            int minIdx = 0;
            int maxIdx = 0;
            for (int i = 1; i < p.Length; i++)
            {
                if (p[i].x < p[minIdx].x || p[i].x == p[minIdx].x && p[i].y < p[minIdx].y)
                    minIdx = i;
                if (p[i].x > p[maxIdx].x || p[i].x == p[maxIdx].x && p[i].y > p[maxIdx].y)
                    maxIdx = i;
            }

            Point A = p[minIdx];
            Point B = p[maxIdx];
            Segment AB = new Segment(A, B);
            List<Point> L = new List<Point>();
            List<Point> S1 = new List<Point>();
            List<Point> S2 = new List<Point>();
            for (int i = 0; i < p.Length; i++)
            {
                if (AB.Direction(p[i]) == Geometry.ClockWise)
                    S1.Add(p[i]);
                else if (AB.Direction(p[i]) == Geometry.AntiClockWise)
                    S2.Add(p[i]);
            }
                
            L.Add(A);
            QuickHullRec(A, B, S1.ToArray(), ref L);
            L.Add(B);
            QuickHullRec(B, A, S2.ToArray(), ref L);

            return L.ToArray();
        }

        public static void QuickHullRec(Point A, Point B, Point[] S, ref List<Point> L)
        {
            int maxIdx = 0;
            double dist = 0.0;
            double _A = A.y - B.y; // zmiana znaku
            double _B = B.x - A.x;
            double _C = A.y * (B.x - A.x) - A.x * (B.y - A.y);

            for (int i = 0; i < S.Length; i++)
                if (Math.Abs(_A * S[i].x + _B * S[i].y + _C) > dist)
                {
                    maxIdx = i;
                    dist = Math.Abs(_A * S[i].x + _B * S[i].y + _C);
                }

            Point C = S[maxIdx];
            Segment AC = new Segment(A, C);
            Segment CB = new Segment(C, B);
            List<Point> S1 = new List<Point>();
            List<Point> S2 = new List<Point>();
            for (int i = 0; i < S.Length; i++)
            {
                if (AC.Direction(S[i]) == Geometry.ClockWise)
                    S1.Add(S[i]);
                else if (CB.Direction(S[i]) == Geometry.ClockWise)
                    S2.Add(S[i]);
            }

            if (S1.Count != 0)
                QuickHullRec(A, C, S1.ToArray(), ref L);
            L.Add(C);
            if (S2.Count != 0)
                QuickHullRec(C, B, S2.ToArray(), ref L);

            #endregion
#endif
        }

        /// <summary>
        /// Znajduje maksymalna odległość między punktami w tablicy points.
        /// (2pkt, w tym otoczka)
        /// </summary>
        /// <remarks>
        /// Odległość należy znaleźć wykorzystując otoczkę wypukłą.
        /// Tzn. Najpierw dla points znaleźć otoczkę, a potem wyszukiwać pary wierzchołków sposród tych znajdujących się na otoczce.
        /// </remarks>
        /// <param name="points">zbiór punktów</param>
        /// <param name="result">Tablica 2 punktów zawierające punkty najbardziej odległe</param>
        /// <returns>Wartość maksymalnej odległości w zbiorze</returns>
        public static double MaxDiameter(Point[] points, out Point[] result)
        {
            double dist = 0.0;
            result = new Point[2];
            var hull = ConvexHull(points);

            for (int i = 0; i < points.Length; i++)
                for (int j = 0; j < points.Length; j++)
                    if (Point.Distance(points[i], points[j]) > dist)
                    {
                        dist = Point.Distance(points[i], points[j]);
                        result[0] = points[i];
                        result[1] = points[j];
                    }

            return dist;
        }

        /// <summary>
        /// Dla podanych współrzędnych ołtarza i murów metoda zwraca informację czy istnieje półprosta o początku 
        /// w ołtarzu nie przecinająca żadnego muru.
        /// (2pkt)
        /// </summary>
        /// <remarks>
        /// Nie należy liczyć bezpośrednio żadnych kątów (funkcje trygonometryczne liczą się powoli)
        /// Można zastosować następujący algorytm
        ///
        ///    A) przygotowanie danych
        ///    Na podstawie tablicy murów (odcinków) utworzyć tablicę punktów (końców odcinków).
        ///    Utworzyć też Dictionary indeksowane punktami i wypełnione wartościami +1 dla "wcześniejszego końca odcinka" i -1 dla "późniejszego końca odcinka".
        ///    "Wcześniejszy koniec odcinka" to koniec, dla którego prosta przechodząca przez ten koniec tworzy mniejszy kąt z osia OX
        ///    niż analogiczna prosta przechodząca przez drugi koniec (ten drugi koniec to "póżniejszy koniec"). Oczywiście nie liczymy bezpośrednio żadnych kątów !!!.
        ///    Policzyć ile murów (odcinków) przecina się z półprostą (długim odcinkiem) zaczepioną w ołtarzu i równoległą do osi OX
        ///
        ///    B) posortować tablicę punktów (końców odcinków) według kąta jaki tworzą odcinki o początku w ołtarzu i końcu w danym punkcie z osią OX
        ///       (skorzystać z biblioteki geom.cs)
        ///
        ///    C) obliczenia
        ///    przetwarzać punkty w kolejności wynikającej z posortowania w czesci B
        ///    dla każdego z punktów zmniejszać bądź zwiekszać licznik przecięć (odbiczony w części A) zależnie od tego czy punkt jest wcześniejszym czy późniejszym końcem odcinka
        ///    (korzystać z przygotowanego Dictionary). Zastanowić się co oznacza wartość licznika równa 0.
        ///
        ///    Uwaga: Nie przejmować się przypadkami szczególnymi (np. ołtarz i końce dwóch murów leżące na jednej prostej) w testach ich nie będzie.
        /// </remarks>
        /// <param name="altar"> Współrzędne ołtarza</param>
        /// <param name="walls">zbiór murów (odcinków)</param>
        /// <returns>true jeśli nie istnieje prosta nieprzecinająca muru, false jeśli taka prosta istnieje</returns>
        public static bool ChineeseAltars(Point altar, Segment[] walls)
        {

            var points = new List<Point>();
            var type = new Dictionary<Point, int>();
            Segment OX = new Segment(altar, new Point(double.MaxValue, altar.y));
            int intersections = 0;
            foreach (Segment wall in walls)
            {
                points.Add(wall.ps);
                points.Add(wall.pe);
                Segment startSeg = new Segment(altar, new Point(wall.ps.x, wall.ps.y));
                Point endSeg = new Point(wall.pe.x, wall.pe.y);
                int startSegOrientation = -startSeg.Direction(endSeg);
                type.Add(wall.ps, startSegOrientation);
                type.Add(wall.pe, -startSegOrientation);
                if (Geometry.Intersection(wall, OX))
                    intersections++;
            }
            Point[] sorted = Geometry.AngleSort(altar, points.ToArray());
            foreach (Point point in sorted)
            {
                intersections -= type[point];
                if (intersections == 0)
                    break;
            }
            return intersections != 0;
        }
    }
}