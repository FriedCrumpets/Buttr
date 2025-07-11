using System;
using UnityEngine;

namespace Buttr.Injection {
    [ DefaultExecutionOrder(-10000) ]
    internal sealed class MonoInjector : MonoBehaviour {
        [SerializeField] private MonoInjectStrategy m_InjectStrategy = MonoInjectStrategy.GameObjectAndChildren;
        [SerializeField, Tooltip(BehaviourInjectorTooltips.BEHAVIOUR_TOOLTIP)] private MonoBehaviour m_Behaviour;
        
        private void Awake() {
            if (m_Behaviour == null) {
                m_Behaviour = gameObject.GetComponent<MonoBehaviour>();
            }

            switch (m_InjectStrategy) {
                case MonoInjectStrategy.Mono:
                    InjectionProcessor.Inject(m_Behaviour);
                    break;
                case MonoInjectStrategy.GameObject:
                    InjectionProcessor.InjectGameObject(m_Behaviour);
                    break;
                case MonoInjectStrategy.GameObjectAndChildren:
                    InjectionProcessor.InjectSelfAndChildren(m_Behaviour);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Somehow... Incredibly... you managed to trigger this... 👍");
            }
            
            Destroy(this);
        }
    }
}