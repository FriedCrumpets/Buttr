using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardInstallationModel {
        public event Action<float> OnInstallProgressChanged;
        public event Action<string> OnInstallPhraseChanged;
        public event Action OnInstallCompleted;
        public event Action<InstallLogEntry> OnInstallLogAdded;
        public event Action<int, InstallLogStatus> OnInstallLogUpdated;
        
        private readonly List<InstallLogEntry> m_InstallLog = new();
        
        private float m_InstallProgress;
        private string m_CurrentPhrase = string.Empty;
        private bool m_InstallComplete;

        public IReadOnlyList<InstallLogEntry> InstallLog => m_InstallLog;

        public float InstallProgress {
            get => m_InstallProgress;
            set {
                if(Mathf.Approximately(m_InstallProgress, value)) return;
                
                m_InstallProgress = value;
                OnInstallProgressChanged?.Invoke(value);
            }
        }

        public string CurrentPhrase {
            get => m_CurrentPhrase;
            set {
                if (m_CurrentPhrase == value) return;
                
                m_CurrentPhrase = value;
                OnInstallPhraseChanged?.Invoke(value);
            }
        }

        public bool InstallComplete {
            get => m_InstallComplete;
            set {
                if (m_InstallComplete == value) return;
                
                m_InstallComplete = value;
                if (value) OnInstallCompleted?.Invoke();
            }
        }

        public void AddLogEntry(InstallLogEntry entry) {
            m_InstallLog.Add(entry);
            OnInstallLogAdded?.Invoke(entry);
        }

        public void UpdateLogEntry(int index, InstallLogStatus status) {
            if (index < 0 || index >= m_InstallLog.Count) return;
            m_InstallLog[index] = m_InstallLog[index].WithStatus(status);
            OnInstallLogUpdated?.Invoke(index, status);
        }
    }
}
