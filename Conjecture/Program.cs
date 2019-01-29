using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conjecture
{
    class Program
    {
        const decimal sqrt2 = 1.4142135623730950488016887M;
        static void Main(string[] args)
        {
            
            decimal count = 0;
            decimal num = 0;
            decimal maxNum = 0;
            int threadsToAllocate = 0;
            bool threadsAllocated = false;
            bool startSet = false;
            bool endSet = false;

            Console.WriteLine($"Detected {Environment.ProcessorCount} processors on this system.");
            Console.WriteLine("If your system is hyperthreaded, you will need to divide that by 2 for your number of real cores.");
            Console.WriteLine("This task is computationally heavy, so it is not advised to allocate more threads than you have");
            Console.WriteLine("physical cores.\r\n");
            while (!threadsAllocated)
            {
                Console.Write("How many threads would you like to allocate? ");
                threadsAllocated = int.TryParse(Console.ReadLine(), out threadsToAllocate);
            }
            while (!startSet)
            {
                Console.Write("Enter start number: ");
                startSet = decimal.TryParse(Console.ReadLine(), out num);
            }
            while (!endSet)
            {
                Console.Write("Enter end number: ");
                endSet = decimal.TryParse(Console.ReadLine(), out maxNum);
            }

            if (maxNum < num)
            {
                Console.WriteLine("ERROR: Starting number is larger than ending number!");
                return;
            }

            decimal range = maxNum - num;
            decimal interval = Math.Floor(range / threadsToAllocate);
            Console.WriteLine($"\r\nVerifying {range} numbers on {threadsToAllocate} threads.");
            Console.WriteLine($"Each thread will verify {interval} numbers.\r\n");

            var start = DateTime.Now;
            List<Task<decimal>> tasks = new List<Task<decimal>>();
            for (int i = 0; i < threadsToAllocate; i++)
            {
                decimal rangeEnd = 0;
                if (i == threadsToAllocate - 1)
                {
                    rangeEnd = maxNum;
                }
                else
                {
                    rangeEnd = num + interval - 1;
                }
                Console.WriteLine($"Thread {i}: {num}-{rangeEnd}");
                tasks.Add(VerifyRange(num, rangeEnd));
                num += interval;
            }

            Console.WriteLine("\r\nCreated worker threads. Please wait...");
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                count += task.Result;
            }
            var duration = DateTime.Now - start;
            Console.WriteLine($"Verified {count.ToString("N")} in {duration.ToString()}");
            Console.ReadLine();
        }

        private static async Task<decimal> VerifyRange(decimal start, decimal end)
        {
            decimal count = 0;
            await Task.Run(() =>
            {
                while (start <= end)
                {
                    if (start == SetA(InverseA(start)) || start == SetB(InverseB(start)))
                    {
                        start = start + 1;
                        count = count + 1;
                    }
                    else
                    {
                        Console.WriteLine(start);
                        start = start + 1;
                    }
                }
            });
            return count;
        }

        private static decimal SetA(decimal num)
        {
            return Math.Floor(num * sqrt2);
        }

        private static decimal SetB(decimal num)
        {
            return Math.Floor(num * (decimal)(2 + sqrt2));
        }

        private static decimal InverseA(decimal a)
        {
            return Math.Ceiling(a / sqrt2);
        }

        private static decimal InverseB(decimal b)
        {
            return Math.Ceiling(b / (2+sqrt2));
        }
    }
}
