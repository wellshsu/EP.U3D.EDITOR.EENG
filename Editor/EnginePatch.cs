//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using EP.U3D.EDITOR.BASE;

namespace EP.U3D.EDITOR.EENG
{
    [InitializeOnLoad]
    public class EnginePatch
    {
        static EnginePatch()
        {
            var pkg = Helper.FindPackage(Assembly.GetExecutingAssembly());
            string pkey = Helper.StrMD5(Application.dataPath);
            int ptime = EditorPrefs.GetInt(pkey);
            int etime = (int)EditorApplication.timeSinceStartup;
            EditorPrefs.SetInt(pkey, etime);
            EditorApplication.quitting += () => EditorPrefs.SetInt(pkey, 0); // 退出时重置时间（强制杀进程可能会失效）
            if (etime <= ptime || ptime == 0) // 利用时间差，确保打开编辑器首次调用
            {
                string tmp = EditorApplication.applicationPath;
                string match = Application.unityVersion;
                string dstEngine = tmp.Substring(0, tmp.IndexOf(match));
                string srcEngine = Constants.ENGINE_PATCH_PATH;
                if (Directory.Exists(dstEngine) == false) return;
                if (Directory.Exists(srcEngine) == false) return;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.FileName = $"{pkg.resolvedPath}/Editor/Libs/Patcher~/eeng.exe";
                startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.Arguments = string.Format("-s \"{0}\" -d \"{1}\" -w {2} -b {3}", srcEngine, dstEngine, "false", "false");
                startInfo.Verb = "runas"; // 以管理员身份运行
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.StackTrace);
                }
            }
        }
    }
}