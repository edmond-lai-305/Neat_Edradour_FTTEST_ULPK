
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
    class QCS8550
    {
    }
}

namespace FTTEST
{
    public partial class FormMain
    {
        bool PRE_QCS8550()
        {
            // SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            int Retry = 1;
            int VOL = 15;
            if (CheckFw() == false) return false;
            if (QCS_PlaySweepAudio(true) == false) return false;
            ReMic:
            if (QCS_RecordTest() == false) return false;
            if (check_QCS_1mic_result() == false)
            {
                Retry++;
                if (Retry<3) goto ReMic;
                return false;
            }
            //if (check_QCS_4mic_result() == false) return false;
            //if (check_QCS_7mic_result() == false) return false;
            if (QCS_PlaySweepAudio(false) == false) return false;
            if (set_Verification_test() == false) return false;
            if (Setbuilddate() == false) return false;
            if (Getbuilddate() == false) return false;
            SetMsg("set vol:" + VOL.ToString(), UDF.COLOR.WORK);
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            if (QCS_WriteMac() == false) return false;
            if (SetTvSn() == false) return false;
            if (GetTvSn() == false) return false;
            if (GetTempSensorValue() == false) return false;
            if (Get_Voc_sensor_Value() == false) return false;
            //if (set_sleep_setting()==false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            if (QCS_Radar_Test() == false) return false;
            // SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool set_Verification_test()
        {
            string ack, ack2;
            int iRetryTime = 0;

            SetMsg("Set Verification to pvt...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k board_ver -v pvt", out ack);
            Delay(2000);
            Get:
            SetMsg("Get Verification pvt...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory", out ack2, 0);
            if (ResultPick(ack, @"(?<=board_ver=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get Verification fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto Get;
                }
            }

            return true;
        }
        bool QCS_WriteMac()
        {
            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;
            string tempeth = string.Empty;
            string tempeth2 = string.Empty;
            string tempbt = string.Empty;
            string tempwifi = string.Empty;
            iRetryTime = 0;
            int use_mode = 0;
            GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (QCS_GetMAC(use_mode) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }
            tempeth = sEthMac;
            WriteEthMac:
            SetMsg("set write eth mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k maceth1 -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting maceth1=" + sEthMac, out ack2) == false)
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

            use_mode = 3;
            
            SetMsg("Get Eth2 MAC...", UDF.COLOR.WORK);
            if (QCS_GetMAC(use_mode) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEth2Mac;
                }
            }
            if (SwitchMacStyle(SDriverX.g_modelInfo.AUXMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.AUXMac, UDF.COLOR.FAIL);
                return false;
            }
            tempeth2 = sEthMac;
            GetEth2Mac:
            SetMsg("set write eth2 mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k maceth2 -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting maceth2=" + sEthMac, out ack2) == false)
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
            use_mode = 1;
            
