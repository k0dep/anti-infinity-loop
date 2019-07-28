using System.IO;
using UnityEditor;
using UnityEngine;

namespace AntiDeathLoop
{
    public static class AntiDeathLoopUnityBridge
    {
        private static DeathLoopWatcher _watcher;

        private static int _waitInterval = 10000;
        private static string _configPath;

        [InitializeOnLoadMethod]
        public static void OnInitialize()
        {
            InitializeWatcher();
            EditorApplication.update += Update;
        }

        [MenuItem("Window/Anti-Death-Loop/Create config file")]
        public static void CreateConfigFile()
        {
            if (File.Exists(_configPath))
            {
                Debug.LogWarning($"File exists at path: {_configPath}");
            }
            else
            {
                File.WriteAllText(_configPath, _waitInterval.ToString());
                Debug.Log($"Created config file at path: {_configPath}");
            }
        }
        
        [MenuItem("Window/Anti-Death-Loop/Restart")]
        public static void Restart()
        {
            Stop();
            InitializeWatcher();
            Debug.Log("Anti-Death-Loop restarted");
        }
        
        [MenuItem("Window/Anti-Death-Loop/Stop")]
        public static void Stop()
        {
            _watcher?.Stop();
            _watcher = null;
            Debug.Log("Anti-Death-Loop stopped");
        }
        
        [MenuItem("Window/Anti-Death-Loop/Start")]
        public static void Start()
        {
            if (_watcher != null)
            {
                Debug.LogWarning("Anti-Death-Loop is running");
                return;
            }
            InitializeWatcher();
        }
        
        [MenuItem("Window/Anti-Death-Loop/Status")]
        public static void Status()
        {
            if (_watcher != null)
            {
                Debug.Log("Anti-Death-Loop is running");
                return;
            }
            
            Debug.Log("Anti-Death-Loop stopped");
        }
        
        private static void InitializeWatcher()
        {
            if (Application.isBatchMode)
            {
                Debug.Log("Anti-Death-Loop disabled in batchmode");
                return;
            }
            
            _configPath = Path.Combine(Application.dataPath, "anti-death-loop.cfg");
            ParseWaitIntervalFromFile();
            _watcher = new DeathLoopWatcher(_waitInterval);
        }

        private static void ParseWaitIntervalFromFile()
        {
            if (File.Exists(_configPath))
            {
                var content = File.ReadAllText(_configPath);
                if (int.TryParse(content, out var waitInterval))
                {
                    _waitInterval = waitInterval;
                }
            }
        }

        private static void Update()
        {
            _watcher?.UpdateMainThread();
        }
    }
}