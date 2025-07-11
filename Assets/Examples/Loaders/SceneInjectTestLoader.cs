using System.Threading;
using Buttr.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttr.Core {
    [CreateAssetMenu(fileName = "SceneInjectTesting", menuName = "Buttr/Examples/Loaders/SceneTesting", order = 0)]
    public sealed class SceneInjectTestLoader : UnityApplicationLoaderBase {
        private const string Scene_1 = "SceneInjectTestScene 1";
        private const string Scene_2 = "SceneInjectTestScene 2";
        private const string Scene_3 = "SceneInjectTestScene 3";

        public override async Awaitable LoadAsync(CancellationToken cancellationToken) {
            await SceneManager.LoadSceneAsync(Scene_1);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scene_1));
            Instantiate(Resources.Load<GameObject>("TestActiveSceneInject"));

            await Awaitable.WaitForSecondsAsync(.5f, cancellationToken);
            var test1Passed = Check(Scene_1);

            Debug.Log($">>>>> ACTIVE SCENE TESTING COMPLETE : PASSED {test1Passed} <<<<<");

            await SceneManager.LoadSceneAsync(Scene_2);
            await SceneManager.LoadSceneAsync(Scene_3, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scene_3));
            Instantiate(Resources.Load<GameObject>("TestThisSceneInject"));
            var test2Passed = Check(Scene_2) == false && Check(Scene_3);
            
            Debug.Log($">>>>> THIS SCENE TESTING COMPLETE : PASSED {test2Passed} <<<<<");
            
            await SceneManager.LoadSceneAsync(Scene_1);
            await SceneManager.LoadSceneAsync(Scene_2, LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync(Scene_3, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scene_1));

            Instantiate(Resources.Load<GameObject>("TestAllSceneInject"));
            var test3Passed = Check(Scene_1) && Check(Scene_2) && Check(Scene_3);
            
            Debug.Log($">>>>> ALL SCENE TESTING COMPLETE : PASSED {test3Passed} <<<<<");
        }

        private static bool Check(string sceneName) {
            var gos = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            var valid = false;
            
            foreach (var go in gos) {
                foreach( var inject in go.GetComponentsInChildren<TestInject>())
                    valid = inject.ConfirmInjections();
            }
            
            return valid;
        }
    }
}