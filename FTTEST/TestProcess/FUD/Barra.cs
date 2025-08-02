using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using FTTEST.AppConfig;
using FTTEST.SDriver;
using wbc;
namespace FTTEST.TestProcess.FWU
{
    class Barra
    {
    }
}




namespace FTTEST
{
    public partial class FormMain
    {
        bool FWU_Barra()
        {
            if (CheckUpgradeToolsVersion_Barra() == false) return false;
            if (Barra_UpgradeFW_Batch() == false) return false;
            if (Barra_UpgradeFW() == false) return false;
            return true;
        }
        

        bool CheckUpgradeToolsVersion_Barra()
        {
            string text;
            string fwufw = "";
            string FLAG = "";
            //SDriverX.g_modelInfo.sWord;           
            string PTH = wbc.iniFile.GetPrivateStringValue("TEST", "BPath", Application.StartupPath + @"\FTTEST.INI");
            if (PTH != "" && PTH != "0")
            {
                SetMsg("读取Bpath路径", UDFs.UDF.COLOR.WORK);
                GlobalConfig.sCmd3Path = PTH;
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER_barra", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER_barra", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("服务器:" + fwufw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFWUVer = fwufw;
                }
                
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fwufw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFWUVer = fwufw;
                }
                //CHECK VER
                // g_sCardFUDVer = fudfw;
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_SO", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_SO", SDriverX.g_modelInfo.sWord, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    g_sBarraFWUVer = fwufw;
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fwufw, UDFs.UDF.COLOR.WORK);
                }
            }

            if (fwufw != ""&& fwufw != "0")
            { g_sBarraFWUVer = fwufw; }
            SetMsg("Read user FW ver...", UDFs.UDF.COLOR.WORK);
            try
            {
                text = File.ReadAllText(GlobalConfig.sCmd3Path + "\\" + "version.txt");
                SetMsg("loc:" + text, UDFs.UDF.COLOR.WORK);
            }
            catch (Exception ex)
            {
                SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }

            //CHECK VER
            if (ResultCheck(text, g_sBarraFWUVer) == false)
            {
                //SetMsg(string.Format("警告：FW版本错误，请向PE、TE核实。'{0}'<>'{1}'", text, g_sBarraFUDVer), UDFs.UDF.COLOR.FAIL);
                SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            FW_TEMP = text;
            return true;
        }

        bool Barra_UpgradeFW()
        {
            string ack = string.Empty;
            int nRetryTime = 0;
            
            SetMsg("FW烧录即将开始...", UDFs.UDF.COLOR.WORK);
            SetMsg("烧录自动进行，请勿手动操作，以免错误", UDFs.UDF.COLOR.WARN);
            SetMsg("烧录时间过长，请耐心等候", UDFs.UDF.COLOR.WARN);
            string PTH = wbc.iniFile.GetPrivateStringValue("TEST", "BPath", Application.StartupPath + @"\FTTEST.INI");
            if (PTH != "" && PTH != "0")
            {
                GlobalConfig.sCmd3Path = PTH;
            }
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = " /c " + GlobalConfig.sCmd3Path + "\\" + "AndroidFlash.bat"; // " /c adb shell cat /sdcard/Download/command_ack.txt"
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            nRetryTime = 0;
            do
            {
                nRetryTime++;
                if (nRetryTime > 280)
                {
                    SetMsg("FW烧录失败，操作超时", UDFs.UDF.COLOR.FAIL);
                    KillFwuAllProcess();
                    return false;
                }
                Delay(1000);
            } while (process.HasExited == false);

            if (process.ExitCode != 0xff00)
            {
                iniFile.WriteFile0(SDriverX.g_modelInfo.sSn+":"+process.ExitCode.ToString(), AppDomain.CurrentDomain.BaseDirectory + "\\barra_erLog.txt");
                SetMsg("退出代码错误," + process.ExitCode.ToString(), UDFs.UDF.COLOR.FAIL);
                return false;
            }
            SetMsg("烧录完成", UDFs.UDF.COLOR.WORK);
            return true;
        }
        bool Barra_UpgradeFW_Batch()
        {
            if (File.Exists(Application.StartupPath + @"\barra_fud.bat"))//fwu ==fud
            {
                string ack = string.Empty;
                SetMsg("Run barra default cmd...", UDFs.UDF.COLOR.WORK);
                RunAdbCmd(Application.StartupPath + @"\barra_fud.bat", out ack);
            }
            return true;
        }
    }
}