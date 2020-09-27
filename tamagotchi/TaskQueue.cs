using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tamagotchi
{
    public class TaskQueue
    {
        int interval; //
        int mul; //
        Dictionary<Action, int> tasks;
        Thread thread;

        public TaskQueue(int interval = 1000, int mul = 10)
        {
            this.interval = interval;
            this.mul = mul;
            tasks = new Dictionary<Action, int>();
            thread = new Thread(new ThreadStart(loop));
        }

        void loop()
        {
            for(;;)
            {
                tasks = tasks.Select(task => new KeyValuePair<Action, int>(task.Key, task.Value - 1)).ToDictionary(x => x.Key, x => x.Value);
                foreach (KeyValuePair<Action, int> task in tasks.Where(task => task.Value == 0)) {
                    task.Key.Invoke();
                }
                Thread.Sleep(interval / mul);
            }
        }

        public void Add(Action act, int interval) => tasks.Add(act, interval * mul);
        public void Clear(Action act, int interval) => tasks.Clear();
        public void Stop() => thread.Abort();

    }
}
