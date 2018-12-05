using Microsoft.VisualStudio.TestTools.UnitTesting;
using CycleModule;

namespace TestCycle
{
    [TestClass]
    public class CycleTests
    {
        [TestMethod]
        public void NormalizeTest()
        {
            var cycle = new Cycle("34512");
            Assert.AreEqual(cycle.ToString(), "12345");
        }

        [TestMethod]
        public void MultiCycleTest()
        {
            var cycle = new Cycle(new int[][] { new int[] { 4, 3 }, new int[] { 2, 1 } });
            Assert.AreEqual(cycle.ToString(), "12,34");
        }

        [TestMethod]
        public void EliminationTest()
        {
            var cycle = new Cycle(new int[][] { new int[] { 4, 3 }, new int[] { 2, 1 } });
            Assert.AreEqual(cycle + cycle, Cycle.Zero);
            Assert.AreEqual(cycle + cycle + cycle, cycle);
            cycle = new Cycle("273849");
            Assert.AreEqual(-cycle + cycle, Cycle.Zero);
        }

        [TestMethod]
        public void ConjugateTest()
        {
            var a = new Cycle("123");
            var b = new Cycle("234");
            Assert.AreEqual(a.Conjugate(b).ToString(), "124");
        }

        [TestMethod]
        public void CommutatorTest()
        {
            var a = new Cycle("123");
            var b = new Cycle("234");
            Assert.AreEqual(a.Commutate(b).ToString(), "14,23");
        }

        [TestMethod]
        public void ZeroTest()
        {
            Assert.AreEqual(Cycle.Zero, Cycle.Zero);
            Assert.AreEqual(Cycle.Zero, new Cycle("0"));
            Assert.AreEqual(Cycle.Zero, new Cycle(new int[][] { }));
        }

        [TestMethod]
        public void MultiplyTest()
        {
            var cycle = new Cycle("12345");
            Assert.AreEqual((cycle * 2).ToString(), "13524");
        }
    }
}
