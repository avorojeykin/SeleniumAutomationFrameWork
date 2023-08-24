using System;
using System.Threading;
using AutoIt;

namespace Common.AutoItXService
{
    public static class DialogBoxHelper
    {
        public static void OpenFile(string fileFullPath, int listeningTimeInSeconds, string dialogWindowHeader, int pauseTime)
        {
            int aiDialogHandle = AutoItX.WinWaitActive(dialogWindowHeader, "", listeningTimeInSeconds);
            if (aiDialogHandle <= 0)
            {
                throw new Exception($"Cannot itentify the dialog window!");
            }
            AutoItX.Send(fileFullPath);
            Thread.Sleep(pauseTime);
            AutoItX.Send("{ENTER}");
            Thread.Sleep(pauseTime);
        }
    }
}
