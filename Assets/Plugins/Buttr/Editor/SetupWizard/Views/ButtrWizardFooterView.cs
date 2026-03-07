using System;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardFooterView : IDisposable {
        public event Action OnLeftButtonClicked;
        public event Action OnRightButtonClicked;
        
        private readonly Button m_LeftButton;
        private readonly Button m_RightButton;
        
        public ButtrWizardFooterView(VisualElement root) {
            m_LeftButton = root.Q<Button>(SetupWizardElements.FooterLeftButton);
            m_LeftButton.clicked += HandleLeftButtonClicked;
            
            m_RightButton = root.Q<Button>(SetupWizardElements.FooterRightButton);
            m_RightButton.clicked += HandleRightButtonClicked;
        }

        public string LeftButtonText {
            set => m_LeftButton.text = value;
        }

        public bool LeftButtonVisible {
            set => m_LeftButton.style.display = value 
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        public bool LeftButtonEnabled {
            set => m_LeftButton.SetEnabled(value);
        }
        
        public string RightButtonText {
            set => m_RightButton.text = value;
        }
        
        public bool RightButtonVisible {
            set => m_RightButton.style.display = value 
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
        
        public bool RightButtonEnabled {
            set => m_RightButton.SetEnabled(value);
        }

        private void HandleLeftButtonClicked() {
            OnLeftButtonClicked?.Invoke();
        }

        private void HandleRightButtonClicked() {
            OnRightButtonClicked?.Invoke();
        }
        
        public void Dispose() {
            m_LeftButton.clicked -= HandleLeftButtonClicked;
            m_RightButton.clicked -= HandleRightButtonClicked;
        }
    }
}