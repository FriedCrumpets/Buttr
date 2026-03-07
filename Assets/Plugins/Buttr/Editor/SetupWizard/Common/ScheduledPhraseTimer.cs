using System;

namespace Buttr.Editor.SetupWizard {
    /// <summary>
    /// Simple timer that calls <see cref="ButtrWizardPresenter.AdvancePhrase"/>
    /// at a fixed interval. Uses EditorApplication.delayCall in a loop
    /// since IVisualElementScheduler isn't reliable across recompiles.
    ///
    /// Replace with EditorApplication.timers when available in your Unity version.
    /// </summary>
    internal sealed class ScheduledPhraseTimer : IDisposable {
        private readonly ButtrWizardPresenter m_Presenter;
        
        private bool m_Disposed;
        private double m_NextTick;

        private const double IntervalSeconds = 2.5;

        public ScheduledPhraseTimer(ButtrWizardPresenter presenter) {
            m_Presenter = presenter;
            m_NextTick = UnityEditor.EditorApplication.timeSinceStartup + IntervalSeconds;
            UnityEditor.EditorApplication.update += OnUpdate;
        }

        private void OnUpdate() {
            if (m_Disposed) return;

            if (UnityEditor.EditorApplication.timeSinceStartup >= m_NextTick) {
                m_Presenter.AdvancePhrase();
                m_NextTick = UnityEditor.EditorApplication.timeSinceStartup + IntervalSeconds;
            }
        }

        public void Dispose() {
            if (m_Disposed) return;
            
            m_Disposed = true;
            UnityEditor.EditorApplication.update -= OnUpdate;
        }
    }
}