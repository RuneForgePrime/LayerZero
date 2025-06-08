using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Managements
{
    /// <summary>
    /// GeasMaster is a task management class
    /// </summary>
    public static class GeasMaster
    {
        private static readonly TaskFactory _taskfactory = new TaskFactory(CancellationToken.None,
                                                                            TaskCreationOptions.None,
                                                                            TaskContinuationOptions.None,
                                                                            TaskScheduler.Default);

        /// <summary>
        /// Executes an asynchronous task with a result synchronously.
        /// </summary>
        /// <typeparam name="T">The result type of the asynchronous task.</typeparam>
        /// <param name="Task">The asynchronous function to execute.</param>
        /// <returns>The result of the asynchronous operation.</returns>
        public static T RunSync<T>(Func<CancellationToken, Task<T>> Task, CancellationToken ct)
            => _taskfactory
            .StartNew(() => Task(ct), ct)
            .Unwrap()
            .GetAwaiter()
            .GetResult();

        public static T RunSync<T>(Func<Task<T>> Task)
            => _taskfactory
            .StartNew(Task)
            .Unwrap()
            .GetAwaiter()
            .GetResult();

        /// <summary>
        /// Execute an async task which has a void return
        /// </summary>
        /// <param name="Task"></param>
        public static void RunSync(Func<CancellationToken, Task> Task, CancellationToken ct)
            => _taskfactory
            .StartNew(() => Task(ct), ct)
            .Unwrap()
            .GetAwaiter()
            .GetResult();

        public static void RunSync(Func<Task> Task)
            => _taskfactory
            .StartNew(Task)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
            
    }
}
