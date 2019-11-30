namespace RadeonResetBugFixService.ThirdParty.DisableDevice
{
    // Code taken from https://stackoverflow.com/a/1610140
    // Code for obtaining device status is mine

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using Microsoft.Win32.SafeHandles;
    using System.Security;
    using System.Runtime.ConstrainedExecution;


    [Flags()]
    internal enum SetupDiGetClassDevsFlags
    {
        Default = 1,
        Present = 2,
        AllClasses = 4,
        Profile = 8,
        DeviceInterface = (int)0x10
    }

    internal enum DiFunction
    {
        SelectDevice = 1,
        InstallDevice = 2,
        AssignResources = 3,
        Properties = 4,
        Remove = 5,
        FirstTimeSetup = 6,
        FoundDevice = 7,
        SelectClassDrivers = 8,
        ValidateClassDrivers = 9,
        InstallClassDrivers = (int)0xa,
        CalcDiskSpace = (int)0xb,
        DestroyPrivateData = (int)0xc,
        ValidateDriver = (int)0xd,
        Detect = (int)0xf,
        InstallWizard = (int)0x10,
        DestroyWizardData = (int)0x11,
        PropertyChange = (int)0x12,
        EnableClass = (int)0x13,
        DetectVerify = (int)0x14,
        InstallDeviceFiles = (int)0x15,
        UnRemove = (int)0x16,
        SelectBestCompatDrv = (int)0x17,
        AllowInstall = (int)0x18,
        RegisterDevice = (int)0x19,
        NewDeviceWizardPreSelect = (int)0x1a,
        NewDeviceWizardSelect = (int)0x1b,
        NewDeviceWizardPreAnalyze = (int)0x1c,
        NewDeviceWizardPostAnalyze = (int)0x1d,
        NewDeviceWizardFinishInstall = (int)0x1e,
        Unused1 = (int)0x1f,
        InstallInterfaces = (int)0x20,
        DetectCancel = (int)0x21,
        RegisterCoInstallers = (int)0x22,
        AddPropertyPageAdvanced = (int)0x23,
        AddPropertyPageBasic = (int)0x24,
        Reserved1 = (int)0x25,
        Troubleshooter = (int)0x26,
        PowerMessageWake = (int)0x27,
        AddRemotePropertyPageAdvanced = (int)0x28,
        UpdateDriverUI = (int)0x29,
        Reserved2 = (int)0x30
    }

    internal enum StateChangeAction
    {
        Enable = 1,
        Disable = 2,
        PropChange = 3,
        Start = 4,
        Stop = 5
    }

    [Flags()]
    internal enum Scopes
    {
        Global = 1,
        ConfigSpecific = 2,
        ConfigGeneral = 4
    }

    internal enum SetupApiError
    {
        NoAssociatedClass = unchecked((int)0xe0000200),
        ClassMismatch = unchecked((int)0xe0000201),
        DuplicateFound = unchecked((int)0xe0000202),
        NoDriverSelected = unchecked((int)0xe0000203),
        KeyDoesNotExist = unchecked((int)0xe0000204),
        InvalidDevinstName = unchecked((int)0xe0000205),
        InvalidClass = unchecked((int)0xe0000206),
        DevinstAlreadyExists = unchecked((int)0xe0000207),
        DevinfoNotRegistered = unchecked((int)0xe0000208),
        InvalidRegProperty = unchecked((int)0xe0000209),
        NoInf = unchecked((int)0xe000020a),
        NoSuchHDevinst = unchecked((int)0xe000020b),
        CantLoadClassIcon = unchecked((int)0xe000020c),
        InvalidClassInstaller = unchecked((int)0xe000020d),
        DiDoDefault = unchecked((int)0xe000020e),
        DiNoFileCopy = unchecked((int)0xe000020f),
        InvalidHwProfile = unchecked((int)0xe0000210),
        NoDeviceSelected = unchecked((int)0xe0000211),
        DevinfolistLocked = unchecked((int)0xe0000212),
        DevinfodataLocked = unchecked((int)0xe0000213),
        DiBadPath = unchecked((int)0xe0000214),
        NoClassInstallParams = unchecked((int)0xe0000215),
        FileQueueLocked = unchecked((int)0xe0000216),
        BadServiceInstallSect = unchecked((int)0xe0000217),
        NoClassDriverList = unchecked((int)0xe0000218),
        NoAssociatedService = unchecked((int)0xe0000219),
        NoDefaultDeviceInterface = unchecked((int)0xe000021a),
        DeviceInterfaceActive = unchecked((int)0xe000021b),
        DeviceInterfaceRemoved = unchecked((int)0xe000021c),
        BadInterfaceInstallSect = unchecked((int)0xe000021d),
        NoSuchInterfaceClass = unchecked((int)0xe000021e),
        InvalidReferenceString = unchecked((int)0xe000021f),
        InvalidMachineName = unchecked((int)0xe0000220),
        RemoteCommFailure = unchecked((int)0xe0000221),
        MachineUnavailable = unchecked((int)0xe0000222),
        NoConfigMgrServices = unchecked((int)0xe0000223),
        InvalidPropPageProvider = unchecked((int)0xe0000224),
        NoSuchDeviceInterface = unchecked((int)0xe0000225),
        DiPostProcessingRequired = unchecked((int)0xe0000226),
        InvalidCOInstaller = unchecked((int)0xe0000227),
        NoCompatDrivers = unchecked((int)0xe0000228),
        NoDeviceIcon = unchecked((int)0xe0000229),
        InvalidInfLogConfig = unchecked((int)0xe000022a),
        DiDontInstall = unchecked((int)0xe000022b),
        InvalidFilterDriver = unchecked((int)0xe000022c),
        NonWindowsNTDriver = unchecked((int)0xe000022d),
        NonWindowsDriver = unchecked((int)0xe000022e),
        NoCatalogForOemInf = unchecked((int)0xe000022f),
        DevInstallQueueNonNative = unchecked((int)0xe0000230),
        NotDisableable = unchecked((int)0xe0000231),
        CantRemoveDevinst = unchecked((int)0xe0000232),
        InvalidTarget = unchecked((int)0xe0000233),
        DriverNonNative = unchecked((int)0xe0000234),
        InWow64 = unchecked((int)0xe0000235),
        SetSystemRestorePoint = unchecked((int)0xe0000236),
        IncorrectlyCopiedInf = unchecked((int)0xe0000237),
        SceDisabled = unchecked((int)0xe0000238),
        UnknownException = unchecked((int)0xe0000239),
        PnpRegistryError = unchecked((int)0xe000023a),
        RemoteRequestUnsupported = unchecked((int)0xe000023b),
        NotAnInstalledOemInf = unchecked((int)0xe000023c),
        InfInUseByDevices = unchecked((int)0xe000023d),
        DiFunctionObsolete = unchecked((int)0xe000023e),
        NoAuthenticodeCatalog = unchecked((int)0xe000023f),
        AuthenticodeDisallowed = unchecked((int)0xe0000240),
        AuthenticodeTrustedPublisher = unchecked((int)0xe0000241),
        AuthenticodeTrustNotEstablished = unchecked((int)0xe0000242),
        AuthenticodePublisherNotTrusted = unchecked((int)0xe0000243),
        SignatureOSAttributeMismatch = unchecked((int)0xe0000244),
        OnlyValidateViaAuthenticode = unchecked((int)0xe0000245)
    }

    // CR_ return values from Cfgmgr32.h
    internal enum CmReturnCode : int
    {
        CR_SUCCESS = 0x00000000,
        CR_DEFAULT = 0x00000001,
        CR_OUT_OF_MEMORY = 0x00000002,
        CR_INVALID_POINTER = 0x00000003,
        CR_INVALID_FLAG = 0x00000004,
        CR_INVALID_DEVNODE = 0x00000005,
        CR_INVALID_RES_DES = 0x00000006,
        CR_INVALID_LOG_CONF = 0x00000007,
        CR_INVALID_ARBITRATOR = 0x00000008,
        CR_INVALID_NODELIST = 0x00000009,
        CR_DEVNODE_HAS_REQS = 0x0000000A,
        CR_INVALID_RESOURCEID = 0x0000000B,
        CR_DLVXD_NOT_FOUND = 0x0000000C,
        CR_NO_SUCH_DEVNODE = 0x0000000D,
        CR_NO_MORE_LOG_CONF = 0x0000000E,
        CR_NO_MORE_RES_DES = 0x0000000F,
        CR_ALREADY_SUCH_DEVNODE = 0x00000010,
        CR_INVALID_RANGE_LIST = 0x00000011,
        CR_INVALID_RANGE = 0x00000012,
        CR_FAILURE = 0x00000013,
        CR_NO_SUCH_LOGICAL_DEV = 0x00000014,
        CR_CREATE_BLOCKED = 0x00000015,
        CR_NOT_SYSTEM_VM = 0x00000016,
        CR_REMOVE_VETOED = 0x00000017,
        CR_APM_VETOED = 0x00000018,
        CR_INVALID_LOAD_TYPE = 0x00000019,
        CR_BUFFER_SMALL = 0x0000001A,
        CR_NO_ARBITRATOR = 0x0000001B,
        CR_NO_REGISTRY_HANDLE = 0x0000001C,
        CR_REGISTRY_ERROR = 0x0000001D,
        CR_INVALID_DEVICE_ID = 0x0000001E,
        CR_INVALID_DATA = 0x0000001F,
        CR_INVALID_API = 0x00000020,
        CR_DEVLOADER_NOT_READY = 0x00000021,
        CR_NEED_RESTART = 0x00000022,
        CR_NO_MORE_HW_PROFILES = 0x00000023,
        CR_DEVICE_NOT_THERE = 0x00000024,
        CR_NO_SUCH_VALUE = 0x00000025,
        CR_WRONG_TYPE = 0x00000026,
        CR_INVALID_PRIORITY = 0x00000027,
        CR_NOT_DISABLEABLE = 0x00000028,
        CR_FREE_RESOURCES = 0x00000029,
        CR_QUERY_VETOED = 0x0000002A,
        CR_CANT_SHARE_IRQ = 0x0000002B,
        CR_NO_DEPENDENT = 0x0000002C,
        CR_SAME_RESOURCES = 0x0000002D,
        CR_NO_SUCH_REGISTRY_KEY = 0x0000002E,
        CR_INVALID_MACHINENAME = 0x0000002F,
        CR_REMOTE_COMM_FAILURE = 0x00000030,
        CR_MACHINE_UNAVAILABLE = 0x00000031,
        CR_NO_CM_SERVICES = 0x00000032,
        CR_ACCESS_DENIED = 0x00000033,
        CR_CALL_NOT_IMPLEMENTED = 0x00000034,
        CR_INVALID_PROPERTY = 0x00000035,
        CR_DEVICE_INTERFACE_ACTIVE = 0x00000036,
        CR_NO_SUCH_DEVICE_INTERFACE = 0x00000037,
        CR_INVALID_REFERENCE_STRING = 0x00000038,
        CR_INVALID_CONFLICT_LIST = 0x00000039,
        CR_INVALID_INDEX = 0x0000003A,
        CR_INVALID_STRUCTURE_SIZE = 0x0000003B,
    }

        // DN_ flags from Cfg.h
        [Flags]
    internal enum CmStatus : ulong
    {
        DN_ROOT_ENUMERATED = 0x00000001, /* Was enumerated by ROOT */
        DN_DRIVER_LOADED = 0x00000002, /* Has Register_Device_Driver */
        DN_ENUM_LOADED = 0x00000004, /* Has Register_Enumerator */
        DN_STARTED = 0x00000008, /* Is currently configured */
        DN_MANUAL = 0x00000010, /* Manually installed */
        DN_NEED_TO_ENUM = 0x00000020, /* May need reenumeration */
        DN_NOT_FIRST_TIME = 0x00000040, /* Has received a config */
        DN_HARDWARE_ENUM = 0x00000080, /* Enum generates hardware ID */
        DN_LIAR = 0x00000100, /* Lied about can reconfig once */
        DN_HAS_MARK = 0x00000200, /* Not CM_Create_DevNode lately */
        DN_HAS_PROBLEM = 0x00000400, /* Need device installer */
        DN_FILTERED = 0x00000800, /* Is filtered */
        DN_MOVED = 0x00001000, /* Has been moved */
        DN_DISABLEABLE = 0x00002000, /* Can be rebalanced */
        DN_REMOVABLE = 0x00004000, /* Can be removed */
        DN_PRIVATE_PROBLEM = 0x00008000, /* Has a private problem */
        DN_MF_PARENT = 0x00010000, /* Multi function parent */
        DN_MF_CHILD = 0x00020000, /* Multi function child */
        DN_WILL_BE_REMOVED = 0x00040000, /* Devnode is being removed */
    }

    // CM_PROB_ problem values defined in Cfg.h
    internal enum CmProblem : ulong
    {
        CM_PROB_NOT_CONFIGURED = 0x00000001,
        CM_PROB_DEVLOADER_FAILED = 0x00000002,
        CM_PROB_OUT_OF_MEMORY = 0x00000003,
        CM_PROB_ENTRY_IS_WRONG_TYPE = 0x00000004,
        CM_PROB_LACKED_ARBITRATOR = 0x00000005,
        CM_PROB_BOOT_CONFIG_CONFLICT = 0x00000006,
        CM_PROB_FAILED_FILTER = 0x00000007,
        CM_PROB_DEVLOADER_NOT_FOUND = 0x00000008,
        CM_PROB_INVALID_DATA = 0x00000009,
        CM_PROB_FAILED_START = 0x0000000A,
        CM_PROB_LIAR = 0x0000000B,
        CM_PROB_NORMAL_CONFLICT = 0x0000000C,
        CM_PROB_NOT_VERIFIED = 0x0000000D,
        CM_PROB_NEED_RESTART = 0x0000000E,
        CM_PROB_REENUMERATION = 0x0000000F,
        CM_PROB_PARTIAL_LOG_CONF = 0x00000010,
        CM_PROB_UNKNOWN_RESOURCE = 0x00000011,
        CM_PROB_REINSTALL = 0x00000012,
        CM_PROB_REGISTRY = 0x00000013,
        CM_PROB_VXDLDR = 0x00000014,
        CM_PROB_WILL_BE_REMOVED = 0x00000015,
        CM_PROB_DISABLED = 0x00000016,
        CM_PROB_DEVLOADER_NOT_READY = 0x00000017,
        CM_PROB_DEVICE_NOT_THERE = 0x00000018,
        CM_PROB_MOVED = 0x00000019,
        CM_PROB_TOO_EARLY = 0x0000001A,
        CM_PROB_NO_VALID_LOG_CONF = 0x0000001B,
        CM_PROB_FAILED_INSTALL = 0x0000001C,
        CM_PROB_HARDWARE_DISABLED = 0x0000001D,
        CM_PROB_CANT_SHARE_IRQ = 0x0000001E,
        CM_PROB_FAILED_ADD = 0x0000001F,
        CM_PROB_DISABLED_SERVICE = 0x00000020,
        CM_PROB_TRANSLATION_FAILED = 0x00000021,
        CM_PROB_NO_SOFTCONFIG = 0x00000022,
        CM_PROB_BIOS_TABLE = 0x00000023,
        CM_PROB_IRQ_TRANSLATION_FAILED = 0x00000024,
        CM_PROB_FAILED_DRIVER_ENTRY = 0x00000025,
        CM_PROB_DRIVER_FAILED_PRIOR_UNLOAD = 0x00000026,
        CM_PROB_DRIVER_FAILED_LOAD = 0x00000027,
        CM_PROB_DRIVER_SERVICE_KEY_INVALID = 0x00000028,
        CM_PROB_LEGACY_SERVICE_NO_DEVICES = 0x00000029,
        CM_PROB_DUPLICATE_DEVICE = 0x0000002A,
        CM_PROB_FAILED_POST_START = 0x0000002B,
        CM_PROB_HALTED = 0x0000002C,
        CM_PROB_PHANTOM = 0x0000002D,
        CM_PROB_SYSTEM_SHUTDOWN = 0x0000002E,
        CM_PROB_HELD_FOR_EJECT = 0x0000002F,
        CM_PROB_DRIVER_BLOCKED = 0x00000030,
        CM_PROB_REGISTRY_TOO_LARGE = 0x00000031,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DeviceInfoData
    {
        public int Size;
        public Guid ClassGuid;
        public int DevInst;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PropertyChangeParameters
    {
        public int Size;
        // part of header. It's flattened out into 1 structure.
        public DiFunction DiFunction;
        public StateChangeAction StateChange;
        public Scopes Scope;
        public int HwProfile;
    }

    internal class NativeMethods
    {

        private const string setupapi = "setupapi.dll";

        private const string cfgmgr32 = "cfgmgr32.dll";

        private NativeMethods()
        {
        }

        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiCallClassInstaller(DiFunction installFunction, SafeDeviceInfoSetHandle deviceInfoSet, [In()]
ref DeviceInfoData deviceInfoData);

        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiEnumDeviceInfo(SafeDeviceInfoSetHandle deviceInfoSet, int memberIndex, ref DeviceInfoData deviceInfoData);

        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeDeviceInfoSetHandle SetupDiGetClassDevs([In()]
ref Guid classGuid, [MarshalAs(UnmanagedType.LPWStr)]
string enumerator, IntPtr hwndParent, SetupDiGetClassDevsFlags flags);

        /*
        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInstanceId(SafeDeviceInfoSetHandle deviceInfoSet, [In()]
ref DeviceInfoData did, [MarshalAs(UnmanagedType.LPTStr)]
StringBuilder deviceInstanceId, int deviceInstanceIdSize, [Out()]
ref int requiredSize);
        */
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInstanceId(
           IntPtr DeviceInfoSet,
           ref DeviceInfoData did,
           [MarshalAs(UnmanagedType.LPTStr)] StringBuilder DeviceInstanceId,
           int DeviceInstanceIdSize,
           out int RequiredSize
        );

        [SuppressUnmanagedCodeSecurity()]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport(setupapi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiSetClassInstallParams(SafeDeviceInfoSetHandle deviceInfoSet, [In()]
ref DeviceInfoData deviceInfoData, [In()]
ref PropertyChangeParameters classInstallParams, int classInstallParamsSize);

        [DllImport(cfgmgr32, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern CmReturnCode CM_Get_DevNode_Status(ref CmStatus pulStatus,
ref CmProblem pulProblemNumber,
int dnDevInst, ulong ulFlags);

    }

    internal class SafeDeviceInfoSetHandle : SafeHandleZeroOrMinusOneIsInvalid
    {

        public SafeDeviceInfoSetHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.SetupDiDestroyDeviceInfoList(this.handle);
        }

    }

    public sealed class DeviceHelper
    {

        private DeviceHelper()
        {
        }

        /// <summary>
        /// Enable or disable a device.
        /// </summary>
        /// <param name="classGuid">The class guid of the device. Available in the device manager.</param>
        /// <param name="instanceId">The device instance id of the device. Available in the device manager.</param>
        /// <param name="enable">True to enable, False to disable.</param>
        /// <remarks>Will throw an exception if the device is not Disableable.</remarks>
        public static void SetDeviceEnabled(Guid classGuid, string instanceId, bool enable)
        {
            SafeDeviceInfoSetHandle diSetHandle = null;
            try
            {
                // Get the handle to a device information set for all devices matching classGuid that are present on the 
                // system.
                diSetHandle = NativeMethods.SetupDiGetClassDevs(ref classGuid, null, IntPtr.Zero, SetupDiGetClassDevsFlags.Present);
                // Get the device information data for each matching device.
                DeviceInfoData[] diData = GetDeviceInfoData(diSetHandle);
                // Find the index of our instance. i.e. the touchpad mouse - I have 3 mice attached...
                int index = GetIndexOfInstance(diSetHandle, diData, instanceId);
                // Disable...
                EnableDevice(diSetHandle, diData[index], enable);
            }
            finally
            {
                if (diSetHandle != null)
                {
                    if (diSetHandle.IsClosed == false)
                    {
                        diSetHandle.Close();
                    }
                    diSetHandle.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns true if device is disabled
        /// </summary>
        /// <param name="classGuid">The class guid of the device. Available in the device manager.</param>
        /// <param name="instanceId">The device instance id of the device. Available in the device manager.</param>
        public static bool? IsDeviceDisabled(Guid classGuid, string instanceId)
        {
            SafeDeviceInfoSetHandle diSetHandle = null;
            try
            {
                // Get the handle to a device information set for all devices matching classGuid that are present on the 
                // system.
                diSetHandle = NativeMethods.SetupDiGetClassDevs(ref classGuid, null, IntPtr.Zero, SetupDiGetClassDevsFlags.Present);
                // Get the device information data for each matching device.
                DeviceInfoData[] diData = GetDeviceInfoData(diSetHandle);
                // Find the index of our instance. i.e. the touchpad mouse - I have 3 mice attached...
                int index = GetIndexOfInstance(diSetHandle, diData, instanceId);

                if (index == -1)
                {
                    return null;
                }

                // Get status...
                var status = default(CmStatus);
                var problem = default(CmProblem);
                var result = NativeMethods.CM_Get_DevNode_Status(ref status, ref problem, diData[index].DevInst, 0);

                if (result != CmReturnCode.CR_SUCCESS)
                {
                    throw new Win32Exception($"Cfgmgr32 error: {result}");
                }

                return problem == CmProblem.CM_PROB_DISABLED;
            }
            finally
            {
                if (diSetHandle != null)
                {
                    if (diSetHandle.IsClosed == false)
                    {
                        diSetHandle.Close();
                    }
                    diSetHandle.Dispose();
                }
            }
        }

        private static DeviceInfoData[] GetDeviceInfoData(SafeDeviceInfoSetHandle handle)
        {
            List<DeviceInfoData> data = new List<DeviceInfoData>();
            DeviceInfoData did = new DeviceInfoData();
            int didSize = Marshal.SizeOf(did);
            did.Size = didSize;
            int index = 0;
            while (NativeMethods.SetupDiEnumDeviceInfo(handle, index, ref did))
            {
                data.Add(did);
                index += 1;
                did = new DeviceInfoData();
                did.Size = didSize;
            }
            return data.ToArray();
        }

        // Find the index of the particular DeviceInfoData for the instanceId.
        private static int GetIndexOfInstance(SafeDeviceInfoSetHandle handle, DeviceInfoData[] diData, string instanceId)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            for (int index = 0; index <= diData.Length - 1; index++)
            {
                StringBuilder sb = new StringBuilder(1);
                int requiredSize = 0;
                bool result = NativeMethods.SetupDiGetDeviceInstanceId(handle.DangerousGetHandle(), ref diData[index], sb, sb.Capacity, out requiredSize);
                if (result == false && Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    sb.Capacity = requiredSize;
                    result = NativeMethods.SetupDiGetDeviceInstanceId(handle.DangerousGetHandle(), ref diData[index], sb, sb.Capacity, out requiredSize);
                }
                if (result == false)
                    throw new Win32Exception();
                if (instanceId.Equals(sb.ToString()))
                {
                    return index;
                }
            }
            // not found
            return -1;
        }

        // enable/disable...
        private static void EnableDevice(SafeDeviceInfoSetHandle handle, DeviceInfoData diData, bool enable)
        {
            PropertyChangeParameters @params = new PropertyChangeParameters();
            // The size is just the size of the header, but we've flattened the structure.
            // The header comprises the first two fields, both integer.
            @params.Size = 8;
            @params.DiFunction = DiFunction.PropertyChange;
            @params.Scope = Scopes.Global;
            if (enable)
            {
                @params.StateChange = StateChangeAction.Enable;
            }
            else
            {
                @params.StateChange = StateChangeAction.Disable;
            }

            bool result = NativeMethods.SetupDiSetClassInstallParams(handle, ref diData, ref @params, Marshal.SizeOf(@params));
            if (result == false) throw new Win32Exception();
            result = NativeMethods.SetupDiCallClassInstaller(DiFunction.PropertyChange, handle, ref diData);
            if (result == false)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == (int)SetupApiError.NotDisableable)
                    throw new ArgumentException("Device can't be disabled (programmatically or in Device Manager).");
                else if (err >= (int)SetupApiError.NoAssociatedClass && err <= (int)SetupApiError.OnlyValidateViaAuthenticode)
                    throw new Win32Exception("SetupAPI error: " + ((SetupApiError)err).ToString());
                else
                    throw new Win32Exception();
            }
        }
    }
}
