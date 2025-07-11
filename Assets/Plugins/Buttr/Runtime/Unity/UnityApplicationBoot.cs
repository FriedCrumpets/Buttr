using UnityEngine;

namespace Buttr.Unity {
    public sealed class UnityApplicationBoot : MonoBehaviour {
        [SerializeField] private bool m_DontDestroyOnLoad;
        [SerializeField] private UnityApplicationLoaderBase[] m_ApplicationLoaders;

        private void Awake(){ 
            if(m_DontDestroyOnLoad) { DontDestroyOnLoad(gameObject); }
        }

        private async void Start() {
            foreach (var loader in m_ApplicationLoaders) {
                await loader.LoadAsync(destroyCancellationToken);
            }
        }

        private async void OnDestroy() {
            foreach (var loader in m_ApplicationLoaders) {
                await loader.UnloadAsync();
            }
        }
    }
}