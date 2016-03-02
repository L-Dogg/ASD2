using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2___dynamic_Cormen
{
	class Program
	{
		static void Main(string[] args)
		{
			Cashier cr = new Cashier();
			Console.WriteLine(cr.getChange(10));
		}
	}
}
