using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;

//16. Рассматривая Xi как множество (значения вектора не должны повторяться),
//    найти симметрическую разность объединений (X0 X1 … Xk/2)∆(Xk/2+1 Xr/2+2 … Xk), (где k– количество запущенных процессов).
//    Каждый процесс передает свои значения нулевому, который находит симметрическую разность объединений и выводит результат.

namespace Lab_1
{
    class Program
    {
        private static Random random = new Random();
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator communicator = Communicator.world;
                int rank = communicator.Rank;
                int size = communicator.Size;
                if (size < 2)
                {
                    Console.WriteLine("Необходимо как минимум два процесса!");
                    return;
                }

                //Console.WriteLine("Hello World from rank " + Communicator.world.Rank
                //+ " (running on " + MPI.Environment.ProcessorName + ")");

                if (rank == 0)
                {
                    HashSet<int>[] sets = new HashSet<int>[size - 1];
                    HashSet<int> set;
                    for (int i = 0; i < size - 1; i++)
                    {
                        communicator.Receive(i + 1, 0, out set);
                        sets[i] = set;
                    }
                    HashSet<int> unionSet1 = new HashSet<int>();
                    HashSet<int> unionSet2 = new HashSet<int>();
                    for (int i = 0; i < sets.Length/2; i++)
                    {
                        unionSet1.UnionWith(sets[i]);
                    }
                    Console.WriteLine("1-е объединение: " + String.Join(",", unionSet1));
                    for (int i = sets.Length / 2; i < sets.Length; i++)
                    {
                        unionSet2.UnionWith(sets[i]);
                    }
                    Console.WriteLine("2-е объединение: " + String.Join(",", unionSet2));
                    unionSet1.SymmetricExceptWith(unionSet2);
                    Console.WriteLine("Результат: " + String.Join(",", unionSet1));
                }
                else
                {
                    HashSet<int> set = new HashSet<int>();
                    int lenght = random.Next(5, 10);
                    for (int i = 0; i < lenght; i++)
                    {
                        int value = random.Next(10);
                        set.Add(value);
                    }
                    Console.WriteLine($"Процесс {rank}: " + String.Join(",", set));
                    communicator.Send(set, 0, 0);
                }
            }
        }
    }
}
