using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AntiDeathLoop
{
    public class DeathLoopWatcher
    {
        private readonly int _waitInterval;
        
        private readonly Thread _mainThread;
        private readonly Thread _watcherThread;
        
        private bool _checkFlag;

        private readonly string[] _buttons = {"Wait", "Interrupt", "Abort", "Stop"};

        public DeathLoopWatcher(int waitInterval)
        {
            _waitInterval = waitInterval;
            _mainThread = Thread.CurrentThread;
            _watcherThread = new Thread(Watcher);
            _watcherThread.Start();
        }

        ~DeathLoopWatcher()
        {
            Stop();
        }

        public void Stop()
        {
            _watcherThread?.Abort();
        }
        
        public void UpdateMainThread()
        {
            _checkFlag = true;
        }

        private void Watcher()
        {
            while (true)
            {
                try
                {
                    _checkFlag = false;
                    Thread.Sleep(_waitInterval);
                    if (!_checkFlag)
                    {
                        var message = $"Main thread did not respond throughout {_waitInterval}ms";
                        var answer = NativeMessageBox.Show("Unity not response", message, _buttons);

                        if (answer == 1)
                        {
                            _mainThread.Interrupt();
                        }
                        else if (answer == 2)
                        {
                            _mainThread.Abort();
                        }
                        else if (answer == 3)
                        {

                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }
    }
}