using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.GameConsole
{
    internal struct GameConsoleLogItemData
    {
        public DateTime Time { get; set; }
        public GameConsoleLogType LogType { get; set; }
        public string Message { get; set; }
        public StackTrace StackTrace { get; set; }
    }
    
    public enum GameConsoleLogType
    {
        Info,
        Warn,
        Error
    }

    public class GameConsoleManager : MonoSingleton<GameConsoleManager>
    {

        [Header("Binding")]
        [SerializeField] private GameConsoleLogWindow _logWindow;
        [SerializeField] private GameConsoleLogPopup _logPopup;
        [SerializeField] private GameConsoleConfig _config;

        internal event Action<GameConsoleLogItemData> OnPushLog;
        internal event Action OnClearLogs;

        internal readonly List<GameConsoleLogItemData> LogItemDataList = new List<GameConsoleLogItemData>();

        private readonly IBindableValue<int> _infoLogCount = new BindableValue<int>();
        private readonly IBindableValue<int> _warnLogCount = new BindableValue<int>();
        private readonly IBindableValue<int> _errorLogCount = new BindableValue<int>();

        public IReadonlyBindableValue<int> InfoLogCount => _infoLogCount;
        public IReadonlyBindableValue<int> WarnLogCount => _warnLogCount;
        public IReadonlyBindableValue<int> ErrorLogCount => _errorLogCount;
        public GameConsoleConfig Config => _config;

        public void ClearLogs()
        {
            LogItemDataList.Clear();
            OnClearLogs?.Invoke();

            _infoLogCount.SetValue(0);
            _warnLogCount.SetValue(0);
            _errorLogCount.SetValue(0);
        }

        public void ShowLogWindow()
        {
            _logWindow.Show();
            _logPopup.Hide();
        }

        public void HideLogWindow()
        {
            _logWindow.Hide();
            _logPopup.Show();
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void LogInfo(string message)
        {
            Log(GameConsoleLogType.Info, message);
        }

        public void LogWarn(string message)
        {
            Log(GameConsoleLogType.Warn, message);
        }

        public void LogError(string message)
        {
            Log(GameConsoleLogType.Error, message);
        }


        public void Log(GameConsoleLogType logType, string message)
        {
            Log(logType, message, new StackTrace(true));
        }

        public void Log(GameConsoleLogType logType, string message, StackTrace stackTrace)
        {
            var data = new GameConsoleLogItemData()
            {
                Time = DateTime.Now,
                LogType = logType,
                Message = message,
                StackTrace = stackTrace
            };
            LogItemDataList.Add(data);
            OnPushLog?.Invoke(data);
            switch (logType)
            {
                case GameConsoleLogType.Info:
                    _infoLogCount.SetValue(_infoLogCount.Value + 1);
                    break;
                case GameConsoleLogType.Warn:
                    _warnLogCount.SetValue(_warnLogCount.Value + 1);
                    break;
                case GameConsoleLogType.Error:
                    _errorLogCount.SetValue(_errorLogCount.Value + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}
