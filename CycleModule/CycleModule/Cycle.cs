using System;
using System.Collections.Generic;
using System.Linq;

namespace CycleModule
{
    // TODO add Alg property and do same calcs as for Label.
    // TODO make Label a single identifier. 
    // TODO add property for turn algorithm and associated calculations (simplifications)
    // TODO add lookup - or do it manually?
    // TODO generate URLs for alg.cubing.net
    public class Cycle
    {
        public Cycle(int[][] a, string label = "", string algorithm = "")
        {
            Value = ValidateAndNormalize(a);
            Label = label ?? throw new NullReferenceException(
                $"{nameof(Label)} can't be null.");
            Algorithm = algorithm ?? throw new NullReferenceException(
                $"{nameof(Algorithm)} can't be null.");
        }

        public Cycle(int[] a, string label = "", string algorithm = "") :
            this(new int[][] { a }, label, algorithm) { }

        public Cycle(IEnumerable<int> e, string label = "", string algorithm = "") :
            this(e.ToArray(), label, algorithm) { }

        public Cycle(IEnumerable<IEnumerable<int>> e, 
            string label = "", string algorithm = "") :
            this(e.Select(e2 => e2.ToArray()).ToArray(), label, algorithm) { }

        public Cycle(string s, string label = "", string algorithm = "") :
            this(s.Split(',').Select(s2 => s2.Select(c => c - 48)), label, algorithm) { }

        public Cycle(int c, string label = "", string algorithm = "") : 
            this(c.ToString(), label, algorithm) { }

        private int[][] Value { get; set; }

        public string Label { get; set; }

        public string Algorithm { get; set; }

        public static Cycle Zero { get; } = new Cycle(new int[][] { });

        private static int[][] ValidateAndNormalize(int[][] aa)
        {
            if (aa.Length == 1 && aa[0].Length == 1)
            {
                if (aa[0][0] != 0)
                {
                    throw new ArgumentException("Must be Zero or have multiple elements.");
                }
            }
            else foreach (var array in aa)
            {
                if (array.Distinct().Count() != array.Length)
                {
                    throw new ArgumentException("Each element must be unique.");
                }

                // This can be relaxed, but would require changes to 
                // the string ctor and ToString.
                if (array.Any(i => i < 1 || i > 9))
                {
                    throw new ArgumentException("Each element must be in the 1-9 range.");
                }
            }
            return aa.Length == 0 ? aa :
                aa.Select(a =>
                {
                    int first = a.OrderBy(v => v).First();
                    bool isNotFirst(int v) => v != first;
                    return a.SkipWhile(isNotFirst).Concat(a.TakeWhile(isNotFirst)).ToArray();
                }).OrderBy(a2 => a2.First()).ToArray();
        }

        public override string ToString() => Value.Length == 0 ? "0" : string.Join(',',
            Value.Select(arr => new string(arr.Select(v => (char)(v + 48)).ToArray())));

        public override bool Equals(object obj) => obj is Cycle && Equals((Cycle)obj);

        public bool Equals(Cycle other) => other is null || ToString() == other.ToString();

        public override int GetHashCode() => ToString().GetHashCode();

        public int After(int v)
        {
            if (Value.Length == 0 || !Value.Any(arr => arr.Contains(v))) return v;
            var array = Value.First(arr => arr.Contains(v));
            var result = array.SkipWhile(i => i != v).Skip(1).FirstOrDefault();
            return result == 0 ? array.First() : result;
        }

        public Cycle Commutate(Cycle b)
        {
            var cycle = this + b - this - b;
            string format(string s1, string s2) => $"[{s1},{s2}]";
            cycle.Label = format(Label, b.Label);
            cycle.Algorithm = format(Algorithm, b.Algorithm); 
            return cycle;
        }

        public Cycle Conjugate(Cycle b)
        {
            var cycle = this + b - this;
            string format(string s1, string s2) => $"[{s1}:{s2}]";
            cycle.Label = format(Label, b.Label);
            cycle.Algorithm = format(Algorithm, b.Algorithm);
            return cycle;
        }

        public static Cycle operator +(Cycle a, Cycle b)
        {
            if (a.Value.Length == 0) return b;
            if (b.Value.Length == 0) return a;

            int next(int v) => b.After(a.After(v));
            IEnumerable<int> cycl(int first)
            {
                var second = next(first);
                if (second != first) 
                {
                    IEnumerable<int> remaining(int v)
                    {
                        var n = next(v);
                        if (n == first) yield break;
                        yield return n;
                        foreach (var v2 in remaining(n)) yield return v2;
                    }
                    yield return first;
                    yield return second;
                    foreach (var r in remaining(second)) yield return r;
                }
            }

            var nums = a.Value.SelectMany(v => v);
            var numsList = nums.ToList();
            var output = new List<int[]>();

            foreach (int v in nums)
            {
                if (!numsList.Contains(v)) continue;
                var array = cycl(v).ToArray();
                if (array.Length > 0) output.Add(array);
                foreach (int v2 in array) numsList.Remove(v2);
            }

            return new Cycle(output.ToArray(), a.Label + b.Label, a.Algorithm + b.Algorithm);
        }

        public static Cycle operator *(Cycle a, int count)
        {
            var cycle = Enumerable.Repeat(a, count).Aggregate((agg, c) => agg + c);
            string format(string s) => s.Length == 1 ? $"{s}{count}" : $"({s}){count}";
            cycle.Label = format(a.Label);
            cycle.Algorithm = format(a.Algorithm);
            return cycle;
        }

        public static Cycle operator -(Cycle a, Cycle b)
        {
            var cycle = a + new Cycle(b.Value.Select(array => array.Reverse()));
            string format(string s1, string s2) => 
                s2.Length == 1 ? $"{s1}{s2}'" : $"{s1}({s2})'";
            cycle.Label = format(a.Label, b.Label);
            cycle.Algorithm = format(a.Algorithm, b.Algorithm);
            return cycle;
        }

        public static Cycle operator -(Cycle a) => Zero - a;

        public static bool operator ==(Cycle cycle1, Cycle cycle2) => cycle1.Equals(cycle2);

        public static bool operator !=(Cycle cycle1, Cycle cycle2) => !(cycle1 == cycle2);
    }
}
