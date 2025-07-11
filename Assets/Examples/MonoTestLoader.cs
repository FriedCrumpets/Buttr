using System.Threading;
using Buttr;
using Buttr.Core;
using Buttr.Unity;
using UnityEngine;

namespace Examples {
    
    public sealed class MonoTestLoader : UnityApplicationLoaderBase {
        private GameObject m_MonoInjectObj;
        private GameObject m_GameObjectInjectObj;
        private GameObject m_GameObjectAndChildrenInjectObj;
        
        public override Awaitable LoadAsync(CancellationToken cancellationToken) { 
            var monoInject = Resources.Load<TestInject>("TestMonoInject");
            m_MonoInjectObj = Instantiate(monoInject).gameObject;
            
            var gameObjectInject = Resources.Load<TestInject>("TestGameObjectInject");
            m_GameObjectInjectObj = Instantiate(gameObjectInject).gameObject;
            
            var gameObjectAndChildrenInject = Resources.Load<TestInject>("TestGameObjectAndChildrenInject");
            m_GameObjectAndChildrenInjectObj = Instantiate(gameObjectAndChildrenInject).gameObject;
            
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            Destroy(m_MonoInjectObj);
            Destroy(m_GameObjectInjectObj);
            Destroy(m_GameObjectAndChildrenInjectObj);
            
            return AwaitableUtility.CompletedTask;
        }
    }
}