using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizard3View {
        private readonly VisualElement m_Root;
        
        private readonly VisualElement m_InstallerPanel;
        private readonly Label m_InstallingHeader;
        private readonly Label m_InstallingSubheader;
        
        public ButtrWizard3View(VisualElement root) {
            m_Root = root;
            
            m_InstallerPanel = root.Q<VisualElement>(SetupWizardElements.Installer);
            m_InstallingHeader = root.Q<Label>(SetupWizardElements.InstallerHeader);
            m_InstallingSubheader = root.Q<VisualElement>(SetupWizardElements.InstallerAdditional)?.Q<Label>();
        }

        public VisualElement Root {
            get => m_Root;
        }

        public VisualElement InstallerPanel {
            get => m_InstallerPanel;
        }
        
        public string InstallingHeader {
            set => m_InstallingHeader.text = value;
        }
        
        public string InstallingSubheader {
            set => m_InstallingSubheader.text = value;
        }
        
        public void AppendLogEntry(InstallLogEntry entry) {
            if (m_InstallerPanel == null) return;

            var line = entry.CreateLogLine();
            m_InstallerPanel.Add(line);
        }

        public void UpdateLogEntry(int index, InstallLogStatus status) {
            if (m_InstallerPanel == null) return;
            if (index < 0 || index >= m_InstallerPanel.childCount) return;

            var existing = m_InstallerPanel[index] as Label;
            var replacement = existing.UpdateLogLine(status);
            m_InstallerPanel.Insert(index, replacement);
            m_InstallerPanel.Remove(existing);
        }
    }
}