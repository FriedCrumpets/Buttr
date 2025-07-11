using UnityEngine;

namespace Buttr.Injection {
    [ DefaultExecutionOrder(-10000) ]
    internal sealed class SceneInjector : MonoBehaviour {
        private void Awake() {
            InjectionProcessor.InjectScene(gameObject.scene);
            Destroy(this);
        }
    }
}