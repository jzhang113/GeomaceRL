using Pcg;
using System;

namespace GeomaceRL
{
    public static class RandomExtensions
    {
        public static double NextNormal(this PcgRandom rand, double mean, double variance)
        {
            // Box-Muller transform
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + (Math.Sqrt(variance) * randStdNormal);
        }

        public static int NextBinomial(this PcgRandom rand, int n, double p)
        {
            if (n <= 30)
            {
                // Run Bernouilli samples for small n.
                int successes = 0;
                for (int i = 0; i < n; i++)
                {
                    if (rand.NextDouble() < p)
                        successes++;
                }

                return successes;
            }
            else
            {
                // Approximate with the normal distribution for large n.
                int normal = (int)rand.NextNormal(n * p, n * p * (1 - p));
                if (normal < 0)
                    return 0;
                else if (normal > n)
                    return n;
                else
                    return normal;
            }
        }

        public static double NextGamma(this PcgRandom rand, double a, double b)
        {
            // Marsaliga and Tsang - assumes a > 0
            if (a < 1)
            {
                return rand.NextGamma(1.0 + a, b) * Math.Pow(rand.NextDouble(), 1.0 / a);
            }

            double d = a - (1.0 / 3.0);
            double c = 1.0 / Math.Sqrt(9.0 * d);
            double z, u, v;

            do
            {
                do
                {
                    z = rand.NextNormal(0, 1.0);
                    v = 1.0 + (c * z);
                }
                while (v <= 0);

                v = v * v * v;
                u = rand.NextDouble();

                if (u < 1 - (0.0331 * z * z * z * z))
                    break;
            }
            while (Math.Log(u) >= (0.5 * z * z) + (d * (1 - v + Math.Log(v))));

            return d * v / b;
        }
    }
}
