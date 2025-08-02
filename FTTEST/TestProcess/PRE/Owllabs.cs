
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FTTEST.UDFs;
using FTTEST.AppConfig;
using FTTEST.Database;
using FTTEST.SDriver;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FTTEST.TestProcess.PRE
{
    class OWlLabs
    {
    }
}

namespace FTTEST
{
    public partial class FormMain
    {
        bool PRE_OwlLbas()
        {
            // SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            int VOL = 15;

            if (CheckFw() == false) return false;
            if (Checkusb() == false) return false;

            //if (Check_HDI_LT6911_Version() == false) return false;
            //if (Check_HDI_LT6911_HDCP() == false) return false;
            //if (Check_HDO1_LT9611_Version() == false) return false;
            //if (Check_HDO1_LT9611_HDCP() == false) return false;
            //if (Check_HDO2_LT8711_Version() == false) return false;
            //if (Check_HDO2_LT8711_HDCP() == false) return false;

            if (Setbuilddate() == false) return false;
            if (Getbuilddate() == false) return false;
            SetMsg("set vol:" + VOL.ToString(), UDF.COLOR.WORK);
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            if (Barra_WriteEthMacWithExe() == false) return false;
            if (Eeeprom_WriteEthMac()==false) return false;
            if (Owllabs_WriteWifiMac()==false ) return false;
            if (Owllabs_WriteBtMac() == false) return false;
            if (SetTvSn() == false) return false;
            if (GetTvSn() == false) return false;
            if (SetMicvendor() == false) return false;
            if (GetMicvendor() == false) return false;
            if (set_sleep_setting()==false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            // SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool CheckFw()
        {
            string ack, ack2, progid;
            int iRetryTime = new int();

            iRetryTime = 0;
            GetFw:
            SetMsg("Get fw version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell getprop ro.build.id", out ack);
            if (ResultPick(ack, @"T", out ack2) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get fw version fail!", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }

            if ((g_iPtInitFlag & INIT_FW) == INIT_FW)  //如果已经有了首次查询了机种FW信息，则比较缓存数据不再联网查询
            {
                //if ((ack2.IndexOf(g_sFwVer1) == -1) && (ack2.IndexOf(g_sFwVer2) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
                SetMsg("机台FW:" + ack + "\n数据库FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.WORK);
                if (!ack.Contains(g_sFwVer1) && !ack.Contains(g_sFwVer2))

                {
                    //FW 比对失败
                    SetMsg("FW比对错误！机台FW:" + ack + "\n数据库FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.FAIL);
                    return false;
                }
            }
            else //否则从数据库更新 FW信息，下一次不再联网查询，除非切换了新工单
            {
                if (QueryProgIdSql(out progid) == false)
                {
                    SetMsg("查询机种 progid fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    //g_iPtInitFlag = INIT_ZERO;  //工单切换后，这个 flag 会重置为 0，以通知其他部分更新信息，比如从数据库更新此工单应该卡的 FW 信息
                    if (CheckFwVerSql(progid, ack) == false) return false;
                }
            }
            return true;
        }

        bool set_sleep()
        {
            string ack;
            SetMsg("set sleep off...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\OwlLabs_1st_time.bat", out ack);
            return true;
        }
        bool Checkusb()
        {
            string ack;
            string USBFW = "";
            SetMsg("CHECK U Disk...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root ls /mnt/media_rw", out ack);

            if (ack == "")
            {
                SetMsg("U Disk not found", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                SetMsg("U Disk:"+ ack, UDF.COLOR.WORK);
                return true;
            }

            USBFW = ReadIniFile("TEST", "USB_NAME", GlobalConfig.sIniPath);
            if (USBFW == "")
            {
                SetMsg(GlobalConfig.sIniPath + "[TEST]: USB_NAME not found", UDF.COLOR.FAIL);
                return false;
            }
            textBoxUn.Text = USBFW;

            if ((ack.IndexOf(USBFW) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("check u disk fail！udisk name:" + ack + " Settingg USB_NAME:" + USBFW, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Setbuilddate()
        {
            string ack;
            RunAdbCmd("adb shell getprop ro.build.date.utc", out ack);
            Delay(2000);
            builddate = ack.Replace("\r\n", "");
            SetMsg(ack, UDF.COLOR.WORK);
            SetMsg("Set build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k fswtstamp -v " + ack, out ack);
            return true;
        }
        bool Getbuilddate()
        {
            string ack;
            SetMsg("Get build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory > data_commaand_ack.txt ", out ack);
            string s = Application.StartupPath + "\\data_commaand_ack.txt";
            string[] contents = File.ReadAllLines(s, Encoding.Default);
            for (int i = 0; i < contents.Length; i++)
            {
                while (contents[i].Substring(0, 10) == "fswtstamp=")
                {
                    if (contents[i].Substring(10, contents[i].Length - 10) == builddate)
                    {
                        return true;
                    }

                }
            }
            SetMsg("Get build date fail...", UDF.COLOR.FAIL);
            return false;
        }
        bool SetTvSn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
            SetSn:
            SetMsg("Set tv sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k serialno -v " + textBoxSn.Text.Trim(), out ack);
            if (ResultCheck(ack, "Setting serialno=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set sn fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetSn;
                }
            }
            return true;
        }

        bool GetTvSn()
        {
            string ack, ack2;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
            GetMac:
            SetMsg("Get tv sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory", out ack, 0);
            //if (ResultPick(ack, @"serialno=" + textBoxSn.Text, out ack2) == false)
            if (ResultPick(ack, @"(?<=serialno=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get tv sn fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }
            SetMsg("TV sn:" + ack2, UDF.COLOR.WORK);
            SetMsg("Compare TV sn...", UDF.COLOR.WORK);
            if (ack2 != SDriverX.g_modelInfo.sSn)
            {
                SetMsg("Compare TV sn fail!", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Eeeprom_WriteEthMac()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;
            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            WriteEthMac:
            SetMsg("set vendor/persist eth mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k maceth -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting maceth=" + sEthMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Write Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }
            CheckEthMac:
            SetMsg("check vendor/persist eth mac...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_owllabs_eth_mac.bat",
                "type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 1);
            if (ResultPick(ack, @"maceth=" + sEthMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Check Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            return true;
        }
        bool Owllabs_WriteWifiMac()
        {
            string ack,ack2;
            string sWifiMac = string.Empty;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;


            iRetryTime = 0;
            GetWifiMac:
            SetMsg("Get WiFi MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.WIFI) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetWifiMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sWifiMac, UDF.MAC_STYLE.LC17, out sWifiMac) == false)
            {
                SetMsg("Eth Mac检查失败！WiFi Mac:" + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
            WriteEthMac:
            SetMsg("set partition wifi mac...", UDF.COLOR.WORK);
            SetMsg(Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", UDF.COLOR.WORK);
            RunAdbCmd(GlobalConfig.sCmd1Path + " -WLAN " + sWifiMac,
                "type " + Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", out ack, 1);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Write wifi MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            iRetryTime = 0;
            MacUpdate:
            SetMsg("set vendor/persist wifi mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k macwlan -v " + sWifiMac, out ack);
            if (ResultCheck(ack, "Setting macwlan=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("set write wifi mac fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto MacUpdate;
                }
            }

            CheckEthMac:
            SetMsg("check vendor/persist wifi mac...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_owllabs_wifi_mac.bat",
                "type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 1);
            if (ResultPick(ack, @"macwlan=" + sWifiMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Check Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            //adb shell am startservice -n com.amtran.factory /.FactoryService--es Action GetWiFiMAC
            //adb pull / sdcard / Download / data_command_ack.txt
            return true;
        }

        bool Owllabs_WriteBtMac()
        {
            string ack, ack2;
            string sBtMac = string.Empty;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;


            iRetryTime = 0;
            GetBtMac:
            SetMsg("Get WiFi MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.BT) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetBtMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sBtMac, UDF.MAC_STYLE.LC17, out sBtMac) == false)
            {
                SetMsg("bt Mac检查失败！bt Mac:" + SDriverX.g_modelInfo.sBtMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
            WriteEthMac:
            SetMsg("set partition bt mac...", UDF.COLOR.WORK);
            SetMsg(Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", UDF.COLOR.WORK);
            RunAdbCmd(GlobalConfig.sCmd1Path + " -BT " + sBtMac,
                "type " + Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", out ack, 1);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Write wifi BT fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            iRetryTime = 0;
            MacUpdate:
            SetMsg("set vendor/persist bt mac..", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k macbt -v " + sBtMac, out ack);
            if (ResultCheck(ack, "Setting macbt") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("write bt mac fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto MacUpdate;
                }
            }

            CheckEthMac:
            SetMsg("check vendor/persist bt mac...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_owllabs_bt_mac.bat",
                "type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 1);
            if (ResultPick(ack, @"macbt=" + sBtMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Check BT MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            return true;
        }
        bool Check_HDI_LT6911_HDCP()
        {
            string ack, ack2;
            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            SetMsg("Get HDMI OUT1 LT6911 HDCP STATUS...", UDF.COLOR.WORK);
            RunAdbCmd(@"adb shell su root cat /sys/devices/platform/soc/soc\:hdmi_bdg_irq_handler/get_hdcp_status > data_command_ack.txt", out ack, 1);
            Delay(100);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);
            g_sFwVer1 = ReadIniFile("spec", "HDI_LT6911_HDCP", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack2.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("HDMI OUT1 LT6911 HDCP check fail！MB STATUS:" + ack2 + " Setting STATUS:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Check_HDO1_LT9611_HDCP()
        {
            string ack, ack2;
            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            SetMsg("Get HDMI OUT1 LT6911 HDCP...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root cat /sys/bus/i2c/devices/0-002b/get_hdcp_status > data_command_ack.txt", out ack, 1);
            Delay(100);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);
            g_sFwVer1 = ReadIniFile("spec", "HDO1_LT9611_HDCP", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack2.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("HDMI OUT1 LT6911 HDCP check fail！MB status:" + ack2 + " Setting status:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Check_HDO2_LT8711_HDCP()
        {
            string ack, ack2;
            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            SetMsg("Get HDMI OUT2 LT8711 HDCP...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root cat /sys/bus/i2c/devices/0-003f/get_hdcp_status > data_command_ack.txt", out ack, 1);

            Delay(100);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);
            g_sFwVer1 = ReadIniFile("spec", "HDO2_LT8711_HDCP", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack2.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("HDMI OUT2 LT8711 HDCP check fail！mb status:" + ack2 + " setting status:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Check_HDO2_LT8711_Version()
        {
            string ack, ack2;
            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            SetMsg("Get HDMI OUT2 LT8711 Version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root cat /sys/bus/i2c/devices/0-003f/get_fw_version >data_command_ack.txt", out ack, 1);
            Delay(100);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);
            g_sFwVer1 = ReadIniFile("spec", "HDO2_LT8711_VER", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack2.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("HDMI OUT2 LT8711 Version check fail！mb Version:" + ack2 + " setting Version:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Check_HDI_LT6911_Version()
        {
            string ack;

            SetMsg("Get HDI LT6911 Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\check_HDI_LT6911_owl.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            //if (ResultPick(ack, "x", out ack2) == false)
            //{
            //    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
            //    SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
            //    if (iRetryTime > 2)
            //        return false;
            //    else
            //    {
            //        iRetryTime++;
            //        goto GetFw;
            //    }
            //}
            g_sFwVer1 = ReadIniFile("spec", "HDI_LT6911_VER", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("Get HDI LT6911 Version check fail！MB Version:" + ack + "\n Setting Version:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Check_HDO1_LT9611_Version()
        {
            string ack, ack2;
            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            SetMsg("Get HDMI OUT1 LT9611 Version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root cat /sys/bus/i2c/devices/0-002b/get_fw_version >data_command_ack.txt", out ack, 1);
            Delay(100);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);

            g_sFwVer1 = ReadIniFile("spec", "HDO1_LT9611_VER", @"\\10.2.100.27\\File Bak\\Edradour\\spec\\" + textBoxSn.Text.Substring(2, 4) + ".ini");
            if ((ack2.IndexOf(g_sFwVer1) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
            {
                //FW 比对失败
                SetMsg("HDMI OUT1 LT6911 Version check fail！MB Version:" + ack2 + " Setting Version:" + g_sFwVer1, UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool CheckLed()
        {
            string ack;

            SetMsg("Check Camera led status All Open...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\Check_led_on.bat", out ack);
            DialogResult result = MessageBox.Show("Check Camera led /status led/ Error led All Open OK？", "CHECK LED STATUS...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result != DialogResult.Yes)
            {
                SetMsg("TEST FAIL！", UDF.COLOR.FAIL);
                RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\Check_led_off.bat", out ack);
                return false;
            }
            else
            {
                RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\Check_led_off.bat", out ack);
            }
            return true;
        }

        bool SetMicvendor()
        {
            string ack;

            if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "KT1")
            {
                SetMsg("Set merry Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root amt_factory -s -k instmic -v merry", out ack);
            }
            else
            {
                SetMsg("Set infineon Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root amt_factory -s -k instmic -v infineon", out ack);
            }
            return true;
        }

        bool GetMicvendor()
        {
            string ack;
            if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "KT1")
            {
                SetMsg("Get merry Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root amt_factory > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "instmic=merry")
                    {
                        return true;
                    }
                }
                SetMsg("Get Mic vendor fail...", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                SetMsg("Get infineon Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root amt_factory > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "instmic=infineon")
                    {
                        return true;
                    }
                }
                SetMsg("Get Mic vendor fail...", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool set_sleep_setting()
        {
            string ack;
            SetMsg("set no sleep setting...", UDF.COLOR.WORK);
            RunAdbCmd( Directory.GetCurrentDirectory() + @"\WriteEthMac\set_OwlLabs_1st_time.bat", out ack);
            Delay(2000);
            RunAdbCmd( Directory.GetCurrentDirectory() + @"\WriteEthMac\set_OwlLabs_1st_time.bat", out ack);
            Delay(500);
            return true;
        }
    }
}
