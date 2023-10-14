using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using System.Security;
using HANDLE = System.IntPtr;

namespace Ght84.AppsLauncherLibrary.Helpers
{

    public class WindowsSession
    {
        public int Id;
        public string UserName;
        public string Domain;
        public ConnectState State;
        public string StationName;
    }


    public enum ConnectState
    {
        Active =0 ,
        Connected =1,
        ConnectQuery = 2,
        Shadow = 3,
        Disconnected = 4,
        Idle = 5,
        Listen = 6,
        Reset = 7,
        Down = 8,
        Init = 9
    }
    


    public static class WindowsSessionHelper
    {

        enum WTSInfoClass
        {
            InitialProgram,
            ApplicationName,
            WorkingDirectory,
            OEMId,
            SessionId,
            UserName,
            WinStationName,
            DomainName,
            ConnectState,
            ClientBuildNumber,
            ClientName,
            ClientDirectory,
            ClientProductId,
            ClientHardwareId,
            ClientAddress,
            ClientDisplay,
            ClientProtocolType
        }


        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern int WTSEnumerateSessions(System.IntPtr hServer,
        int Reserved,
        int Version,
        ref System.IntPtr ppSessionInfo,
        ref int pCount);
        
        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern bool WTSQuerySessionInformation(System.IntPtr hServer,
        int sessionId,
        WTSInfoClass wtsInfoClass,
        ref System.IntPtr ppBuffer,
        ref uint pBytesReturned);

        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern IntPtr WTSOpenServer(string ServerName);
        
        [DllImport("wtsapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern void WTSCloseServer(HANDLE hServer);
        
        [DllImport("wtsapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern void WTSFreeMemory(IntPtr pMemory);
        [StructLayout(LayoutKind.Sequential)]

        struct WTSSessionInfo
        {
            internal uint SessionId;
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string pWinStationName;
            internal uint State;
        }

        public static WindowsSession GetActiveWindowsSession()
        {
            List<WindowsSession> allWindowsSessions = GetAllWindowsSessions();
            WindowsSession activeWindows;

            activeWindows = allWindowsSessions.Where(s=> s.State == ConnectState.Active).FirstOrDefault(); 

            return activeWindows;

        }


        public static List<WindowsSession> GetAllWindowsSessions()
        {
            HANDLE hServer = IntPtr.Zero;
            IntPtr pInfo = IntPtr.Zero;
            IntPtr pInfoSave = IntPtr.Zero;
            WTSSessionInfo WTSsi; // Reference to ProcessInfo struct
            IntPtr ppBuffer = IntPtr.Zero;
            uint bCount = 0;
            int count = 0;
            int iPtr = 0;


            int sessionId;
            string sessionStationName;
            string sessionUserName;
            string sessionDomain;
            ConnectState sessionState;

            List<WindowsSession> windowsSessions = new List<WindowsSession>();   

            try
            {
                // Ouverture du serveur locahost
                hServer = WTSOpenServer("");
                if (hServer == IntPtr.Zero)
                    throw new Exception($"{Marshal.GetLastWin32Error()} ");
                 
                
                // Enumérer les sessions Windows
                if (WTSEnumerateSessions(hServer, 0, 1, ref pInfo, ref count) != 0)
                {
                    pInfoSave = pInfo;
          
                    for (int n = 0; n < count; n++)
                    {
                        WTSsi = (WTSSessionInfo)Marshal.PtrToStructure(pInfo, typeof(WTSSessionInfo));
                        iPtr = (int)(pInfo) + Marshal.SizeOf(WTSsi);
                        pInfo = (IntPtr)(iPtr);

                        sessionId = (int)WTSsi.SessionId;
                        sessionStationName = WTSsi.pWinStationName;
                        sessionState = (ConnectState)WTSsi.State;

                        if (WTSQuerySessionInformation(hServer, (int)WTSsi.SessionId, WTSInfoClass.UserName, ref ppBuffer, ref bCount))
                            sessionUserName = Marshal.PtrToStringAuto(ppBuffer);
                        else
                            sessionUserName = "";

                        if (WTSQuerySessionInformation(hServer, (int)WTSsi.SessionId, WTSInfoClass.DomainName, ref ppBuffer, ref bCount))
                            sessionDomain = Marshal.PtrToStringAuto(ppBuffer);
                        else
                            sessionDomain = "";

                  

                        windowsSessions.Add(new WindowsSession()
                        {
                            UserName = sessionUserName,
                            Domain = sessionDomain,
                            Id = sessionId,
                            State = sessionState,   
                            StationName = sessionStationName
               
                        });


                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Echec GetWindowsSession {e.Message}");
            }
            finally
            {
                WTSFreeMemory(pInfoSave);
                WTSCloseServer(hServer);
            }

            return windowsSessions;

        }



    }
}






