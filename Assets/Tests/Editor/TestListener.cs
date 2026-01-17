using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.TestTools.TestRunner.Api;

namespace Tests.Editor
{
    /// <summary>
    /// Test runner callback listener that outputs test results to a file.
    /// </summary>
    public class TestListener : ICallbacks
    {
        private const string ResultFileName = "TestResults.txt";
        private static readonly string ResultFilePath = Path.Combine("ProjectSettings", "TestResults", ResultFileName);

        private readonly List<ITestResultAdaptor> _failedTests = new();

        /// <summary>
        /// Called when the test run starts.
        /// </summary>
        /// <param name="testsToRun">The test suite to be executed.</param>
        public void RunStarted(ITestAdaptor testsToRun)
        {
            _failedTests.Clear();
        }

        /// <summary>
        /// Called when the test run finishes. Outputs all failures to a file.
        /// </summary>
        /// <param name="result">The overall test result.</param>
        public void RunFinished(ITestResultAdaptor result)
        {
            if (_failedTests.Count == 0 && result.PassCount > 0)
            {
                WriteSuccessResult(result);
                return;
            }

            WriteFailureResults(result);
        }

        /// <summary>
        /// Called when a single test starts.
        /// </summary>
        /// <param name="test">The test being started.</param>
        public void TestStarted(ITestAdaptor test)
        {
        }

        /// <summary>
        /// Called when a single test finishes. Collects failed tests.
        /// </summary>
        /// <param name="result">The test result.</param>
        public void TestFinished(ITestResultAdaptor result)
        {
            // Only collect leaf test cases, not parent test suites
            if (result.TestStatus == TestStatus.Failed && !result.Test.HasChildren)
            {
                _failedTests.Add(result);
            }
        }

        private void WriteSuccessResult(ITestResultAdaptor result)
        {
            var content = new StringBuilder();
            content.AppendLine($"=== Test Run Successful ===");
            content.AppendLine($"Passed: {result.PassCount}");
            content.AppendLine($"Duration: {result.Duration:F2}s");
            content.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            WriteToFile(content.ToString());
        }

        private void WriteFailureResults(ITestResultAdaptor result)
        {
            var content = new StringBuilder();
            content.AppendLine($"=== Test Run Failed ===");
            content.AppendLine($"Passed: {result.PassCount}");
            content.AppendLine($"Failed: {_failedTests.Count}");
            content.AppendLine($"Duration: {result.Duration:F2}s");
            content.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine();

            for (int i = 0; i < _failedTests.Count; i++)
            {
                var failedTest = _failedTests[i];
                content.AppendLine($"--- Failure {i + 1}: {failedTest.Name} ---");
                content.AppendLine($"Status: {failedTest.TestStatus}");
                content.AppendLine($"Message: {failedTest.Message}");

                if (!string.IsNullOrEmpty(failedTest.StackTrace))
                {
                    content.AppendLine($"Stack Trace:");
                    content.AppendLine(failedTest.StackTrace);
                }

                content.AppendLine();
            }

            WriteToFile(content.ToString());
        }

        private void WriteToFile(string content)
        {
            var directory = Path.GetDirectoryName(ResultFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(ResultFilePath, content);
            UnityEngine.Debug.Log($"Test results written to: {Path.GetFullPath(ResultFilePath)}");
        }
    }
}
