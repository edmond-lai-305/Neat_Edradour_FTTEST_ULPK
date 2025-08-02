using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FTTEST.AppConfig;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using FTTEST.SDriver;
namespace FTTEST.TestProcess.FWU
{
    class Arran
    {
    }
}


namespace FTTEST
{
    public partial class FormMain
    {
        bool FWU_Arran()
        {
            if (Barra_GetTvSn2() == false) return false;
            if (CheckUpgradeToolsVersion() == false) return false;
            if (Arran_UpgradeFW_Batch() == false) return false;
            //if (Arran_UpgradeFW() == false) return false;
             if (Arran_UpgradeFW1() == false) return false;
            return true;
        }

        bool Arran_UpgradeFW_Batch()
        {
            string ack = "";
            SetMsg("Run arran default cmd...", UDFs.UDF.COLOR.WORK);
            if (File.Exists(Application.StartupPath + @"\arran_fud.bat"))//fuw == fud , fw update
            {
                ack = string.Empty;
                SetMsg("Run arran default cmd...", UDFs.UDF.COLOR.WORK);
                RunAdbCmd(Application.StartupPath + @"\arran_fud.bat", out ack);
            }
            //return true;




            //while(File.Exists(GlobalConfig.sCmd3Path + @"\command_ack.txt"))
            //{
            //    File.Delete(GlobalConfig.sCmd3Path + @"\command_ack.txt");
            //    //System.Threading.Thread.Sleep(2000);
            //}
            SetMsg("Run arran default cmd...", UDFs.UDF.COLOR.WORK);
            if (File.Exists(GlobalConfig.sCmd3Path + @"\fastloader.bat"))
            {
                //Process[] pro = Process.GetProcessesByName("cmd");
                //for (int i = 0; i < pro.Length; i++)
                //{
                //    pro[i].Kill();
                //}
                SetMsg("Run arran default cmd...", UDFs.UDF.COLOR.WORK);
                
                ProcessStartInfo psi = new ProcessStartInfo();
                //psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(GlobalConfig.sCmd3Path + @"\fastloader.bat");
                
            }
            else
            {
                SetMsg("fastloader.bat丢失", UDFs.UDF.COLOR.WORK);
                return false;
            }
            //if (File.Exists(GlobalConfig.sCmd3Path + @"\fastloader.bat"))
            //{
            //    string ack = string.Empty;
            //    SetMsg("Run arran fastloader...", UDFs.UDF.COLOR.WORK);
            //    RunAdbCmd(GlobalConfig.sCmd3Path + @"\fastloader.bat", out ack);
            //}
            System.Threading.Thread.Sleep(6000);
            //if (File.Exists(GlobalConfig.sCmd3Path + @"\command_ack.txt"))
            //{
            //    string na = File.ReadAllText(GlobalConfig.sCmd3Path + @"\command_ack.txt");
            //    if(na.ToUpper().Contains("OK"))
            //    {
            //        System.Threading.Thread.Sleep(3000);
            //    }
            //}
            //else
            //{ 
               
               // SetMsg("确认机台是否连接,或重新启动机台", UDFs.UDF.COLOR.FAIL);
                //return false;
            //}
            return true;
        }
        bool CheckUpgradeToolsVersion()
        {
            string text;
            string fwufw = "";
            string FLAG = "";
            string PTH = wbc.iniFile.GetPrivateStringValue("TEST", "APath", Application.StartupPath + @"\FTTEST.INI");
            if (PTH != "" && PTH != "0")
            {
                SetMsg("读取Apath路径", UDFs.UDF.COLOR.WORK);
                GlobalConfig.sCmd3Path = PTH;
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER_arran", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER_arran", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("读取服务器分位" + fwufw, UDFs.UDF.COLOR.WORK);
                    //g_sArranFUDVer2 = fudfw;
                    //g_sArranFUDVer3 = fudfw;
                    g_sArranFWUVer = fwufw;
                }
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("读取服务器分位" + fwufw, UDFs.UDF.COLOR.WORK);
                    //g_sArranFUDVer2 = fudfw;
                    //g_sArranFUDVer3 = fudfw;
                    g_sArranFWUVer = fwufw;
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
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("读取服务器分位" + fwufw, UDFs.UDF.COLOR.WORK);
                    //g_sArranFUDVer2 = fudfw;
                    //g_sArranFUDVer3 = fudfw;
                    g_sArranFWUVer = fwufw;
                }
            }
            SetMsg("Read user FW ver...", UDFs.UDF.COLOR.WORK);
            if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "A1F")
            {
                try
                {
                    text = File.ReadAllText(GlobalConfig.sCmd4Path + "\\" + "version.txt");
                }
                catch (Exception ex)
                {
                    SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.WORK); 
                    return false;
                }
                if (fwufw != "" && fwufw != "0")
                { g_sArranFWUVer2 = fwufw; }
                //CHECK VER
                if (ResultCheck(text, g_sArranFWUVer2) == false)
                {
                    SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
            }
            else if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "A1C")
            {
                try
                {
                    text = File.ReadAllText(GlobalConfig.sCmd5Path + "\\" + "version.txt");
                }
                catch (Exception ex)
                {
                    SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                    return false;
                }
                if (fwufw != "" && fwufw != "0")
                { g_sArranFWUVer3 = fwufw; }
                //CHECK VER
                if (ResultCheck(text, g_sArranFWUVer3) == false)
                {
                    SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.WORK);
                    System.Diagnostics.Process.Start("notepad.exe", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                    System.Diagnostics.Process.Start("notepad.exe", GlobalConfig.sCmd3Path + "\\" + "version.txt");
                    return false;
                }
            }
            else
            {
                try
                {
                    text = File.ReadAllText(GlobalConfig.sCmd3Path + "\\" + "version.txt");
                }
                catch (Exception ex)
                {
                    SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                    return false;
                }
                if (fwufw != "" && fwufw != "0")
                { g_sArranFWUVer = fwufw; }
                //CHECK VER
                if (ResultCheck(text, g_sArranFWUVer) == false)
                {
                    SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
            }
            FW_TEMP = text;
            return true;
        }
        
       bool Arran_UpgradeFW1()
        {
            re:
            if (FGA_TEST())
            {
                SetMsg("烧录完成", UDFs.UDF.COLOR.WORK);
                if (Convert.ToInt16 (textBoxCycle.Text) <20)
                {
                    goto re;
                }
                return true;
            }
            return false;
        }

       

        bool Arran_UpgradeFW()
        {
            string ack = string.Empty;
            int nRetryTime = 0;

            SetMsg("FW烧录即将开始...", UDFs.UDF.COLOR.WORK);
            SetMsg("烧录自动进行，请勿手动操作，以免错误", UDFs.UDF.COLOR.WARN);
            SetMsg("烧录时间过长，请耐心等候", UDFs.UDF.COLOR.WARN);

            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = " /c autoit.exe"; // " /c adb shell cat /sdcard/Download/command_ack.txt"
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            nRetryTime = 0;
            do
            {
                nRetryTime++;
                if (nRetryTime > 240)
                {
                    SetMsg("FW烧录失败，操作超时", UDFs.UDF.COLOR.FAIL);
                    KillFwuAllProcess();
                    return false;
                }
                Delay(1000);
            } while (process.HasExited == false);
            if (process.ExitCode != 0xff00)
            {
                SetMsg("退出代码错误", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            SetMsg("烧录完成", UDFs.UDF.COLOR.WORK);
            return true;
        }


        void KillFwuAllProcess()
        {
            string ack = string.Empty;
            RunAdbCmd("taskkill /f /im cmd.exe", out ack, 1);
            RunAdbCmd("taskkill /f /im adb.exe /t", out ack, 1);
            RunAdbCmd("taskkill /f /im fastboot.exe /t", out ack, 1);
            RunAdbCmd("taskkill /f /im AFPTool.exe /t", out ack, 1);
            RunAdbCmd("taskkill /f /im RKImageMaker.exe /t", out ack, 1);
        }
    }
}