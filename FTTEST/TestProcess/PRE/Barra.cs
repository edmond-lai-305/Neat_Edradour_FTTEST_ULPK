//#define checkToch //check touch pad fw version

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FTTEST.UDFs;
using FTTEST.AppConfig;
using FTTEST.Database;
using FTTEST.SDriver;
//using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.EnterpriseServices;

namespace FTTEST.TestProcess.PRE
{
    class Barra { }
}

namespace FTTEST
{
    public partial class FormMain
    {
        [DllImport("Kernel32.dll")]
        private static extern long GetPrivateProfileString(string section,string key,
             string def, StringBuilder retVal, int size, string filePath);
        public static string ReadIniFile(string section, string key, string filePath)
        {
            StringBuilder keyString = new StringBuilder(1024);
            string def = null;
            GetPrivateProfileString(section,key, def, keyString, 1024, filePath);
            return keyString.ToString().Trim();
        }
        public static string sinst_mic = string.Empty;
        public static string builddate2 = string.Empty;
        public static string builddate = string.Empty;


  

        bool PRE_Barra()
        {
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (SDriverX.g_modelInfo.sPart_no == "AXXUSNFB----.NBARB12")
            {
                if (WriteExtSn() == false) return false;
            }            
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_barra", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_barra", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 78;
                }
            }
            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (Barra_CheckFw() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;

            if (Barra_SetMicvendor() == false) return false;
            if (Barra_GetMicvendor() == false) return false;

            if (Barra_Setbuilddate() == false) return false;
            //if (Barra_Getbuilddate() == false) return false;
            
            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;

            //if (UpdateBiTime() == false) return false;
            if (UpdateBiTime1() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool PRE_Cardhu()
        {
            string ack = string.Empty;
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_PRE", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_PRE", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 20;
                }
            }
            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (GetScalerFwForCardhu() == false) return false;
            if (Barra_CheckFw() == false) return false;

            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;

            if (Cardhu_SetMicvendor() == false) return false;
            if (Cardhu_GetMicvendor() == false) return false;

            if (Barra_Setbuilddate() == false) return false;
            if (Barra_Getbuilddate() == false) return false;
            if (Barra_GetLightSensorValue() == false) return false;
            if (Barra_GetTmpSensorValue() == false) return false;


            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            //set model index
            if (SetModelIndexForCardhu() == false) return false;
            //set bright max 
            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x02 0x00 0x64 0xF3 i", out ack, 1);
            if (SDriverX.g_modelInfo.sPart_no.Substring(9, 3).ToUpper() == "EO1")
            {
                //if (UpdateBiTime() == false) return false;
                if (UpdateBiTime1() == false) return false;
            }
            //if (UpdateBiTime() == false) return false;            
            if (UpdateBiTime1() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool PRE_QCS_TEST()
        {
            if (adb_Devices() == false) return false;
            if (get_hdmiin_LT6911_fw() == false) return false;
            if (get_hdmi1_LT6911_fw() == false) return false;
            if (get_hdmi1sub_LT6911_fw() == false) return false;
            if (get_hdmi23_LT6911_Minor_fw() == false) return false;
            if (get_hdmi23_LT6911_Major_fw() == false) return false;
            if (get_hdmi23_LT6911_Build_fw() == false) return false;
            if (QCS_WriteEthMacWithExe() == false) return false;
            return true;
        }
        bool PRE_Edradour()
        {
            string ack = string.Empty;
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_Edra_PRE", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_Edra_PRE", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 20;
                }
            }
            SetMsg("vol:" + VOL.ToString(), UDF.COLOR.WORK);
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Neat_CheckFw() == false) return false;
            if (Neat_Camsensor() == false) return false;
            //if (Neat_Install_Apk() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            if (Edradour_WriteEthMac() == false) return false; //未更新指令
            if (Neat_WriteWifiBTMac() == false) return false;
            if (Neat_SetTvSn() == false) return false;
            if (Neat_GetTvSn() == false) return false;

            if (Neat_Setbuilddate() == false) return false;
            if (Neat_Getbuilddate() == false) return false;
            if (Barra_GetLightSensorValue() == false) return false;

            if (Edradour_SetMicvendor() == false) return false;
            if (Edradour_GetMicvendor() == false) return false;
            if (SDF() == false) return false;
            //if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;

