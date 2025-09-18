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
using System.Data.OleDb;
using FTTEST.TestProcess.FWU;
using System.Xml.Linq;
using System.Threading;


namespace FTTEST.TestProcess.ULP
{
    class kavala
    {
    }
}

namespace FTTEST
{
    public partial class FormMain
    {
        bool ULP_Edradour()
        {
            if (adb_Devices() == false) return false;
            if (Neat_check_sn() == false) return false;
            //if (Edradour_SetBurnInOff() == false) return false;//

            if (Edradour_Set_BYOD() == false) return false;//
                                                           //if (Neat_touch_test() == false) return false;

            if (preset_CloseCameraVirtualCam() == false) return false;
            if (Neat_camera_test() == false) return false;

            if (Neat_CHECK_ETH_MAC() == false) return false;
            if (Neat_check_btwlan_mac() == false) return false;
            if (Neat_Ethernet_test() == false) return false;
            if (Edradour_SetHdmiInOn() == false) return false;
            Edradour_SetHdmiInOff();
            //if (preset_CloseCameraVirtualCam() == false) return false;
            //if (Neat_bt_test() == false) return false;
            //if (Edradour_SetHdmiInOn() == false) return false;
           // Edradour_SetHdmiInOff();
            //if (Neat_camera_test() == false) return false;
            //if (Neat_camera_tele_test() == false) return false;
            //if (Neat_Ethernet_2_test() == false) return false;
           // if (Neat_check_poe() == false) return false;

            return true;
        }

        //adb shell nf_cfgctl put /device/net/tethering/eth1/mode tethering
        bool Neat_check_poe()
        {
            int iRetryTime = new int();
            string ack;
            string sEthMac = string.Empty;

            Checksn:
            SetMsg("Check poe function...", UDFs.UDF.COLOR.WORK);
            //string text = $"引号中加\"引号\"";
            DialogResult UserSelect = MessageBox.Show(null, "请检查当前ARRAN机台是否是开机状态，确定OK并拔下AUX网络线 ？开机按确认，不开机按否\n",
            "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (UserSelect == DialogResult.No)
            {
                RunAdbCmd("adb shell nf_cfgctl put /device/net/tethering/eth1/mode tethering", out ack, 1);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto Checksn;
                }
            }
            SetMsg("set camera off...", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory /.FactoryService--es Action SetDPHdmiInput--ez setCamera 0", out ack, 1);

            return true;

        }
        bool Neat_check_btwlan_mac()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;

            GetEthMac:
            SetMsg("Get BT WLAN MAC...", UDFs.UDF.COLOR.WORK);
            if (Barra_GetMAC(UDFs.UDF.MAC_TYPE.WIFI) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sWifiMac, UDFs.UDF.MAC_STYLE.UC17, out sEthMac) == false)
            {
                SetMsg("BT WLAN Mac检查失败！ Mac:" + SDriverX.g_modelInfo.sEthMac, UDFs.UDF.COLOR.FAIL);
                return false;
            }

            CheckEthMac:
            SetMsg("Check BT WLAN MAC...", UDFs.UDF.COLOR.WORK);
            //string text = $"引号中加\"引号\"";
            RunAdbCmd("adb shell su root \"nf_factory | grep macbtwlan \"", out ack);
            if (ResultCheck(ack, sEthMac) == false)
            {
                SetMsg("Err:" + ack, UDFs.UDF.COLOR.FAIL);
                SetMsg("Check BT WLAN MAC fail!", UDFs.UDF.COLOR.FAIL);
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

        bool Neat_CHECK_ETH_MAC()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;

            GetEthMac:
            SetMsg("Get Eth MAC...", UDFs.UDF.COLOR.WORK);
        
            if (Barra_GetMAC(UDFs.UDF.MAC_TYPE.ETH) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }
            SetMsg("Get Eth MAC.. sEthMac=" + sEthMac, UDFs.UDF.COLOR.WORK);
            Thread.Sleep(2000);

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDFs.UDF.MAC_STYLE.UC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDFs.UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("Get Eth MAC.. sEthMac=" + sEthMac, UDFs.UDF.COLOR.WORK);
            Thread.Sleep(2000);
        //string text = $"引号中加\"引号\"";

        CheckEthMac:
            SetMsg("Check Eth MAC...", UDFs.UDF.COLOR.WORK);
            //string text = $"引号中加\"引号\"";
            int cnt1 = 0;
            do {
                RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetEthMAC",
                    "adb shell cat /sdcard/download/data_command_ack.txt", out ack, 1);
                if (!string.IsNullOrEmpty(ack.Trim()))
                    break;
            }
            while (cnt1++ <=10);

