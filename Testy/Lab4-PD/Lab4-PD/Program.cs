using System;
using System.Threading;

namespace ASD
{
    class TestCase
    {
        static bool catchExceptions = true;
        static double mercyConstant = 2;

        public char[] sequence;
        public char[][] patterns;

        public bool? result;
        public int? crossoutNumber;
        public int? minimumRemainder;

        public TestCase(char[] seq, char[][] pat, bool res, int cn, int mr)
        {
            sequence = seq;
            patterns = pat;
            result = res;
            crossoutNumber = cn;
            minimumRemainder = mr;
        }


        public TestCase(char[] seq, char[][] pat)
        {
            sequence = seq;
            patterns = pat;
            result = null;
            crossoutNumber = null;
            minimumRemainder = null;
        }

        public TestResult TestResults(out string message)
        {
            DateTime t1 = DateTime.Now;
            int b;
            bool x = CrossoutChecker_rozw.Erasable(sequence, patterns, out b);
            if (result == null)
                result = x;
            TimeSpan ts = DateTime.Now - t1;
            bool res = false;
            int w;
            TestResult ret = TestResult.OK;
            string msg = "OK";
            Thread th = new Thread(() =>
            {
                if (catchExceptions)
                {
                    try
                    {
                        res = CrossoutChecker.Erasable(sequence, patterns, out w);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        ret = TestResult.Exception;
                        msg = "Wyjatek!";
                        return;
                    }
                }
                else
                    res = CrossoutChecker.Erasable(sequence, patterns, out w);
                if (res != result)
                {
                    ret = TestResult.BadResult;
                    msg = "BLAD: jest " + res.ToString() + " a powinno byc " + result.ToString();
                }
            });
            th.Start();
            if (!th.Join((int)(ts.TotalMilliseconds * mercyConstant) + 50))
            {
                ret = TestResult.Timeout;
                msg = "Timeout!";
                th.Abort();
            }
            th.Join();
            message = msg;
            return ret;
        }

        public TestResult TestCrossoutsNumber(out string message)
        {
            DateTime t1 = DateTime.Now;
            int b;
            CrossoutChecker_rozw.Erasable(sequence, patterns, out b);
            if (crossoutNumber == null)
                crossoutNumber = b;
            TimeSpan ts = DateTime.Now - t1;
            bool res;
            int w = -1;
            TestResult ret = TestResult.OK;
            string msg = "OK";
            Thread th = new Thread(() =>
            {
                if (catchExceptions)
                {
                    try
                    {
                        res = CrossoutChecker.Erasable(sequence, patterns, out w);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        ret = TestResult.Exception;
                        msg = "Wyjatek!";
                        return;
                    }
                }
                else
                    res = CrossoutChecker.Erasable(sequence, patterns, out w);
                if (w != crossoutNumber)
                {
                    ret = TestResult.BadCrossoutsNumber;
                    msg = "BLAD: jest " + w.ToString() + " a powinno byc " + crossoutNumber.ToString();
                }
            });
            th.Start();
            if (!th.Join((int)(ts.TotalMilliseconds * mercyConstant) + 50))
            {
                ret = TestResult.Timeout;
                msg = "Timeout!";
                th.Abort();
            }
            th.Join();
            message = msg;
            return ret;
        }

