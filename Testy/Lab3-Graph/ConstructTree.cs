
using System;
using ASD.Graphs;

class Test
    {

    public static void Main ()
        {
        Graph g, t1, t2, t3, t4;
        int cc;
        int treeweight=0;
        bool[] visited = null;
        int[] from = null;
        Graph tree = null;
        string msg;

        // metoda wykorzystywana do konstruowania drzewa
        // związanego z odpowiednim algorytmem przeszukiwania
        Predicate<Edge> ConstructTree = delegate (Edge e)
             {
             if ( !visited[e.From] )
                 visited[e.From]=true;
             if ( !visited[e.To] )
                 {
                 treeweight+=e.Weight;
                 tree.AddEdge(e);
                 visited[e.To]=true;
                 }
             return true;
             };

        // metoda wykorzystywana do sprawdzania
        // czy dany graf jest drzewem
        Predicate<Edge> IsTree = delegate (Edge e)
             {
             if ( !visited[e.From] )
                 {
                 visited[e.From]=true;
                 from[e.From]=e.From;
                 }
             if ( visited[e.To] && e.To!=from[e.From] )
                 return false;
             visited[e.To]=true;
             from[e.To]=e.From;
             return true;
             };

        // przykładowy graf
        g = new AdjacencyMatrixGraph(false,15);
        g.AddEdge(0,1,3);
        g.AddEdge(0,4,5);
        g.AddEdge(1,4,6);
        g.AddEdge(1,5,4);
        g.AddEdge(2,6,5);
        g.AddEdge(4,7,8);
        g.AddEdge(4,8,2);
        g.AddEdge(4,10,12);
        g.AddEdge(5,8,1);
        g.AddEdge(5,9,7);
        g.AddEdge(6,11,4);
        g.AddEdge(6,14,5);
        g.AddEdge(7,8,9);
        g.AddEdge(8,12,3);
        g.AddEdge(10,12,2);
        g.AddEdge(10,13,3);

        // konstruowanie drzewa związanego z przeszukiwaniem w głąb
        // (algorytm korzysta ze stosu krawędzi)
        treeweight=0;
        visited = new bool[g.VerticesCount];
        tree = g.IsolatedVerticesGraph();
        g.GeneralSearchAll<EdgesStack>(null,ConstructTree, out cc);
        t1=tree;
        Console.WriteLine("Stos        - waga drzewa: {0,3}",treeweight);

        // konstruowanie drzewa związanego z przeszukiwaniem wszerz
        // (algorytm korzysta z kolejki krawędzi)
        treeweight=0;
        visited = new bool[g.VerticesCount];
        tree = g.IsolatedVerticesGraph();
        g.GeneralSearchAll<EdgesQueue>(null,ConstructTree, out cc);
        t2=tree;
        Console.WriteLine("Kolejka     - waga drzewa: {0,3}",treeweight);

        // konstruowanie minimalnego drzewa rozpinającego
        // (algorytm korzysta z kolejki priorytetowej krawędzi, priorytet minimalna waga)
        // jest to algorytm Prima
        treeweight=0;
        visited = new bool[g.VerticesCount];
        tree = g.IsolatedVerticesGraph();
        g.GeneralSearchAll<EdgesMinPriorityQueue>(null,ConstructTree, out cc);
        t3=tree;
        Console.WriteLine("Kolejka Min - waga drzewa: {0,3}",treeweight);

        // konstruowanie maksymalnego drzewa rozpinającego
        // (algorytm korzysta z kolejki priorytetowej krawędzi, priorytet maksymalna waga)
        treeweight=0;
        visited = new bool[g.VerticesCount];
        tree = g.IsolatedVerticesGraph();
        g.GeneralSearchAll<EdgesMaxPriorityQueue>(null,ConstructTree, out cc);
        t4=tree;
        Console.WriteLine("Kolejka Max - waga drzewa: {0,3}",treeweight);

        // sprawdzanie czy wyjściowy graf jest drzewem - nie jest
        visited = new bool[g.VerticesCount];
        from = new int[g.VerticesCount];
        msg = g.GeneralSearchAll<EdgesQueue>(null,IsTree, out cc) ? "jest" : "nie jest" ;
        Console.WriteLine("Graf g {0} drzewem (lasem)",msg); 

        // potwierdzenie, że pierwsze skonstruowane drzewo rzeczywiście jest drzewem
        visited = new bool[g.VerticesCount];
        from = new int[g.VerticesCount];
        msg = t1.GeneralSearchAll<EdgesQueue>(null,IsTree, out cc) ? "jest" : "nie jest" ;
        Console.WriteLine("Graf t1 {0} drzewem (lasem)",msg); 

        // potwierdzenie, że drugie skonstruowane drzewo rzeczywiście jest drzewem
        visited = new bool[g.VerticesCount];
        from = new int[g.VerticesCount];
        msg = t2.GeneralSearchAll<EdgesQueue>(null,IsTree, out cc) ? "jest" : "nie jest" ;
        Console.WriteLine("Graf t2 {0} drzewem (lasem)",msg); 

        // potwierdzenie, że trzecie skonstruowane drzewo rzeczywiście jest drzewem
        visited = new bool[g.VerticesCount];
        from = new int[g.VerticesCount];
        msg = t3.GeneralSearchAll<EdgesQueue>(null,IsTree, out cc) ? "jest" : "nie jest" ;
        Console.WriteLine("Graf t3 {0} drzewem (lasem)",msg); 

        // potwierdzenie, że czwarte skonstruowane drzewo rzeczywiście jest drzewem
        visited = new bool[g.VerticesCount];
        from = new int[g.VerticesCount];
        msg = t4.GeneralSearchAll<EdgesQueue>(null,IsTree, out cc) ? "jest" : "nie jest" ;
        Console.WriteLine("Graf t4 {0} drzewem (lasem)",msg); 

        // wyświetlanie grafu wyjściowego i skonstruowanych drzew
        GraphExport ge = new GraphExport();
        ge.Export(g,null,"Graph");
        ge.Export(t1,null,"StackTree");
        ge.Export(t2,null,"QueueTree");
        ge.Export(t3,null,"MinQueueTree");
        ge.Export(t4,null,"MaxQueueTree");
        }

    }
