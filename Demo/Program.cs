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

        static void Main(string[] args)
        {
            Natural.DefaultNumberSize = 4;

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

            Console.ReadLine();
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
