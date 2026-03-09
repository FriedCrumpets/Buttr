using System;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardView : IDisposable {
        private readonly ButtrWizardWindowView m_Window;
        private readonly ButtrWizardFooterView m_Footer;
        
        internal ButtrWizardView(VisualElement root) {
            m_Window = new ButtrWizardWindowView(root.Q<VisualElement>(ButtrWizardElements.Window1));
            m_Footer = new ButtrWizardFooterView(root.Q<VisualElement>(ButtrWizardElements.Footer));
        }
        
        public ButtrWizardWindowView Window => m_Window;
        public ButtrWizardFooterView Footer => m_Footer;
        
        public void Dispose() {
            m_Window?.Dispose();
            m_Footer?.Dispose();
        }
    }

}