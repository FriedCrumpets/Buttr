using UnityEngine;

namespace Buttr.Injection {
    [ DefaultExecutionOrder(-9000) ]
    internal sealed class SceneInjector : MonoBehaviour {
        private void Awake() {
            InjectionProcessor.InjectScene(gameObject.scene);
            Destroy(this);
        }
    }
}