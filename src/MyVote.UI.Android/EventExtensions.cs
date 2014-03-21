using System;

namespace MyVote.UI.Droid
{
    public static class EventExtensions{
        public static void SafeInvoke<T> (this EventHandler<T> e, object sender, T ea) where T: EventArgs
        {
            if (e != null) e.Invoke (sender, ea);
        }

        public static void SafeInvoke(this EventHandler e, object sender, EventArgs ea)
        {
            if (e != null) e.Invoke (sender, ea);
        }
    }
}

