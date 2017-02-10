using System;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Tests
{
    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, string descriptionOfTask)
        {
            await WithTimeout(task, 10, descriptionOfTask);
        }

        public static async Task WithTimeout(this Task task, int seconds, string descriptionOfTask)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(seconds * 1000));

            if (completedTask != task)
            {
                throw new TimeoutException("Timeout waiting for " + descriptionOfTask);
            }
        }
    }
}