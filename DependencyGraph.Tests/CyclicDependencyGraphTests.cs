using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnamSoft.DependencyGraph.Tests
{
    [TestClass]
    public class CyclicDependencyGraphTests : DependencyGraphTests
    {
        [TestInitialize]
        public void Init()
        {
            Graph = new DependencyGraph<string>(true);
            Assert.IsTrue(Graph.IsEmpty);
        }

        [TestMethod]
        public void CyclicDependencyTest()
        {
            Graph.AddDependency("1", "2");
            Graph.AddDependency("2", "3");
            var res = Graph.AddDependency("3", "1");

            Assert.IsTrue(res);

            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDepends("2", "3"));
            Assert.IsTrue(Graph.IsDepends("3", "1"));
            Assert.IsTrue(Graph.IsDepends("1", "1"));
            Assert.IsTrue(Graph.HasCyclic());

            var allDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(3, allDependees.Count);
            Assert.IsTrue(allDependees.Contains("1"));
            Assert.IsTrue(allDependees.Contains("2"));
            Assert.IsTrue(allDependees.Contains("3"));

            res = Graph.RemoveDependency("3", "1");

            Assert.IsTrue(res);
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