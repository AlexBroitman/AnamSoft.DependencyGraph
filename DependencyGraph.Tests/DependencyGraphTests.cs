using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnamSoft.DependencyGraph.Tests
{
    public abstract class DependencyGraphTests
    {
        protected DependencyGraph<string> Graph;

        [TestMethod]
        public void DirectDependencyTest()
        {
            var res = Graph.AddDependency("1", "2");

            Assert.IsTrue(res);
            Assert.IsFalse(Graph.IsEmpty);
            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDirectlyDepends("1", "2"));
            Assert.IsFalse(Graph.IsDepends("2", "1"));
            Assert.IsFalse(Graph.HasCyclic());

            var directDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(1, directDependees.Count);
            Assert.IsTrue(directDependees.Contains("2"));

            res = Graph.RemoveDependency("1", "2");

            Assert.IsTrue(res);
            Assert.IsTrue(Graph.IsEmpty);
            Assert.IsFalse(Graph.IsDepends("1", "2"));
            Assert.IsFalse(Graph.IsDirectlyDepends("1", "2"));
            Assert.IsFalse(Graph.IsDepends("2", "1"));

            directDependees = Graph.GetDirectDependencies("1");

            Assert.IsTrue(directDependees.IsEmpty);
            Assert.IsFalse(directDependees.Contains("2"));
            Assert.IsFalse(Graph.HasCyclic());
        }

        [TestMethod]
        public void IndirectDependencyTest()
        {
            Graph.AddDependency("1", "2");
            Graph.AddDependency("2", "3");
            Graph.AddDependency("3", "4");

            Assert.IsFalse(Graph.IsEmpty);

            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDepends("2", "3"));
            Assert.IsTrue(Graph.IsDepends("3", "4"));
            Assert.IsTrue(Graph.IsDepends("1", "3"));
            Assert.IsTrue(Graph.IsDepends("1", "4"));
            Assert.IsTrue(Graph.IsDepends("2", "4"));
            Assert.IsFalse(Graph.IsDirectlyDepends("1", "3"));
            Assert.IsFalse(Graph.IsDirectlyDepends("1", "4"));
            Assert.IsFalse(Graph.IsDirectlyDepends("2", "4"));

            Assert.IsFalse(Graph.IsDepends("2", "1"));
            Assert.IsFalse(Graph.IsDepends("3", "1"));
            Assert.IsFalse(Graph.IsDepends("4", "1"));
            Assert.IsFalse(Graph.IsDepends("3", "2"));
            Assert.IsFalse(Graph.IsDepends("4", "2"));
            Assert.IsFalse(Graph.IsDepends("4", "3"));

            Assert.IsFalse(Graph.HasCyclic());

            var directDependees = Graph.GetDirectDependencies("1");

            Assert.AreEqual(1, directDependees.Count);
            Assert.IsTrue(directDependees.Contains("2"));

            var allDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(3, allDependees.Count);
            Assert.IsTrue(allDependees.Contains("2"));
            Assert.IsTrue(allDependees.Contains("3"));
            Assert.IsTrue(allDependees.Contains("4"));

            Graph.RemoveDependency("3", "4");

            Assert.IsFalse(Graph.IsEmpty);

            Assert.IsTrue(Graph.IsDepends("1", "2"));
            Assert.IsTrue(Graph.IsDepends("2", "3"));
            Assert.IsFalse(Graph.IsDepends("3", "4"));
            Assert.IsTrue(Graph.IsDepends("1", "3"));
            Assert.IsFalse(Graph.IsDepends("1", "4"));
            Assert.IsFalse(Graph.IsDepends("2", "4"));

            Assert.IsFalse(Graph.IsDepends("2", "1"));
            Assert.IsFalse(Graph.IsDepends("3", "1"));
            Assert.IsFalse(Graph.IsDepends("4", "1"));
            Assert.IsFalse(Graph.IsDepends("3", "2"));
            Assert.IsFalse(Graph.IsDepends("4", "2"));
            Assert.IsFalse(Graph.IsDepends("4", "3"));

            directDependees = Graph.GetDirectDependencies("1");

            Assert.AreEqual(1, directDependees.Count);
            Assert.IsTrue(directDependees.Contains("2"));
            Assert.IsFalse(directDependees.Contains("3"));
            Assert.IsFalse(directDependees.Contains("4"));

            allDependees = Graph.GetAllDependencies("1");

            Assert.AreEqual(2, allDependees.Count);
            Assert.IsTrue(allDependees.Contains("2"));
            Assert.IsTrue(allDependees.Contains("3"));
            Assert.IsFalse(allDependees.Contains("4"));

            Assert.IsFalse(Graph.HasCyclic());
        }
    }
}