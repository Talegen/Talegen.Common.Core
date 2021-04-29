/*
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

namespace Talegen.Common.Core.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains thread task related extensions.
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        /// Creates the parallel for each options.
        /// </summary>
        /// <param name="maxDegreesParallel">The maximum degrees parallel.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a new ParallelOptions object.</returns>
        public static ParallelOptions CreateOptions(int maxDegreesParallel = -1, CancellationToken cancellationToken = default)
        {
            int calculatedCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0));
            ThreadPool.GetMinThreads(out int workerThreads, out _);

            // if we're specifying a thread count, and existing worker threads are greater than the max specified and greater or equal to calculated, set to
            // worker thread count value.
            if (maxDegreesParallel > 0 && (workerThreads > maxDegreesParallel && workerThreads >= calculatedCount))
            {
                maxDegreesParallel = workerThreads;
            }

            // if max degrees is 0 or less, or calculated is greater than max degrees, use calculated, otherwise use max degrees specified.
            return new ParallelOptions
            {
                // if the max degrees is set to 0 or a value below -1, the number shall be calculated.
                MaxDegreeOfParallelism = maxDegreesParallel <= 0 || calculatedCount > maxDegreesParallel ? calculatedCount : maxDegreesParallel,
                CancellationToken = cancellationToken,
                TaskScheduler = TaskScheduler.Default
            };
        }
    }
}