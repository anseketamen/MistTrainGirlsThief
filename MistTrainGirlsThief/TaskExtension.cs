using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MistTrainGirlsThief
{
    public static class TaskExtension
    {
        public static async Task Timeout(this Task task, TimeSpan timeout)
        {
            var delay = Task.Delay(timeout);
            if (await Task.WhenAny(task, delay) == delay)
            {
                throw new TimeoutException();
            }
        }

        public static async Task<T> Timeout<T>(this Task<T> task, TimeSpan timeout)
        {
            await ((Task)task).Timeout(timeout);
            return await task;
        }
    }
}
