using System;

namespace MMFrame.Windows.GlobalHook
{
    public static class MouseHook
    {
        private static class NativeMethods
        {

            public delegate System.IntPtr MouseHookCallback(int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);

            [System.Runtime.InteropServices.DllImport("user32")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, MouseHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);

            [System.Runtime.InteropServices.DllImport("user32")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);

            [System.Runtime.InteropServices.DllImport("user32")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);


            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);

        }

        public struct StateMouse
        {
            public Stroke Stroke;
            public int X;
            public int Y;
            public uint Data;
            public uint Flags;
            public uint Time;
            public System.IntPtr ExtraInfo;
        }

        public enum Stroke
        {
            MOVE,
            LEFT_DOWN,
            LEFT_UP,
            RIGHT_DOWN,
            RIGHT_UP,
            MIDDLE_DOWN,
            MIDDLE_UP,
            WHEEL_DOWN,
            WHEEL_UP,
            X1_DOWN,
            X1_UP,
            X2_DOWN,
            X2_UP,
            UNKNOWN
        }

        public static bool IsHooking
        {
            get;
            private set;
        }

        public static bool IsPaused
        {
            get;
            private set;
        }

        public static StateMouse State;
        public delegate void HookHandler(ref StateMouse state);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        private static System.IntPtr Handle;
 
        private static bool IsCancel;

        private static System.Collections.Generic.List<HookHandler> Events;

        private static event HookHandler HookEvent;

        private static event NativeMethods.MouseHookCallback hookCallback;

        public static void Start()
        {
            if (IsHooking)
            {
                return;
            }

            IsHooking = true;
            IsPaused = false;

            hookCallback = HookProcedure;
            System.IntPtr h = IntPtr.Zero;
            Handle = NativeMethods.SetWindowsHookEx(7, hookCallback, h, (uint)AppDomain.GetCurrentThreadId());

            if (Handle == System.IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                throw new System.ComponentModel.Win32Exception();

            }
        }

        public static void Stop()
        {
            if (!IsHooking)
            {
                return;
            }

            if (Handle != System.IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                ClearEvent();

                NativeMethods.UnhookWindowsHookEx(Handle);
                Handle = System.IntPtr.Zero;
                hookCallback -= HookProcedure;
            }
        }

        public static void Enable()
        {
            IsCancel = false;
        }

        public static void Disable()
        {
            IsCancel = true;
        }

        public static void Pause()
        {
            IsPaused = true;
        }

        public static void AddEvent(HookHandler hookHandler)
        {
            if (Events == null)
            {
                Events = new System.Collections.Generic.List<HookHandler>();
            }

            Events.Add(hookHandler);
            HookEvent += hookHandler;
        }

        public static void RemoveEvent(HookHandler hookHandler)
        {
            if (Events == null)
            {
                return;
            }

            HookEvent -= hookHandler;
            Events.Remove(hookHandler);
        }

        public static void ClearEvent()
        {
            if (Events == null)
            {
                return;
            }

            foreach (HookHandler e in Events)
            {
                HookEvent -= e;
            }

            Events.Clear();
        }

        private static System.IntPtr HookProcedure(int nCode, uint msg, ref MSLLHOOKSTRUCT s)
        {
            if (nCode >= 0 && HookEvent != null && !IsPaused)
            {
                State.Stroke = GetStroke(msg, ref s);
                State.X = s.pt.x;
                State.Y = s.pt.y;
                State.Data = s.mouseData;
                State.Flags = s.flags;
                State.Time = s.time;
                State.ExtraInfo = s.dwExtraInfo;

                HookEvent(ref State);

                if (IsCancel)
                {
                    return (System.IntPtr)1;
                }
            }
            return NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s);
        }

        private static Stroke GetStroke(uint msg, ref MSLLHOOKSTRUCT s)
        {
            switch (msg)
            {
                case 0x0200:
                    // WM_MOUSEMOVE
                    return Stroke.MOVE;
                case 0x0201:
                    // WM_LBUTTONDOWN
                    return Stroke.LEFT_DOWN;
                case 0x0202:
                    // WM_LBUTTONUP
                    return Stroke.LEFT_UP;
                case 0x0204:
                    // WM_RBUTTONDOWN
                    return Stroke.RIGHT_DOWN;
                case 0x0205:
                    // WM_RBUTTONUP
                    return Stroke.RIGHT_UP;
                case 0x0207:
                    // WM_MBUTTONDOWN
                    return Stroke.MIDDLE_DOWN;
                case 0x0208:
                    // WM_MBUTTONUP
                    return Stroke.MIDDLE_UP;
                case 0x020A:
                    // WM_MOUSEWHEE
                    return ((short)((s.mouseData >> 16) & 0xffff) > 0) ? Stroke.WHEEL_UP : Stroke.WHEEL_DOWN;
                case 0x20B:
                    // WM_XBUTTONDOWN
                    switch (s.mouseData >> 16)
                    {
                        case 1:
                            return Stroke.X1_DOWN;
                        case 2:
                            return Stroke.X2_DOWN;
                        default:
                            return Stroke.UNKNOWN;
                    }
                case 0x20C:
                    // WM_XBUTTONUP
                    switch (s.mouseData >> 16)
                    {
                        case 1:
                            return Stroke.X1_UP;
                        case 2:
                            return Stroke.X2_UP;
                        default:
                            return Stroke.UNKNOWN;
                    }
                default:
                    return Stroke.UNKNOWN;
            }
        }

        public static void SetCursorPos(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

    }
}
