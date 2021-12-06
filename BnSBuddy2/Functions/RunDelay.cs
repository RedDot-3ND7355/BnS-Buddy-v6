using System;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public static class RunDelay
    {
        public static void Method(Action method) =>
            Task.Delay(50).ContinueWith(delegate { method(); });
    }
}