        public TestResult TestMinimumRemainder(out string message)
        {
            DateTime t1 = DateTime.Now;
            int x = CrossoutChecker_rozw.MinimumRemainder(sequence, patterns);
            if (minimumRemainder == null)
                minimumRemainder = x;
            TimeSpan ts = DateTime.Now - t1;
            int w = -1;
            TestResult ret = TestResult.OK;
            string msg = "OK";
            Thread th = new Thread(() =>
            {
                if (catchExceptions)
                {
                    try
                    {
                        w = CrossoutChecker.MinimumRemainder(sequence, patterns);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        ret = TestResult.Exception;
                        msg = "Wyjatek!";
                        return;
                    }
                }
                else
                    w = CrossoutChecker.MinimumRemainder(sequence, patterns);
                if (w != minimumRemainder)
                {
                    ret = TestResult.BadMinimumRemainder;
                    msg = "BLAD: jest " + w.ToString() + " a powinno byc " + minimumRemainder.ToString();
                }
            });
            th.Start();
            if (!th.Join((int)(ts.TotalMilliseconds * mercyConstant) + 50))
            {
                ret = TestResult.Timeout;
                msg = "Timeout!";
                th.Abort();
            }
            th.Join();
            message = msg;
            return ret;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool zajeciaok = true, etap1ok = true, etap2ok = true, etap3ok = true;
            bool etap1zzajecok = true;
            TestCase[] testyZZajec =
            {
                makeTestCase(true, 1, 0, "()", "()"),
                makeTestCase(false, int.MaxValue, 1, "())", "()"),
                makeTestCase(true, 8, 0, "(()(()()))()(())", "()"),
                makeTestCase(false, int.MaxValue, 3, "())())(()", "()"),
                makeTestCase(true, 10, 0, "()?..().) (.)(.)", ".", "()", " ", ")?", ".."),
                makeTestCase(false, int.MaxValue, 3, "abcdefgabcdefgabcdefg", "ab", "bc", "cd", "de", "ef", "fg"),
                makeTestCase(true, 5, 0, "xAAAxBxC", "x", "xA", "xB", "xC", "AB", "AC"),
                makeTestCase(false, int.MaxValue, 10, "(xAAxBxC)(xAAxBxC)(xAAxBxC)(xAAxBxC)(xAAxBxC)", "x", "xA", "xB", "xC", "AB", "AC"),
                makeTestCase(true, 250, 0, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "AA", "A"),
                makeTestCase(false, int.MaxValue, 1, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "AA"),
            };

            int[] wynikiZZajec = new int[(int)TestResult.BadMinimumRemainder + 1];

            Console.WriteLine("********************** Testy z zajec **********************");

            Console.WriteLine("Wynik Erasable:");
            int i = 0;
            foreach (TestCase tc in testyZZajec)
            {
                string msg;
                wynikiZZajec[(int)tc.TestResults(out msg)]++;
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }
            if (wynikiZZajec[(int)TestResult.OK] < testyZZajec.GetLength(0))
                etap1zzajecok = false;

            Console.WriteLine("\n\nOptymalna liczba skreslen:");
            i = 0;
            foreach (TestCase tc in testyZZajec)
            {
                string msg;
                wynikiZZajec[(int)tc.TestCrossoutsNumber(out msg)]++;
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }

            Console.WriteLine("\n\nMinimalna liczba nieskreslonych syboli:");
            i = 0;
            foreach (TestCase tc in testyZZajec)
            {
                string msg;
                wynikiZZajec[(int)tc.TestMinimumRemainder(out msg)]++;
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }

            if(wynikiZZajec[(int)TestResult.OK]!=3*testyZZajec.GetLength(0))
            {
                Console.WriteLine("\n\nWynik: 0p (testy z zajec nie przechodza!)\n");
                zajeciaok = false;
            }

            Console.WriteLine("\n************************ Nowe testy ***********************");

            TestCase[] NoweTesty =
            {
                makeTestCase(true, 200, 0, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB","AB"),
                makeTestCase(true, 50, 0, "()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()()", "()"),
                makeTestCase(true,5, 0, "XXAAAAABBB","A", "B", "AB", "XA"),
                makeTestCase(true,9,0,"SSXXAAAAABBBAA", "A", "B","AB", "XB", "SA", "SS", "S"),
                makeTestCase(true,5,0,"XYDXBCAB","XB","Y","A","XD","CB","D","C"),
                makeTestCase(true,4, 0,"XZRGABG","X","XB","AB","GG","RG","ZR","XG"),
                makeTestCase(false,int.MaxValue,4,"(((((())))))xxxxxxyy","()",")x","(y"),
                makeTestCase(false,int.MaxValue,2, "(())xxyzww[[]]","()",")x","(y","w[","z]","yz","[]"),
                makeTestCase(false, int.MaxValue, 10, "(())xxyzww[[]](())xxyzww[[]](())xxyzww[[]](())xxyzww[[]](())xxyzww[[]]","()",")x","(y","w[","z]","yz","[]"),
                makeTestCase(false,int.MaxValue,10, "(())xxy(())xxy(())xxy(())xxy(())xxyzww[[]]zww[[]]zww[[]]zww[[]]zww[[]]","()",")x","(y","w[","z]","yz","[]"),
                makeTestCase(false,int.MaxValue,2,"yyyyy(((())))xxxxx","()","y(",")x"),
                makeTestCase(false,int.MaxValue,1,"yyy((((())))xxx","()","y(",")x"),
                makeRandomTestCase(200,5,false),
                makeRandomTestCase(200,5,true),
                makeRandomTestCase(500,3,false),
                makeRandomTestCase(500,3,true)
            };

            bool[] bylblad = new bool[NoweTesty.GetLength(0)];
            int bledywnowych = 0;


            int[] wynikiZNowych = new int[(int)TestResult.BadMinimumRemainder + 1];

            i = 0;
            int prevok = 0;
            Console.WriteLine("Wynik Erasable:");
            foreach (TestCase tc in NoweTesty)
            {
                string msg;
                TestResult tr = tc.TestResults(out msg);
                wynikiZNowych[(int)tr]++;
                if(tr!=TestResult.OK&& !bylblad[i])
                {
                    bylblad[i] = true;
                    bledywnowych++;
                }
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }
            if (wynikiZNowych[(int)TestResult.OK] < NoweTesty.GetLength(0))
                etap1ok = false;
            prevok = wynikiZNowych[(int)TestResult.OK];

            i = 0;
            Console.WriteLine("\n\nOptymalna liczba skreslen:");
            foreach (TestCase tc in NoweTesty)
            {
                string msg;
                TestResult tr = tc.TestCrossoutsNumber(out msg);
                wynikiZNowych[(int)tr]++;
                if (tr != TestResult.OK && !bylblad[i])
                {
                    bylblad[i] = true;
                    bledywnowych++;
                }
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }
            if (wynikiZNowych[(int)TestResult.OK] - prevok < NoweTesty.GetLength(0))
                etap2ok = false;
            prevok = wynikiZNowych[(int)TestResult.OK];

            i = 0;
            Console.WriteLine("\n\nMinimalna liczba nieskreslonych syboli:");
            foreach (TestCase tc in NoweTesty)
            {
                string msg;
                TestResult tr = tc.TestMinimumRemainder(out msg);
                wynikiZNowych[(int)tr]++;
                if (tr != TestResult.OK && !bylblad[i])
                {
                    bylblad[i] = true;
                    bledywnowych++;
                }
                Console.WriteLine("Test " + (i++).ToString() + ": \t" + msg);
            }
            if (wynikiZNowych[(int)TestResult.OK] - prevok < NoweTesty.GetLength(0))
                etap3ok = false;

            Console.WriteLine("\nPodsumowanie nowych testow:");
            for(int ii=0;ii<=(int)TestResult.BadMinimumRemainder;ii++)
            {
                Console.WriteLine(wynikiZNowych[ii].ToString() + " wynikow " + ((TestResult)ii).ToString());
            }

            double oc = 0;
            oc += (etap1zzajecok ? 0.5 + (zajeciaok ? 0.5 : 0) : 0);
            if (etap1ok || bledywnowych <= 4)
                oc += 0.5;
            if (etap1ok && etap2ok && etap3ok)
                oc += 0.5;
            Console.WriteLine("Ocena = " + oc.ToString());
        }

        static Random rand = new Random(13);

        static TestCase makeRandomTestCase(int length, int symbols, bool allowSingleDeletes)
        {
            char[] seq = new char[length];
            char[][] pat = new char[allowSingleDeletes ? 2 * symbols : symbols][];
            int j = 0;
            if (allowSingleDeletes)
                for (int i = 0; i < symbols; i++)
                {
                    pat[j] = new char[1];
                    pat[j][0] = (char)((int)'A' + i);
                    j++;
                }
            for (int i = 0; i < symbols - 1; i++)
            {
                pat[j] = new char[2];
                pat[j][0] = (char)((int)'A' + i);
                pat[j][1] = (char)((int)'A' + i + 1);
                j++;
            }
            pat[j] = new char[2];
            pat[j][0] = (char)((int)'A' + symbols - 1);
            pat[j][1] = 'A';
            for (int i = 0; i < length; i++)
                seq[i] = (char)((int)'A' + rand.Next(symbols));
            return new TestCase(seq, pat);
        }

        static TestCase makeTestCase(bool result, int minCrossouts, int minRemainder, string sequence, params string[] patterns)
        {
            char[] seq = new char[sequence.Length];
            for (int i = 0; i < sequence.Length; i++)
                seq[i] = sequence[i];
            char[][] pat = new char[patterns.GetLength(0)][];
            for (int i = 0; i < patterns.GetLength(0); i++)
            {
                pat[i] = new char[patterns[i].Length];
                for (int j = 0; j < patterns[i].Length; j++)
                    pat[i][j] = patterns[i][j];
            }
            return new TestCase(seq, pat, result, minCrossouts, minRemainder);
        }

        static TestCase makeTestCase(string sequence, params string[] patterns)
        {
            char[] seq = new char[sequence.Length];
            for (int i = 0; i < sequence.Length; i++)
                seq[i] = sequence[i];
            char[][] pat = new char[patterns.GetLength(0)][];
            for (int i = 0; i < patterns.GetLength(0); i++)
            {
                pat[i] = new char[patterns[i].Length];
                for (int j = 0; j < patterns[i].Length; j++)
                    pat[i][j] = patterns[i][j];
            }
            return new TestCase(seq, pat);
        }
    }

    enum TestResult
    {
        OK, Timeout, Exception, BadResult, BadCrossoutsNumber, BadMinimumRemainder
    }

    
}