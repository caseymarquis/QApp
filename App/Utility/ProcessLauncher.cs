using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

//****************************** Class Header *******************************\
// Project Name: LaunchService
// Class Name:   WindowsAPI
// File Name:    WindowsAPI.vb
// Author:       fonbr01
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//***************************************************************************/

// Imports
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;

namespace QApp {

    // Windows API Class
    public sealed class ProcessLauncher {

        // *************************
        // * Windows API Functions
        // *************************

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges([In()]
            SafeTokenHandle TokenHandle, [In(), MarshalAs(UnmanagedType.Bool)]
            bool DisableAllPrivileges, [In()]
            ref TOKEN_PRIVILEGES NewState, [In()]
            UInt32 BufferLengthInBytes, [In(), Out()]
            ref TOKEN_PRIVILEGES PreviousState, [In(), Out()]
            ref UInt32 ReturnLengthInBytes);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateProcessAsUser([In()]
            SafeTokenHandle hToken, [In(), MarshalAs(UnmanagedType.LPWStr)]
            string lpApplicationName, [In(), Out(), MarshalAs(UnmanagedType.LPWStr)]
            string lpCommandLine, [In()]
            ref SECURITY_ATTRIBUTES lpProcessAttributes, [In()]
            ref SECURITY_ATTRIBUTES lpThreadAttributes, [In(), MarshalAs(UnmanagedType.Bool)]
            bool bInheritHandles, [In()]
            uint dwCreationFlags, [In()]
            IntPtr lpEnvironment, [In(), MarshalAs(UnmanagedType.LPWStr)]
            string lpCurrentDirectory, [In()]
            ref STARTUPINFO lpStartupInfo,
                       [In(), Out()]
            ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DuplicateToken([In()]
            SafeTokenHandle ExistingTokenHandle, [In()]
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, [In(), Out()]
            ref SafeTokenHandle DuplicateTokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DuplicateTokenEx([In()]
            IntPtr hExistingToken, [In()]
            uint dwDesiredAccess, [In()]
            ref SECURITY_ATTRIBUTES lpTokenAttributes, [In()]
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, [In()]
            TOKEN_TYPE TokenType, [In(), Out()]
            ref SafeTokenHandle phNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue([In(), MarshalAs(UnmanagedType.LPWStr)]
            string lpSystemName, [In(), MarshalAs(UnmanagedType.LPWStr)]
            string lpName, [In(), Out()]
            ref LUID lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken([In()]
            IntPtr hProcess, [In()]
            UInt32 desiredAccess, [In(), Out()]
            ref SafeTokenHandle hToken);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess([In()] IntPtr hProcess, [In(), Out()] ref UInt32 lpExitCode);


        // *************************
        // * Structures
        // *************************

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION {
            public IntPtr hProcess;
            public IntPtr hThread;
            public System.UInt32 dwProcessId;
            public System.UInt32 dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES {
            public System.UInt32 nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO {
            public System.UInt32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public System.UInt32 dwX;
            public System.UInt32 dwY;
            public System.UInt32 dwXSize;
            public System.UInt32 dwYSize;
            public System.UInt32 dwXCountChars;
            public System.UInt32 dwYCountChars;
            public System.UInt32 dwFillAttribute;
            public System.UInt32 dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        private struct LUID {
            public UInt32 LowPart;
            public int HighPart;
        }

        private struct LUID_AND_ATTRIBUTES {
            public LUID Luid;
            public uint Attributes;
        }

        private struct TOKEN_PRIVILEGES {
            public UInt32 PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }


        // ******************************
        // * Enumerations
        // ******************************

        private enum CreateProcessFlags {
            DEBUG_PROCESS = 0x1,
            DEBUG_ONLY_THIS_PROCESS = 0x2,
            CREATE_SUSPENDED = 0x4,
            DETACHED_PROCESS = 0x8,
            CREATE_NEW_CONSOLE = 0x10,
            NORMAL_PRIORITY_CLASS = 0x20,
            IDLE_PRIORITY_CLASS = 0x40,
            HIGH_PRIORITY_CLASS = 0x80,
            REALTIME_PRIORITY_CLASS = 0x100,
            CREATE_NEW_PROCESS_GROUP = 0x200,
            CREATE_UNICODE_ENVIRONMENT = 0x400,
            CREATE_SEPARATE_WOW_VDM = 0x800,
            CREATE_SHARED_WOW_VDM = 0x1000,
            CREATE_FORCEDOS = 0x2000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x4000,
            ABOVE_NORMAL_PRIORITY_CLASS = 0x8000,
            INHERIT_PARENT_AFFINITY = 0x10000,
            INHERIT_CALLER_PRIORITY = 0x20000,
            CREATE_PROTECTED_PROCESS = 0x40000,
            EXTENDED_STARTUPINFO_PRESENT = 0x80000,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x100000,
            PROCESS_MODE_BACKGROUND_END = 0x200000,
            CREATE_BREAKAWAY_FROM_JOB = 0x1000000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000,
            CREATE_DEFAULT_ERROR_MODE = 0x4000000,
            CREATE_NO_WINDOW = 0x8000000,
            PROFILE_USER = 0x10000000,
            PROFILE_KERNEL = 0x20000000,
            PROFILE_SERVER = 0x40000000,
            CREATE_IGNORE_SYSTEM_DEFAULT = unchecked((int)0x80000000),
        }

        private enum SECURITY_IMPERSONATION_LEVEL {
            SecurityAnonymous = 0,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        private enum TOKEN_TYPE {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }


        // ******************************
        // * Constants
        // ******************************

        private const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
        private const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
        private const string SE_TCB_NAME = "SeTcbPrivilege";

        private const UInt32 SE_PRIVILEGE_ENABLED = 0x2;

        // ******************************
        // * Safe Token Handle Class
        // ******************************

        private class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid {

            internal SafeTokenHandle()
                : base(true) {
            }

            internal SafeTokenHandle(IntPtr handle)
                : base(true) {
                base.SetHandle(handle);
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            static internal extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle() {
                return SafeTokenHandle.CloseHandle(base.handle);
            }

        }


        // ******************************
        // * Increase Privileges Function
        // ******************************

        public static bool IncreasePrivileges() {

            // Local variables
            SafeTokenHandle hToken = null;
            LUID luid = default(LUID);
            TOKEN_PRIVILEGES NewState = default(TOKEN_PRIVILEGES);
            NewState.PrivilegeCount = 1;
            NewState.Privileges = new LUID_AND_ATTRIBUTES[1];

            // Get current process token
            var currentProc = System.Diagnostics.Process.GetCurrentProcess().Handle;
            hToken = new SafeTokenHandle();
            if (OpenProcessToken(currentProc, Convert.ToUInt32(TokenAccessLevels.MaximumAllowed), ref hToken) == false) {
                // Write debug
                WriteEvent("Error: Windows API OpenProcessToken function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Lookup SeIncreaseQuotaPrivilege

            if (!LookupPrivilegeValue(null, SE_INCREASE_QUOTA_NAME, ref luid)) {
                // Write debug
                WriteEvent("Error: Windows API LookupPrivilegeValue function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Enable SeIncreaseQuotaPrivilege
            NewState.Privileges[0].Luid = luid;
            NewState.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

            // Adjust the token privileges

            var dontcare1 = new TOKEN_PRIVILEGES();
            uint dontcare2 = 0;
            if (!AdjustTokenPrivileges(hToken, false, ref NewState, Convert.ToUInt32(Marshal.SizeOf(NewState)), ref dontcare1, ref dontcare2)) {
                // Write debug
                WriteEvent("Error: Windows API AdjustTokenPrivileges function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Lookup SeAssignPrimaryTokenPrivilege

            if (!LookupPrivilegeValue(null, SE_ASSIGNPRIMARYTOKEN_NAME, ref luid)) {
                // Write debug
                WriteEvent("Error: Windows API LookupPrivilegeValue function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Enable SeAssignPrimaryTokenPrivilege
            NewState.Privileges[0].Luid = luid;
            NewState.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

            // Adjust the token privileges

            if (!AdjustTokenPrivileges(hToken, false, ref NewState, Convert.ToUInt32(Marshal.SizeOf(NewState)), ref dontcare1, ref dontcare2)) {
                // Write debug
                WriteEvent("Error: Windows API AdjustTokenPrivileges function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Lookup SeTcbPrivilege

            if (!LookupPrivilegeValue(null, SE_TCB_NAME, ref luid)) {
                // Write debug
                WriteEvent("Error: Windows API LookupPrivilegeValue function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Enable SeTcbPrivilege
            NewState.Privileges[0].Luid = luid;
            NewState.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

            // Adjust the token privileges

            if (!AdjustTokenPrivileges(hToken, false, ref NewState, Convert.ToUInt32(Marshal.SizeOf(NewState)), ref dontcare1, ref dontcare2)) {
                // Write debug
                WriteEvent("Error: Windows API AdjustTokenPrivileges function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                // Return
                return false;

            }

            // Return
            return true;

        }


        // ******************************
        // * Launch Process Sub
        // ******************************

        /// <summary>
        /// Launches a process for a particular user, or for all users if a user is not specified.
        /// </summary>
        /// <param name="CmdLine"></param>
        /// <param name="args"></param>
        /// <param name="user">In the form DOMAIN\USER . Not cap sensitive.</param>
        /// <remarks></remarks>

        public static Process LaunchProcess(string CmdLine, string[] args, string user) {
            // Local variables
            string Arguments = "";
            Process[] ExplorerProcesses = null;
            SafeTokenHandle hToken = null;
            WindowsIdentity principle = default(WindowsIdentity);
            SafeTokenHandle phNewToken = null;
            STARTUPINFO si = default(STARTUPINFO);
            PROCESS_INFORMATION pi = default(PROCESS_INFORMATION);

            // Process arguments

            foreach (string arg in args) {
                // Build argument string
                Arguments += " " + arg;

            }

            // Increase Privileges

            if (IncreasePrivileges() == false) {
                // Write debug
                WriteEvent("Warning: Failed to increase current process privileges.");

            }

            // Get all explorer.exe IDs
            ExplorerProcesses = Process.GetProcessesByName("explorer");

            // Verify explorers were found

            if (ExplorerProcesses.Length == 0) {
                // Write debug
                WriteEvent("Warning: No explorer.exe processes found.");

                // Return
                throw new Exception("Explorer process not found. Likely no users logged in to Service PC.");
            }

            // Iterate each explorer.exe process

            foreach (Process hProcess in ExplorerProcesses) {
                try {
                    // Get the user token handle
                    hToken = new SafeTokenHandle();
                    if (OpenProcessToken(hProcess.Handle, Convert.ToUInt32(TokenAccessLevels.MaximumAllowed), ref hToken) == false) {
                        // Write debug
                        WriteEvent("Error: Windows API OpenProcessToken function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                        // Iterate the next process
                        continue;

                    }

                    // Get the windows identity
                    principle = new WindowsIdentity(hToken.DangerousGetHandle());

                    if (user != null && principle.Name.ToUpper() != user.ToUpper()) {
                        hToken.Close();
                        hToken = null;
                        continue;
                    }

                    // Get a primary token

                    var dontcare3 = new SECURITY_ATTRIBUTES();
                    phNewToken = new SafeTokenHandle();
                    if (!DuplicateTokenEx(hToken.DangerousGetHandle(), Convert.ToUInt32(TokenAccessLevels.MaximumAllowed), ref dontcare3, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, ref phNewToken)) {
                        // Write debug
                        WriteEvent("Error: Windows API DuplicateTokenEx function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());

                        // Iterate the next process
                        continue;
                    }

                    // Initialize process and startup info
                    pi = new PROCESS_INFORMATION();
                    si = new STARTUPINFO();
                    si.cb = Convert.ToUInt32(Marshal.SizeOf(si));
                    si.lpDesktop = null;

                    var dontcare4 = new System.IntPtr();
                    // Launch the process in the client's logon session
                    if (!CreateProcessAsUser(
                        phNewToken,
                        null, CmdLine + Arguments,
                        ref dontcare3, ref dontcare3,
                        false,
                        Convert.ToUInt32(CreateProcessFlags.CREATE_UNICODE_ENVIRONMENT),
                        dontcare4,
                        Path.GetDirectoryName(CmdLine),
                        ref si,
                        ref pi)) {
                        throw new Exception("Error: Windows API CreateProcessAsUser function returns an error." + Environment.NewLine + "Windows API error code: " + Marshal.GetLastWin32Error().ToString());
                    }
                    WriteEvent("Created new user process: " + Environment.NewLine + "User:     " + principle.Name + Environment.NewLine + "Process:  " + CmdLine + Arguments + Environment.NewLine + "PID:      " + pi.dwProcessId.ToString());

                    return Process.GetProcessById((int)pi.dwProcessId);
                }
                catch (Exception ex) {
                    throw ex;
                }
                finally {
                    // Free resources
                    if (hToken != null) {
                        hToken.Close();
                        hToken = null;
                    }
                    if (phNewToken != null) {
                        phNewToken.Close();
                        phNewToken = null;
                    }
                    principle = null;
                }
            }
            throw new Exception("User " + (user ?? "null") + " not found. Ensure that the user is logged in on the PC running the DNC Service.");
        }

        // ******************************
        // * Write Event Log Sub
        // ******************************
        public static void WriteEvent(string msg) {
            Util.Log.RealTime(null, nameof(ProcessLauncher), msg);
        }

    }

}
