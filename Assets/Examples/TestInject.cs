using Buttr.Injection;
using Examples;
using UnityEngine;

namespace Buttr {
    public partial class TestInject : MonoBehaviour {
        [Inject] public ITestService m_TestServiceAbstract;
        [Inject] public TestService m_TestServiceConcrete;

        public bool ConfirmInjections() {
            return m_TestServiceAbstract is not null && m_TestServiceConcrete is not null;
        }
    }
}