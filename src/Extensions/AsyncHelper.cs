﻿/*
 *
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Talegen.Common.Core.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains helper methods that allow a developer to run an asynchronous method from within a synchronous method.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Contains a new task factory.
        /// </summary>
        private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        /// <summary>
        /// Executes and returns the value of the asynchronous method function specified.
        /// </summary>
        /// <typeparam name="TResult">Contains the type of the result to return.</typeparam>
        /// <param name="func">Contains the function to execute.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the result of the function.</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default(CancellationToken))
        {
            return TaskFactory.StartNew(func, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the asynchronous method function specified.
        /// </summary>
        /// <param name="func">Contains the function to execute.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        public static void RunSync(Func<Task> func, CancellationToken cancellationToken = default(CancellationToken))
        {
            TaskFactory.StartNew(func, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
    }
}