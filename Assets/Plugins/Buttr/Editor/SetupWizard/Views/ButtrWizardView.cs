using System;
using UnityEngine.UIElements;
using E = Buttr.Editor.SetupWizard.SetupWizardElements;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardView : IDisposable {
        private readonly ButtrWizard1View m_Window1;
        private readonly ButtrWizard3View m_Window3;
        private readonly ButtrWizardFooterView m_Footer;
        
        internal ButtrWizardView(VisualElement root) {
            m_Window1 = new ButtrWizard1View(root.Q<VisualElement>(E.Window1));
            m_Window3 = new ButtrWizard3View(root.Q<VisualElement>(E.Window3));
            m_Footer = new ButtrWizardFooterView(root.Q<VisualElement>(E.Footer));
        }
        
        public ButtrWizard1View Window1 => m_Window1;
        public ButtrWizard3View Window3 => m_Window3;
        public ButtrWizardFooterView Footer => m_Footer;
        
        public void NavigateToPage(WizardPage page) {
            switch (page)
            {
                case WizardPage.Selection:
                    m_Window1.Root.style.left = 0;
                    m_Window3.Root.style.left = 520;
                    //m_Window2.Root.style.left = 520;
                    break;
                case WizardPage.Installing:
                    m_Window1.Root.style.left = -520;
                    m_Window3.Root.style.left = 0;
                    //m_Window2.Root.style.left = -520;
                    break;
                case WizardPage.CustomSetup:
                    m_Window1.Root.style.left = -520;
                    m_Window3.Root.style.left = 520;
                    //m_Window2.Root.style.left = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }
        
        public void Dispose() {
            m_Window1?.Dispose();
            m_Footer?.Dispose();
        }
    }

}