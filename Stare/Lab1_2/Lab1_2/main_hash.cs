﻿
using System;
using ASD;

class Lab01
{

    static void Main()
    {
        Func<int, int, int>[] hash = { HashTable.ModHash, HashTable.MultiplyHash };
        Func<int, int, int>[] shift = { HashTable.Shift1, HashTable.Shift5, HashTable.Shiftk, HashTable.Shifth };
        string[] hashm = { "haszowanie modulo", "haszowanie multiplikatywne" };
        string[] shiftm = { "sekwencyjne rozwiazywanie kolizji",
                            "liniowe rozwiazywanie kolizji", 
                            "rozw. kolizji z rosnacym krokiem",
                            "rozw. kolizji przy pomocy haszowania dwukrotnego" };
        int cit, cif, cst, csf, crt, crf;

        for (int h = 0; h < 2; ++h)
            for (int s = 0; s < 4; ++s)
            {
                HashTable t = new HashTable(hash[h], shift[s]);
                Random r = new Random(1234);
                cit = cif = cst = csf = crt = crf = 0;
                Console.WriteLine("Lecim");
                for (int i = 0; i < 1000; ++i)
                    switch (r.Next() % 3)
                    {
                        case 0:
                            if (t.Insert(r.Next(50)) == true)
                                ++cit;
                            else
                                ++cif;
                            break;
                        case 1:
                            if (t.Search(r.Next(50)) == true)
								++cst;
                            else
                                ++csf;
                            break;
                        case 2:
                            if (t.Remove(r.Next(50)) == true)
								++crt;
                            else
                                ++crf;
                            break;
                    }
                Console.WriteLine(hashm[h] + " - " + shiftm[s]);
                Console.WriteLine("  {0,5} udanych wstawien      - powinno byc 187,  {1}", cit, cit == 187);
                Console.WriteLine("  {0,5} nieudanych wstawien   - powinno byc 148,  {1}", cif, cif == 148);
                Console.WriteLine("  {0,5} udanych wyszukiwan    - powinno byc 133,  {1}", cst, cst == 133);
                Console.WriteLine("  {0,5} nieudanych wyszukiwan - powinno byc 181,  {1}", csf, csf == 181);
                Console.WriteLine("  {0,5} udanych usuniec       - powinno byc 162,  {1}", crt, crt == 162);
                Console.WriteLine("  {0,5} nieudanych usuniec    - powinno byc 189,  {1}", crf, crf == 189);
                Console.WriteLine("  {0,5} dostepow", t.AccessCount);
            }

        Console.WriteLine();

    }

}
