using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Tests.Editor
{
    public class TestSetup
    {
        private static TestRunnerApi s_api;

        [InitializeOnLoadMethod]
        public static void EnableCallbacks()
        {
            s_api = ScriptableObject.CreateInstance<TestRunnerApi>();
            s_api.RegisterCallbacks(new TestListener());

            Debug.Log("Test Runner 监听已启动");
        }
    }
}
