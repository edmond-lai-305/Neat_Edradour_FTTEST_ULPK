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
using FTTEST.UDFs;
namespace FTTEST.TestProcess.FWU
{
    class Harris
    {
    }
}

namespace FTTEST
{
    public partial class FormMain
    {
        bool FWU_Harr()
        {
            if (Barra_GetTvSn2() == false) return false;
            if (CheckUpgradeToolsVersion_Harr() == false) return false;
            if (Harr_UpgradeFW_Batch() == false) return false;
            if (Harr_UpgradeFW() == false) return false;
            return true;
        }
        bool CheckUpgradeToolsVersion_Harr()
        {
            string text;
            string fwufw = "";
            string FLAG = "";
 
            GlobalConfig.sCmd4Path = wbc.iniFile.GetPrivateStringValue("TEST", "HPath", Application.StartupPath + @"\FTTEST.INI");
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER_Harr", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"); // 1
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER_Harr", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");

                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径 Truy cập đường dẫn máy chủ" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("服务器 máy chủ:" + fwufw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFWUVer = fwufw;
                    textBoxUn.Text = fwufw;
                }
                else
                {
                    SetMsg("讀取FW版本失敗 Đọc phiên bản FW thất bại", UDFs.UDF.COLOR.FAIL);
                    SetMsg("請檢查 "+@"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"+", FW_USER_Harr, FW 設定,", UDFs.UDF.COLOR.FAIL);
                    SetMsg("Vui lòng kiểm tra FW_USER_Harr trong file FW.ini tại đường dẫn "+ @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini "+ "và cài đặt FW", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"); // 0
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_USER", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (fwufw != "" && fwufw != "0")
                {
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fwufw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFWUVer = fwufw;
                }

            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_SO", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"); // 0
            if (FLAG == "1")
            {
                fwufw = wbc.iniFile.GetPrivateStringValue("FW_SO", SDriverX.g_modelInfo.sWord, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");

                if (fwufw != "" && fwufw != "0")
                {
                    g_sBarraFWUVer = fwufw;
                    SetMsg("读取服务器路径1" + @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fwufw, UDFs.UDF.COLOR.WORK);
                }
            }

            if (fwufw != "" && fwufw != "0")
            { g_sBarraFWUVer = fwufw; }
            SetMsg("讀取使用者FW Đọc phiên bản FW người dùng...", UDFs.UDF.COLOR.WORK);
            try
            {
                text = File.ReadAllText(GlobalConfig.sCmd4Path + "\\" + "version.txt"); // D:\\harris\\FWU\\bin\\debug\\version.txt
                SetMsg("loc:" + text, UDFs.UDF.COLOR.WORK);
            }
            catch (Exception ex)
            {
                SetMsg("讀取使用者FW版本失敗 Đọc phiên bản FW người dùng Thất bại:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }

            //CHECK VER
            if (ResultCheck(text, g_sBarraFWUVer) == false)
            {
                SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                SetMsg("Cảnh báo: Phiên bản FW lỗi, vui lòng xác nhận với PE, TE.", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            FW_TEMP = text;
            return true;
        }
        bool Harr_UpgradeFW()
        {
            string ack = string.Empty;
            int nRetryTime = 0;
            GlobalConfig.sCmd4Path = wbc.iniFile.GetPrivateStringValue("TEST", "HPath", Application.StartupPath + @"\FTTEST.INI");
            SetMsg("FW烧录即将开始 Quá trình ghi FW sắp bắt đầu...", UDFs.UDF.COLOR.WORK);
            SetMsg("烧录自动进行，请勿手动操作，以免错误", UDFs.UDF.COLOR.WARN);
            SetMsg("Quá trình ghi tự động đang diễn ra, vui lòng không thao tác thủ công để tránh lỗi.", UDFs.UDF.COLOR.WARN);
            SetMsg("烧录时间过长，请耐心等候", UDFs.UDF.COLOR.WARN);
            SetMsg("Thời gian ghi quá lâu, vui lòng kiên nhẫn chờ đợi.", UDFs.UDF.COLOR.WARN);

            string bat_path = GlobalConfig.sCmd4Path + @"\AndroidFlash.bat";

            if (!File.Exists(bat_path))
            {
                SetMsg(bat_path + "不存在 Không tồn tại", UDF.COLOR.FAIL);
                return false;
            }

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{bat_path}\""; // " /c adb shell cat /sdcard/Download/command_ack.txt"
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            nRetryTime = 0;
            process.WaitForExit();
            do
            {
                nRetryTime++;
                if (nRetryTime > 280)
                {
                    SetMsg("FW烧录失败,操作超时 Lỗi ghi Firmware: Quá thời gian thao tác", UDFs.UDF.COLOR.FAIL);
                    KillFwuAllProcess();
                    return false;
                }
                Delay(1000);
            } while (process.HasExited == false);


            if (process.ExitCode != 0xff00)
            {
                SetMsg("退出代码错误 Lỗi mã thoát: " + process.ExitCode.ToString(), UDFs.UDF.COLOR.FAIL);
                
                return false;
            }

            SetMsg("烧录完成 Ghi dữ liệu hoàn tất", UDFs.UDF.COLOR.WORK);

            return true;
        }
        bool Harr_UpgradeFW_Batch()
        {
            if (File.Exists(Application.StartupPath + @"\barra_fud.bat"))
            {
                string ack = string.Empty;
                SetMsg("啟動 barra 預設指令 Chạy lệnh mặc định của barra...", UDFs.UDF.COLOR.WORK);
                RunAdbCmd(Application.StartupPath + @"\barra_fud.bat", out ack);
                //SetMsg(ack, UDF.COLOR.WORK);
                return true;
            }
            else
            {
                SetMsg("barra_fud.bat 不存在 không tồn tại", UDFs.UDF.COLOR.FAIL);
                return false;
            }
        }
    }
}