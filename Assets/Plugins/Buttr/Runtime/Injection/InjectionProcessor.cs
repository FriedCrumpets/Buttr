using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

// ReSharper disable PossibleNullReferenceException

namespace Buttr.Injection {
    public static class InjectionProcessor {
        private static readonly Dictionary<Type, Action<object>> m_Injectors = new();

        static InjectionProcessor() {
            Application.quitting += OnQuit;
        }

        public static void Register<T>(Action<T> injector) where T : IInjectable {
            if (injector == null)
                throw new InjectionException("Cannot Register a null injector");

            m_Injectors.Add(typeof(T), obj => injector((T)obj));
        }

        public static void InjectScene(Scene scene) {
            foreach (var root in scene.GetRootGameObjects()) {
                InjectSelfAndChildren(root.GetComponent<MonoBehaviour>());
            }
        }
        
        public static void InjectActiveScene() {
            var scene = SceneManager.GetActiveScene();
            InjectScene(scene);
        }

        public static void InjectAllLoadedScenes() {
            for (var i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                InjectScene(scene);
            }
        }

        internal static void Inject(object instance) {
            if (instance == null) throw new InjectionException("Cannot inject into a null instance");
            if (instance is not (MonoBehaviour or IInjectable)) throw new InjectionException("Can only inject into MonoBehaviour Instances through attribute injection");
            if (instance is IInjectable { Injected: true }) {
                Debug.LogWarning("Attempting to Inject into the same object twice... Skipping object");
                return;
            }
            
            if (m_Injectors.TryGetValue(instance.GetType(), out var del)) {
                del(instance);
                (instance as IInjectable).Injected = true;
            }
            else {
                throw new InjectionException($"[Injection] No injector registered for type {instance.GetType()}");
            }
        }

        internal static void InjectSelfAndChildren(object instance) {
            if (instance == null) throw new InjectionException("Cannot inject into a null instance");
            if(instance is not MonoBehaviour mono) throw new InjectionException("Can only inject into MonoBehaviour Instances through attribute injection");

            var buffer = ListPool<MonoBehaviour>.Get();
            mono.GetComponentsInChildren(true, buffer);

            foreach (var mb in buffer) {
                if (mb is IInjectable injectable)
                    Inject(injectable);
            }

            ListPool<MonoBehaviour>.Release(buffer);
        }

        internal static void InjectGameObject(object instance) {
            if (instance == null) throw new InjectionException("Cannot inject into a null instance");
            if(instance is not MonoBehaviour mono) throw new InjectionException("Can only inject into MonoBehaviour Instances through attribute injection");

            var buffer = ListPool<MonoBehaviour>.Get();
            buffer.AddRange(mono.gameObject.GetComponents<MonoBehaviour>());

            foreach (var mb in buffer) {
                if (mb is IInjectable injectable)
                    Inject(injectable);
            }

            ListPool<MonoBehaviour>.Release(buffer);
        }

        private static void OnQuit() {
            Application.quitting -= OnQuit;
            m_Injectors.Clear();
        }
    }
}