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
			Console.WriteLine("Get Change: {0}", cr.getChange(10));

			RodCutting rc = new RodCutting();
			Console.WriteLine("Rod Cutting: {0}", rc.cutRod(9));

			MatrixChainOrder mco = new MatrixChainOrder();
			int[] dims = { 35, 15, 5, 10, 20 };
            Console.WriteLine("Matrix Parens Order: {0}", mco.MatrixChain(dims));
			Console.WriteLine();
			mco.printParens(1, dims.Length - 1);
		}
	}
}
