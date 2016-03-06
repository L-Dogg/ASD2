
using System;
using ASD.Graphs;

class Test
{

public static void Main()
    {
    Graph[] g = new Graph[4];
    string[] m = new string[] { "Matrix    " ,
                                "HashTable " ,
                                "SimplyList" ,
                                "AVL tree  " };
    int s, n=2000;
    ulong c1,c2;
    DateTime t1,t2;

    RandomGraphGenerator rgg = new RandomGraphGenerator(12345);

// Elementy tabgicy g zawierają ten sam graf w różnych reprezentacjach
    g[0] = rgg.DirectedGraph(typeof(AdjacencyMatrixGraph),n,0.05,-99,99);
    g[1] = new AdjacencyListsGraph<HashTableAdjacencyList>(g[0]);
    g[2] = new AdjacencyListsGraph<SimplyAdjacencyList>(g[0]);
    g[3] = new AdjacencyListsGraph<AVLAdjacencyList>(g[0]);

// Liczymy sumę wag wszystkich krawędzi grafu

// Sposób 1 - najlepszy, korzystamy jedynie z metody OutEdges
    Console.WriteLine();
    for ( int i=0 ; i<g.Length ; ++i )
        {
        c1=Graph.Counter;
        t1=DateTime.Now;
        s=0;
        for ( int v=0 ; v<n ; ++v )
            foreach ( var e in g[i].OutEdges(v) )
                s+=e.Weight;
        t2=DateTime.Now;
        c2=Graph.Counter;
        Console.WriteLine(" A {0} # suma {1}  -  licznik:  {2,11} , czas:  {3}",m[i], s, c2-c1, t2-t1);
        }

// Sposób 2 - gorszy, korzystamy z metod OutEdges i GetEdgeWeight
    Console.WriteLine();
    for ( int i=0 ; i<g.Length ; ++i )
        {
        c1=Graph.Counter;
        t1=DateTime.Now;
        s=0;
        for ( int v=0 ; v<n ; ++v )
            foreach ( var e in g[i].OutEdges(v) )
                s+=(int)g[i].GetEdgeWeight(e.From,e.To);
        t2=DateTime.Now;
        c2=Graph.Counter;
        Console.WriteLine(" B {0} # suma {1}  -  licznik:  {2,11} , czas:  {3}",m[i], s, c2-c1, t2-t1);
        }

// Sposób 3 - najgorszy, korzystamy jedynie z metody GetEdgeWeight - nie robić tak!
    Console.WriteLine();
    int? w;
    for ( int i=0 ; i<g.Length ; ++i )
        {
        c1=Graph.Counter;
        t1=DateTime.Now;
        s=0;
        for ( int v=0 ; v<n ; ++v )
            for ( int vv=0 ; vv<n ; ++vv )
                {
                w=g[i].GetEdgeWeight(v,vv);
                if ( w!=null ) s+=(int)w;
                }
        t2=DateTime.Now;
        c2=Graph.Counter;
        Console.WriteLine(" C {0} # suma {1}  -  licznik:  {2,11} , czas:  {3}",m[i], s, c2-c1, t2-t1);
        }

    Console.WriteLine();
    }

}