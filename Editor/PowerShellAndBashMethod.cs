using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Wolffun.BuildPipeline
{
    public static class PowerShellAndBashMethod 
    {
        public static void RunErrorBash()
        {
            string bashScript = "echo '##[error]Error message'; exit 1";

            // Tạo một quy trình (Process) để thực thi bash
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{bashScript}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            // Bắt đầu thực thi quy trình
            process.Start();
        }
    }
}
