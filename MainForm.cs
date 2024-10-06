using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using static PaltalkNickReader.MainForm;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Runtime.CompilerServices;

namespace PaltalkNickReader
{
    public partial class MainForm : Form
    {
        // Constants
        private const int MAXITEMTXT = 256;
        private const string PaltalkClassName = "DlgGroupChat Window Class";
        private const string ListViewClassName = "SysHeader32";
        private const int LVM_GETITEMCOUNT = 0x1004;
        private const int LVM_GETITEM = 0x1005;

        // P/Invoke Structures and Delegates
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public uint state;
            public uint stateMask;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            // Other members are omitted as they are not used
        }

         [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LVITEMW
        {
            public int mask;

            public int iItem;

            public int iSubItem;

            public int state;

            public int stateMask;

            public IntPtr pszText;

            public int cchTextMax;

            public int iImage;

            public IntPtr lParam;

            public int iIndent;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // P/Invoke Functions
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
         int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        // Process access rights
        private const uint PROCESS_VM_OPERATION = 0x0008;
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_VM_WRITE = 0x0020;

        // Virtual memory constants
        private const uint MEM_COMMIT = 0x00001000;
        private const uint MEM_RELEASE = 0x00008000;
        private const uint PAGE_READWRITE = 0x04;

        // Global Variables
        private IntPtr ghMain = IntPtr.Zero;
        private IntPtr ghList = IntPtr.Zero;
        private IntPtr ghPtRoom = IntPtr.Zero;
        private IntPtr ghPtLv = IntPtr.Zero;
        private IntPtr ghPtLvPr = IntPtr.Zero;

        public byte[] StructureToByteArray(object obj)
        {
            int num = Marshal.SizeOf(RuntimeHelpers.GetObjectValue(obj));
            byte[] array = new byte[checked(num - 1 + 1)];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr(RuntimeHelpers.GetObjectValue(obj), intPtr, fDeleteOld: true);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

        public object ByteArrayToStructure(byte[] arr)
        {
            int num = arr.Length;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.Copy(arr, 0, intPtr, num);
            object objectValue = RuntimeHelpers.GetObjectValue(Marshal.PtrToStructure(intPtr, typeof(LVITEMW)));
            Marshal.FreeHGlobal(intPtr);
            return objectValue;
        }

        public MainForm()
        {
            InitializeComponent();
            ghMain = this.Handle;
        }

        // Event Handlers
        private void BtnGetPaltalkWindows_Click(object sender, EventArgs e) => GetPaltalkWindows();

        private void BtnReadNick_Click(object sender, EventArgs e)
        {
            if (!GetMicUser())
            {
                MessageBox.Show("Error getting nicks", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearNickList_Click(object sender, EventArgs e) =>
            // MessageBox.Show("clear list clicked");
            lstNicks.Items.Clear();


        // Method to find Paltalk windows
        private void GetPaltalkWindows()
        {
            ghPtLv = IntPtr.Zero;
            ghPtRoom = IntPtr.Zero;

            // Find the Paltalk chat room window
            ghPtRoom = FindWindow(PaltalkClassName, null);

            if (ghPtRoom != IntPtr.Zero)
            {
                // Get window text
                StringBuilder sbWindowText = new StringBuilder(200);
                GetWindowText(ghPtRoom, sbWindowText, sbWindowText.Capacity);
                string windowText = sbWindowText.ToString();

                // Enumerate child windows to find the list view
                EnumChildWindows(ghPtRoom, EnumPaltalkWindowsCallback, IntPtr.Zero);

                // Update form title
                this.Text = $"Timing: {windowText}";
            }
            else
            {
                MessageBox.Show("No Paltalk Window Found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Callback method for EnumChildWindows
        private bool EnumPaltalkWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder sbClassName = new StringBuilder(256);
            GetClassName(hWnd, sbClassName, sbClassName.Capacity);
            string className = sbClassName.ToString();
            Debug.WriteLine(className);

            if (className == ListViewClassName)
            {
                ghPtLv = hWnd;
               // ghPtLvPr = GetParent(hWnd);
                Debug.WriteLine($"We got list view handle: {ghPtLv}");
               //Debug.WriteLine($"We got list view Parent handle: {ghPtLvPr}");
            }

            return true; // Continue enumeration
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

       
        // Retrieve the list view items (nicknames)
        private bool GetMicUser()
        {
            if (ghPtLv == IntPtr.Zero) // 
                return false; // No Paltalk ListView handle
          //  MessageBox.Show("Get User on mic clicked", "whatever", MessageBoxButtons.OK, MessageBoxIcon.Information);

            checked
            {
                try
                {
                    IntPtr userListHdl = ghPtLv;
                    
                    int iNuberOfNiks = (int)SendMessage(userListHdl, 4100, IntPtr.Zero, IntPtr.Zero);
                    uint lpdwProcessId = default(int);
                    GetWindowThreadProcessId(userListHdl, out lpdwProcessId);

                    IntPtr hPorc = OpenProcess(2035711, bInheritHandle: false, (uint)lpdwProcessId);
                    int iNumForIndex = iNuberOfNiks - 1;

                    for (int i = 0; i <= iNumForIndex; i++)
                    {
                        ListViewItem listViewItem = new ListViewItem();
                        int iSizeOfLvItemW = Marshal.SizeOf(typeof(LVITEMW));

                        IntPtr pRemoteMem = VirtualAllocEx(hPorc, IntPtr.Zero, iSizeOfLvItemW, 12288u, 4u);

                        LVITEMW lVITEMW = default;
                        lVITEMW.mask = 3;
                        lVITEMW.iSubItem = 0;
                        lVITEMW.iItem = i;
                        int iTxtSize = 256;
                        IntPtr pRemoteNick = (lVITEMW.pszText = VirtualAllocEx(hPorc, IntPtr.Zero, iTxtSize, 12288u, 4u));  // 
                        lVITEMW.cchTextMax = iTxtSize;

                        byte[] byteArr2Write = StructureToByteArray(lVITEMW);
                        IntPtr lpNumberOfBytesWritten = (IntPtr)0;
                        WriteProcessMemory(hPorc, pRemoteMem, byteArr2Write, iSizeOfLvItemW, out lpNumberOfBytesWritten);
                        Debug.WriteLine($"Number of bytes written to remote memory: {lpNumberOfBytesWritten}");

                        SendMessage(userListHdl, 4171, (IntPtr) i, pRemoteMem); // win32 API to get list view item
                        IntPtr lpNumberOfBytesRead = (IntPtr) 0;
                        ReadProcessMemory(hPorc, pRemoteMem, byteArr2Write, iSizeOfLvItemW, out lpNumberOfBytesRead);
                        Debug.WriteLine($"Number of bytes read from remote memory: {lpNumberOfBytesRead}");

                        object obj = ByteArrayToStructure(byteArr2Write);
                        listViewItem.ImageIndex = ((obj != null) ? ((LVITEMW)obj) : default).iImage;
                        Debug.WriteLine($"Nicks image index: {listViewItem.ImageIndex}");

                        if (listViewItem.ImageIndex == 10)
                        {
                            SendMessage(userListHdl, 4141, (IntPtr)i, pRemoteMem); // Get the remote data byte array
                            byte[] array2 = new byte[iTxtSize - 1 + 1]; //make a buff for the nickname byte array 
                            lpNumberOfBytesRead = (IntPtr)0;
                            // Read the remote byte array containing the nickname 
                            ReadProcessMemory(hPorc, pRemoteNick, array2, iTxtSize, out lpNumberOfBytesRead);
                            Debug.WriteLine($"Number of bytes read from remote memory: {lpNumberOfBytesRead}");

                            // Turn the byte array into text 
                            listViewItem.Text = Encoding.Default.GetString(array2).TrimEnd(default(char));
                        }

                        VirtualFreeEx(hPorc, pRemoteNick, 0, 32768); // Free the remote memory of the Nick
                        VirtualFreeEx(hPorc, pRemoteMem, 0, 32768); // Free the remote memory of List View Item

                        if (listViewItem.ImageIndex == 10)
                        {
                            // we have the Nickname om mic, add it to the List Box
                            string strNickname = listViewItem.Text.ToString();
                           lstNicks.Items.Add(strNickname);
                          //  MessageBox.Show(strNickname, "Nickname Reader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    CloseHandle(hPorc);
                }
                catch (Exception ex)
                {
                   Exception ex2 = ex;
                                       
                    return false;
                }
                
            }

            return true;
        }
        
        // Helper method to get process ID
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Override OnLoad to set up enumeration callback
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Assign the callback
            EnumWindowsProc callback = new EnumWindowsProc(EnumPaltalkWindowsCallback);
            // No action needed on load
        }
    }
}