            Thread.Sleep(1000);
            SetMsg("Get  devices MAC..." + ack, UDFs.UDF.COLOR.WORK);
            Thread.Sleep(1000);
            if (ResultCheck(ack.ToUpper(), sEthMac.ToUpper()) == false)
            {
                SetMsg("Get Eth MAC..." + sEthMac, UDFs.UDF.COLOR.WORK);
                SetMsg("Err:" + ack, UDFs.UDF.COLOR.FAIL);
                SetMsg("CheckEth MAC fail!", UDFs.UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }

            CheckEEP_Mac:
            SetMsg("Check Eth MAC...", UDFs.UDF.COLOR.WORK);
            //string text = $"引号中加\"引号\"";
            RunAdbCmd("adb shell su root \"nf_factory | grep maceth \"", out ack2);
            if (ResultCheck(ack2, sEthMac) == false)
            {
                SetMsg("Err:" + ack2, UDFs.UDF.COLOR.FAIL);
                SetMsg("Check EEP Eth MAC fail:" + ack2, UDFs.UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEEP_Mac;
                }
            }
            return true;
        }

        bool Neat_check_sn()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;

            Checksn:
            SetMsg("Check sn...", UDFs.UDF.COLOR.WORK);
            //string text = $"引号中加\"引号\"";
            RunAdbCmd("adb shell su root \"nf_factory | grep serialno \"", out ack);
            if (ResultCheck(ack, SDriverX.g_modelInfo.sSn) == false)
            {
                SetMsg("Err:" + ack, UDFs.UDF.COLOR.FAIL);
                SetMsg("Check sn fail!" + ack, UDFs.UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto Checksn;
                }
            }
            SetMsg("set camera off...", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory /.FactoryService--es Action SetDPHdmiInput--ez setCamera 0", out ack, 1);       
            return true;
        }

