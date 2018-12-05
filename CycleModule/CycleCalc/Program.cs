using System;
using System.Collections.Generic;
using System.Linq;
using CycleModule;

namespace CycleCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var coreCycles = new Cycle[]
            {
                new Cycle(13657, "a", "R'FDRFD'F2"), // A
                new Cycle(26374, "b", "[UF:R'FDRFD'F2]"), // B
                new Cycle(14752, "c", "RUFRU'R2F'"), // C
                new Cycle(14362, "d", "[R'FDF',RUFRU'R2F']"), // D
                new Cycle(12357, "e", "FD'F2UF2DRF'U'R'"), // E
                new Cycle(23457, "c2", "UFRUF'U2R'"), // Cxy aka [xy:C] aka c2
                new Cycle(17324, "c3", "FRUFR'F2U'"), // Cy'x' aka [y'x':C] aka c3
                new Cycle("365,241", "g", "R'FDRUFR'D'F'RF'U'"), // G
                new Cycle(13756, "h", "[xyLw:R'FDRFD'F2]"), // H
                new Cycle(456, "k", "F2DRF'U'RUR2D'F'"), // K
                new Cycle("23,47", "m", "FR2UR2D'F2U'F2DF'"), // M
                new Cycle("1657,23", "n", "R'FD2RD'F2UF2D'F'RU'R'"), // N
                new Cycle("34,16", "p", "R'FDRU'R2DR'UR2D2F'R"), // P
                new Cycle("346,157", "q", "R'FDRU'R2DR'UR'D'F'RFR'D'F'"), // Q
                new Cycle(361, "r", "FDRF'U'R'UF2D'F2") // R
            };
            var pairs = coreCycles.SelectMany((c, i) =>
                coreCycles.Skip(i + 1), (a, b) => (a, b));
            var cycles = coreCycles.SelectMany(CycleHelper.Variations)
                .Concat(pairs.SelectMany(pair => pair.a.CombinationsWith(pair.b)));

            // TODO this eliminates haphazardly. Preserve the simplest ones, after simplification.
            var distinctCycles = cycles.DistinctBy(c => c.ToString());

            void report(string target)
            {
                foreach (var cycle in cycles.Where(c => c.ToString() == target)
                    .OrderBy(c => c.ToString().Length))
                {
                    Console.WriteLine(
                        cycle is null ? $"{target} has not been found.\n" :
                        $"{target}: \n{cycle.Label}\n{cycle.Algorithm}\n");
                }
            }

            //report("247");
            //report("46,57");
            report("12647");

            //Console.WriteLine(distinctCycles.Count());

            Console.ReadLine();
        }
    }

    public static class CycleHelper
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            source.GroupBy(keySelector).Select(group => group.First());

        public static IEnumerable<Cycle> Variations(this Cycle cycle)
        {
            yield return cycle;
            yield return cycle * 2;
            yield return -cycle;
            yield return -cycle * 2;
        }

        public static IEnumerable<Cycle> CombinationsWith(this Cycle a, Cycle b)
        {
            var report =
                (a + b).Variations()
                .Concat((b + a).Variations())
                .Concat((a - b).Variations())
                .Concat((-b + a).Variations())
                .Concat(a.Conjugate(b).Variations())
                .Concat((-a).Conjugate(b).Variations())
                .Concat(b.Conjugate(a).Variations())
                .Concat((-b).Conjugate(a).Variations())
                .Concat(a.Commutate(b).Variations())
                .Concat(a.Commutate(-b).Variations())
                .Concat((-a).Commutate(b).Variations())
                .Concat((-a).Commutate(-b).Variations());
            foreach (var r in report) yield return r;
        }
    }
}
