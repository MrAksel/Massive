using Massive.Mathematics.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static ulong[] samples = new ulong[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1, ulong.MaxValue / 2, ulong.MaxValue };

        static uint iterations = (uint)Math.Pow(10, 3);
        static uint inner = 1;

        static void Main(string[] args)
        {
            Console.BufferWidth += 50;
            Console.WindowWidth += 50;

            Natural.DefaultNumberSize = 4;

            Test64Bit64BitDiv();
            Test64BitVariableDiv();
            TestLargeNum(64);
            TestLargeNum(1024);

            TestMethods();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void Test64BitVariableDiv()
        {
            Stopwatch stop = new Stopwatch();
            Random rand = new Random();
            byte[] buff;
            int l;

            for (uint i = 0; i < iterations / inner; i++)
            {
                buff = new byte[8];
                rand.NextBytes(buff);
                buff[7] = (byte)rand.Next(1, 256);
                Natural n1 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                l = rand.Next(1, 8);
                buff = new byte[l];
                rand.NextBytes(buff);
                buff[l - 1] = (byte)rand.Next(1, 256);
                Natural n2 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                stop.Start();
                for (uint j = 0; j < inner; j++)
                {
                    Natural res = n1 / n2;
                }
                stop.Stop();
            }

            long nspTick = 1000L * 1000L * 1000L / Stopwatch.Frequency;
            Console.WriteLine("{0} milliseconds for {1} 64-bit by random size divisions. {2} ticks/div ({3}ns/div, {4}ms/div)", stop.ElapsedMilliseconds, iterations, stop.ElapsedTicks / iterations, (stop.ElapsedTicks * nspTick) / iterations, (stop.ElapsedTicks * nspTick / 1000000L) / iterations);
        }

        private static void Test64Bit64BitDiv()
        {
            Stopwatch stop = new Stopwatch();
            Random rand = new Random();
            byte[] buff = new byte[8];

            for (uint i = 0; i < iterations / inner; i++)
            {
                rand.NextBytes(buff);
                buff[7] = (byte)rand.Next(1, 256);
                Natural n1 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                rand.NextBytes(buff);
                buff[7] = (byte)rand.Next(1, 256);
                Natural n2 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                stop.Start();
                for (uint j = 0; j < inner; j++)
                {
                    Natural res = n1 / n2;
                }
                stop.Stop();
            }

            long nspTick = 1000L * 1000L * 1000L / Stopwatch.Frequency;
            Console.WriteLine("{0} milliseconds for {1} 64 by 64-bit divisions. {2} ticks/div ({3}ns/div, {4}ms/div)", stop.ElapsedMilliseconds, iterations, stop.ElapsedTicks / iterations, (stop.ElapsedTicks * nspTick) / iterations, (stop.ElapsedTicks * nspTick / 1000000L) / iterations);
        }

        private static void TestLargeNum(int bits)
        {
            int size = bits / 8; //Number of bytes

            Stopwatch stop = new Stopwatch();
            Random rand = new Random();
            byte[] buff;
            int l;

            for (uint i = 0; i < iterations / inner; i++)
            {
                buff = new byte[size];
                rand.NextBytes(buff);
                buff[size - 1] = (byte)rand.Next(1, 256);
                Natural n1 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                l = rand.Next(1, size);
                buff = new byte[l];
                rand.NextBytes(buff);
                buff[l - 1] = (byte)rand.Next(1, 256);
                Natural n2 = new Natural(Massive.Helper.ConvertByteArrayToUIntArrayLE(buff));

                stop.Start();
                for (uint j = 0; j < inner; j++)
                {
                    Natural res = n1 / n2;
                }
                stop.Stop();
            }
            long nspTick = 1000L * 1000L * 1000L / Stopwatch.Frequency;
            Console.WriteLine("{0} milliseconds for {1} {5}-bit by random size divisions. {2} ticks/div ({3}ns/div, {4}ms/div)", stop.ElapsedMilliseconds, iterations, stop.ElapsedTicks / iterations, (stop.ElapsedTicks * nspTick) / iterations, (stop.ElapsedTicks * nspTick / 1000000L) / iterations, bits);
        }

        private static void TestMethods()
        {
            Type t = typeof(Massive.Testing.Mathematics.NaturalTests);
            object instance = new Massive.Testing.Mathematics.NaturalTests();

            foreach (MethodInfo m in t.GetMethods(BindingFlags.Default | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    Console.WriteLine("Testing method {0}", m.Name);
                    m.Invoke(instance, null);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        private static string A<T>(T[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in array)
            {
                sb.AppendFormat("{0} ", t);
            }
            return sb.ToString();
        }
    }
}
