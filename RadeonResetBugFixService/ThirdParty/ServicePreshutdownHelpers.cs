namespace RadeonResetBugFixService.ThirdParty.ServicePreshutdownHelpers
{
    // Code taken from https://social.msdn.microsoft.com/Forums/vstudio/en-US/d14549e2-d0bc-47fb-bb01-7e0ac57fa712/keep-windows-service-alive-for-more-then-3-minutes-when-system-shut-down
    using System;
    using System.Runtime.InteropServices;

    [Flags]
    public enum SERVICE_ACCESS : uint
    {
        STANDARD_RIGHTS_REQUIRED = 0xF0000,
        SERVICE_QUERY_CONFIG = 0x00001,
        SERVICE_CHANGE_CONFIG = 0x00002,
        SERVICE_QUERY_STATUS = 0x00004,
        SERVICE_ENUMERATE_DEPENDENTS = 0x00008,
        SERVICE_START = 0x00010,
        SERVICE_STOP = 0x00020,
        SERVICE_PAUSE_CONTINUE = 0x00040,
        SERVICE_INTERROGATE = 0x00080,
        SERVICE_USER_DEFINED_CONTROL = 0x00100,
        SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
          SERVICE_QUERY_CONFIG |
          SERVICE_CHANGE_CONFIG |
          SERVICE_QUERY_STATUS |
          SERVICE_ENUMERATE_DEPENDENTS |
          SERVICE_START |
          SERVICE_STOP |
          SERVICE_PAUSE_CONTINUE |
          SERVICE_INTERROGATE |
          SERVICE_USER_DEFINED_CONTROL)
    }

    [Flags]
    public enum SCM_ACCESS : uint
    {
        STANDARD_RIGHTS_REQUIRED = 0xF0000,
        SC_MANAGER_CONNECT = 0x00001,
        SC_MANAGER_CREATE_SERVICE = 0x00002,
        SC_MANAGER_ENUMERATE_SERVICE = 0x00004,
        SC_MANAGER_LOCK = 0x00008,
        SC_MANAGER_QUERY_LOCK_STATUS = 0x00010,
        SC_MANAGER_MODIFY_BOOT_CONFIG = 0x00020,
        SC_MANAGER_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED |
          SC_MANAGER_CONNECT |
          SC_MANAGER_CREATE_SERVICE |
          SC_MANAGER_ENUMERATE_SERVICE |
          SC_MANAGER_LOCK |
          SC_MANAGER_QUERY_LOCK_STATUS |
          SC_MANAGER_MODIFY_BOOT_CONFIG
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_STATUS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    }

    public enum SERVICE_STATE : uint
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007
    }

    public enum INFO_LEVEL : uint
    {
        SERVICE_CONFIG_DESCRIPTION = 0x00000001,
        SERVICE_CONFIG_FAILURE_ACTIONS = 0x00000002,
        SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 0x00000003,
        SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 0x00000004,
        SERVICE_CONFIG_SERVICE_SID_INFO = 0x00000005,
        SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 0x00000006,
        SERVICE_CONFIG_PRESHUTDOWN_INFO = 0x00000007,
        SERVICE_CONFIG_TRIGGER_INFO = 0x00000008,
        SERVICE_CONFIG_PREFERRED_NODE = 0x00000009
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_PRESHUTDOWN_INFO
    {
        public UInt32 dwPreshutdownTimeout;
    }

    [Flags]
    public enum SERVICE_CONTROL : uint
    {
        STOP = 0x00000001,
        PAUSE = 0x00000002,
        CONTINUE = 0x00000003,
        INTERROGATE = 0x00000004,
        SHUTDOWN = 0x00000005,
        PARAMCHANGE = 0x00000006,
        NETBINDADD = 0x00000007,
        NETBINDREMOVE = 0x00000008,
        NETBINDENABLE = 0x00000009,
        NETBINDDISABLE = 0x0000000A,
        DEVICEEVENT = 0x0000000B,
        HARDWAREPROFILECHANGE = 0x0000000C,
        POWEREVENT = 0x0000000D,
        SESSIONCHANGE = 0x0000000E
    }

    public enum ControlsAccepted
    {
        ACCEPT_STOP = 1,
        ACCEPT_PAUSE_CONTINUE = 2,
        ACCEPT_SHUTDOWN = 4,
        ACCEPT_PRESHUTDOWN = 0xf,
        ACCEPT_POWER_EVENT = 64,
        ACCEPT_SESSION_CHANGE = 128
    }
    internal class Interop
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ControlService(
          IntPtr hService,
          SERVICE_CONTROL dwControl,
          ref SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr OpenSCManager(
          string machineName,
          string databaseName,
          uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "QueryServiceStatus", CharSet = CharSet.Auto)]
        internal static extern bool QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS dwServiceStatus);

        [DllImport("advapi32.dll")]
        internal static extern bool SetServiceStatus(IntPtr hServiceStatus, ref SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(
          IntPtr hService,
          int dwInfoLevel,
          IntPtr lpInfo);
    }

    class ServicePreshutdownHelpers
    {
        public static void SetPreShutdownTimeOut(string serviceName, uint milliseconds)
        {
            // get sc manager handle
            IntPtr hMngr = Interop.OpenSCManager(null, null, (uint)SCM_ACCESS.SC_MANAGER_ALL_ACCESS);

            if (hMngr == IntPtr.Zero)
                throw new Exception("Failed to open SC Manager handle");
            else
            {
                // get the service's handle
                IntPtr hSvc = Interop.OpenService(hMngr, serviceName, (uint)SCM_ACCESS.SC_MANAGER_ALL_ACCESS);

                if (hSvc == IntPtr.Zero)
                    throw new Exception("Failed to open service handle");
                else
                {
                    SERVICE_PRESHUTDOWN_INFO spi = new SERVICE_PRESHUTDOWN_INFO();
                    spi.dwPreshutdownTimeout = milliseconds;

                    IntPtr lpInfo = Marshal.AllocHGlobal(Marshal.SizeOf(spi));
                    if (lpInfo == IntPtr.Zero)
                    {
                        throw new Exception(String.Format("Unable to allocate memory for service action, error was: 0x{0:X}", Marshal.GetLastWin32Error()));
                    }

                    Marshal.StructureToPtr(spi, lpInfo, false);

                    // apply the new timeout value
                    if (!Interop.ChangeServiceConfig2(hSvc, (int)INFO_LEVEL.SERVICE_CONFIG_PRESHUTDOWN_INFO, lpInfo))
                        throw new Exception("Failed to change service timeout");

                    Interop.CloseServiceHandle(hSvc);
                }

                Interop.CloseServiceHandle(hMngr);
            }
        }
    }
}
