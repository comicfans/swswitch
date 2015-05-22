using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace WindowsFormsApplication
{
    using HWND = IntPtr;
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            listBox_result.DisplayMember = "text";
            listBox_result.ValueMember = "hwnd";
  int id = 0;     // The id of the hotkey.
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.D3.GetHashCode());       // Register Shift + A as global hotkey.

          
        }

        

        protected delegate bool EnumWindowsProc(HWND hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();

      

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd); 

        [DllImport("Psapi.dll", EntryPoint = "GetModuleFileNameEx")]
        public static extern uint GetModuleFileNameEx(uint handle, IntPtr hModule, [Out] StringBuilder lpszFileName, uint nSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        private IDictionary<HWND, string[]> cachedResult;


        public IDictionary<HWND, string[]> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string[]> windows = new Dictionary<HWND, string[]>();

            EnumWindows(delegate(HWND hWnd, IntPtr lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
//
                
                //exclude self
                if(hWnd==this.Handle)return true;


                var process = GetProcessHandleFromHwnd(hWnd);

                uint pid;

                GetWindowThreadProcessId(hWnd, out pid);



                Process p=Process.GetProcessById((int)pid);

                if (p == null)
                {
                    return true;
                }

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                var exeName = "";
                try
                {
                    exeName = p.MainModule.FileName;

                }catch(System.ComponentModel.Win32Exception ex){
                    //can not get exe name
                }

                windows[hWnd] = new string[]{builder.ToString(),exeName};
                return true;

            }, IntPtr.Zero);

            return windows;
        }

		void TextBox_inputTextChanged(object sender, EventArgs e)
		{
			if (textBox_input.Text.Trim().Equals("") || cachedResult==null)
            {
                cachedResult=GetOpenWindows();
            }
            refreshResult();
            
        }

        private void toTray()
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500);
            this.Visible=false;
            fromTray=true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {

                toTray();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
      //      this.TopMost = true;

            restoreFromTray();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if(fromTray){

                cachedResult=GetOpenWindows();
            
                fromTray=false;
                refreshResult();
            
                textBox_input.SelectAll();
            }

        }


        private void Handle_EscKey(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
          
            e.SuppressKeyPress = true;
            toTray();
                return;
            }
            
            if (e.KeyData==Keys.Return){
            	if(listBox_result.SelectedIndices.Count>0){
            		listBox_result_SelectedIndexChanged(null,null);
            		toTray();
            	}
            }

        }

        public class ListItem {
            public string text { get; set; }
            public HWND hwnd { get; set; }
        }

        private void refreshResult()
        {
            listBox_result.Items.Clear();

            var filter = textBox_input.Text.ToLower();

            foreach (var hw in cachedResult.Keys)
            {
                var array = cachedResult[hw];

                
                foreach(var test in array){
                    if (test.ToLower().Contains(filter)){
                        listBox_result.Items.Add(new ListItem {text=array[0]+ array[1],hwnd=hw });
                        break;
                    }
                }
                
            }

            foreach (var it in listBox_result.Items)
            {
                listBox_result.SelectedIndex = 0;
                break;
            }


            
        }

        private void restoreFromTray()
        {
            this.Visible = true;

            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.textBox_input.Focus();
        }
 
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.Visible)
            {
                this.Activate();
                return;
            } 


            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */
 
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.
                restoreFromTray();

                // do something
            }
        }
  

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        enum KeyModifier { None = 0, Alt = 1, Control = 2, Shift = 4, WinKey = 8 }


        private void Handle_ToResultKey(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                toTray();
                return;
            }
        

            if ( 
                e.KeyCode !=Keys.Enter &&
                e.KeyCode !=Keys.Up &&
                e.KeyCode!=Keys.Down &&
                e.KeyCode!=Keys.Tab)
            {

                return;
            }
            e.SuppressKeyPress=true;
            

            listBox_result.Focus();

            if (e.KeyCode ==Keys.Enter)
            {
                toTray();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", SetLastError=true)]
static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        private bool fromTray=false;
        
        
        
       [DllImport("user32.dll", SetLastError=true)]
       static extern bool ShowWindow(HWND hWnd,  int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, int flags); 
         
        
        
         [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }
    
             [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool  IsIconic(HWND hWnd);
    
    
             [DllImport("user32.dll")]
        static extern HWND  GetTopWindow(HWND hWnd);

             [DllImport("user32.dll")]
        static extern HWND  GetNextWindow(HWND hWnd);

    const UInt32 SW_HIDE =         0;
    const UInt32 SW_SHOWNORMAL =       1;
    const UInt32 SW_NORMAL =       1;
    const UInt32 SW_SHOWMINIMIZED =    2;
    const UInt32 SW_SHOWMAXIMIZED =    3;
    const UInt32 SW_MAXIMIZE =     3;
    const UInt32 SW_SHOWNOACTIVATE =   4;
    const UInt32 SW_SHOW =         5;
    const UInt32 SW_MINIMIZE =     6;
    const UInt32 SW_SHOWMINNOACTIVE =  7;
    const UInt32 SW_SHOWNA =       8;
    const UInt32 SW_RESTORE =      9;
        
        private void listBox_result_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBox_result.SelectedItem as ListItem;

            RECT rect;

            var plac=new WINDOWPLACEMENT();
            
            GetWindowPlacement(item.hwnd,ref plac);
            
            if(IsIconic(item.hwnd)){
            	ShowWindow(item.hwnd,(int)SW_RESTORE);
            }
            
            ForceWindowToForeground(item.hwnd,this.Handle);
            
            
           
            //SetWindowPos(item.hwnd, this.Handle, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
        }
		void TextBox_inputPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			
			if(e.KeyData==Keys.Enter || e.KeyData==Keys.Up || e.KeyData==Keys.Tab || e.KeyData==Keys.Down){
				e.IsInputKey=true;
			}
			
		}
		
		
		 [DllImport("user32.dll", SetLastError = true)] public static extern bool BringWindowToTop(IntPtr hWnd);
		 
		  [DllImport("kernel32.dll")] public static extern uint GetCurrentThreadId(); 
		  
		   [DllImport("user32.dll")] public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach); 
		   
		   [DllImport("user32.dll")] public static extern HWND SetFocus(HWND HWND);
		
		[DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		public static void AttachedThreadInputAction(Action action,HWND hwnd,HWND self)
		{
			
			var foreThread = GetWindowThreadProcessId(hwnd,
			                                         IntPtr.Zero);
			var appThread = GetCurrentThreadId();
			bool threadsAttached = false;

			try
			{
				threadsAttached =
					foreThread == appThread ||
					AttachThreadInput(foreThread, appThread, true);

				if (threadsAttached) action();
				else {
					RECT rect;
					GetWindowRect(hwnd,out rect);
					
					SetForegroundWindow(hwnd);
				}
			}
			finally
			{
				if (threadsAttached)
					AttachThreadInput(foreThread, appThread, false);
			}
		}
		
		public static void ForceWindowToForeground(IntPtr hwnd,IntPtr self)
		{
			AttachedThreadInputAction(
				() =>
				{
					//BringWindowToTop(hwnd);
					SetForegroundWindow(hwnd);
					//ShowWindow(hwnd,(int) SW_SHOW);
				},hwnd,self);
		}

		public static IntPtr SetFocusAttached(IntPtr hWnd,IntPtr self)
		{
			var result = new IntPtr();
			AttachedThreadInputAction(
				() =>
				{
					result = SetFocus(hWnd);
				},hWnd,self );
			return result;
		}
		
		
		
		    }


    
}