        bool Neat_camera_test()
        {
            int iRetryTime = new int();
            string ack;
            string[] Result;
            string sEthMac = string.Empty;
            string fileContent = "";
            //string destinationPath = @"\\192.168.158.26\cisco\Barra";//@@@
            string destinationPath = @"\\10.2.100.27\TeData\NEAT\Edradour\Neat_camera_test";
            
            string filePath = @"D:\ForTestProgramMerge\Result\" + SDriverX.g_modelInfo.sSn + "_0_FFB.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            SetMsg("set camera test..", UDFs.UDF.COLOR.WORK);
            //RunAdbCmdMIC(@"D:\ForTestProgramMerge\CCMTesterAndroidProject.exe " + SDriverX.g_modelInfo.sSn + " 0 0", out ack,1);
            //@@@###
            RunAdbCmd_NOACK_EdradourULK(@"D:\ForTestProgramMerge\CCMTesterAndroidProject.exe " + SDriverX.g_modelInfo.sSn + " 0 0", out ack, 1);
            //RunAdbCmd(Directory.GetCurrentDirectory() + @"\set_camera.bat", out ack, 1);
            Application.DoEvents();
            Delay(10000);
            Recheck:
            SetMsg("get camera result..", UDFs.UDF.COLOR.WORK);
            filePath = @"D:\ForTestProgramMerge\Result\" + SDriverX.g_modelInfo.sSn +  "_0_FFB.txt";
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        fileContent = File.ReadAllText(filePath);
                        fileContent = fileContent.ToString().Trim().Replace("\r", "").Replace("\n", "|");
                        Result = fileContent.ToString().Split('|');
                        try
                        {
                            if (Convert.ToInt16(Result[0]) != 0)
                            {
                                SetMsg("读取摄像头回传值：" + Result[0], UDFs.UDF.COLOR.FAIL);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            SetMsg("读取摄像头回传值异常:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        // 处理读取文件失败的异常情况
                        SetMsg("读取文件报错：" + e.Message, UDFs.UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    iRetryTime++;
                    SetMsg("拍照倒计时..." + iRetryTime, UDFs.UDF.COLOR.WORK);
                    Delay(1000);
                    if (iRetryTime<10) goto Recheck;
                    SetMsg("无法获取到摄像头测试的结果" , UDFs.UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetMsg("发生错误:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }
            File.Copy(filePath, @destinationPath + "\\" + SDriverX.g_modelInfo.sSn + "_FFB.txt", true);
            SetMsg("camera test pass..", UDFs.UDF.COLOR.PASS);
            return true;
        }

        //Neat_touch_test
        /*
        bool Neat_touch_test()
        {
            int iRetryTime = new int();
            string ack;
            string sEthMac = string.Empty;
            string filePath = string.Empty;
            string fileContent = "";
            string destinationPath = @"\\192.168.158.26\cisco\NEATFRAME\kavalan";

            if (g_sTouchSN== SDriverX.g_modelInfo.sSn)
            {
                return true;
            }
            SetMsg("delete result.txt file...", UDFs.UDF.COLOR.WORK);
            filePath = Directory.GetCurrentDirectory() + @"\Neat_cmd\touch_test_result.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            SetMsg("set touch test,please wait later..", UDFs.UDF.COLOR.WORK);

            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Touch_test_kavalan.bat", out ack, 1);
            Recheck:
            Application.DoEvents();
            Delay(1000);
            
            filePath = Directory.GetCurrentDirectory() + @"\Neat_cmd\touch_test_result.txt";
            if (filePath=="")
            {
                iRetryTime++;
                SetMsg("touch test ...: " + iRetryTime, UDFs.UDF.COLOR.WORK);
                if (iRetryTime < 30) goto Recheck;
                SetMsg("touch test fail..", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        fileContent = File.ReadAllText(filePath);
                        if (ResultCheck(fileContent, ">> FAIL <<") ==true)
                        {
                            File.Copy(filePath, @destinationPath + "\\" + SDriverX.g_modelInfo.sSn + "touch_test_result_ng.txt", true);
                            SetMsg("Err:" + fileContent, UDFs.UDF.COLOR.FAIL);
                            return false;
                        }
                        if (ResultCheck(fileContent, ">> PASS <<") == true)
                        {
                            File.Copy(filePath, @destinationPath + "\\" + SDriverX.g_modelInfo.sSn + "touch_test_result.txt", true);
                        }
                    }
                    catch (Exception e)
                    {
                        // 处理读取文件失败的异常情况
                        SetMsg("读取文件报错：" + e.Message, UDFs.UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMsg("发生错误:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }
            g_sTouchSN = SDriverX.g_modelInfo.sSn;
            SetMsg("touch test pass..", UDFs.UDF.COLOR.PASS);
            return true;
        }
        */
        bool Neat_camera_tele_test()
        {
            int iRetryTime = new int();
            string ack;
            string[] Result;
            string sEthMac = string.Empty;
            string fileContent = "";
            //string destinationPath =@"\\192.168.158.26\cisco\Barra";
            string destinationPath = @"\\10.2.100.27\TeData\NEAT\Edradour\Neat_camera_test";
            

            SetMsg("set aux mode on..", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell nf_cfgctl put /device/net/tethering/eth1/mode tethering", out ack, 1);

            SetMsg("set tele camera test..", UDFs.UDF.COLOR.WORK);
            RunAdbCmdMIC(@"D:\ForTestProgramMerge\CCMTesterAndroidProject.exe " + SDriverX.g_modelInfo.sSn + " 0 1", out ack, 1);
            Application.DoEvents();
            Delay(10000);
            Recheck:
            string filePath = @"D:\ForTestProgramMerge\Result\" + SDriverX.g_modelInfo.sSn + "_1_FFB.txt";
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        fileContent = File.ReadAllText(filePath);
                        fileContent = fileContent.ToString().Trim().Replace("\r", "").Replace("\n", "|");
                        Result = fileContent.ToString().Split('|');
                        try
                        {
                            if (Convert.ToInt16(Result[0]) != 0)
                            {
                                SetMsg("读取摄像头回传值：" + Result[0], UDFs.UDF.COLOR.FAIL);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            SetMsg("读取摄像头回传值异常:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        // 处理读取文件失败的异常情况
                        SetMsg("读取文件报错：" + e.Message, UDFs.UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    iRetryTime++;
                    SetMsg("拍照倒计时..." + iRetryTime, UDFs.UDF.COLOR.WORK);
                    Delay(1000);
                    if (iRetryTime < 10) goto Recheck;
                    SetMsg("无法获取到摄像头测试的结果", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetMsg("发生错误:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }
            File.Copy(filePath, @destinationPath + "\\" + SDriverX.g_modelInfo.sSn + "_FFB_2.txt", true);
            SetMsg("tele camera test pass..", UDFs.UDF.COLOR.PASS);
            RunAdbCmd("adb shell nf_cfgctl put /device/net/tethering/eth1/mode tethering", out ack, 1);
            Delay(1000);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\set_poe_power_on.bat", out ack);
            return true;
        }
        bool Neat_bt_test()
        {
            string ack, ack2;
            int iRetryTime = 0;
            DateTime dt1 = DateTime.Now;
            string okng = string.Empty;
            string sql = string.Empty;
            string db = string.Empty;
            double dBTrssi = 0;

            string strBT = ReadIniFile("TEST", "BTMAC", GlobalConfig.sIniPath);
            if (strBT == "")
            {
                SetMsg("BT MAC is not setting", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            SetMsg("get bt rssi spec...", UDFs.UDF.COLOR.WORK);
            try
            {
                //dBTrssi = Convert.ToDouble(ReadIniFile("BT_RSSI", "RSSI", @"\\192.168.158.25\te\setup\Barra_tools\Barra_spec\kavalan.ini"));
                dBTrssi = Convert.ToDouble(ReadIniFile("Spec", "BTRSSILimit", GlobalConfig.sIniPath));
            }
            catch (Exception ex)
            {
                SetMsg("BT RSSI 发生错误:" + ex.Message + ":" +dBTrssi, UDFs.UDF.COLOR.FAIL);
                return false;
            }
            SetMsg("set bt off...", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBtEnable  --ez Bt_Enable 0", out ack, 1);
            Application.DoEvents();
            Delay(500);

            ReBTTEST:
            SetMsg("set bt test...", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetBTRssi --es BT_MAC_Address " + strBT, out ack, 1);
            Application.DoEvents();
            Delay(11000);
            RunAdbCmd("adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=BTRssi=).*?(?=\r|\n)", out ack2) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDFs.UDF.COLOR.FAIL);
                    SetMsg("Get bt rssi fail!", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReBTTEST;
                }
            }
            SetMsg("get bt rssi:" + ack2, UDFs.UDF.COLOR.WORK);
            SetMsg("Compare bt rssi spec...", UDFs.UDF.COLOR.WORK);
            double bt_rssi = Convert.ToDouble(ack2);
            if (bt_rssi < dBTrssi)
            {
                SetMsg("Compare bt rssi out spec: " + bt_rssi, UDFs.UDF.COLOR.FAIL);
                okng = "BT_NG";
                //@@@###
               // Insert_ORALCEdatabase("BT", ack2);
                return false;
            }
            okng = "OK";
            //@@@###
           // Insert_ORALCEdatabase("BT", ack2);
            return true;
        }
        bool Neat_Ethernet_test()
        {
            string ack;
            int iRetryTime = 0;
            string strIP = ReadIniFile("TEST", "Ethernet_IP", GlobalConfig.sIniPath);
            if (strIP == "")
            {
                SetMsg("Ethernet_IP is not setting", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            ReEthtest:
            SetMsg("set ethernet test...", UDFs.UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action EthTest --es Eth_IPaddress " + strIP, out ack, 1);
            Application.DoEvents();
            Delay(8000);
            RunAdbCmd("adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultCheck(ack, "EthTest=1") == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDFs.UDF.COLOR.FAIL);
                    SetMsg("set sethernet test fail!", UDFs.UDF.COLOR.FAIL);
                    return false;
                }

                else
                {
                    iRetryTime++;
                    goto ReEthtest;
                }
            }
            return true;
        }

        bool Neat_Ethernet_2_test()
        {
            string ack, ack2;
            int iRetryTime = 0;
            string strIP = ReadIniFile("TEST", "Ethernet_IP_2", GlobalConfig.sIniPath);
            if (strIP == "")
            {
                SetMsg("Ethernet_IP is not setting", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            ReEthtest:

            RunAdbCmd("del " + Directory.GetCurrentDirectory() + @"\ipv4.txt", out ack, 1);
            SetMsg("set eth aux test...", UDFs.UDF.COLOR.WORK);
            //ipconfig | find /I "ipv4" >ipv4.txt  \"nf_factory | grep maceth \"
            //RunAdbCmd(Directory.GetCurrentDirectory() + @"\ipconfig | find /I \"ipv4 \"", out ack, 1);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\set_eth1_test.bat", out ack, 1);
            Application.DoEvents();
            Delay(1000);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\ipv4.txt", out ack2);
            if (ResultCheck(ack2, strIP) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack2, UDFs.UDF.COLOR.FAIL);
                    SetMsg("set eth aux ip fail!", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReEthtest;
                }
            }
            return true;
        }

     }
}