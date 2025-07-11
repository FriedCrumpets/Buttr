using System.Threading;
using Buttr.Unity;
using Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttr.Core {
    [CreateAssetMenu(fileName = "MonoInjectTesting", menuName = "Buttr/Examples/Loaders/MonoTesting", order = 0)]
    public sealed class MonoInjectTestLoader : UnityApplicationLoaderBase {
        private GameObject m_MonoInjectObj;
        private GameObject m_GameObjectInjectObj;
        private GameObject m_GameObjectAndChildrenInjectObj;
        
        public override async Awaitable LoadAsync(CancellationToken cancellationToken) {
            await SceneManager.LoadSceneAsync("MonoInjectTesting");
            await Awaitable.WaitForSecondsAsync(.5f, cancellationToken);
            
            var monoInject = Resources.Load<TestInject>("TestMonoInject");
            m_MonoInjectObj = Instantiate(monoInject).gameObject;
            
            var gameObjectInject = Resources.Load<TestInject>("TestGameObjectInject");
            m_GameObjectInjectObj = Instantiate(gameObjectInject).gameObject;
            
            var gameObjectAndChildrenInject = Resources.Load<TestInject>("TestGameObjectAndChildrenInject");
            m_GameObjectAndChildrenInjectObj = Instantiate(gameObjectAndChildrenInject).gameObject;

            await Awaitable.WaitForSecondsAsync(.5f, cancellationToken);

            var passed = false;
            var gos = SceneManager.GetSceneByName("MonoInjectTesting").GetRootGameObjects();
            foreach (var go in gos) {
                foreach( var inject in go.GetComponentsInChildren<TestInject>())
                    passed = inject.ConfirmInjections();
            }
            
            Debug.Log($">>>>> MONO TESTING COMPLETE :: PASSED {passed} <<<<<");
        }

        public override Awaitable UnloadAsync() {
            Destroy(m_MonoInjectObj);
            Destroy(m_GameObjectInjectObj);
            Destroy(m_GameObjectAndChildrenInjectObj);
            
            return AwaitableUtility.CompletedTask;
        }
    }
}