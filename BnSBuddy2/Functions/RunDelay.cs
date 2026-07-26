using System;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public static class RunDelay
    {
        public static void Method(Action method, int ms = 50) =>
            Task.Delay(ms).ContinueWith(delegate { method(); });
    }
}
