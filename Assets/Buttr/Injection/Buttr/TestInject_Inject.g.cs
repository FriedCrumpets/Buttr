using UnityEngine;
using Buttr.Injection;
using Buttr.Core;

namespace Buttr {
    public partial class TestInject : IInjectable {
        bool IInjectable.Injected { get; set; }

        [RuntimeInitializeOnLoadMethod]
        static void __RegisterInjectionHandler() => InjectionProcessor.Register<TestInject>(Inject);

        private static void Inject(TestInject instance) {
            instance.m_TestServiceAbstract = Application<Examples.ITestService>.Get();
            instance.m_TestServiceConcrete = Application<Examples.TestService3>.Get();
        }
    }
}
