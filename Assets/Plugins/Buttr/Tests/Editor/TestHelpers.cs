namespace Buttr.Tests.Editor {
    public interface ITestService1 { }
    public interface ITestService2 { }
    public interface ITestService3 { }
    public interface ITestService4 { }
    public interface ITestService5 { }
    public interface ITestService6 { }
    public interface ITestService7 { }
    public interface ITestService8 { }
    public interface ITestService9 { }

    public sealed class TestService0 { }
    
    public sealed class TestService1 : ITestService1 {
        private readonly TestService0 m_Service;
        
        public TestService1(TestService0 service) {
            m_Service = service;
        }
    }

    public sealed class TestService2 : ITestService2 {
        private readonly ITestService1 m_Service;
        
        public TestService2(ITestService1 service) {
            m_Service = service;
        }
    }
    
    public sealed class TestService3 : ITestService3 {
        private readonly ITestService2 m_Service;
        
        public TestService3(ITestService2 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService4 : ITestService4 {
        private readonly ITestService3 m_Service;
        
        public TestService4(ITestService3 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService5 : ITestService5 {
        private readonly ITestService4 m_Service;
        
        public TestService5(ITestService4 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService6 : ITestService6 {
        private readonly ITestService5 m_Service;
        
        public TestService6(ITestService5 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService7 : ITestService7 {
        private readonly ITestService6 m_Service;
        
        public TestService7(ITestService6 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService8 : ITestService8 {
        private readonly ITestService7 m_Service;
        
        public TestService8(ITestService7 service) {
            m_Service = service;
        }
     }
    
    public sealed class TestService9 : ITestService9 {
        private readonly ITestService8 m_Service;
        
        public TestService9(ITestService8 service) {
            m_Service = service;
        }
     }
}