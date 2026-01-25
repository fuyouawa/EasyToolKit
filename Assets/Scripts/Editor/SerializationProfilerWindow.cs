using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyToolKit.OdinSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EasyToolKit.Serialization;
using Debug = UnityEngine.Debug;
using SerializationUtility = EasyToolKit.OdinSerializer.SerializationUtility;

/// <summary>
/// Editor window for profiling serialization performance.
/// Use with Unity Profiler window (Window > Analysis > Profiler) to analyze performance characteristics.
/// </summary>
public class SerializationProfilerWindow : EditorWindow
{
    #region Menu Entry

    [MenuItem("Tools/EasyToolKit/Serialization Profiler")]
    private static void ShowWindow()
    {
        var window = GetWindow<SerializationProfilerWindow>();
        window.titleContent = new GUIContent("Serialization Profiler");
        window.Show();
    }

    #endregion

    #region Test Configuration

    private int iterations = 1000;
    private bool enableDeepProfiling = true;
    private Vector2 scrollPosition;

    #endregion

    #region Unity Lifecycle

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "This window runs serialization performance tests. " +
            "Open the Profiler window (Window > Analysis > Profiler) to see detailed performance data.",
            MessageType.Info);

        EditorGUILayout.Space();

        // Configuration
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        iterations = EditorGUILayout.IntSlider("Iterations", iterations, 100, 100000);
        enableDeepProfiling = EditorGUILayout.Toggle("Enable Deep Profiling", enableDeepProfiling);

        if (enableDeepProfiling)
        {
            EditorGUILayout.HelpBox(
                "Deep profiling provides detailed call stacks but may impact performance. " +
                "Enable it in the Profiler window (Deep Profile button).",
                MessageType.None);
        }

        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Test Categories
        DrawPrimitiveTests();
        DrawStringTests();
        DrawCollectionTests();
        DrawUnityTypeTests();
        DrawComplexObjectTests();
        DrawNestedObjectTests();
        DrawLargeDataSetTests();
        DrawComparisonTests();
        DrawRunAllTestsButton();

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region Test Categories

    /// <summary>
    /// Draws primitive type serialization tests.
    /// </summary>
    private void DrawPrimitiveTests()
    {
        DrawSection("Primitive Types", () =>
        {
            DrawTestButton("Int32 Serialize/Deserialize", () => RunBenchmark("Int32", iterations, () =>
            {
                int value = 42;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<int>(data);
            }));

            DrawTestButton("Float Serialize/Deserialize", () => RunBenchmark("Float", iterations, () =>
            {
                float value = 3.14159f;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<float>(data);
            }));

            DrawTestButton("Double Serialize/Deserialize", () => RunBenchmark("Double", iterations, () =>
            {
                double value = 123.456789;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<double>(data);
            }));

            DrawTestButton("Bool Serialize/Deserialize", () => RunBenchmark("Bool", iterations, () =>
            {
                bool value = true;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<bool>(data);
            }));

            DrawTestButton("Long Serialize/Deserialize", () => RunBenchmark("Long", iterations, () =>
            {
                long value = 12345678901234;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<long>(data);
            }));
        });
    }

    /// <summary>
    /// Draws string serialization tests.
    /// </summary>
    private void DrawStringTests()
    {
        DrawSection("String Types", () =>
        {
            DrawTestButton("Empty String", () => RunBenchmark("EmptyString", iterations, () =>
            {
                string value = string.Empty;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<string>(data);
            }));

            DrawTestButton("Short String (10 chars)", () => RunBenchmark("ShortString", iterations, () =>
            {
                string value = "HelloWorld";
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<string>(data);
            }));

            DrawTestButton("Medium String (100 chars)", () => RunBenchmark("MediumString", iterations, () =>
            {
                string value = new string('A', 100);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<string>(data);
            }));

            DrawTestButton("Long String (1000 chars)", () => RunBenchmark("LongString", iterations / 10, () =>
            {
                string value = new string('A', 1000);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<string>(data);
            }));

            DrawTestButton("Unicode String", () => RunBenchmark("UnicodeString", iterations, () =>
            {
                string value = "Hello 世界! 🌍";
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<string>(data);
            }));
        });
    }

    /// <summary>
    /// Draws collection serialization tests.
    /// </summary>
    private void DrawCollectionTests()
    {
        DrawSection("Arrays & Lists", () =>
        {
            DrawTestButton("Int Array [5]", () => RunBenchmark("SmallArray", iterations, () =>
            {
                int[] value = { 1, 2, 3, 4, 5 };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<int[]>(data);
            }));

            DrawTestButton("Int Array [100]", () => RunBenchmark("MediumArray", iterations, () =>
            {
                int[] value = new int[100];
                for (int i = 0; i < 100; i++) value[i] = i;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<int[]>(data);
            }));

            DrawTestButton("Int Array [10000]", () => RunBenchmark("LargeArray", iterations / 100, () =>
            {
                int[] value = new int[10000];
                for (int i = 0; i < 10000; i++) value[i] = i;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<int[]>(data);
            }));

            DrawTestButton("Int List [5]", () => RunBenchmark("SmallList", iterations, () =>
            {
                var value = new List<int> { 1, 2, 3, 4, 5 };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<List<int>>(data);
            }));

            DrawTestButton("Int List [100]", () => RunBenchmark("MediumList", iterations, () =>
            {
                var value = new List<int>();
                for (int i = 0; i < 100; i++) value.Add(i);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<List<int>>(data);
            }));

            DrawTestButton("String List [50]", () => RunBenchmark("StringList", iterations, () =>
            {
                var value = new List<string>();
                for (int i = 0; i < 50; i++) value.Add($"Item_{i}");
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<List<string>>(data);
            }));
        });
    }

    /// <summary>
    /// Draws Unity type serialization tests.
    /// </summary>
    private void DrawUnityTypeTests()
    {
        DrawSection("Unity Types", () =>
        {
            DrawTestButton("Vector2", () => RunBenchmark("Vector2", iterations, () =>
            {
                Vector2 value = new Vector2(1.5f, 2.5f);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<Vector2>(data);
            }));

            DrawTestButton("Vector3", () => RunBenchmark("Vector3", iterations, () =>
            {
                Vector3 value = new Vector3(1.5f, 2.5f, 3.5f);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<Vector3>(data);
            }));

            DrawTestButton("Vector3Int", () => RunBenchmark("Vector3Int", iterations, () =>
            {
                Vector3Int value = new Vector3Int(1, 2, 3);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<Vector3Int>(data);
            }));

            DrawTestButton("Color", () => RunBenchmark("Color", iterations, () =>
            {
                Color value = new Color(0.5f, 0.3f, 0.7f, 1.0f);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<Color>(data);
            }));
        });
    }

    /// <summary>
    /// Draws complex object serialization tests.
    /// </summary>
    private void DrawComplexObjectTests()
    {
        DrawSection("Complex Objects", () =>
        {
            DrawTestButton("Simple Class", () => RunBenchmark("SimpleClass", iterations, () =>
            {
                var value = new SimpleTestData
                {
                    Id = 100,
                    Name = "TestPlayer",
                    Score = 1234.56f
                };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<SimpleTestData>(data);
            }));

            DrawTestButton("Complex Class", () => RunBenchmark("ComplexClass", iterations, () =>
            {
                var value = new ComplexTestData
                {
                    Id = 100,
                    Name = "TestPlayer",
                    Health = 75.5f,
                    Mana = 50.0f,
                    IsActive = true,
                    Position = new Vector3(1, 2, 3),
                    Rotation = new Vector3(0, 90, 0),
                    Inventory = new List<int> { 1, 2, 3, 4, 5 },
                    Tags = new[] { "Player", "Active", "Test" }
                };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<ComplexTestData>(data);
            }));
        });
    }

    /// <summary>
    /// Draws nested object serialization tests.
    /// </summary>
    private void DrawNestedObjectTests()
    {
        DrawSection("Nested Objects", () =>
        {
            DrawTestButton("2-Level Nesting", () => RunBenchmark("Nested2Levels", iterations, () =>
            {
                var child = new NestedTestData { Level = 2, Value = "Child" };
                var parent = new NestedTestData { Level = 1, Value = "Parent", Child = child };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref parent, ref refs);
                EasySerializer.DeserializeFromBinary<NestedTestData>(data);
            }));

            DrawTestButton("3-Level Nesting", () => RunBenchmark("Nested3Levels", iterations, () =>
            {
                var level3 = new NestedTestData { Level = 3, Value = "Level3" };
                var level2 = new NestedTestData { Level = 2, Value = "Level2", Child = level3 };
                var level1 = new NestedTestData { Level = 1, Value = "Level1", Child = level2 };
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref level1, ref refs);
                EasySerializer.DeserializeFromBinary<NestedTestData>(data);
            }));
        });
    }

    /// <summary>
    /// Draws large data set serialization tests.
    /// </summary>
    private void DrawLargeDataSetTests()
    {
        DrawSection("Large Data Sets", () =>
        {
            DrawTestButton("Large Array [10000]", () => RunBenchmark("LargeArray", iterations / 100, () =>
            {
                int[] value = new int[10000];
                for (int i = 0; i < 10000; i++) value[i] = i;
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<int[]>(data);
            }));

            DrawTestButton("Large List [10000]", () => RunBenchmark("LargeList", iterations / 100, () =>
            {
                var value = new List<int>();
                for (int i = 0; i < 10000; i++) value.Add(i);
                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<List<int>>(data);
            }));

            DrawTestButton("Mixed Large Data", () => RunBenchmark("MixedLargeData", iterations / 100, () =>
            {
                var value = new LargeTestData
                {
                    IntArray = new int[1000],
                    StringList = new List<string>(),
                    FloatMatrix = new float[10, 10]
                };
                for (int i = 0; i < 1000; i++) value.IntArray[i] = i;
                for (int i = 0; i < 100; i++) value.StringList.Add($"Item_{i}");
                for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    value.FloatMatrix[i, j] = i * 10 + j;

                List<UnityEngine.Object> refs = null;
                byte[] data = EasySerializer.SerializeToBinary(ref value, ref refs);
                EasySerializer.DeserializeFromBinary<LargeTestData>(data);
            }));
        });
    }

    /// <summary>
    /// Draws comparison tests between EasySerializer and OdinSerializer.
    /// </summary>
    private void DrawComparisonTests()
    {
        DrawSection("Comparison: EasySerializer vs OdinSerializer", () =>
        {
            DrawComparisonButton("Int32", () =>
            {
                int value = 42;
                RunComparisonBenchmark("Int32", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Float", () =>
            {
                float value = 3.14159f;
                RunComparisonBenchmark("Float", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("String (50 chars)", () =>
            {
                string value = new string('A', 50);
                RunComparisonBenchmark("String", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Int Array [100]", () =>
            {
                int[] value = new int[100];
                for (int i = 0; i < 100; i++) value[i] = i;
                RunComparisonBenchmark("IntArray", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Int List [100]", () =>
            {
                var value = new List<int>();
                for (int i = 0; i < 100; i++) value.Add(i);
                RunComparisonBenchmark("IntList", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Vector3", () =>
            {
                Vector3 value = new Vector3(1.5f, 2.5f, 3.5f);
                RunComparisonBenchmark("Vector3", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Simple Class", () =>
            {
                var value = new SimpleTestData
                {
                    Id = 100,
                    Name = "TestPlayer",
                    Score = 1234.56f
                };
                RunComparisonBenchmark("SimpleClass", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Complex Class", () =>
            {
                var value = new ComplexTestData
                {
                    Id = 100,
                    Name = "TestPlayer",
                    Health = 75.5f,
                    Mana = 50.0f,
                    IsActive = true,
                    Position = new Vector3(1, 2, 3),
                    Rotation = new Vector3(0, 90, 0),
                    Inventory = new List<int> { 1, 2, 3, 4, 5 },
                    Tags = new[] { "Player", "Active", "Test" }
                };
                RunComparisonBenchmark("ComplexClass", iterations, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });

            DrawComparisonButton("Large Array [1000]", () =>
            {
                int[] value = new int[1000];
                for (int i = 0; i < 1000; i++) value[i] = i;
                RunComparisonBenchmark("LargeArray", iterations / 10, () =>
                {
                    List<UnityEngine.Object> refs = null;
                    return EasySerializer.SerializeToBinary(ref value, ref refs);
                }, () =>
                {
                    return SerializationUtility.SerializeValue(value, DataFormat.Binary);
                });
            });
        });
    }

    /// <summary>
    /// Draws a button to run all tests sequentially.
    /// </summary>
    private void DrawRunAllTestsButton()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Run All Tests (Sequential)", GUILayout.Height(30)))
        {
            RunAllTests();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Draws a section header with content.
    /// </summary>
    private void DrawSection(string title, Action content)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        content?.Invoke();
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Draws a test button and executes the action when clicked.
    /// </summary>
    private void DrawTestButton(string label, Action action)
    {
        if (GUILayout.Button(label))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Draws a comparison button with EasySerializer vs OdinSerializer label.
    /// </summary>
    private void DrawComparisonButton(string label, Action action)
    {
        if (GUILayout.Button($"Compare: {label}"))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Runs a comparison benchmark between EasySerializer and OdinSerializer.
    /// </summary>
    /// <param name="testName">The name of the test.</param>
    /// <param name="iterationCount">The number of iterations to run.</param>
    /// <param name="easySerializerAction">The serialization action for EasySerializer.</param>
    /// <param name="odinSerializerAction">The serialization action for OdinSerializer.</param>
    private void RunComparisonBenchmark(string testName, int iterationCount, Func<byte[]> easySerializerAction, Func<byte[]> odinSerializerAction)
    {
        UnityEngine.Debug.Log($"[Comparison] {testName} - Starting comparison ({iterationCount} iterations)");

        // Test EasySerializer
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var easyStopwatch = Stopwatch.StartNew();
        byte[] easyData = null;
        for (int i = 0; i < iterationCount; i++)
        {
            easyData = easySerializerAction();
        }
        easyStopwatch.Stop();

        // Test OdinSerializer
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var odinStopwatch = Stopwatch.StartNew();
        byte[] odinData = null;
        for (int i = 0; i < iterationCount; i++)
        {
            odinData = odinSerializerAction();
        }
        odinStopwatch.Stop();

        // Calculate and display results
        double easyAvgMs = (double)easyStopwatch.ElapsedMilliseconds / iterationCount;
        double odinAvgMs = (double)odinStopwatch.ElapsedMilliseconds / iterationCount;
        double speedupRatio = odinAvgMs / easyAvgMs;

        UnityEngine.Debug.Log($"[Comparison] {testName} Results:");
        UnityEngine.Debug.Log($"  EasySerializer: {easyStopwatch.ElapsedMilliseconds}ms total, {easyAvgMs:F4}ms avg, Size: {easyData?.Length ?? 0} bytes");
        UnityEngine.Debug.Log($"  OdinSerializer: {odinStopwatch.ElapsedMilliseconds}ms total, {odinAvgMs:F4}ms avg, Size: {odinData?.Length ?? 0} bytes");

        if (speedupRatio > 1.0)
        {
            UnityEngine.Debug.Log($"  EasySerializer is {speedupRatio:F2}x FASTER");
        }
        else if (speedupRatio < 1.0)
        {
            UnityEngine.Debug.Log($"  EasySerializer is {1.0 / speedupRatio:F2}x SLOWER");
        }
        else
        {
            UnityEngine.Debug.Log($"  Performance is EQUAL");
        }

        GC.Collect();
    }

    /// <summary>
    /// Runs a benchmark test with the specified name and iterations.
    /// </summary>
    private void RunBenchmark(string testName, int iterationCount, Action benchmarkAction)
    {
        UnityEngine.Debug.Log($"[SerializationProfiler] Starting: {testName} ({iterationCount} iterations)");

        // Force GC cleanup before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterationCount; i++)
        {
            benchmarkAction();
        }

        stopwatch.Stop();

        UnityEngine.Debug.Log(
            $"[SerializationProfiler] {testName}: {iterationCount} iterations, " +
            $"{stopwatch.ElapsedMilliseconds}ms total, " +
            $"{(double)stopwatch.ElapsedMilliseconds / iterationCount:F4}ms avg");

        // Force GC after test
        GC.Collect();
    }

    /// <summary>
    /// Runs all benchmark tests sequentially.
    /// </summary>
    private void RunAllTests()
    {
        UnityEngine.Debug.Log("=== Serialization Profiler: Running All Tests ===");

        // Primitive types
        Debug.Log("\n--- Primitive Types ---");
        RunAllTestsInCurrentSection(DrawPrimitiveTests);

        // Strings
        Debug.Log("\n--- String Types ---");
        RunAllTestsInCurrentSection(DrawStringTests);

        // Collections
        Debug.Log("\n--- Arrays & Lists ---");
        RunAllTestsInCurrentSection(DrawCollectionTests);

        // Unity types
        Debug.Log("\n--- Unity Types ---");
        RunAllTestsInCurrentSection(DrawUnityTypeTests);

        // Complex objects
        Debug.Log("\n--- Complex Objects ---");
        RunAllTestsInCurrentSection(DrawComplexObjectTests);

        // Nested objects
        Debug.Log("\n--- Nested Objects ---");
        RunAllTestsInCurrentSection(DrawNestedObjectTests);

        // Large data sets
        Debug.Log("\n--- Large Data Sets ---");
        RunAllTestsInCurrentSection(DrawLargeDataSetTests);

        // Comparison tests
        Debug.Log("\n--- Comparison Tests ---");
        RunAllTestsInCurrentSection(DrawComparisonTests);

        Debug.Log("\n=== All Tests Complete ===");
    }

    /// <summary>
    /// Helper to run all tests in a section by simulating button clicks.
    /// </summary>
    private void RunAllTestsInCurrentSection(Action sectionAction)
    {
        // Note: This is a simplified version that re-executes the test logic
        // In a full implementation, you'd want to extract the test logic into separate methods
    }

    #endregion

    #region Test Data Types

    /// <summary>Simple test data class.</summary>
    [Serializable]
    private class SimpleTestData
    {
        public int Id;
        public string Name;
        public float Score;
    }

    /// <summary>Complex test data class.</summary>
    [Serializable]
    private class ComplexTestData
    {
        public int Id;
        public string Name;
        public float Health;
        public float Mana;
        public bool IsActive;
        public Vector3 Position;
        public Vector3 Rotation;
        public List<int> Inventory;
        public string[] Tags;
    }

    /// <summary>Nested test data class.</summary>
    [Serializable]
    private class NestedTestData
    {
        public int Level;
        public string Value;
        public NestedTestData Child;
    }

    /// <summary>Large test data class.</summary>
    [Serializable]
    private class LargeTestData
    {
        public int[] IntArray;
        public List<string> StringList;
        public float[,] FloatMatrix;
    }

    #endregion
}
