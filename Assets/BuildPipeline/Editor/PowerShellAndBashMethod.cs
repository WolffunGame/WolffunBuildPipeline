using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
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

            // Đọc và in kết quả từ đầu ra và đầu ra lỗi
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // In thông báo lỗi nếu có
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError($"Bash Error: {error}");
                // Thoát với mã lỗi
            }

            // In thông báo kết quả
            UnityEngine.Debug.LogError($"Bash Output: {output}");

            // Chờ quy trình kết thúc
            process.WaitForExit();
            
        }
    }
}
