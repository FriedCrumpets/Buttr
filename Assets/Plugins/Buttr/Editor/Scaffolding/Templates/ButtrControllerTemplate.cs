namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrControllerTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrControllerTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using Buttr.Unity;
using UnityEngine;

namespace {m_Ns} {{
    public sealed partial class {m_Name}Controller : MonoBehaviour {{
        // [Inject] private {m_Name}Registry i_Registry;
        // [Inject] private {m_Name}InstanceModel i_Model; // this is a transient model, it's usually a good idea to keep your data in an object like this

        // private IDisposable m_Registration; 

        private void Awake(){{
            // m_Registration = i_Registry.Register(GetEntityId(), this);
        }}

        private void OnDestroy() {{ 
            // m_Registration?.Dispose(); // we use a '?' here if the object is destroyed before being enabled
        }}
    }}
}}
";
        }
    }
}