            SetMsg("Get BT MAC...", UDF.COLOR.WORK);
            if (QCS_GetMAC(use_mode) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetbtMac;
                }
            }
            if (SwitchMacStyle(SDriverX.g_modelInfo.sBtMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sBtMac, UDF.COLOR.FAIL);
                return false;
            }
            tempbt= sEthMac;
            GetbtMac:
            SetMsg("set write bt mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k macbt -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting macbt=" + sEthMac, out ack2) == false)
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
            use_mode = 2;
            
            SetMsg("Get WIFI MAC...", UDF.COLOR.WORK);
            if (QCS_GetMAC(use_mode) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetwifiMac;
                }
            }
            if (SwitchMacStyle(SDriverX.g_modelInfo.sWifiMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }
            tempwifi = sEthMac;
            GetwifiMac:
            SetMsg("set write wifi mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory -s -k macwlan -v " + sEthMac, out ack);
            if (ResultPick(ack, @"Setting macwlan=" + sEthMac, out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Write wifi MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }
            CheckEthMac:
            SetMsg("check mac...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root amt_factory", out ack, 0);
            //if (ResultPick(ack, @"maceth=" + sEthMac, out ack2) == false)
             if (ResultPick(ack, @"(?<=maceth1=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get MB sn fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            SetMsg("Read eth mac:" + ack2, UDF.COLOR.WORK);
            if (ack2 != tempeth)
            {
                SetMsg("Compare MB sn fail!", UDF.COLOR.FAIL);
                return false;
            }

            if (ResultPick(ack, @"(?<=maceth2=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get eth2 mac fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            SetMsg("Read eth2 mac:" + ack2, UDF.COLOR.WORK);
            if (ack2 != tempeth2)
            {
                SetMsg("Compare eth2 mac fail!", UDF.COLOR.FAIL);
                return false;
            }

            if (ResultPick(ack, @"(?<=macbt=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get eth2 mac fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            SetMsg("Read bt mac:" + ack2, UDF.COLOR.WORK);
            if (ack2 != tempbt)
            {
                SetMsg("Compare bt mac fail!", UDF.COLOR.FAIL);
                return false;
            }

            if (ResultPick(ack, @"(?<=macwlan=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get wifi mac fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto CheckEthMac;
                }
            }
            SetMsg("Read wifi mac:" + ack2, UDF.COLOR.WORK);
            if (ack2 != tempwifi)
            {
                SetMsg("Compare wifi mac fail!", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Get_Voc_sensor_Value()
        {
            string ack, ack2, ack3, ack4, ack5;
            int iRetryTime = new int();
            double lux = 0;
            int a = 0;
            iRetryTime = 0;
            GetSensorValue:
            RunAdbCmd("DEL" + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack, 0);
            SetMsg("set voc sensor Value...", UDF.COLOR.WORK);
            string filePath = Directory.GetCurrentDirectory() + @"\Neat_cmd\Get_Voc_Value.bat";
            if (File.Exists(filePath)==false)
            {
                SetMsg(Directory.GetCurrentDirectory() + @"\Neat_cmd\Get_Voc_Value.bat 路径下文件不存在", UDF.COLOR.FAIL);
                return false;
            }
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Get_Voc_Value.bat", out ack, 1);
            Application.DoEvents();
            Delay(1000);
            SetMsg("Get vaule result...", UDF.COLOR.WORK);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\data_command_ack.txt", out ack2);

            try
            {
                lux = Convert.ToDouble(ack2);
                if (lux < 1)
                {
                    SetMsg("VOC data out of range:" + lux, UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                SetMsg("VOM PASS,dac: " + lux, UDF.COLOR.WORK);
            }
            catch
            {
                if (iRetryTime > 4)
                {
                    SetMsg("Read voc dac: " + lux + " Fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Delay(1000);
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            return true;
        }
        bool GetTempSensorValue()
        {
            string ack, ack2, ack3, ack4, ack5;
            int iRetryTime = new int();
            double lux = 0;
            int a = 0;
            iRetryTime = 0;
            GetSensorValue:
            ack = "";
            SetMsg("Get Temp Humi sensor Value...", UDF.COLOR.WORK);
            RunAdbCmd("adb.exe shell am startservice -n com.amtran.factory/.FactoryService --es Action GetTempHumidity", out ack);
            Application.DoEvents();
            Delay(2500);
            SetMsg("Get Temp Humi sensor  result...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell cat /sdcard/Download/data_command_ack.txt", out ack, 0);
            if (ResultPick(ack, @"(?<=Temp=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get Temp Humi sensor fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            try
            {
                lux = Convert.ToDouble(ack2);
                if (lux < 1 || lux > 60)
                {
                    SetMsg("Temp data out of range: " + lux, UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                SetMsg("Temp PASS,dac: " + lux, UDF.COLOR.WORK);
            }
            catch
            {
                if (iRetryTime > 4)
                {
                    SetMsg("Read Temp dac: " + lux + " Fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Delay(1000);
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            if (ResultPick(ack, @"(?<=Humidity=).*?(?=\r|\n)", out ack3) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get Humi  fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            try
            {
                lux = Convert.ToDouble(ack3);
                if (lux < 5 || lux > 100)
                {
                    SetMsg(string.Format("Humi data out of range"), UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                SetMsg("Humi PASS,dac: " + lux, UDF.COLOR.WORK);
            }
            catch
            {
                if (iRetryTime > 4)
                {
                    SetMsg("Read Humi dac: " + lux + " Fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Delay(1000);
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }
            return true;
        }

        bool QCS_RecordTest()
        {
            string ack;

            SetMsg("1 Mic Channel Recording test,Please watting ...", UDF.COLOR.WORK);
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Record_audio_1_QCS.bat", out ack, 1);
            Application.DoEvents();
            Delay(500);

            SetMsg("1 Mic Channel Recording test,Please watting ...", UDF.COLOR.WORK);
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Record_audio_1_QCS.bat", out ack, 1);
            Application.DoEvents();
            //SetMsg("4 Mic Channel Recording test,Please watting ...", UDF.COLOR.WORK);
            //RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Record_audio_4_QCS.bat", out ack, 1);
            //Application.DoEvents();
            //Delay(1000);

            //SetMsg("7 Mic Channel Recording test,Please watting ...", UDF.COLOR.WORK);
            //RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\Record_audio_7_QCS.bat", out ack, 1);

            //Delay(1000);
            return true;
        }

        bool QCS_PlaySweepAudio(bool flag)
        {
            string ack;
            int VOL = 0;


            SetMsg("copy audio wav to ,Please watting ...", UDF.COLOR.WORK);
            RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\insert_onetwo.bat", out ack, 1);
            Application.DoEvents();
            Delay(500);
            if (flag == true)
            {

                SetMsg("set VOL TO 10..", UDF.COLOR.WORK);
                RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetMediaVolume --ei mediaVolume 7", out ack);
                Delay(1000);
                SetMsg("Play Sweep audio on...", UDF.COLOR.WORK);
                RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\play_Audio_ON_QCS.bat", out ack, 1);
                Application.DoEvents();
                Delay(3000);
            }
            else
            {
                SetMsg("Play Sweep audio off...", UDF.COLOR.WORK);
                RunAdbCmdMIC(Directory.GetCurrentDirectory() + @"\Neat_cmd\play_Audio_OFF_QCS.bat", out ack, 1);
            }

            return true;
        }

        bool check_QCS_4mic_result()
        {
            string ack, ack2;
            string result = string.Empty;
            int iRetryTime = 0;
            string[] MaxLevel;
            string[] MinLevel;
            string[] PkLevel;

            Single g_Micdb = -65;
            SetMsg("auto analyze 4 mic.wav...", UDF.COLOR.WORK);
            GetCmd:
            RunAdbCmd("DEL" + Directory.GetCurrentDirectory() + @"\sox.txt", out ack, 0);
            RunAdbCmd("sox.exe tx_4ch.wav -n stats  2>sox.txt", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\sox.txt", out ack2);
            if (ResultCheck(ack2, @"Num samples") == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get analyze 4 mic fail!", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto GetCmd;
                }
            }
            //string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
            try
            {
                result = MidStrEx(ack2, "Min level", "Max level");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                result = Regex.Replace(result, @"\s+", "|");

                MinLevel = result.ToString().Split('|');

                result = MidStrEx(ack2, "Max level", "Pk lev dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                //result = result.ToString().Trim().Replace(" ", "");
                MaxLevel = result.ToString().Split(' ');

                result = MidStrEx(ack2, "RMS lev dB", "RMS Pk dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                result = Regex.Replace(result, @"\s+", "|");
                PkLevel = result.ToString().Split('|');

                if (Convert.ToSingle(MinLevel[1]) == Convert.ToSingle(MaxLevel[2]) || Convert.ToSingle(MinLevel[2]) == Convert.ToSingle(MaxLevel[4]) || Convert.ToSingle(MinLevel[4]) == Convert.ToSingle(MaxLevel[8]))
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 4mic data fail:" + MinLevel[1] + MaxLevel[2] + MinLevel[2] + MaxLevel[4] + MinLevel[3] + MaxLevel[6] + MinLevel[4] + MaxLevel[8] + MinLevel[5] + MaxLevel[10], UDF.COLOR.FAIL);
                    return false;
                }
                if (Convert.ToSingle(MaxLevel[2]) == 0 || Convert.ToSingle(MaxLevel[4]) == 0 || Convert.ToSingle(MaxLevel[6]) == 0 || Convert.ToSingle(MaxLevel[8]) == 0)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 4mic data MaxLevel is Zero fail:" + MaxLevel[2] + MaxLevel[4] + MaxLevel[6] + MaxLevel[8], UDF.COLOR.FAIL);

                    return false;
                }
                else
                {
                    if (Convert.ToSingle(MaxLevel[2]) >= 1 || Convert.ToSingle(MaxLevel[4]) >= 1 || Convert.ToSingle(MaxLevel[8]) >= 1)
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 4mic data MaxLevel is out >=1:" + MaxLevel[2] + MaxLevel[4] + MaxLevel[6] + MaxLevel[8], UDF.COLOR.FAIL);

                        return false;
                    }
                }
                if (Convert.ToSingle(PkLevel[1]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[2]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[3]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[4]) < Convert.ToSingle(g_Micdb))
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 4mic db data fail:" + PkLevel[1] + PkLevel[2] + PkLevel[3] + PkLevel[4] + " <spec： " + g_Micdb, UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    if (Convert.ToSingle(PkLevel[1]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[2]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[3]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[4]) < Convert.ToSingle(g_Micdb))
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 4mic db data fail:" + PkLevel[1] + PkLevel[2] + PkLevel[3] + PkLevel[4] + " >spec： " + g_Micdb, UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                SetMsg("get tx_4ch Retry:", UDF.COLOR.WORK);
                iRetryTime++;
                if (iRetryTime < 3) goto GetCmd;
                SetMsg("get tx_4Ch Fail:", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool check_QCS_7mic_result()
        {
            string ack, ack2;
            string result = string.Empty;
            int iRetryTime = 0;
            string[] MaxLevel;
            string[] MinLevel;
            string[] PkLevel;

            Single g_Micdb = -65;
            SetMsg("auto analyze 7 mic.wav...", UDF.COLOR.WORK);
            GetCmd:
            RunAdbCmd("DEL" + Directory.GetCurrentDirectory() + @"\sox.txt", out ack, 0);
            RunAdbCmd("sox.exe tx_7ch.wav -n stats  2>sox.txt", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\sox.txt", out ack2);
            if (ResultCheck(ack2, @"Num samples") == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get analyze 7 mic fail!", UDF.COLOR.FAIL);

                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto GetCmd;
                }
            }
            //string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
            try
            {
                result = MidStrEx(ack2, "Min level", "Max level");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                //MinLevel = result.ToString().Split(' ');
                result = Regex.Replace(result, @"\s+", "|");
                MinLevel = result.ToString().Split('|');

                result = MidStrEx(ack2, "Max level", "Pk lev dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                //result = result.ToString().Trim().Replace(" ", "");
                result = Regex.Replace(result, @"\s+", "|");
                MaxLevel = result.ToString().Split('|');

                result = MidStrEx(ack2, "RMS lev dB", "RMS Pk dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                result = Regex.Replace(result, @"\s+", "|");
                PkLevel = result.ToString().Split('|');

                if (Convert.ToSingle(MinLevel[1]) == Convert.ToSingle(MaxLevel[1]) || Convert.ToSingle(MinLevel[2]) == Convert.ToSingle(MaxLevel[2]) || Convert.ToSingle(MinLevel[3]) == Convert.ToSingle(MaxLevel[3]) || Convert.ToSingle(MinLevel[4]) == Convert.ToSingle(MaxLevel[4]) || Convert.ToSingle(MinLevel[5]) == Convert.ToSingle(MaxLevel[5]) || Convert.ToSingle(MinLevel[6]) == Convert.ToSingle(MaxLevel[6]) || Convert.ToSingle(MinLevel[7]) == Convert.ToSingle(MaxLevel[7]) || Convert.ToSingle(MinLevel[8]) == Convert.ToSingle(MaxLevel[8]))
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 7mic data fail:" + MinLevel[1] + MaxLevel[2] + MinLevel[2] + MaxLevel[4] + MinLevel[3] + MaxLevel[6] + MinLevel[4] + MaxLevel[8] + MinLevel[5] + MaxLevel[10], UDF.COLOR.FAIL);
 
                    return false;
                }
                if (Convert.ToSingle(MaxLevel[1]) == 0 || Convert.ToSingle(MaxLevel[2]) == 0 || Convert.ToSingle(MaxLevel[3]) == 0 || Convert.ToSingle(MaxLevel[4]) == 0 || Convert.ToSingle(MaxLevel[5]) == 0 || Convert.ToSingle(MaxLevel[6]) == 0 || Convert.ToSingle(MaxLevel[7]) == 0 || Convert.ToSingle(MaxLevel[8]) == 0)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 7mic data MaxLevel is Zero fail:" + MaxLevel[2] + MaxLevel[4], UDF.COLOR.FAIL);

                    return false;
                }
                else
                {
                    if (Convert.ToSingle(MaxLevel[1]) >= 1 || Convert.ToSingle(MaxLevel[2]) >= 1 || Convert.ToSingle(MaxLevel[3]) >= 1 || Convert.ToSingle(MaxLevel[8]) >= 1 || Convert.ToSingle(MaxLevel[4]) >= 1 || Convert.ToSingle(MaxLevel[5]) >= 1 || Convert.ToSingle(MaxLevel[6]) >= 1 || Convert.ToSingle(MaxLevel[7]) >= 1 || Convert.ToSingle(MaxLevel[8]) >= 1)
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 7mic data MaxLevel is Zero fail:" + MaxLevel[2] + MaxLevel[4], UDF.COLOR.FAIL);
     
                        return false;
                    }
                }
                if (Convert.ToSingle(PkLevel[1]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[2]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[3]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[4]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[5]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[6]) < Convert.ToSingle(g_Micdb) || Convert.ToSingle(PkLevel[7]) < Convert.ToSingle(g_Micdb))
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 7mic db data fail:" + PkLevel[1] + PkLevel[2] + PkLevel[3] + PkLevel[4] + PkLevel[5] + PkLevel[6] + PkLevel[7] + " <spec： " + g_Micdb, UDF.COLOR.FAIL);
 
                    return false;
                }
            }
            catch (Exception exp)
            {
                SetMsg("get tx_7ch Retry:", UDF.COLOR.WORK);
                iRetryTime++;
                if (iRetryTime < 3) goto GetCmd;
                SetMsg("get tx_7ch Fail:", UDF.COLOR.FAIL);

                return false;
            }
            return true;
        }
        bool check_QCS_1mic_result()
        {
            string ack, ack2;
            string result = string.Empty;
            int iRetryTime = 0;
            string[] MaxLevel;
            string[] MinLevel;
            string[] PkLevel;

            Single g_Micdb = -65;
            SetMsg("auto analyze 1 mic.wav...", UDF.COLOR.WORK);
            GetCmd:
            RunAdbCmd("DEL" + Directory.GetCurrentDirectory() + @"\sox.txt", out ack, 0);
            RunAdbCmd("sox.exe tx_1ch.wav -n stats  2>sox.txt", out ack, 1);
            Delay(500);
            RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\sox.txt", out ack2);
            if (ResultCheck(ack2, @"Num samples") == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get auto analyze 1 data fail!", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto GetCmd;
                }
            }
            //string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
            try
            {
                result = MidStrEx(ack2, "Min level", "Max level");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                result = Regex.Replace(result, @"\s+", "|");

                MinLevel = result.ToString().Split('|');

                result = MidStrEx(ack2, "Max level", "Pk lev dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                //result = result.ToString().Trim().Replace(" ", "");
                MaxLevel = result.ToString().Split(' ');

                result = MidStrEx(ack2, "RMS lev dB", "RMS Pk dB");
                result = result.ToString().Trim().Replace("\r", "").Replace("\n", "");
                result = Regex.Replace(result, @"\s+", "|");
                PkLevel = result.ToString().Split('|');

                if (Convert.ToSingle(MinLevel[1]) == Convert.ToSingle(MaxLevel[2]) )
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 1mic data fail:" + MinLevel[1] + MaxLevel[2] + MinLevel[2] + MaxLevel[4] , UDF.COLOR.FAIL);
                    return false;
                }
                if (Convert.ToSingle(MaxLevel[2]) == 0 )
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 1mic data MaxLevel is Zero fail:" + MaxLevel[2] + MaxLevel[4] + MaxLevel[6] + MaxLevel[8], UDF.COLOR.FAIL);

                    return false;
                }
                else
                {
                    if (Convert.ToSingle(MaxLevel[2]) >= 1 )
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 1mic data MaxLevel is out >=1:" + MaxLevel[2] , UDF.COLOR.FAIL);

                        return false;
                    }
                    else if (Convert.ToSingle(MaxLevel[2]) <0.01 )
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 1mic data MaxLevel is out <0.01:" + MaxLevel[2] , UDF.COLOR.FAIL);

                        return false;
                    }
                }
                if (Convert.ToSingle(PkLevel[1]) < Convert.ToSingle(g_Micdb) )
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("get 1mic db data fail:" + PkLevel[1] + PkLevel[2]  + " <spec： " + g_Micdb, UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    if (Convert.ToSingle(PkLevel[1]) > 0 )
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 1mic db data fail:" + PkLevel[1] + PkLevel[2]  + " >spec：0" , UDF.COLOR.FAIL);
                        return false;
                    }
                    else if (Convert.ToSingle(PkLevel[1]) == Convert.ToSingle(PkLevel[2]) )
                    {
                        SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                        SetMsg("get 1mic db data fail:" + PkLevel[1] + PkLevel[2] + " >spec：0", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                SetMsg("get tx_1ch Retry:", UDF.COLOR.WORK);
                iRetryTime++;
                if (iRetryTime < 3) goto GetCmd;
                SetMsg("get tx_1Ch Fail:", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }


        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;

            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {
                //string Log.WriteLog("MidStrEx Err:" + ex.Message);
            }
            return result;
        }
    }
}
