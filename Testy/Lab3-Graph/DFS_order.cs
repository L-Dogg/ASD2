
using System;
using ASD.Graphs;

class Test
    {

    public static void Main ()
        {
        Graph  g1, g2;
        ulong  licz;
        int    cc;
        int    orda = 0;
        int[]  ord  = null;  // kolejnosc odwiedzania wierzcholkow

        // metoda wykorzystywana do numerowania wierzchołków
        Predicate<int> vv = delegate(int n)
            {
            Console.WriteLine(n);
            ord[n]=++orda;
            return true;
            };

        // przykładowy graf w reprezentacji macierzowej
        g1 = new AdjacencyMatrixGraph(false,15);
        g1.AddEdge(0,1,3);
        g1.AddEdge(0,4,5);
        g1.AddEdge(1,4,6);
        g1.AddEdge(1,5,4);
        g1.AddEdge(2,6,5);
        g1.AddEdge(4,7,8);
        g1.AddEdge(4,8,2);
        g1.AddEdge(4,10,12);
        g1.AddEdge(5,8,1);
        g1.AddEdge(5,9,7);
        g1.AddEdge(6,11,4);
        g1.AddEdge(6,14,5);
        g1.AddEdge(7,8,9);
        g1.AddEdge(8,12,3);
        g1.AddEdge(10,12,2);
        g1.AddEdge(10,13,3);
        Graph.Write(g1,"graf15.xml");
        Console.WriteLine();

        Console.WriteLine("algorytm rekurencyjny");
        Console.WriteLine("przeszukiwanie grafu w głąb i numerowanie wierzchołków");
        Console.WriteLine("graf w reprezentacji macierzowej");
        Console.WriteLine();
        ord = new int[g1.VerticesCount];
        orda = 0;
        licz=Graph.Counter;
        g1.DFSearchAll(vv,null,out cc);
        Console.WriteLine("\nSpojne skladowe:  {0}",cc);
        Console.WriteLine("Liczba odwolan:  {0,3}\n",Graph.Counter-licz);
        for ( int i=0 ; i<g1.VerticesCount ; ++i )
            Console.WriteLine("  {0}  {1}",i,ord[i]);
        Console.WriteLine();
        
        // ten sam graf w reprezentacji listowej (tablice haszowane)
        g2 = Graph.Read(typeof(AdjacencyListsGraph<HashTableAdjacencyList>),"Graf15.xml");

        Console.WriteLine("algorytm rekurencyjny");
        Console.WriteLine("przeszukiwanie grafu w głąb i numerowanie wierzchołków");
        Console.WriteLine("graf w reprezentacji listowej (tablice haszowane)");
        Console.WriteLine();
        ord = new int[g2.VerticesCount];
        orda = 0;
        licz=Graph.Counter;
        g2.DFSearchAll(vv,null,out cc);
        Console.WriteLine("\nSpojne skladowe:  {0}",cc);
        Console.WriteLine("Liczba odwolan:  {0,3}\n",Graph.Counter-licz);
        for ( int i=0 ; i<g2.VerticesCount ; ++i )
            Console.WriteLine("  {0}  {1}",i,ord[i]);
        Console.WriteLine();

        Console.WriteLine("algorytm ogólny z wykorzystaniem stosu");
        Console.WriteLine("przeszukiwanie grafu w głąb i numerowanie wierzchołków");
        Console.WriteLine("graf w reprezentacji macierzowej");
        Console.WriteLine();
        ord = new int[g1.VerticesCount];
        orda = 0;
        licz=Graph.Counter;
        g1.GeneralSearchAll<EdgesStack>(vv,null, out cc);
        Console.WriteLine("\nSpojne skladowe:  {0}",cc);
        Console.WriteLine("Liczba odwolan:  {0,3}\n",Graph.Counter-licz);
        for ( int i=0 ; i<g1.VerticesCount ; ++i )
            Console.WriteLine("  {0}  {1}",i,ord[i]);
        Console.WriteLine();

        // wyświetlenie grafu
        GraphExport ge = new GraphExport();
        ge.Export(g1);
        }

    }
