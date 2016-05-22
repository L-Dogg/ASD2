using System;
using System.Collections.Generic;

namespace discs
{
    public struct Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }

        public override string ToString()
        {
            return string.Format("[{0};{1}]", X, Y);
        }


        public bool IsRightOf(Point b)
        {
            return (this.X > b.X || (this.X == b.X && this.Y > b.Y));
        }
    }
    
    public enum IntersectionType
    {
        Disjoint,
        Contains,
        IsContained,
        Identical,
        Touches,
        Crosses
    }

    public struct Disk
    {
        public Point Center { get; private set; }
        public double Radius { get; private set; }

        public Disk(Point center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public bool Contains(Point p)
        {
            return (p.X - Center.X) * (p.X - Center.X) + (p.Y - Center.Y) * (p.Y - Center.Y) <= Radius * Radius + Program.epsilon;
        }

		/// <summary>
		///  Funkcja sprawdza wzajemne położenie dwóch kół.
		/// </summary>
		/// <param name="other">drugie koło</param>
		/// <param name="crossingPoints">
		/// Punkty przecięcia obwodów kół, jeśli zwracana jest wartość: Touches albo Crosses.
		/// Pusta tablica wpp.
		/// <returns>
		/// Disjoint - kiedy koła nie mają punktów wspólnych
		/// Contains - kiedy pierwsze koło całkowicie zawiera drugie
		/// IsContained - kiedy pierwsze koło jest całkowicie zawarte w drugim
		/// Identical - kiedy koła pokrywają się
		/// Touches - kiedy koła mają dokładnie jeden punkt wspólny
		/// Crosses - kiedy obwody kół mają dokładnie dwa punkty wspólne
		/// </returns>
		/*    public enum IntersectionType
    {
        Disjoint, DONE
        Contains, DONE
        IsContained, DONE
        Identical, DONE
        Touches,
        Crosses
    }*/
        public IntersectionType GetIntersectionType(Disk other, out Point[] crossingPoints)
        {
            double dX = other.Center.X - this.Center.X;
            double dY = other.Center.Y - this.Center.Y;
            double dist2 = dX * dX + dY * dY;
            double dist = Math.Sqrt(dist2);

			/*
             * tu zajmij się wszystkimi przypadkami wzajemnego położenia kół,
             * oprócz Crosses i Touches
             */
			crossingPoints = new Point[0];
			if (dist - (this.Radius + other.Radius) > Program.epsilon) 
			{
				return IntersectionType.Disjoint;
			}
			if(dist < Program.epsilon && this.Radius - other.Radius < Program.epsilon)
			{
				return IntersectionType.Identical;
			}

			if(this.Radius - other.Radius > Program.epsilon)
			{
				if (this.Radius - dist >= other.Radius - Program.epsilon)
					return IntersectionType.Contains;
			}
			if(other.Radius - this.Radius > Program.epsilon)
			{
				if (other.Radius - dist >= this.Radius - Program.epsilon)
					return IntersectionType.IsContained;
			}
			// odległość od środka aktualnego koła (this) do punktu P,
			// który jest punktem przecięcia odcinka łączącego środki kół (this i other)
			// z odcinkiem łączącym punkty wspólne obwodów naszych kół
			double a = (this.Radius * this.Radius - other.Radius * other.Radius + dist2) / (2 * dist);

            // odległość punktów przecięcia obwodów do punktu P
            double h = Math.Sqrt(this.Radius * this.Radius - a * a);

            // punkt P
            double px = this.Center.X + (dX * a / dist);
            double py = this.Center.Y + (dY * a / dist);

			/*
             * teraz wiesz już wszystko co potrzebne do rozpoznania położenia Touches
             * zajmij się tym
             */
			if (h < Program.epsilon)
			{
				crossingPoints = new Point[1];
				crossingPoints[0] = new Point(px, py);
				return IntersectionType.Touches;
			}
            // przypadek Crosses - dwa punkty przecięcia - już jest zrobiony

            double rX = -dY * h / dist;
            double rY = dX * h / dist;
            
            crossingPoints = new Point[2];
            crossingPoints[0] = new Point(px + rX, py + rY);
            crossingPoints[1] = new Point(px - rX, py - rY);
            return IntersectionType.Crosses;
        }


        /*
         * dopisz wszystkie inne metody, które uznasz za stosowne         
         * 
         */ 

    }

    static class IntersectionFinder
    {

        public static Point? FindCommonPoint(Disk[] disks)
        {
			if (disks == null)
				return null;

			int n = disks.Length;
			// Wstepne sprawdzenie rozłącznych i usunięcie niepotrzebnych
			Point[,] maxRight = new Point[n, n];
			for (int i = 0; i < n; i++)
			{
				Point[] crossingPoints;
				for(int j = 0; j < n; j++)
				{
					if (i == j)
						continue;
					var type = disks[i].GetIntersectionType(disks[j], out crossingPoints);
					if (type == IntersectionType.Disjoint)
						return null;
					else if (type == IntersectionType.Contains || type == IntersectionType.Identical)
						maxRight[i, j] = new Point(disks[j].Center.X + disks[j].Radius, disks[j].Center.Y);
					else if (type == IntersectionType.IsContained)
						maxRight[i,j] = new Point(disks[i].Center.X + disks[i].Radius, disks[i].Center.Y);
					else if (type == IntersectionType.Touches)
						maxRight[i, j] = crossingPoints[0];
					else
					{
						//a crosses to rightmost = skrajnie prawy z lewego, jezeli jego Y jest pomiedzy Y-ami dwóch punktów przecięci
						//aalbo bardziej prawy z obu punktów przecięcia
						Disk leftDisk = (disks[i].Center.X <= disks[j].Center.X) ? disks[i] : disks[j];
						Point rightPoint = new Point(leftDisk.Center.X + leftDisk.Radius, leftDisk.Center.Y);

					}

				}
			}
			
            return null;
        }

    }
}
