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
    class Cardhu{}
}


namespace FTTEST
{
    public partial class FormMain
    {
        bool FWU_Cardhu()
        {
            //serialPort.Write("su" + "\r");
            //Delay(1000);
            //serialPort.Write("echo 1 > /sys/devices/soc/c17a000.i2c/i2c-6/6-002d/flex" + "\r");
            //Delay(3000);          
            if (Barra_GetTvSn2() == false) return false;
            //if (Barra_GetTvSn() == false) return false;
            if (CheckUpgradeToolsVersion_Cardhu() == false) return false;
            if (Cardhu_UpgradeFW_Batch() == false) return false;
            if (Cardhu_UpgradeFW() == false) return false;
            return true;
        }
        bool CheckUpgradeToolsVersion_Cardhu()
        {
            string text;
            string fwufw = "";
            string strBuf = "";
            try
            {
                strBuf = ReadIniFile("TEST", "Cmd3Path", GlobalConfig.sIniPath);
                if (strBuf.Length != 0)
                {
                    GlobalConfig.sCmd3Path = strBuf.Trim();
                }
                string PTH = wbc.iniFile.GetPrivateStringValue("TEST", "CPath", Application.StartupPath + @"\FTTEST.INI");
                if (PTH != "" && PTH != "0")
                {
                    SetMsg("读取Cpath路径", UDFs.UDF.COLOR.WORK);
                    GlobalConfig.sCmd3Path = PTH;
                }
            }
            catch (Exception EX)
            {
                SetMsg("00路径fail", UDFs.UDF.COLOR.FAIL);
            }
           
            SetMsg("Read user FW ver...", UDFs.UDF.COLOR.WORK);
            try
            {
                text = File.ReadAllText(GlobalConfig.sCmd3Path + "\\" + "version.txt");
                SetMsg("v:"+text, UDFs.UDF.COLOR.WORK);
            }
            catch (Exception ex)
            {
                SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }
            string FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER_all", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER_all", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    g_sCardFWUVer = fwufw;
                    SetMsg("服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("服务器分位" + fwufw, UDFs.UDF.COLOR.WORK);
                }
               
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                //CHECK VER
                if (fwufw != "" && fwufw != "0")
                {
                    g_sCardFWUVer = fwufw;
                    SetMsg("服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("服务器pn分位" + fwufw, UDFs.UDF.COLOR.WORK);
                }
                else
                {
                    SetMsg("读取服务器pn分位:" + fwufw, UDFs.UDF.COLOR.WORK);
                }
            }
            
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_SO", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_SO", SDriverX.g_modelInfo.sWord, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    g_sCardFWUVer = fwufw;
                    strBuf = ReadIniFile("TEST", "soPath", Application.StartupPath + "\\FTTEST.ini");
                    if (strBuf.Length != 0)
                    {
                        GlobalConfig.sCmd3Path = strBuf.Trim();
                        try
                        {
                            text = File.ReadAllText(GlobalConfig.sCmd3Path + "\\" + "version.txt");
                            SetMsg("Read soFW ver:" + text, UDFs.UDF.COLOR.WORK);
                        }
                        catch (Exception ex)
                        {
                            SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                            return false;
                        }
                        //flag = true;
                    }
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("读取服务器分位so:" + fwufw, UDFs.UDF.COLOR.WORK);
                }
                else 
                {
                    SetMsg("读取服务器分位so:" + fwufw, UDFs.UDF.COLOR.WORK);
                }
            }
            if (ResultCheck(text, g_sCardFWUVer) == false) 
            {
                //SetMsg(string.Format("警告：FW版本错误，请向PE、TE核实。'{0}'<>'{1}'", text, g_sBarraFUDVer), UDFs.UDF.COLOR.FAIL);
                SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                System.Diagnostics.Process.Start("notepad.exe", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                System.Diagnostics.Process.Start("notepad.exe", GlobalConfig.sCmd3Path + "\\" + "version.txt");
                return false;
            }
            FW_TEMP = text;
            return true;
        }

        bool Cardhu_UpgradeFW()
        {
            string ack = string.Empty;
            int nRetryTime = 0;
            string PTH= wbc.iniFile.GetPrivateStringValue("TEST", "CPath", Application.StartupPath + @"\FTTEST.INI");
            if(PTH!="" && PTH!="0")
            {
                GlobalConfig.sCmd3Path = PTH;
            }
            SetMsg("FW烧录即将开始...", UDFs.UDF.COLOR.WORK);
            SetMsg("烧录自动进行，请勿手动操作，以免错误", UDFs.UDF.COLOR.WARN);
            SetMsg("烧录时间过长，请耐心等候", UDFs.UDF.COLOR.WARN);

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
                iniFile.WriteFile0(SDriverX.g_modelInfo.sSn + ":" + process.ExitCode.ToString(), AppDomain.CurrentDomain.BaseDirectory + "\\cardu_erLog.txt");
                SetMsg("退出代码错误," + process.ExitCode.ToString(), UDFs.UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("烧录完成", UDFs.UDF.COLOR.WORK);

            return true;
        }
        bool Cardhu_UpgradeFW_Batch()
        {
            if (File.Exists(Application.StartupPath + @"\cardhu_fud.bat"))//fwu == fud
            {
                string ack = string.Empty;
                SetMsg("Run cardhu default cmd...", UDFs.UDF.COLOR.WORK);
                RunAdbCmd(Application.StartupPath + @"\cardhu_fud.bat", out ack);
            }
            return true;
        }
    }
}