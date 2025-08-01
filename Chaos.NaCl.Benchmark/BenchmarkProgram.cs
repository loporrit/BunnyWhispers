﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;

namespace Chaos.NaCl.Benchmark
{
    public class BenchmarkProgram
    {
        public const int CpuFreq = 5000;

        static void Benchmark(string name, Action action, int n, int bytes = 0)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(name);
            Console.ForegroundColor = ConsoleColor.Gray;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var watch = new Stopwatch();
            var start = DateTime.UtcNow;
            var values = new float[n];
            for (int i = 0; i < n; i++)
            {
                watch.Restart();
                action();
                watch.Stop();
                double thisIteration = watch.Elapsed.TotalSeconds;
                values[i] = (float)thisIteration;
            }
            var total = (DateTime.UtcNow - start).TotalSeconds;
            var perIteration = total / n;
            Array.Sort(values);
            double sum = values.Sum();
            double sumOfSquares = values.Sum(x => x * x);
            double average = sum / n;
            double stdDev = Math.Sqrt(sumOfSquares / n - average * average);
            double median = values[n / 2];
            double min = values.Min();
            double max = values.Max();

            double low90 = values[n / 10];
            double high90 = values[n - 1 - n / 10];
            double delta90 = (high90 - low90) / 2;
            double relativeDelta90 = delta90 / median;
            double average90 = values.Where(x => (x >= low90) && (x <= high90)).Average();

            double low75 = values[n / 4];
            double high75 = values[n - 1 - n / 4];
            double delta75 = (high75 - low75) / 2;
            double relativeDelta75 = delta75 / median;
            double average75 = values.Where(x => (x >= low75) && (x <= high75)).Average();

            Console.WriteLine("{0} us / {1} per second",
                Math.Round(average90 * 1E6, 2), Math.Round(1 / average90));
            Console.WriteLine("Average {0} us, Median {1} us, min {2}, max {3}", Math.Round(average * 1E6, 2),
                              Math.Round(median * 1E6, 2), Math.Round(min * 1E6, 2), Math.Round(max * 1E6, 2));
            Console.WriteLine("80% within ±{0}% average {1} | 50% within ±{2}% average {3}",
                Math.Round(relativeDelta90 * 100, 2), Math.Round(average90 * 1E6, 2),
                Math.Round(relativeDelta75 * 100, 2), Math.Round(average75 * 1E6, 2));
            if (bytes > 0)
            {
                double bytesPerSecond = bytes / average90;
                Console.WriteLine("{0} MB/s",
                    Math.Round(bytesPerSecond / 1E6, 2));
            }
            Console.WriteLine();
        }

        public static void Main()
        {
            const int n = 10000;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Console.WriteLine("Architecture: {0} bit", IntPtr.Size * 8);
            Console.WriteLine("Assumed CPU-Frequency: {0} MHz", CpuFreq);
            Console.WriteLine();

            var m = new byte[100];
            var seed = new byte[32];
            byte[] privateKey;
            byte[] publicKey;
            Ed25519.KeyPairFromSeed(out publicKey, out privateKey, seed);
            var sig = Ed25519.Sign(m, privateKey);
            Ed25519.Sign(m, privateKey);

            if (!Ed25519.Verify(sig, m, publicKey))
                throw new Exception("Bug");
            if (Ed25519.Verify(sig, m.Concat(new byte[] { 1 }).ToArray(), publicKey))
                throw new Exception("Bug");

            Console.BackgroundColor = ConsoleColor.Black;

            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== Edwards ===");
                Benchmark("KeyGen", () => Ed25519.KeyPairFromSeed(out publicKey, out privateKey, seed), n);
                Benchmark("Sign", () => Ed25519.Sign(m, privateKey), n);
                Benchmark("Verify", () => Ed25519.Verify(sig, m, publicKey), n);
                Benchmark("KeyExchange", () => Ed25519.KeyExchange(publicKey, privateKey), n);
                Console.WriteLine();
            }

            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== Montgomery ===");
                Benchmark("KeyGen", () => MontgomeryCurve25519.GetPublicKey(seed), n);
                Benchmark("KeyExchange", () => MontgomeryCurve25519.KeyExchange(publicKey, seed), n);
                Console.WriteLine();
            }
        }

        private static string SizeToString(int size)
        {
            if (size > 2048)
                return String.Format("{0} KiB", size / 1024);
            else
                return String.Format("{0} B", size);
        }
    }
}
