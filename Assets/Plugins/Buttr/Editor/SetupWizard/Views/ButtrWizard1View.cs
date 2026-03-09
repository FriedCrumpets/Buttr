using System;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    
    internal delegate void SetupModeChangedCallback(int previousValue, int newValue);
    
    internal sealed class ButtrWizard1View : IDisposable {
        public event SetupModeChangedCallback OnSetupModeChanged;
        
        private readonly RadioButtonGroup m_SetupRadioGroup;
        
        private readonly VisualElement m_Root;
        
        private readonly Label m_UnityVersionLabel;
        private readonly Label m_ProjectLabel;
        private readonly Label m_ButtrVersionLabel;
        private readonly Label m_DescriptionLabel;
        
        public ButtrWizard1View(VisualElement root) {
            m_Root = root;
            
            m_SetupRadioGroup = root.Q<RadioButtonGroup>(ButtrWizardElements.SetupRadioButtonGroup);
            
            m_UnityVersionLabel = root.Q<Label>(ButtrWizardElements.Window1UnityVersion);
            m_ProjectLabel = root.Q<Label>(ButtrWizardElements.Window1Project);
            m_ButtrVersionLabel = root.Q<Label>(ButtrWizardElements.Window1ButtrVersion);
            m_DescriptionLabel = root.Q<Label>(ButtrWizardElements.SetupDescriptionLabel);

            m_SetupRadioGroup.UnregisterValueChangedCallback(OnSetupRadioGroupChanged);
            m_SetupRadioGroup.RegisterValueChangedCallback(OnSetupRadioGroupChanged);
        }

        public RadioButtonGroup SetupRadioGroup {
            get => m_SetupRadioGroup;
        }

        public VisualElement Root {
            get => m_Root;
        }
        
        public string UnityVersion {
            set => m_UnityVersionLabel.text = value;
        }
        
        public string Project {
            set => m_ProjectLabel.text = value;
        }
        
        public string ButtrVersion {
            set => m_ButtrVersionLabel.text = value;
        }
        
        public string Description {
            set => m_DescriptionLabel.text = value;
        }

        private void OnSetupRadioGroupChanged(ChangeEvent<int> evt) {
            OnSetupModeChanged?.Invoke(evt.previousValue, evt.newValue);
        }
        
        public void Dispose() {
            m_SetupRadioGroup.UnregisterValueChangedCallback(OnSetupRadioGroupChanged);
        }
    }
}