            if (UpdateBiTime() == false) return false;

            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool check_result(string sn)
        {
            try
            {
                //textBoxAck.AppendText("\\192.168.158.26\\cisco\\NEATFRAME\\TDM\\PASS\\" + "\r\n");
                textBoxAck.AppendText(@"\\10.2.100.27\TeData\NEAT\Harris\TDM\PASS\" + "\r\n");
                //foreach (var file in Directory.GetFiles(@"\\192.168.158.26\cisco\NEATFRAME\TDM\PASS\", sn + "*.csv"))
                foreach (var file in Directory.GetFiles(@"\\10.2.100.27\TeData\NEAT\Harris\TDM\PASS\", sn + "*.csv"))
                {
                    FileInfo fileinfo = new FileInfo(file);
                    
                    textBoxAck.AppendText(file.ToString() + "\r\n");
                    if (file.IndexOf("OK") < 1)
                    {
                        textBoxAck.AppendText("没有找到对应文件\r\n");
                        return false; 
                    }
                    else
                        return true;
                    //fileinfo.Delete();
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        bool PRE_Harr()
        {
            int itime = 0;
            //SetMsg("read\\192.168.158.26\\cisco\\NEATFRAME\\TDM\\PASS", UDF.COLOR.WORK);
            if ( check_result(SDriver.SDriverX.g_modelInfo.sSn)==false)
            {
                //SetMsg("读取26文件记录fail", UDF.COLOR.FAIL);
                SetMsg("读取27文件记录fail", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                //SetMsg("读取26文件记录ok", UDF.COLOR.WORK);
                SetMsg("读取27文件记录ok", UDF.COLOR.WORK);
            }
            re:
            if (harris_CheckFw() == false)
            {
                if (itime < 3)
                {
                    itime++;
                    goto re;
                }
                SetMsg("读取touch fw fail", UDF.COLOR.FAIL);
                savelogs("读取touch fw fail");
                return false;
            }
            string ack = string.Empty;
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_harris_PRE", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_harris_PRE", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 20;
                }
            }
            SetMsg("vol:" + VOL.ToString(), UDF.COLOR.WORK);
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Neat_CheckFw() == false) return false;
            if (Neat_Camsensor() == false) return false;
            //if (Neat_Install_Apk() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            if (Edradour_WriteEthMac() == false) return false; //未更新指令
            if (Neat_WriteWifiBTMac() == false) return false;
            if (Neat_SetTvSn() == false) return false;
            if (Neat_GetTvSn() == false) return false;

            if (Neat_Setbuilddate() == false) return false;
            if (Neat_Getbuilddate() == false) return false;

            if (Harr_GetLightSensorValue() == false) return false;
            if (Harr_GetVOCValue() == false) return false;

            if (Barra_GetTmpSensorValue() == false) return false;

            if (Harr_SetMicvendor() == false) return false;
            if (Harr_GetMicvendor() == false) return false;
            if(SDF() == false) return false;
             
            //if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;

            if (UpdateBiTime() == false) return false;

            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool PRE_Kava()
        {
            int itime = 0;
            if (Neat_CheckFw() == false) return false;
            if (check_result(SDriver.SDriverX.g_modelInfo.sSn) == false)
            {
                //SetMsg("读取26文件记录fail", UDF.COLOR.FAIL);
                SetMsg("读取27文件记录fail", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                //SetMsg("读取26文件记录ok", UDF.COLOR.WORK);
                SetMsg("读取27文件记录ok", UDF.COLOR.WORK);
            }
            re:
            if (harris_CheckFw() == false)
            {
                if (itime < 3) { goto re; }
                SetMsg("读取 touch fw fail", UDF.COLOR.FAIL);
                return false;
            }
            string ack = string.Empty;
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_Kava_PRE", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_Kava_PRE", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 20;
                }
            }
            SetMsg("vol:" + VOL.ToString(), UDF.COLOR.WORK);
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Neat_Install_Apk() == false) return false;
            if (Neat_Camsensor() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            if (Kava_WriteEthMac() == false) return false; //未更新指令
            if (Edradour_WriteEthMac() == false) return false; //未更新指令
            if (Neat_WriteWifiBTMac() == false) return false;
            if (Neat_SetTvSn() == false) return false;
            if (Neat_GetTvSn() == false) return false;

            if (Neat_Setbuilddate() == false) return false;
            if (Neat_Getbuilddate() == false) return false;

            if (Harr_GetLightSensorValue() == false) return false;
            if (Harr_GetVOCValue() == false) return false;

            if (Barra_GetTmpSensorValue() == false) return false;

            if (Harr_SetMicvendor() == false) return false;
            if (Harr_GetMicvendor() == false) return false;
            if (SDF() == false) return false;
            //if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;

            if (UpdateBiTime() == false) return false;

            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool PCB_QCS8250_TEST()
        {

            return true;
        }
        bool FTA_Cardhu()
        {
            string ack = string.Empty;
           
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_FTA", SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
            if (VOL == 0) {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_FTA", "VOL", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 78;
                }
            }
            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            //if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            //if (GetScalerFwForCardhu() == false) return false;
            //if (Barra_CheckFw() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            //if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            //if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            //if (Barra_SetTvSn() == false) return false;
            //if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;
            if (Barra_GetLightSensorValue() == false) return false;
            //if (Cardhu_SetMicvendor() == false) return false;
            //if (Cardhu_GetMicvendor() == false) return false;

            //if (Barra_Setbuilddate() == false) return false;
            //if (Barra_Getbuilddate() == false) return false;

            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            //set model index
            if (SetModelIndexForCardhu() == false) return false;
            //set bright max 
            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x02 0x00 0x64 0xF3 i", out ack, 1);
            if (UpdateBiTime() == false) return false;
            if (UpdateBiTime1() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }

        bool PRE_Barra_DEBUG()
        {
            SDriverX.g_modelInfo = new UDF.ModelInfo { sSn = "NB11941000022" };
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG             
           
            //if (SDriverX.g_modelInfo.sPart_no == "AXXUSNFB----.NBARB12")
            //{
            //    if (WriteExtSn() == false) return false;
            //}

            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (Barra_CheckFw() == false) /*return false*/;
            if (Barra_SetAudioVol(13) == false) return false;
            if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令 ----
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            //if (Barra_SetVocCo2SelfTest() == false) return false;
            //if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            if (UpdateBiTime() == false) return false;
            if (UpdateBiTime1() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }

        bool Barra_StartFactoryService()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        StartFacyService:
            SetMsg("Start factory services...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService", out ack);
            if (ResultCheck(ack, "Starting service") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Start factory services fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto StartFacyService;
                }
            }

            return true;
        }

        bool Neat_Camsensor()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
            //if (SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARH" || SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARJ" || SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARK" || SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARL" || SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARM" || SDriverX.g_modelInfo.sPart_no == "VXXWWNFB----.NFEBARN" 
            //    || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISD" || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISE" || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISF" || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISG" || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISH" || SDriverX.g_modelInfo.sPart_no == "V50WWNFB2AO-.HARRISJ"
            //    || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLAN9" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANA" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANB" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANC" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLAND" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANE" 
            //    || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANF" || SDriverX.g_modelInfo.sPart_no == "V65WWNFB2AO-.KAVLANG")
            //{
                StartFacyService:
                //NEAT Edradour/Harris/Kavalan 2nd 相機模組注意事項
                SetMsg("Set Camera_2nd camsensor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory -s -k camsensor -v s5kgnksp", out ack); // 删除设定 adb shell su root nf_factory -d -k camsensor
                if (ResultCheck(ack, "Setting camsensor=") == false)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Set Camera_2nd camsensor fail!", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto StartFacyService;
                    }
                }
           // }
            return true;
        }

        bool Barra_EntryFactryMode()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        FactMode:
            SetMsg("Factory mode on...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetFactoryMode --ez setMode 1",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, "FactoryMode=1") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Factory mode on fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto FactMode;
                }
            }
            Delay(500);
            if (SDriverX.g_modelInfo.sPart_no.Substring(14, 2).ToUpper() == "BA")
            {
                SetMsg("set barra byod setting...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su 0 nf_persist -s -k byod -v true", out ack);
            }
            return true;
        }

        bool Barra_AdbRoot()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        AdbRoot:
            SetMsg("adb root...", UDF.COLOR.WORK);
            RunAdbCmd("adb root", out ack);
            if (ResultCheck(ack, "as root") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("adb root fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto AdbRoot;
                }
            }
            return true;
        }

        bool Barra_CheckFw()
        {
            string ack, ack2, progid;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetFw:
            SetMsg("Get fw version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetFWVersion",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=FWVersion=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get fw version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }

            if ((g_iPtInitFlag & INIT_FW) == INIT_FW)  //如果已经有了首次查询了机种FW信息，则比较缓存数据不再联网查询
            {
                if ((ack2.IndexOf(g_sFwVer1) == -1) && (ack2.IndexOf(g_sFwVer2) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
                {
                    //FW 比对失败
                    SetMsg("FW比对错误！机台FW:" + ack2 + "\n数据库FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.FAIL);
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
                    textBoxRC_SN.Text = progid.ToString();
                    //g_iPtInitFlag = INIT_ZERO;  //工单切换后，这个 flag 会重置为 0，以通知其他部分更新信息，比如从数据库更新此工单应该卡的 FW 信息
                    if (CheckFwVerSql(progid, ack2) == false) return false;
                }
            }


            //Compare FW
            //SetMsg("Read FW:" + ack2, UDF.COLOR.WORK);
            //SetMsg("Compare FW:" + ack2, UDF.COLOR.WORK);
            //if (ack2 != GlobalConfig.sFw && ack2 != GlobalConfig.sFw2)
            //{
            //    SetMsg("Compare Fw fail, sepc:" + GlobalConfig.sFw + "/" + GlobalConfig.sFw2, UDF.COLOR.FAIL);
            //    return false;
            //}

            return true;
        }
        bool adb_Devices()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
            FactMode:
            SetMsg("adb Connect Devices...", UDF.COLOR.WORK);
            RunAdbCmd("adb devices", out ack);
            if (ack.Length < 35)
            //if (ResultCheck(ack, "MediaVolume=") == false)
            {
                //SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                {
                    SetMsg("adb connect fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto FactMode;
                }
            }
            return true;
        }
        bool Edradour_SetBurnInOff()//@@@###
        {
            string ack;
            //set
            SetMsg("Edradour_SetBurnInOff", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningMode --ez setMode 0", out ack);

            RunAdbCmd("adb pull /sdcard/Download/command_ack.txt", out ack);
            Thread.Sleep(1000);
            //get
            SetMsg("GetBurningMode", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetBurningModeadb pull /sdcard/Download/data_command_ack.txt", out ack);
            savelogs("GetBurningMode = " + ack);
            if (ack.Contains("BurningMode") && ack.Contains("0"))
            {
                SetMsg("BurningMode PASS", UDF.COLOR.WORK);
                return true;
            }
            else 
            {
                SetMsg("BurningMode FAIL", UDF.COLOR.FAIL);
                return false;
            }


           
        }
        bool preset_CloseCameraVirtualCam()
        {
            string ack = "";
            SetMsg("start to Run edradour default cmd...", UDFs.UDF.COLOR.WORK);
     
            if (File.Exists(Application.StartupPath + @"\preset.bat"))//fuw == fud , fw update
            {
                ack = string.Empty;
                SetMsg("Run edradour default cmd...", UDFs.UDF.COLOR.WORK);
                savelogs("Run edradour default cmd...");
                RunAdbCmd(Application.StartupPath + @"\preset.bat", out ack);
                       savelogs("Run edradour default cmd...");
            }
            else 
            {
                SetMsg("preset.bat no exit", UDFs.UDF.COLOR.FAIL);
                savelogs("preset.bat no exit");
                return false;
            }
            return true;
      
        }
        bool Edradour_SetHdmiInOn()//@@@###
        {
            string ack;
            //set
            SetMsg("Edradour_SetHdmiInOn", UDF.COLOR.WORK);
            savelogs("Edradour_SetHdmiInOn");
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetDPHdmiInput --ei delayTime 0 --ez setCamera 1", out ack);

           // RunAdbCmd("adb pull /sdcard/Download/command_ack.txt", out ack);
            Thread.Sleep(1000);
            //get
            // SetMsg("GetBurningMode", UDF.COLOR.WORK);
            // RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetBurningModeadb pull /sdcard/Download/data_command_ack.txt", out ack);
            // savelogs("GetBurningMode = " + ack);
            // if (ack.Contains("BurningMode") && ack.Contains("0"))
            // {
            //   SetMsg("BurningMode PASS", UDF.COLOR.WORK);
            DialogResult UserSelect = MessageBox.Show(null, "Check HDMI IN function OK?   \n",
                                                     "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (UserSelect == DialogResult.No)
            {
                SetMsg("HDMI IN function FAIL!", UDF.COLOR.FAIL);
                return false;
            }

            return true;
          //  }
           // else
            //{
               // SetMsg("BurningMode FAIL", UDF.COLOR.FAIL);
              //  return false;
          //  }



        }
        bool Edradour_SetHdmiInOff()//@@@###
        {
            string ack;
            //set
            SetMsg("Edradour_SetHdmiInOff", UDF.COLOR.WORK);
            savelogs("Edradour_SetHdmiInOff");
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetDPHdmiInput --ez setCamera 0", out ack);

            Thread.Sleep(1000);
            //get
            //SetMsg("GetBurningMode", UDF.COLOR.WORK);
           // RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetBurningModeadb pull /sdcard/Download/data_command_ack.txt", out ack);
           // savelogs("GetBurningMode = " + ack);
            //if (ack.Contains("BurningMode") && ack.Contains("0"))
          //  {
             //   SetMsg("BurningMode PASS", UDF.COLOR.WORK);
               return true;
           // }
           // else
           // {
            //    SetMsg("BurningMode FAIL", UDF.COLOR.FAIL);
             //   return false;
           // }



        }
        bool Edradour_Set_BYOD()//@@@###
        {
            string ack;
            string ack2;
            //set
            SetMsg("Edradour_Set_BYOD", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su 0 nf_factory -s -k byod -v true", out ack);
            //get
            SetMsg("GetBurningMode", UDF.COLOR.WORK);
            
            RunAdbCmd("adb shell su root nf_factory", out ack2);
            //RunAdbCmd("adb shell su root \"nf_factory | grep byod\" ", out ack2);
            // RunAdbCmd_BYOD("shell su root \"nf_factory | grep byod\"", out ack2);

            savelogs("ack Get bod = " + ack);
            savelogs("ack2 Get bod = " + ack2); 

            if (ack2.Contains("byod") && ack2.Contains("true"))
            {
                SetMsg("Edradour_Set_BYODf PASS", UDF.COLOR.WORK);
                return true;
            }
            else
            {
                SetMsg("Edradour_Set_BYODf Fail", UDF.COLOR.FAIL);
                return false;
            }



        }
        //bool RunAdbCmd_NOACK(@"D:\ForTestProgramMerge\CCMTesterAndroidProject.exe " + SDriverX.g_modelInfo.sSn + " 0 0", out ack, 1)//@@@###
        void RunAdbCmd_NOACK_EdradourULK(string cmd, out string ack,int idx)//@@@###
        {
         
            //set
            SetMsg("setting Camera pattern", UDF.COLOR.WORK);
            savelogs("setting Camera pattern");
            RunAdbCmd(cmd, out ack);
            savelogs(cmd + "  ack =" + ack);

        }

        bool harris_CheckFw()
        {
            string ack, ack2, progid;
            int iRetryTime = new int();
            int iRetryTime2 = 0;
            string code = string.Empty;
            iRetryTime = 0;
         
#if checkToch//check touch pad fw version
            if (SDriverX.g_modelInfo.sPart_no.Substring(13, 2).ToUpper() == "HA")
            {

                //code = wbc.iniFile.GetPrivateStringValue("FW_USER_Harr", "touch_fw", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                //if (code=="")
                //{
                //    code = "509";
                //}
                if (SDriverX.g_modelInfo.sPart_no.EndsWith("4"))
                {
                    code = wbc.iniFile.GetPrivateStringValue("FW_USER_Harr", "touch_fw4", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                    if (code == "")
                    {
                        code = "509";
                    }
                }
                else
                {
                    code = wbc.iniFile.GetPrivateStringValue("FW_USER_Harr", "touch_fw3", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                    if (code == "")
                    {
                        code = "600";
                    }
                }
            }
            else
            {

                code = wbc.iniFile.GetPrivateStringValue("FW_USER_Kava", "touch_fw", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (code == "")
                {
                    code = "503";
                }
            }
            SetMsg("Get"+ code + "fw version...", UDF.COLOR.WORK);
            savelogs("Get" + code + "fw version..");
#endif

            if (Directory.Exists(Application.StartupPath + @"\Neat_cmd\") == false)
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Neat_cmd");
            }
            if (File.Exists(Application.StartupPath + @"\Neat_cmd\get_touch_fw.bat") == false)
            {
                SetMsg("copy get_touch_fw.bat", UDF.COLOR.PASS);
                File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\get_touch_fw.bat", @Application.StartupPath + @"\Neat_cmd\get_touch_fw.bat",true);
            }
            //SetMsg("Get fw:"+ builddate, UDF.COLOR.WORK);
    re:
            try
            {
                if (File.Exists(Application.StartupPath + "\\HARRIS2.txt"))
                {
                    File.Delete(Application.StartupPath + @"\HARRIS2.txt");
                    File.Delete(Application.StartupPath + @"\Neat_cmd\HARRIS2.txt");
                    Delay(1000);
                }
                Application.DoEvents();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Application.StartupPath + @"\Neat_cmd\get_touch_fw.bat";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
                Delay(1500);
                Application.DoEvents();
                if (File.Exists(Application.StartupPath + "\\HARRIS2.txt") == false)
                {
                    if (iRetryTime < 6)
                    {
                        ack2 = "等待HARRIS2.txt" + iRetryTime.ToString() + "\r\n";
                        textBoxAck.AppendText(ack2);
                        Delay(100);
                        iRetryTime++;
                        goto re;
                    }
                    return false;
                }
                re1:
                Delay(200);
                Application.DoEvents();
                try
                {
                    ack = File.ReadAllText(@Application.StartupPath + "\\HARRIS2.txt" );

                } catch (Exception ex )
                {
                    goto re1 ;
                }
#if checkToch//check touch pad fw version
                //"00000509"

                if (ResultCheck(ack, code) == false)
                {
                    ack2 = "查找adb:" + ack + iRetryTime.ToString() + "\r\n";
                    textBoxAck.AppendText(ack2);

                    if (iRetryTime2 < 3)
                    {
                        iRetryTime2++;
                        goto re;
                    }
                    SetMsg(ack, UDF.COLOR.FAIL);
                    return false;
                }
#endif
                SetMsg(ack, UDF.COLOR.PASS);
                textBoxAck.AppendText(ack);
                DateTime dt1 = DateTime.Now;
#if checkToch//check touch pad fw version
                SetMsg("上传fw测试记录 ", UDF.COLOR.WORK);

                if (code == "600")//@@@
                {
                    RunAdbCmd("adb shell su root nf_factory -s -k insttdm -v second > command_ack.txt", out ack);
                    savelogs("adb shell su root nf_factory -s -k insttdm -v second > command_ack.txt:" + ack);
                    Thread.Sleep(1000);
                    RunAdbCmd("adb shell su root nf_factory", out ack);
                    savelogs("adb shell su root nf_factory");

                    if (ResultPick(ack, @"(?<=insttdm=).*?(?=\r|\n)", out ack2) == false)
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("Get touch insttdm fail!", UDF.COLOR.FAIL);
                        savelogs("Err:" + ack);
                        savelogs("Get touch insttdm fail!");
                        return false;
                    }
                    SetMsg("touch insttdm sn:" + ack2, UDF.COLOR.WORK);
                    SetMsg("Compare touch insttdm...", UDF.COLOR.WORK);
                    savelogs("touch insttdm sn:" + ack2);
                    savelogs("Compare touch insttdm...");

                    if (!ack2.Contains("second"))
                    {
                        SetMsg("Compare touch insttdm fail!", UDF.COLOR.FAIL);
                        savelogs("Compare touch insttdm fail!");
                        return false;
                    }

                }

                if (Database.Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + SDriverX.g_modelInfo.sSn + "','" + GlobalConfig.sMesEid.Substring(5, 3) + "', '" + code + "' ,'touch', 'OK')") == false)
                {
                    SetMsg("上传测试记录失败", UDF.COLOR.FAIL);
                    return false;
                }
#endif

            }
            catch (Exception ex)
            {
                if(ex.ToString().Contains("being used"))
                {

                    if (iRetryTime2 < 6)
                    {
                        iRetryTime2++;
                        goto re;
                    }

                }
                return false;
            }
            try
            {
                File.Delete(Application.StartupPath + @"\HARRIS2.txt");
            }catch(Exception ex)
            {

            }

            return true;
        }
        bool Neat_CheckFw()
        {
            string ack, ack2, progid;
            int iRetryTime = new int();

            iRetryTime = 0;
            GetFw:
            SetMsg("Get fw version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell getprop ro.build.id", out ack);
            builddate = ack.Replace("\r\n", "");
            if (ResultPick(ack, @"(?<=NF).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get fw version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            //@@@### need to check edradour sql
            if (!SDriverX.g_modelInfo.sPart_no.Substring(10, 2).ToUpper().Contains("FE"))
            {
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
                        textBoxRC_SN.Text = progid.ToString();
                        //g_iPtInitFlag = INIT_ZERO;  //工单切换后，这个 flag 会重置为 0，以通知其他部分更新信息，比如从数据库更新此工单应该卡的 FW 信息
                        if (CheckFwVerSql(progid, ack) == false) return false;
                    }
                }
            }
            return true;
        }
        bool Neat_SetTvSn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
            SetSn:
            SetMsg("Set sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory -s -k serialno -v " + SDriver.SDriverX.g_modelInfo.sSn, out ack);
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
        bool Neat_GetTvSn()
        {
            string ack, ack2;
            int iRetryTime = new int();
            iRetryTime = 0;
            GetMac:
            SetMsg("Get TV sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory", out ack);
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
        bool Neat_Install_Apk()
        {
            string ack="";
            int iRetryTime = new int();
            int iRetryTime2 = new int();
            int ret = 0;
            int sum = 10;
            ProcessStartInfo psi = new ProcessStartInfo();
            try
            {
                if (File.Exists(@Application.StartupPath + "\\result.txt"))
                {
                    File.Delete(@Application.StartupPath + "\\result.txt");
                }
                if (File.Exists(@Application.StartupPath + "\\Neat_cmd\\result.txt"))
                {
                    File.Delete(@Application.StartupPath + "\\Neat_cmd\\result.txt");
                }
            }
            catch (Exception ex)
            {
                RunAdbCmd("taskkill /f /im result.txt /t", out ack, 1);
            }
            //RunAdbCmd("taskkill /f /im result.txt /t", out ack, 1);
            setApk:
   try {
               
                SetMsg("set Factory APP...", UDF.COLOR.WORK);
            
            Delay(1000);
            Application.DoEvents();
            if (File.Exists(Directory.GetCurrentDirectory() + @"\Neat_cmd\set_apk.bat")==false )
            {
                File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\set_apk.bat", Directory.GetCurrentDirectory() + @"\Neat_cmd\set_apk.bat");
            }
            if (File.Exists(Directory.GetCurrentDirectory() + @"\Neat_cmd\FactoryApp.apk") == false)
            {
                File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\kava\FactoryApp.apk", Directory.GetCurrentDirectory() + @"\Neat_cmd\FactoryApp.apk");
                Delay(1000);
            }
                if (File.Exists(@Application.StartupPath + @"\Neat_cmd\get_apk.bat") == false)
                {
                    File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\get_apk.bat", Directory.GetCurrentDirectory() + @"\Neat_cmd\get_apk.bat");
                }
                    Application.DoEvents();
                //RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\set_apk.bat",
                //     "type " + Directory.GetCurrentDirectory() + @"\result.txt", out ack, 1);


                
                psi.FileName = Application.StartupPath + @"\Neat_cmd\set_apk.bat";
                // psi.WindowStyle = ProcessWindowStyle.Hidden;
                //psi.RedirectStandardOutput = true;
                Process.Start(psi);
                textBoxAck.AppendText("CMD:\r\n" + "wait 20s"+ "\r\n");
                //Delay(20000);
        read:
                for (int j=0;j< sum;j++)
                {
                    System.Threading.Thread.Sleep(1000);
                    textBoxAck.AppendText("wait:" + j + "\r\n");
                    Application.DoEvents();
                }
            
            
                if (File.Exists(@Application.StartupPath + "\\result.txt"))
                {
                    string[] ack1 = File.ReadAllLines(@Application.StartupPath + "\\result.txt");
                    
                    if (ack1[0] == "")
                    {
                        //ack1 = File.ReadAllLines(@Application.StartupPath + "\\Neat_cmd\\result.txt");
                    }
                    ack = ack1[0];
                    if (ack1.Length == 2)
                    {
                        ack = ack1[1];
                    }
                    textBoxAck.AppendText("CMD:\r\n" + ack + "\r\n");
                }
                else if(File.Exists(@Application.StartupPath + "\\Neat_cmd\\result.txt"))
                {

                    string[] ack1 = File.ReadAllLines(@Application.StartupPath + "\\result.txt");
                    if (ack1[0] == "")
                    {
                        //ack1 = File.ReadAllLines(@Application.StartupPath + "\\Neat_cmd\\result.txt");
                    }
                    ack = ack1[0];
                    if (ack1.Length == 2)
                    {
                        ack = ack1[1];
                    }
                    textBoxAck.AppendText("CMD:\r\n" + ack + "\r\n");
                }
                else
                {
                    if (ret < 6)
                    {
                        ret++;
                        Delay(1000);
                        goto setApk;
                    }
                    return false;
                }
                SetMsg("result:" + ack, UDF.COLOR.WORK);
                if (ack.Contains("Performing"))
                {
                    System.Threading.Thread.Sleep(1000);
                    sum = 10;
                    ret++;
                    if (ret < 6)
                    {
                        goto read;
                    }
                }
            }
            catch(Exception ex)
            {
                textBoxAck.AppendText("CMD:\r\n" + ex.ToString().Substring(0,150) + "\r\n");
                if (ex.ToString().Contains("being used"))
                {
                    RunAdbCmd("taskkill /f /im result.txt /t", out ack, 1);
                    if (iRetryTime2 < 15)
                    {
                        iRetryTime2++;
                        goto setApk;
                    }
                }
                return false;
            }
            SetMsg("result:" + ack, UDF.COLOR.WORK);
            //if (ResultCheck(ack.ToUpper(), "Success".ToUpper()) == false)
            if (ack.ToUpper().Contains("Success".ToUpper()) == false)
                {
                if (iRetryTime > 2)
                {
                    SetMsg("Factory APK install fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto setApk;
                }
            }
            if (File.Exists(@Application.StartupPath + "\\result.txt"))
            {
                File.Delete(@Application.StartupPath + "\\result.txt");
            }
            if (File.Exists(@Application.StartupPath + "\\Neat_cmd\\result.txt"))
            {
                File.Delete(@Application.StartupPath + "\\Neat_cmd\\result.txt");
            }
     GetFw:
            try
            {
                SetMsg("apk fw version 1.0.938 ", UDF.COLOR.WORK);
                psi.FileName = Application.StartupPath + @"\Neat_cmd\get_apk.bat";
                // psi.WindowStyle = ProcessWindowStyle.Hidden;
                //psi.RedirectStandardOutput = true;
                Process.Start(psi);
                Delay(2000);
                if (File.Exists(@Application.StartupPath + "\\ver.txt"))
                {
                    string[] ack1 = File.ReadAllLines(@Application.StartupPath + "\\ver.txt");

                    if (ack1[0] == "")
                    {
                        //ack1 = File.ReadAllLines(@Application.StartupPath + "\\Neat_cmd\\result.txt");
                    }
                    ack = ack1[0];
                    if (ack1.Length == 2)
                    {
                        ack = ack1[1];
                    }
                    textBoxAck.AppendText("CMD:\r\n" + ack + "\r\n");
                }
                else
                {
                    goto GetFw;
                }
                SetMsg("read apk fw:"+ack, UDF.COLOR.WORK);
                if (ack.Contains("1.0.938")==false)
                {
                    SetMsg("apk fw fail:", UDF.COLOR.WORK);
                    goto setApk;
                }
                if (File.Exists(@Application.StartupPath + "\\ver.txt"))
                {
                    File.Delete(@Application.StartupPath + "\\ver.txt");
                }
            }
            catch (Exception ex)
            {

            }
            
            
            return true;
        }
        bool get_hdmiin_LT6911_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();

            g_sFwVer1 = ReadIniFile("TEST", "hdmiin_LT6911_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get HDI LT6911 Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmiin_LT6911_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }

        bool get_hdmi1_LT6911_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();
            g_sFwVer1 = ReadIniFile("TEST", "hdmi1_LT6911_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get HDMI IN LT6911 Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmi1_LT6911_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }
        bool get_hdmi1sub_LT6911_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();
            g_sFwVer1 = ReadIniFile("TEST", "hdmi1sub_LT6911_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get hdmi1sub LT6911 Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmi1sub_LT6911_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }

        bool get_hdmi23_LT6911_Minor_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();
            g_sFwVer1 = ReadIniFile("TEST", "hdmi23_LT6911_Minor_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get hdmi23 LT6911 Minor Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmi23_LT6911_Minor_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }

        bool get_hdmi23_LT6911_Major_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();
            g_sFwVer1 = ReadIniFile("TEST", "hdmi23_LT6911_Major_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get hdmi23 LT6911 Major Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmi23_LT6911_Major_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }

        bool get_hdmi23_LT6911_Build_fw()
        {
            string ack, ack2;
            int iRetryTime = new int();
            g_sFwVer1 = ReadIniFile("TEST", "hdmi23_LT6911_Build_fw", GlobalConfig.sIniPath);
            iRetryTime = 0;
            GetFw:
            SetMsg("Get hdmi23 LT6911 Build Version...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\get_hdmi23_LT6911_Build_fw.bat", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack);
            if (ResultPick(ack, g_sFwVer1, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get HDI LT6911 version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }
            return true;
        }
        bool Barra_SetAudioVol(int vol = 6)
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        FactMode:
            SetMsg("set media Volume...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetMediaVolume --ei mediaVolume " + vol.ToString(),
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, "MediaVolume=" + vol.ToString()) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("set media Volume fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto FactMode;
                }
            }

            return true;
        }

        bool Barra_GetMAC(UDF.MAC_TYPE mac_type)
        {
            string sql = string.Empty;
            string sEthMac = string.Empty;
            string sWifiMac = string.Empty;
            string sBtMac = string.Empty;
            bool bErr = new bool();
            int flag = new int();
            OracleDataReader reader;

            if (mac_type == UDF.MAC_TYPE.ETH)
            {
                #region ETH MAC
                SetMsg("正在从数据库检索 ETH MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                {
                 sql = string.Format("select * from rknmgr.insp_Owllabs_Mac where use_mode = 0 and ssn = '{0}'",
                                          SDriverX.g_modelInfo.sSn);
                }
                else
                {
                    sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 0 and ssn = '{0}'",
                      SDriverX.g_modelInfo.sSn);
                }
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        flag = 1;
                        sEthMac = reader[0].ToString();
                        SDriverX.g_modelInfo.sEthMac = reader[0].ToString().ToUpper();
                        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    }
                    else
                    {
                        //没有数据
                        flag = 2;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    return false;
                }

                if (flag == 2)
                {
                    //正常情况下是，打印标签时分配 ETH MAC的。未分配则提示报错
                    SetMsg("当前SN未分配ETH MAC，请联系MES或LABLE室", UDF.COLOR.FAIL);
                    return false;
                }

                //保留自申请功能
                //申请key并更新表
                //if (flag == 2)
                //{
                //    //如果之前没有申请Eth Mac,现在申请
                //    sql = "select * from rknmgr.insp_barrakey_t where use_mode = 0 and use_flag = 0 and rownum = 1";
                //    bErr = Oracle.ServerExecute(sql, out reader);
                //    if (bErr)
                //    {
                //        reader.Read();
                //        if (reader.HasRows)
                //        {
                //            //已有数据
                //            flag = 1;
                //            sEthMac = reader[0].ToString();
                //            SDriverX.g_modelInfo.sEthMac = StrToMac(reader[0].ToString()).ToUpper();
                //            SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                //        }
                //        else
                //        {
                //            //没有数据
                //            SetMsg("从数据库申请 ETH MAC fail，请联系TE", UDF.COLOR.FAIL);
                //            return false;
                //        }
                //        reader.Close();
                //    }
                //    else
                //    {
                //        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //        return false;
                //    }

                //    //更新表
                //    sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                //        SDriverX.g_modelInfo.sSn,
                //        (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                //        sEthMac);
                //    bErr = Oracle.UpdateServer(sql);
                //    if (!bErr)
                //    {
                //        SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                //        return false;
                //    }

                //    //检验数据库信息
                //    sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 0 and ssn = '{0}'",
                //           SDriverX.g_modelInfo.sSn
                //            );
                //    bErr = Oracle.ServerExecute(sql, out reader);
                //    if (bErr)
                //    {
                //        reader.Read();
                //        if (reader.HasRows)
                //        {
                //            //有数据
                //            if (reader[0].ToString() != sEthMac)
                //            {
                //                SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sEthMac, reader[0].ToString()), UDF.COLOR.FAIL);
                //                reader.Close();
                //                return false;
                //            }
                //        }
                //        else
                //        {
                //            //没有数据
                //            SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                //            reader.Close();
                //            return false;
                //        }
                //        reader.Close();
                //    }
                //    else
                //    {
                //        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //        return false;
                //    }
                //} //end if (flag == 2)
                #endregion END ETH MAC
            }   // end if (mac_type == UDF.MAC_TYPE.ETH)
            else if (mac_type == UDF.MAC_TYPE.BT)
            {
                #region BT MAC
                SetMsg("正在从数据库检索 BT MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                {
                    sql = string.Format("select * from rknmgr.insp_Owllabs_Mac where use_mode = 1 and ssn = '{0}'",
                                             SDriverX.g_modelInfo.sSn);
                }
                else
                {
                    sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 1 and ssn = '{0}'",
                      SDriverX.g_modelInfo.sSn);
                }
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        flag = 1;
                        sBtMac = reader[0].ToString();
                        SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    }
                    else
                    {
                        //没有数据
                        flag = 2;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    return false;
                }

                //申请key并更新表
                if (flag == 2)
                {
                    //如果之前没有申请Eth Mac,现在申请
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {
                        sql =  "select * from rknmgr.insp_Owllabs_Mac where use_mode = 1 and use_flag = 0 and rownum = 1";
                                                  
                    }
                    else
                    {
                        sql = "select * from rknmgr.insp_barrakey_t where use_mode = 1 and use_flag = 0 and rownum = 1";
                    }
                    bErr = Database.Oracle.ServerExecute(sql, out reader);
                    if (bErr)
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            //已有数据
                            flag = 1;
                            sBtMac = reader[0].ToString();
                            SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                        }
                        else
                        {
                            //没有数据
                            SetMsg("从数据库申请 BT MAC fail，请联系TE", UDF.COLOR.FAIL);
                            return false;
                        }
                        reader.Close();
                    }
                    else
                    {
                        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                        return false;
                    }

                    //更新表
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {
                        sql = string.Format("update rknmgr.insp_Owllabs_Mac set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                          SDriverX.g_modelInfo.sSn,
                          (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                          sEthMac);
                    }
                    else
                    {
                        sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                          SDriverX.g_modelInfo.sSn,
                          (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                          sEthMac);
                    }
                    bErr = Database.Oracle.UpdateServer(sql);
                    if (!bErr)
                    {
                        SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                        return false;
                    }

                    //检验数据库信息
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {
                        sql = string.Format("select * from rknmgr.insp_Owllabs_Mac where t.use_mode = 1 and t.ssn = '{0}'",
                             SDriverX.g_modelInfo.sSn
                              );
                    }
                    else
                    {
                        sql = string.Format("select * from rknmgr.insp_barrakey_t where t.use_mode = 1 and t.ssn = '{0}'",
                             SDriverX.g_modelInfo.sSn
                              );
                    }
                    bErr = Database.Oracle.ServerExecute(sql, out reader);
                    if (bErr)
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            //有数据
                            if (reader[0].ToString() != sBtMac)
                            {
                                SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sBtMac, reader[0].ToString()), UDF.COLOR.FAIL);
                                reader.Close();
                                return false;
                            }
                        }
                        else
                        {
                            //没有数据
                            SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                            reader.Close();
                            return false;
                        }
                        reader.Close();
                    }
                    else
                    {
                        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                        return false;
                    }
                } //end if (flag == 2)
                #endregion END BT MAC
            } // end else if (mac_type == UDF.MAC_TYPE.ETH)
            else if (mac_type == UDF.MAC_TYPE.WIFI)
            {
                #region WIFI MAC
                SetMsg("正在从数据库检索 WIFI MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                {
                    sql = string.Format("select * from rknmgr.insp_Owllabs_Mac where use_mode = 2 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                }
                else if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "QC")  //VXXWWQCB----.QC85501
                {
                    sql = string.Format("select * from rknmgr.qcs_mac where use_mode = 2 and ssn = '{0}'",
                                             SDriverX.g_modelInfo.sSn);
                }
                else
                {

                  sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 2 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                }

                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        flag = 1;
                        sWifiMac = reader[0].ToString();
                        SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    }
                    else
                    {
                        //没有数据
                        flag = 2;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败(检索)", UDF.COLOR.FAIL);
                    return false;
                }

                //申请key并更新表
                if (flag == 2)
                {
                    //如果之前没有申请Wifi Mac,现在申请
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {

                        sql = "select * from rknmgr.insp_Owllabs_Mac where use_mode = 2 and use_flag = 0 and rownum = 1";
                    }
                    else
                    {
                        sql = "select * from rknmgr.insp_barrakey_t where use_mode = 2 and use_flag = 0 and rownum = 1";
                    }
                    bErr = Database.Oracle.ServerExecute(sql, out reader);
                    if (bErr)
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            //已有数据
                            flag = 1;
                            sWifiMac = reader[0].ToString();
                            SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                            SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                        }
                        else
                        {
                            //没有数据
                            SetMsg("从数据库申请 WIFI MAC fail，请联系TE", UDF.COLOR.FAIL);
                            return false;
                        }
                        reader.Close();
                    }
                    else
                    {
                        SetMsg("数据库操作失败(申请Key)", UDF.COLOR.FAIL);
                        return false;
                    }

                    //更新表
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {

                        sql = string.Format("update rknmgr.insp_Owllabs_Mac set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                         SDriverX.g_modelInfo.sSn,
                         (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                         sWifiMac);
                    }
                    else
                    {
                        sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                       SDriverX.g_modelInfo.sSn,
                       (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                       sWifiMac);
                    }
                    bErr = Database.Oracle.UpdateServer(sql);
                    if (!bErr)
                    {
                        SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                        return false;
                    }

                    Delay(1000);
                    //检验数据库信息
                    if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")
                    {
                        sql = string.Format("select * from rknmgr.insp_Owllabs_Mac where use_mode = 2 and ssn = '{0}'",
                       SDriverX.g_modelInfo.sSn
                        );
                    }
                    else
                    {  
                        sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 2 and ssn = '{0}'",
                       SDriverX.g_modelInfo.sSn
                        );
                    }
                    bErr = Database.Oracle.ServerExecute(sql, out reader);
                    if (bErr)
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            //有数据
                            if (reader[0].ToString() != sWifiMac)
                            {
                                SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sWifiMac, reader[0].ToString()), UDF.COLOR.FAIL);
                                reader.Close();
                                return false;
                            }
                        }
                        else
                        {
                            //没有数据
                            SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                            reader.Close();
                            return false;
                        }
                        reader.Close();
                    }
                    else
                    {
                        SetMsg("数据库操作失败(校验)", UDF.COLOR.FAIL);
                        return false;
                    }
                } //end if (flag == 2)
                #endregion END WIFI MAC

            } // end else if (mac_type == UDF.MAC_TYPE.ETH)
            else if(mac_type == UDF.MAC_TYPE.AUX)
            {
                #region ETH MAC
                
                //先查询是否已申请

                SetMsg("正在从数据库检索 AUX MAC...", UDF.COLOR.WORK);
                sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 1 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        flag = 1;
                        sEthMac = reader[0].ToString();
                        SDriverX.g_modelInfo.AUXMac = reader[0].ToString().ToUpper();
                        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    }
                    else
                    {
                        //没有数据
                        flag = 2;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    return false;
                }

                if (flag == 2)
                {
                    //正常情况下是，打印标签时分配 ETH MAC的。未分配则提示报错
                    SetMsg("当前SN未分配AUX MAC，请联系MES或LABLE室", UDF.COLOR.FAIL);
                    return false;
                }
              
                #endregion END ETH MAC
            }
            else
            {
                SetMsg("MAC TYPE参数错误，请联系TE工程师", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool QCS_GetMAC(int use_mode)
        {
            string sql = string.Empty;
            string sEthMac = string.Empty;
            string sWifiMac = string.Empty;
            string sBtMac = string.Empty;
            bool bErr = new bool();
            int flag = new int();
            OracleDataReader reader;

            #region ETH MAC
            SetMsg("正在从数据库检索 MAC...", UDF.COLOR.WORK);
            //先查询是否已申请
            sql = string.Format("select * from rknmgr.QCS_MAC where use_mode = '" + use_mode + "' and ssn = '{0}'",
                SDriverX.g_modelInfo.sSn);
            bErr = Database.Oracle.ServerExecute(sql, out reader);
            if (bErr)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    //已有数据
                    flag = 1;
                    sEthMac = reader[0].ToString();
                    if  (use_mode==0)
                    {
                        SDriverX.g_modelInfo.sEthMac = reader[0].ToString().ToUpper();
                    }
                    else if (use_mode == 1)
                    {
                        SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                    }
                    else if (use_mode == 2)
                    {
                        SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                    }
                    else if (use_mode == 3)
                    {
                        SDriverX.g_modelInfo.AUXMac = reader[0].ToString().ToUpper();
                    }
                    SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                }
                else
                {
                    //没有数据
                    flag = 2;
                }
                reader.Close();
            }
            else
            {
                SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                return false;
            }

            //if (flag == 2)
            //{
            //    //正常情况下是，打印标签时分配 ETH MAC的。未分配则提示报错
            //    SetMsg("当前SN未分配ETH MAC，请联系MES或LABLE室", UDF.COLOR.FAIL);
            //    return false;
            //}

            //保留自申请功能
            //申请key并更新表
            if (flag == 2)
            {
                //如果之前没有申请Eth Mac,现在申请
                sql = "select * from rknmgr.QCS_MAC where use_mode = '" + use_mode + "' and use_flag = 0 and rownum = 1";
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        flag = 1;
                        sEthMac = reader[0].ToString();
                        if (use_mode == 0)
                        {
                            SDriverX.g_modelInfo.sEthMac = reader[0].ToString().ToUpper();
                        }
                        else if (use_mode == 1)
                        {
                            SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                        }
                        else if (use_mode == 2)
                        {
                            SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                        }
                        else if (use_mode == 3)
                        {
                            SDriverX.g_modelInfo.AUXMac = reader[0].ToString().ToUpper();
                        }
                        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    }
                    else
                    {
                        //没有数据
                        SetMsg("从数据库申请 ETH MAC fail，请联系TE", UDF.COLOR.FAIL);
                        return false;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    return false;
                }

                //更新表
                sql = string.Format("update rknmgr.QCS_MAC set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                    SDriverX.g_modelInfo.sSn,
                    (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                    sEthMac);
                bErr = Database.Oracle.UpdateServer(sql);
                if (!bErr)
                {
                    SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                    return false;
                }

                //检验数据库信息
                sql = string.Format("select * from rknmgr.QCS_MAC where use_mode = '" + use_mode + "'  and ssn = '{0}'",
                        SDriverX.g_modelInfo.sSn
                        );
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //有数据
                        if (reader[0].ToString() != sEthMac)
                        {
                            SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sEthMac, reader[0].ToString()), UDF.COLOR.FAIL);
                            reader.Close();
                            return false;
                        }
                    }
                    else
                    {
                        //没有数据
                        SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                        reader.Close();
                        return false;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    return false;
                }
                #endregion END ETH MAC
            }   // end if (mac_type == UDF.MAC_TYPE.ETH)
            return true;
        }
        bool Barra_WriteEthMac()
        {

            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;

            iRetryTime = 0;
        GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.ETH) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            SetMsg("[Debug] 指定ETH MAC:C4:63:FB:00:11:2C", UDF.COLOR.WORK);
            SDriverX.g_modelInfo.sEthMac = "C4:63:FB:00:11:2C";

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
        WriteEthMac:
            SetMsg("Write Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.SetEthMAC --es ethMac " + sEthMac,
                "adb shell cat /sys/class/net/eth0/address", out ack);
            if (ResultPick(ack, @"([a-z A-Z 0-9]{2}:){5}[a-z A-Z 0-9]{2}", out ack2) == false)
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

            return true;
        }
        bool QCS_WriteEthMacWithExe()
        {
            int iRetryTime = new int();
            string ack;
            int use_Mode = 0;
            string sEthMac = string.Empty;

            iRetryTime = 0;
            GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (QCS_GetMAC(use_Mode) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            //SetMsg("[Debug] 指定ETH MAC:C4:63:FB:00:11:2C", UDF.COLOR.WORK);
            //SDriverX.g_modelInfo.sEthMac = "C4:63:FB:00:11:2C";

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
            WriteEthMac:
            SetMsg("Write Eth MAC...", UDF.COLOR.WORK);
            SetMsg(Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", UDF.COLOR.WORK);
            RunAdbCmd(GlobalConfig.sCmd1Path + " " + sEthMac,
                "type " + Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", out ack, 1);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Write Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            return true;
        }
        bool Barra_WriteEthMacWithExe()
        {
            int iRetryTime = new int();
            string ack;
            string sEthMac = string.Empty;

            iRetryTime = 0;
        GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.ETH) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            //SetMsg("[Debug] 指定ETH MAC:C4:63:FB:00:11:2C", UDF.COLOR.WORK);
            //SDriverX.g_modelInfo.sEthMac = "C4:63:FB:00:11:2C";
            if (SDriverX.g_modelInfo.sPart_no.Substring(14, 2).ToUpper() == "FE")
            {
                if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.UC17, out sEthMac) == false)
                {
                    SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
                {
                    SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                    return false;
                }
            }
           

            iRetryTime = 0;
        WriteEthMac:
            SetMsg("Write Eth MAC...", UDF.COLOR.WORK);
            SetMsg(Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", UDF.COLOR.WORK);
            RunAdbCmd(GlobalConfig.sCmd1Path + " " + sEthMac,
                "type " + Directory.GetCurrentDirectory() + @"\WriteEthMac\command_ack.txt", out ack, 1);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Write Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            return true;
        }
        bool SDF()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;
            SetMsg("SDF检查...", UDF.COLOR.WORK);
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Neat_cmd\Check_SDF.bat"))
            {
                File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\Check_SDF.bat", @Application.StartupPath + @"\Neat_cmd\Check_SDF.bat");
            }
            CheckEthMac:
            File.Delete(Directory.GetCurrentDirectory() + @"\sdf.txt");
            Delay(200);            
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\Neat_cmd\Check_SDF.bat",
                 "type " + Directory.GetCurrentDirectory() + @"\sdf.txt", out ack, 1);
            if (ack.Contains("/dev/block/sdf\r\n/dev/block/sdf1\r\n/dev/block/sdf2\r\n/dev/block/sdf3\r\n/dev/block/sdf4\r\n/dev/block/sdf5\r\n/dev/block/sdf6\r\n/dev/block/sdf7\r\n/dev/block/sdf8\r\n/dev/block/sdf9"))
            {
                return true;
            }
            else
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Check sdf fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            //if (ResultPick(ack, @"maceth=" + sEthMac, out ack2) == false)
            //{
            //    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
            //    SetMsg("Check Eth MAC fail!", UDF.COLOR.FAIL);
            //    if (iRetryTime > 2)
            //        return false;
            //    else
            //    {
            //        iRetryTime++;
            //        goto CheckEthMac;
            //    }
            //}
            return true;
        }
        bool Edradour_WriteEthMac()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;
            SetMsg("Eth Mac检查...", UDF.COLOR.WORK);
            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.UC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            WriteEthMac:
            SetMsg("Write eeprom Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory -s -k maceth -v " + sEthMac, out ack);
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
            SetMsg("Check eeprom Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_eth.bat",
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
        bool Kava_WriteEthMac()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;
            GetEthMac:
            SetMsg("Get AUX MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.AUX) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            SetMsg("AUX Mac检查...", UDF.COLOR.WORK);
            if (SwitchMacStyle(SDriverX.g_modelInfo.AUXMac, UDF.MAC_STYLE.UC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            WriteEthMac:
            SetMsg("Write eeprom Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory -s -k macethaux -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting macethaux=" + sEthMac, out ack2) == false)
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
            SetMsg("Check aux Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_auxeth.bat",
                "type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 1);
            if (ResultPick(ack, @"macethaux=" + sEthMac, out ack2) == false)
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
        bool Edradour_SetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            SetMsg("Mic vendor: " + flag, UDF.COLOR.WORK);
            if (flag.ToUpper()=="Y"||textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR1" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR7" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR9")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory -s -k instmic -v merry > command_ack.txt", out ack);
            }
            else if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR2")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory -s -k instmic -v infineon > command_ack.txt ", out ack);
            }
            return true;
        }
        bool Edradour_GetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            SetMsg("Mic vendor: " + flag, UDF.COLOR.WORK);
            if (flag.ToUpper()=="Y"||textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR1" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR7" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR9")
            {
                SetMsg("Get merry Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory > data_command_ack.txt", out ack);
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
            else if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "AR2")
            {
                SetMsg("Get infineon Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory > data_command_ack.txt", out ack);
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
        bool Barra_CheckEthMac()
        {
            string ack, ack2;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.GetEthMAC",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=MAC=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg(ack, UDF.COLOR.FAIL);
                SetMsg("Get Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }
            SetMsg("Eth MAC:" + ack2, UDF.COLOR.WORK);

            return true;
        }
        bool Neat_WriteWifiBTMac()
        {
            string ack, ack2;
            string sWifiMac = string.Empty;
            int iRetryTime = new int();

            iRetryTime = 0;
            GetWifiMac:
            SetMsg("Get WiFi/BT MAC...", UDF.COLOR.WORK);
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

            if (SwitchMacStyle(SDriverX.g_modelInfo.sWifiMac, UDF.MAC_STYLE.UC17, out sWifiMac) == false)
            {
                SetMsg("Eth Mac检查失败！WiFi Mac:" + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
            MacUpdate:
            SetMsg("Set WiFi BT MAC..", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory -s -k macbtwlan -v " + sWifiMac, out ack);
            if (ResultCheck(ack, "Setting macbtwlan=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set WiFi BT MAC CMD fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto MacUpdate;
                }
            }

            iRetryTime = 0;
            GetMac:
            SetMsg("Check WiFi BT MAC...", UDF.COLOR.WORK);
            RunAdbCmd(Directory.GetCurrentDirectory() + @"\WriteEthMac\Check_wifi_bt.bat",
                "type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 1);
            if (ResultPick(ack, @"macbtwlan=" + sWifiMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Check WIFI MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }

            SetMsg("WiFi MAC:" + ack2, UDF.COLOR.WORK);
            return true;
        }
        bool Barra_WriteWifiMac()
        {
            string ack;
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
        MacUpdate:
            SetMsg("adb shell mac_update...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root mac_update " + sWifiMac, out ack);
            if (ResultCheck(ack, "completed|updated") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("adb shell mac_update fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto MacUpdate;
                }
            }

            return true;
        }

        bool Barra_CheckWifiMac()
        {
            string ack, ack2;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("Get WiFi MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.GetWiFiMAC",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=MAC=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get WiFi MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }

            SetMsg("WiFi MAC:" + ack2, UDF.COLOR.WORK);

            return true;
        }

        bool Barra_SetTvSn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetSn:
            SetMsg("Set sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k serialno -v " + SDriver.SDriverX.g_modelInfo.sSn, out ack);
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

        bool Barra_SetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            SetMsg("Mic vendor: " + flag, UDF.COLOR.WORK);
            if (flag.ToUpper() == "Y" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B19" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1A" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1B" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1C" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1D")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist -s -k inst_mic -v merry > command_ack.txt ", out ack);
          }
            return true;
        }
        bool Harr_SetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            SetMsg("Mic vendor: " + flag, UDF.COLOR.WORK);
            if (flag.ToUpper() == "Y")
            {
                SetMsg("Set Mic vendor merry...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory -s -k instmic -v merry > command_ack.txt", out ack);
            }
            else
            {
                SetMsg("Set Mic vendor infineon...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory -s -k instmic -v infineon > command_ack.txt ", out ack);
            }
            return true;
        }
        bool Harr_GetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            if (flag.ToUpper() == "Y")
            {
                SetMsg("Get merry Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_factory > data_command_ack.txt", out ack);
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
                RunAdbCmd("adb shell su root nf_factory > data_command_ack.txt", out ack);
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
            //return true;
        }
        bool Barra_GetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            if (flag.ToUpper() == "Y" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B19" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1A" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1B" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1C" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1D")
            {
                SetMsg("Get Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "inst_mic=merry")
                    {
                        return true;
                    }
                }
                SetMsg("Get build date fail...", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Cardhu_SetMicvendor()
        {
            string ack;
            string flag=wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            SetMsg("Mic vendor: "+ flag, UDF.COLOR.WORK);
            if (flag.ToUpper() =="Y" || textBoxPn.Text == "M65WWNFB2AO1.CARDHUQ" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU5" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU6" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU3" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU4" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUB" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUC")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist -s -k inst_mic -v merry > command_ack.txt ", out ack);
             }
            return true;
        }
        bool Cardhu_GetMicvendor()
        {
            string ack;
            string flag = wbc.iniFile.GetPrivateStringValue("Micvendor", textBoxPn.Text, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");//20221209
            if (flag.ToUpper() == "Y" || textBoxPn.Text == "M65WWNFB2AO1.CARDHUQ" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU5" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU6" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU3" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU4" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUB" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUC")
            {
                SetMsg("Get Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "inst_mic=merry")
                    {
                        return true;
                    }
                }
                SetMsg("Get build date fail...", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Barra_Setbuilddate()
        {
            string ack;
            RunAdbCmd("adb shell getprop ro.build.date.utc", out ack);
            Delay(2000);
            builddate = ack.Replace("\r\n", "");
            SetMsg(ack, UDF.COLOR.WORK);
            SetMsg("Set build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k fswtstamp -v "+ ack, out ack);
            return true;
        }
        bool Barra_Getbuilddate()
        {
            string ack;

            SetMsg("Get build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist > data_commaand_ack.txt ", out ack);
            string s = Application.StartupPath + "\\data_commaand_ack.txt";
            string[] contents = File.ReadAllLines(s, Encoding.Default);
            for (int i = 0; i < contents.Length; i++)
            {
                while (contents[i].Substring(0,10) == "fswtstamp=")
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
        bool Neat_Setbuilddate()
        {
            string ack;
            RunAdbCmd("adb shell getprop ro.build.date.utc", out ack);
            Delay(2000);
            builddate = ack.Replace("\r\n", "");
            SetMsg(ack, UDF.COLOR.WORK);
            SetMsg("Set build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory -s -k fswtstamp -v " + ack, out ack);
            return true;
        }
        bool Neat_Getbuilddate()
        {
            string ack;

            SetMsg("Get build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_factory > data_commaand_ack.txt ", out ack);
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
        bool Barra_GetTvSn()
        {
            string ack, ack2;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
            GetMac:
            SetMsg("Get TV sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist", out ack);

            //新版 GET SN（须在第一次写完后，断上电）
            //RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetSN",
            //    "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            //if (ResultPick(ack, @"(?<=SN=).*?(?=\r|\n)", out ack2) == false)            
             
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
        bool Barra_GetTvSn2()
        {
            string ack, ack2;
            int iRetryTime = new int();

            iRetryTime = 0;
        retry:
            SetMsg("準備讀取 sn  Chuẩn bị đọc sn...", UDF.COLOR.WORK);
            RunAdbCmd(@"D:\platform-tools" + "\\" + "fastboot devices", out ack); // fastboot mode 讀取sn
            //SetMsg("sn:" + ack, UDF.COLOR.WORK);
            Match match = Regex.Match(ack.ToString(), @"(\S+)\s+fastboot");
            ack = match.Groups[1].ToString();
            //SetMsg("sn:" + ack + textBoxSn.Text, UDF.COLOR.WORK);
            if (ack.Contains(textBoxSn.Text) == false)
            {
                if (iRetryTime < 2)
                {
                    iRetryTime++;
                    Thread.Sleep(1000);
                    goto retry;
                }
                else
                {
                    //代表抓不到fastboot mode之下的sn , 強制升級FW
                    SetMsg("SN:" + textBoxSn.Text + ":OK", UDF.COLOR.WORK);
                    return true;
                }
            }
            else
            {
                SetMsg("sn:" + ack + ":OK", UDF.COLOR.WORK);
                return true ;
            }

        }
        bool Barra_SetWifiCountryCode()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetWifiCountryCode:
            SetMsg("Set WiFi country code...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k countrycode -v US", out ack);
            if (ResultCheck(ack, "Setting countrycode=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set WiFi country code fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetWifiCountryCode;
                }
            }

            return true;
        }
        bool Barra_GetLightSensorValue()
        {
            string ack, ack2;
            int iRetryTime = new int();
            double lux = 0;

            iRetryTime = 0;
        GetSensorValue:
            SetMsg("Get light sensor value...", UDF.COLOR.WORK);
            
                RunAdbCmd("adb shell nf_sensors", out ack);

                if (ResultCheck(ack, "als:") == false)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get light sensor value fail!", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                else
                {
                    int a = ack.IndexOf("als:");
                    ack = ack.Substring(a + 7, 4);
                    lux = Convert.ToDouble(ack);
                    if (lux < g_dLightLuxSpec[0] || lux > g_dLightLuxSpec[1])
                    {
                        SetMsg(string.Format("Lux值超规,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto GetSensorValue;
                        }
                    }
                }

            SetMsg(string.Format("Lux PASS,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.WORK);

            WriteLog("LightSensorLog.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + SDriverX.g_modelInfo.sSn + "\t\t" + ack.Replace("\r", " ").Replace("\n", ""));

            return true;
        }
        bool Harr_GetLightSensorValue()
        {
            string ack, ack2;
            int iRetryTime = new int();
            double lux = 0;

            iRetryTime = 0;
        GetSensorValue:
            SetMsg("Get light sensor value...", UDF.COLOR.WORK);

            RunAdbCmd("adb shell nf_sensors", out ack);

            if (ResultCheck(ack, "als:") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get light sensor value fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            else
            {
                int a = ack.IndexOf("als:");
                ack = ack.Substring(a + 7, 4);
                lux = Convert.ToDouble(ack);
                if (lux < g_dLightLuxSpec[0] || lux > g_dLightLuxSpec[1])
                {//@@@@@
                    SetMsg(string.Format("Lux值超规,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
            }

            SetMsg(string.Format("Lux PASS,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.WORK);

            WriteLog("LightSensorLog.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + SDriverX.g_modelInfo.sSn + "\t\t" + ack.Replace("\r", " ").Replace("\n", ""));

            return true;
        }
        bool Harr_GetVOCValue()
        {
            string ack, ack2;
            int iRetryTime = new int();
            double lux = 0;

            iRetryTime = 0;
GetSensorValue:
            try
            {
                SetMsg("Get VOC sensor value...", UDF.COLOR.WORK);

                RunAdbCmd("adb shell nf_sensors", out ack);

                if (ResultCheck(ack, "voc_index:") == false)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get voc_co2 value fail!", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                else
                {
                    int a = ack.IndexOf("voc_index:  ");
                    ack2 = ack.Substring(a + 10, 8);
                    lux = Convert.ToDouble(ack2);
                    if (lux < 0)
                    {
                        SetMsg("Lux值超规,小于0", UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto GetSensorValue;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                SetMsg("Lux值超规", UDF.COLOR.FAIL);
                return false;
            }
            

             SetMsg(string.Format("voc_co2 PASS:" + lux), UDF.COLOR.WORK);

            WriteLog("VOCSensorLog.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + SDriverX.g_modelInfo.sSn + "\t\t" + ack.Replace("\r", " ").Replace("\n", ""));

            return true;
        }
        bool Barra_GetTmpSensorValue()
        {
            string ack, ack2; 
            int iRetryTime = new int();
            double temp, humidity;

            iRetryTime = 0;
            GetSensorValue:
            SetMsg("Get Temp & Humidity value...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetTempHumidity",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get Temp & Humidity value fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetSensorValue; 
                }
            }

            return true;
        }
        bool Barra_SetVocCo2SelfTest()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("VOC & CO2 self-test...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action VocCo2SelfTest",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, @"VocCo2SelfTest=OK") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("VOC & CO2 self-test fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }

            return true;
        }

        bool Barra_SetBiWifi()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        WifiConfig:
            SetMsg("Set Burning-WiFi config...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningWiFiConfig --es WiFi_SSID " + GlobalConfig.sBiWifiSsid +
                " --es WiFi_IPaddress " + GlobalConfig.sBiWifiGateway +
                " --ei WiFi_Timeout 40 --ei WiFi_RSSI_Threshold -64 --ei WiFi_Total_Test_Count 3 --ei WiFi_Delay_Time 8",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, @"SSID=" + GlobalConfig.sBiWifiSsid) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set Burning-WiFi config fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WifiConfig;
                }
            }

            return true;
        }

        bool Barra_SetBurnInOn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetWifiOn:
            SetMsg("Set burning mode on...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningMode --ez setMode 1",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, "BurningMode=1") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set burning mode on fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetWifiOn;
                }
            }

            return true;
        }

        bool QCS_Radar_Test()
        {
            string ack;
            int iRetryTime = 0;
            DateTime dt1 = DateTime.Now;
            string okng = string.Empty;
            string sql = string.Empty;
            bool bErr = new bool();
            double dWifirssi = 0;
            string db = string.Empty;
            OracleDataReader reader;

            ReRadarTest:
            SetMsg("Start Radar Test ...", UDF.COLOR.WORK);
            if (File.Exists(Application.StartupPath + @"\\Neat_cmd\Radar_Test_QCS.bat")==false)
            {
                SetMsg(Application.StartupPath + @"\\Neat_cmd\Radar_Test_QCS.bat 文件不存在", UDF.COLOR.FAIL);
                return false;
            }
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Radar_Test_QCS.bat", out ack, 1);
            //RunAdbCmd("adb.exe shell am startservice -n com.amtran.factory /.FactoryService --es Action PresenceTest --ei factoryMode 3 --ei Test_Count 4", out ack, 1);
            Application.DoEvents();
            Delay(5000);
            RunAdbCmd("adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultCheck(ack, "radar_distance_3") == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("set Radar test fail!", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReRadarTest;
                }
            }
            ack = ack.Replace("\r\r\n", "|");
            string[] rssi = ack.ToString().Split(new char[] { '|' });
            //= Trim(Right(dac(3), Len(dac(3)) - InStr(1, dac(3), "=")))  "radar_angle_1=82" "radar_distance_1=128"
            int IndexofA = rssi[0].IndexOf("=");
            int IndexofB = rssi[1].IndexOf("=");
            string radar_angle_1 = rssi[0].Substring(14, rssi[0].Length - IndexofA - 1);
            double angle_1 = Convert.ToDouble(radar_angle_1);

            string radar_distance_1 = rssi[1].Substring(17, rssi[1].Length - IndexofB - 1);
            double distance_1 = Convert.ToDouble(radar_distance_1);

            string radar_angle_2 = rssi[2].Substring(14, rssi[2].Length - IndexofA - 1);
            double angle_2 = Convert.ToDouble(radar_angle_2);

            string radar_distance_2 = rssi[3].Substring(17, rssi[3].Length - IndexofB - 1);
            double distance_2 = Convert.ToDouble(radar_distance_2);

            string radar_angle_3 = rssi[4].Substring(14, rssi[4].Length - IndexofA - 1);
            double angle_3 = Convert.ToDouble(radar_angle_3);

            string radar_distance_3 = rssi[5].Substring(17, rssi[5].Length - IndexofB - 1);
            double distance_3 = Convert.ToDouble(radar_distance_3);


            if ((angle_1 == -999) || (distance_1 == -999) || (angle_2 == -999) || (distance_2 == -999) || (angle_3 == -999) || (distance_3 == -999))
            {
                SetMsg("set Radar data out spec " + radar_angle_1 + radar_distance_1 + radar_angle_2 + radar_distance_2 + radar_angle_3 + radar_distance_3, UDF.COLOR.FAIL);
                return false;
            }
            Delay(1000);
            SetMsg("Set Close Radar...", UDF.COLOR.WORK);
            if (File.Exists(Application.StartupPath + @"\\Neat_cmd\Radar_Test_QCS.bat") == false)
            {
                SetMsg(Application.StartupPath + @"\\Neat_cmd\Radar_Test_Close_QCS.bat 文件不存在", UDF.COLOR.FAIL);
                return false;
            }
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Radar_Test_Close_QCS.bat", out ack, 1);
            Application.DoEvents();
            Delay(4000);


            return true;
        }

        bool UpdateBiTime()
        {
            string sql = string.Empty;
            bool bErr = false;
            SqlDataReader rdr;
            //OracleDataReader reader;
            //SDriverX.g_modelInfo = new UDF.ModelInfo { sSn = "55ME00123456789" }; //DEBUG
            DateTime dt1 = DateTime.Now;
            SetMsg("insert burning time...", UDF.COLOR.WORK);          
            sql = string.Format("select * from BI_TIME where trid = '{0}'", SDriverX.g_modelInfo.sSn);            
            bErr = Database.Sql.ServerExecute(sql, out rdr);
            if (bErr)
            {
                rdr.Read();
                if (rdr.HasRows)
                {                   
                    //已有记录
                    if (rdr.IsClosed == false) rdr.Close();
                    sql = string.Format("update BI_TIME  set bi_time = '{0}' where trid = '{1}'",
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                        SDriverX.g_modelInfo.sSn);

                    if (Database.Sql.UpdateServer(sql) == false)
                    {
                        SetMsg("Update bi time fail!数据库操作失败(update...)", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    //无记录
                    if (rdr.IsClosed == false) rdr.Close();
                    sql = string.Format("insert into  BI_TIME (trid, bi_time) values('{0}','{1}')",
                            SDriverX.g_modelInfo.sSn,
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    if (Sql.UpdateServer(sql) == false)
                    {
                        SetMsg("Update bi time fail!数据库操作失败(insert...)", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            else
            {
                SetMsg("Update bi time fail!数据库操作失败(query...)", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool UpdateBiTime1()
        {
            string sql = string.Empty;
            bool bErr = false;
            SqlDataReader rdr;
            OracleDataReader reader;
            //SDriverX.g_modelInfo = new UDF.ModelInfo { sSn = "55ME00123456789" }; //DEBUG
            DateTime dt1 = DateTime.Now;
            SetMsg("insert burning time...", UDF.COLOR.WORK);
            sql = string.Format("select * from RKNMGR.Insp_BI_TIME where trid = '{0}'", SDriverX.g_modelInfo.sSn);
            if (Database.Oracle.ServerExecute(sql, out reader) == true)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    //已有数据
                    if (reader[0].ToString().Trim()!= string.Empty)
                    {
                        sql = "update RKNMGR.Insp_BI_TIME  set BI_TIME = to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss') where TRID = '" + SDriverX.g_modelInfo.sSn + "'";
                        if (Database.Oracle.UpdateServer(sql) == false)
                        {
                            SetMsg("Update bi time fail!数据库操作失败(更新...)", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else
                    {
                        sql = "insert into  RKNMGR.Insp_BI_TIME(TRID, BI_TIME) values( '" + SDriverX.g_modelInfo.sSn + "', to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'))";
                        if (Database.Oracle.UpdateServer(sql) == false)
                        {
                            SetMsg("INSERT bi time fail!数据库操作失败(插入...)", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                }
                else
                {
                    sql = "insert into  RKNMGR.Insp_BI_TIME(TRID, BI_TIME) values( '" + SDriverX.g_modelInfo.sSn + "', to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'))";
                    if (Database.Oracle.UpdateServer(sql) == false)
                    {
                        SetMsg("INSERT bi time fail!数据库操作失败(插入...)", UDF.COLOR.FAIL);
                        return false;
                    }
                }                        
            }
            else
            {                 
                    SetMsg("Update bi time fail!数据库操作失败(查询...)", UDF.COLOR.FAIL);
                    return false;                 
            }
            SetMsg("insert burning time ok", UDF.COLOR.WORK);
            return true;
        }
        bool WriteExtSn()
        {
            string ack, ack2, sql;
            int iRetryTime = new int();
            bool bErr = new bool();
            int flag = new int();
            OracleDataReader reader;


            SetMsg("获取特别版SN...", UDF.COLOR.WORK);

            //查询数据库
            SetMsg("正在从数据库查询已绑定的特别版SN...", UDF.COLOR.WORK);
            //先查询是否已申请
            sql = string.Format("select lcm_lv from rknmgr.insp_neatframe_data where trid = '{0}'", SDriverX.g_modelInfo.sSn);
            bErr = Database.Oracle.ServerExecute(sql, out reader);
            if (bErr)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    //已有数据
                    if (reader[0].ToString().Trim() == string.Empty)
                    {
                        //为空
                        flag = 3;
                    }
                    else
                    {
                        flag = 1;
                        SDriverX.g_modelInfo.sExtSn = reader[0].ToString().ToUpper();
                    }
                }
                else
                {
                    //没有数据
                    flag = 2;
                }
                reader.Close();
            }
            else
            {
                SetMsg("数据库操作失败(Query)", UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
        GetExtSn:
            SetMsg("请输入特别版SN", UDF.COLOR.WORK);
            if (flag == 2 || flag == 3)
            {
                InputSn inputSn = new InputSn();
                DialogResult result = inputSn.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (inputSn.sSn.Length > 3)
                    {
                        SetMsg("未输入正确的特别版SN", UDF.COLOR.FAIL);
                        if (iRetryTime > 1)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto GetExtSn;
                        }
                    }
                    else
                    {
                        SDriverX.g_modelInfo.sExtSn = inputSn.sSn;
                    }
                }
                else
                {
                    SetMsg("未输入正确的特别版SN", UDF.COLOR.FAIL);
                    if (iRetryTime > 1)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetExtSn;
                    }
                }

                //上传网络信息
                if (flag == 2)
                {
                    sql = string.Format("insert into rknmgr.insp_neatframe_data (trid, lcm_lv, datein) values('{0}', '{1}', sysdate)",
                            SDriverX.g_modelInfo.sSn,
                            SDriverX.g_modelInfo.sExtSn);
                    bErr = Database.Oracle.UpdateServer(sql);
                    if (!bErr)
                    {
                        SetMsg("数据库操作失败,插入数据库表失败", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else if (flag == 3)
                {
                    sql = string.Format("update rknmgr.insp_neatframe_data set lcm_lv = '{0}', datein = sysdate where trid = '{1}'",
                            SDriverX.g_modelInfo.sExtSn,
                            SDriverX.g_modelInfo.sSn);
                    bErr = Database.Oracle.UpdateServer(sql);
                    if (!bErr)
                    {
                        SetMsg("数据库操作失败,更新数据库表失败(update)", UDF.COLOR.FAIL);
                        return false;
                    }
                }

                //较验信息
                Delay(1000);
                //检验数据库信息
                sql = string.Format("select lcm_lv from rknmgr.insp_neatframe_data where trid = '{0}'", SDriverX.g_modelInfo.sSn);
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //有数据
                        if (reader[0].ToString().ToUpper() != SDriverX.g_modelInfo.sExtSn)
                        {
                            SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", SDriverX.g_modelInfo.sExtSn, reader[0].ToString().ToUpper()), UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else
                    {
                        //没有数据
                        SetMsg("数据库信息更新失败(Inset)", UDF.COLOR.FAIL);
                        return false;
                    }
                    reader.Close();
                }
                else
                {
                    SetMsg("数据库操作失败(校验)", UDF.COLOR.FAIL);
                    return false;
                }
            }

            iRetryTime = 0;
        WriteExtSn:
            SetMsg("写入特别版SN", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k edition -v launch_" + SDriverX.g_modelInfo.sExtSn,
                "adb shell su root nf_persist edition", out ack);
            if (ResultPick(ack, @"(?<=edition=launch_).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Read edition fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteExtSn;
                }
            }

            return true;
        }

        bool SwitchMacStyle(string macSource, UDF.MAC_STYLE mac_style, out string macTarget)
        {
            macTarget = string.Empty;
            string target = string.Empty;
            string[] MAC = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

            if (ResultPick(macSource.ToUpper(), @"([a-z A-Z 0-9]{2}([\x20:])?){5}[a-z A-Z 0-9]{2}", out target) == true)
            {
                target = target.Replace(" ", "");
                target = target.Replace(":", "");
                target = target.Trim().ToUpper();
                if (target.Length != 12)
                {
                    SetMsg("MAC匹配错误，RAW MAC:" + macSource, UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    MAC[0] = target.Substring(0, 2);
                    MAC[1] = target.Substring(2, 2);
                    MAC[2] = target.Substring(4, 2);
                    MAC[3] = target.Substring(6, 2);
                    MAC[4] = target.Substring(8, 2);
                    MAC[5] = target.Substring(10, 2);
                }
            }
            else
            {
                SetMsg("MAC匹配错误，RAW MAC:" + macSource, UDF.COLOR.FAIL);
                return false;
            }

            switch (mac_style)
            {
                case UDF.MAC_STYLE.U12:
                    macTarget = target;
                    break;
                case UDF.MAC_STYLE.L12:
                    macTarget = target.ToLower();
                    break;
                case UDF.MAC_STYLE.UC17:
                    macTarget = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]);
                    break;
                case UDF.MAC_STYLE.LC17:
                    macTarget = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]).ToLower();
                    break;
                case UDF.MAC_STYLE.UB17:
                    macTarget = string.Format("{0} {1} {2} {3} {4} {5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]);
                    break;
                case UDF.MAC_STYLE.LB17:
                    macTarget = string.Format("{0} {1} {2} {3} {4} {5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]).ToLower();
                    break;
                default:
                    macTarget = target;
                    break;
            }
            return true;
        }

        bool QueryProgIdSql(out string progid)
        {
            progid = string.Empty;
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            Sql.ServerExecute("select progid from RAKEN_SFIS..ModelList where pn = '" + SDriverX.g_modelInfo.sPart_no + "' and ForProcess =  'WBC' and (forline = 'ALL' or forline = '') order by forline desc", out rdr);
            //select progid from RAKEN_SFIS..ModelList where pn = VXXWWNF2——.NFEBAR3 and ForProcess =  'WBC' and (forline = 'ALL' or forline = '') order by forline desc
            rdr.Read();
            if (rdr.HasRows)
            {
                progid = rdr[0].ToString();
                rdr.Close();

            }
            else
            {
                //rdr.Close();//@@@ 原本mark  
                Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
                MessageBox.Show(null, "在查询数据存储的 FW 信息途中失败，没有查询到对应的 ProgID", "查询数据失败");
                return false;
            }
            return true;
        }

        bool CheckFwVerSql(string progid, string fw)
        {
            string fw1 = string.Empty;
            string fw2 = string.Empty;
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            Sql.ServerExecute("select paravalue from RAKEN_SFIS..ProgParaDetail where progid='" + progid + "' and cmdid='65' and (paraid='3' OR paraid='4')", out rdr);
            rdr.Read();
            if (rdr[0].ToString() == string.Empty)
            {
                SetMsg("警告：数据库 FW_1 为空", UDF.COLOR.WORK);
            }
            else
            {
                fw1 = rdr[0].ToString().Replace(" ", "");
                g_sFwVer1 = fw1;
            }
            rdr.Read();
            if (rdr[0].ToString() == string.Empty)
            {
                SetMsg("警告：数据库 FW_2 为空", UDF.COLOR.WORK);
            }
            else
            {
                fw2 = rdr[0].ToString().Replace(" ", "");
                g_sFwVer2 = fw2;
            }
            rdr.Close();
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
            if (fw1 == fw2 && fw1 == string.Empty)
            {
                MessageBox.Show(null, "未查询到数据库存储的 FW 信息", "查询数据失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            g_iPtInitFlag = INIT_FW;
            this.textBoxUn.Text = g_sFwVer1 + @"/" + g_sFwVer2;
            if ((fw.IndexOf(fw1) == -1) && (fw.IndexOf(fw2) == -1))
            {
                SetMsg("FW比对失败！机台FW:" + fw + "\n数据库FW:" + fw1 + "/" + fw2, UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }

        bool SetModelIndexForCardhu()
        {
            string tmp = null;

            SetMsg("Set model index...", UDF.COLOR.WORK);

            if (SDriver.SDriverX.g_modelInfo.sPart_no.Length < 12)
            {
                SetMsg("Set model index fail (query pn fail)", UDF.COLOR.FAIL);
                return false;
            }
            else if (SDriver.SDriverX.g_modelInfo.sPart_no.Length == 12)
            {
                if (SDriver.SDriverX.g_modelInfo.sPart_no == "356506020300")
                {
                    RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out tmp, 1);
                    RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out tmp, 1);

                }
                else
                {
                    SetMsg("料号未维护请通知TE/PE", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                try
                {
                    string type = SDriver.SDriverX.g_modelInfo.sPart_no.Substring(9, 1);
                    string type2 = SDriver.SDriverX.g_modelInfo.sPart_no.Substring(13, 7);
                    string ack = "";

                    if (type == "E")
                    {
                        if (type2 == "CARDHU1" || type2 == "CARDHU2")
                            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x00 0x82 i", out ack, 1);
                        else if (type2 == "CARDHU3" || type2 == "CARDHU4" || type2 == "CARDHU6" || type2 == "CARDHU5" || type2 == "CARDHU8" || type2 == "CARDHUC" || type2 == "CARDHUB" || type2 == "CARDHU7" || type2 == "CARDHU5"
                           || type2 == "CARDHUL" || type2 == "CARDHUM" || type2 == "CARDHUN")
                            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x01 0x83 i", out ack, 1);
                        else
                        {
                            SetMsg("料号未维护，请联系TE/PE。", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else if (type == "A")
                    {
                        RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out ack, 1);
                        RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out ack, 1);
                    }
                    else
                    {
                        SetMsg("料号未维护，请联系TE/PE。", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                catch
                {
                    SetMsg("Set model index fail", UDF.COLOR.FAIL);
                    return false;
                }
            }

            return true;
        }

        bool GetScalerFwForCardhu()
        {
            string ack;
            string tmp = null;
            string strFw = "", FLAG;
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_PN", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
            if (FLAG == "1")
            {
                strFw = wbc.iniFile.GetPrivateStringValue("FW_PN", SDriver.SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (strFw != "0" || strFw != "")
                {
                    strFw = wbc.iniFile.GetPrivateStringValue("FW_PN", SDriver.SDriverX.g_modelInfo.sPart_no, @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                }
                else
                {
                    SetMsg(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini，服务器没有维护料号FW", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                FLAG = wbc.iniFile.GetPrivateStringValue("FW", "flag", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                if (FLAG == "1")
                {
                    strFw = wbc.iniFile.GetPrivateStringValue("FW", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini");
                    if (strFw =="0"|| strFw =="")
                    {
                        SetMsg(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini，服务器没有维护FW", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    SetMsg(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\FW.ini，服务器没有启动FW防呆；", UDF.COLOR.FAIL);
                    return false;

                }
            }
                SetMsg("Get scaler fw version...", UDF.COLOR.WORK);
                RunAdbCmd(@"adb shell su root cat /sys/devices/soc/c1b7000.i2c/i2c-9/9-0037/fw_version", out ack);
                if (ResultCheck(ack, strFw) == false)
                {
                    SetMsg("Check scaler fw fail!规格:" + strFw + " 读取值" + ack, UDF.COLOR.FAIL);
                    return false;
                }

                return true;
            }
        }
    
}

