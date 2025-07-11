using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Buttr.Tests.Editor.Unity {
    public sealed class UnityApplicationBootTests {
        private GameObject m_Prefab;
        private GameObject m_InstantiatedObjected;
        private TestLoader m_Loader;
        
        [UnitySetUp]
        public IEnumerator Setup() {
            m_Prefab = Resources.Load<GameObject>("TestBoot");
            m_Loader = Resources.Load<TestLoader>("TestLoader");

            m_InstantiatedObjected = Object.Instantiate(m_Prefab);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            m_Loader = null;
            m_Prefab = null;
            
            if (m_InstantiatedObjected != null) {
                Object.Destroy(m_InstantiatedObjected);
            }
            
            yield return null;
        }

        [UnityTest] public IEnumerator LoaderLoadsOnStart() {
            yield return new WaitForSeconds(1);
            
            Assert.IsTrue(m_Loader.IsLoaded == true);
        }
        
        [UnityTest] public IEnumerator LoaderUnload() {
            yield return new WaitForSeconds(1);
            
            Object.Destroy(m_InstantiatedObjected);
            
            yield return new WaitForSeconds(1);
            
            Assert.IsTrue(m_Loader.IsLoaded == false);
        }
    }
}