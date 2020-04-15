using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnamSoft.DependencyGraph.Tests
{
    [TestClass]
    public class NonCyclicDependencyGraphTests : DependencyGraphTests
    {
        [TestInitialize]
        public void Init()
        {
            Graph = new DependencyGraph<string>(false);
            Assert.IsTrue(Graph.IsEmpty);
        }

        [TestMethod]
        public void CyclicDependencyTest()
        {
            Graph.AddDependency("1", "2");
            Graph.AddDependency("2", "3");
            var res = Graph.AddDependency("3", "1");

            Assert.IsFalse(res);

            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDepends("2", "3"));
            Assert.IsFalse(Graph.IsDepends("3", "1"));
            Assert.IsFalse(Graph.IsDepends("1", "1"));
            Assert.IsFalse(Graph.HasCyclic());

            var allDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(2, allDependees.Count);
            Assert.IsFalse(allDependees.Contains("1"));
            Assert.IsTrue(allDependees.Contains("2"));
            Assert.IsTrue(allDependees.Contains("3"));

            res = Graph.RemoveDependency("3", "1");

            Assert.IsFalse(res);
            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDepends("2", "3"));
            Assert.IsFalse(Graph.IsDepends("3", "1"));

            allDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(2, allDependees.Count);
            Assert.IsFalse(allDependees.Contains("1"));
            Assert.IsTrue(allDependees.Contains("2"));
            Assert.IsTrue(allDependees.Contains("3"));

            Assert.IsFalse(Graph.HasCyclic());
        }
    }
}