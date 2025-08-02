using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTTEST;
using FTTEST.UDFs;
using FTTEST.AppConfig;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
//using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using FTTEST.Database;
using FTTEST.SDriver;
using wbc;

namespace FTTEST.UDFs
{
    public class API { }

}

namespace FTTEST
{
    public partial class FormMain
    {
        /// <summary>
        /// 延时函数,单位毫秒,精度 40毫秒
        /// </summary>
        /// <param name="milliSecond">要延时的毫秒数</param>
        public static void Delay(int milliSecond)
        {
            if (milliSecond <= 0) return;
            int i = new int();
            int j = (int)(milliSecond / 40);
            do
            {
                i++;
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(40);
            } while (i < j);
        }
        /// <summary>
        /// 写log文件
        /// </summary>
        /// <param name="fileName">文件名(不包含路径)</param>
        /// <param name="text">文本信息</param>
        public static void WriteLog(string fileName, string text)
        {
            StreamWriter sWrite = new StreamWriter(Application.StartupPath + @"\" + fileName, true);
            sWrite.WriteLine(text);
            sWrite.Close();
        }
        /// <summary>
        /// 清空流程信息显示
        /// </summary>
        private void ClearMsg()
        {
            listBoxSetup.Items.Clear();
            return;
        }
        /// <summary>
        /// 消息通知函数，更新界面提示
        /// </summary>
        /// <param name="msg">要显示的提示信息</param>
        /// <param name="col">自定义的工作颜色</param>
        private void SetMsg(string msg, UDF.COLOR col)
        {
            if (!Directory.Exists("Log"))
            {
                Directory.CreateDirectory("Log");
            }
            string logPath = "Log\\" + g_runMode + "_" + g_sLogFileName;
            string time = System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");

            msg = msg ?? "[空消息。这可能是程序的1个bug或错误，请向 TE 报告此问题。]\r\n[Tin nhắn trống. Đây có thể là 1 lỗi (bug) trong chương trình, vui lòng báo cáo sự cố này cho TE.]";
            if (col == UDF.COLOR.FAIL)  //记录失败记录到本地 log文件
            {
                WriteLog(logPath, time + "\t" + textBoxSn.Text.Trim().ToUpper() + "\tFAIL\tMSG(" + msg.Trim() + ")");
            }
            else if (col == UDF.COLOR.PASS)
            {
                WriteLog(logPath, time + "\t" + textBoxSn.Text.Trim().ToUpper() + "\tPASS\tTestTime(" + g_iTimerCycle + ")");
            }
            else
            {
                WriteLog(logPath, time + "\t" + textBoxSn.Text.Trim().ToUpper() + "\tMSG(" + msg.Trim() + ")");
            }

            if (msg.Length > 16)    //调节信息提示框字体大小，根据内容量
            {
                labelMsgtip.Font = new System.Drawing.Font("宋体", g_fHeightScaling * 14F, System.Drawing.FontStyle.Bold);
            }
            else
            {
                labelMsgtip.Font = new System.Drawing.Font("宋体", g_fHeightScaling * 26F, System.Drawing.FontStyle.Bold);
            }
            labelMsgtip.Text = msg;
            switch (col)
            {
                case UDF.COLOR.PASS:
                    this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(255)))), ((int)(((byte)(136)))));
                    break;
                case UDF.COLOR.FAIL:
                    this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
                    break;
                case UDF.COLOR.WORK:
                    this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
                    break;
                case UDF.COLOR.WORK2:
                    this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(73)))), ((int)(((byte)(164)))));
                    break;
                case UDF.COLOR.WARN:
                    this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(14)))));
                    break;
                default:
                    break;
            }
            //if (listBoxSetup.Items.Count > 100)
            //{
            //    listBoxSetup.Items.Clear();
            //}
            listBoxSetup.Items.Add((listBoxSetup.Items.Count + 1) + ": " + msg);
            listBoxSetup.SelectedIndex = listBoxSetup.Items.Count - 1;
            return;
        }
        //===============
        void savelogs(string log)
        {
            //string path = "D:\\Users\\Confidence\\Testing\\";
            string strFile = "";

            // if (!string.IsNullOrWhiteSpace(SDriverX.g_modelInfo.sSn))

            if (logFlag == true)
            {
                logFlag = false;
                //string date = DateTime.Now.ToString("yyyyMMdd") + ".txt";   //鏃ュ織鏂囦欢鍚庣紑鍚嶏紝渚?*20171101.txt
                strLogFileName = DateTime.Now.ToString("yyyyMMddHHmmss");   //鏃ュ織鏂囦欢鍚庣紑鍚嶏紝渚?*20171101
                if (!string.IsNullOrWhiteSpace(SDriverX.g_modelInfo.sSn))
                {
                    strLogFileName = strLogFileName + "_" + textBoxSn.Text;
                }
            }


            // }
            //else
            //{
            //  return;
            //}

            strLogFileName = strLogFileName.Replace("/", "");

            // DateTime tDate = DateTime.Now;
            // DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (!Directory.Exists(@"C:\\logs\Edradour\"))
            {
                Directory.CreateDirectory(@"C:\\logs\Edradour\");
            }




            log = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + log;

            System.IO.File.AppendAllText(@"C:\\logs\Edradour\" + strLogFileName + ".txt", "\r\n" + log);



        }
        //===============
        /// <summary>
        /// 逻辑十六进制字符串 至 二进制命令转换
        /// 自动测算数据长度
        /// </summary>
        /// <param name="str">字符串命令</param>
        private void StrToCmd(string str)
        {
            string strBuf = string.Empty;
            int len = new int();

            strBuf = str.Replace(" ", "");
            len = Convert.ToInt32("0x" + strBuf.Substring(4, 2), 16);

            for (int i = 0; i < len - 1; i++)
            {
                g_byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }
        /// <summary>
        /// 逻辑十六进制字符串 至 二进制命令转换
        /// </summary>
        /// <param name="str">字符串命令</param>
        /// <param name="len">命令字节长度</param>
        private void StrToCmd(string str, int len)
        {
            string strBuf = string.Empty;
            strBuf = str.Replace(" ", "");
            for (int i = 0; i < len - 1; i++)
            {
                g_byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }
        /// <summary>
        /// 串口发送与接收函数
        /// </summary>
        /// <param name="cmd">包含指令的字节数组</param>
        /// <param name="len">命令字节长度</param>
        /// <param name="cycleDelay">延时参数,50毫秒的倍数</param>
        /// <param name="rtnLen">指定接收到的回传值字节长度</param>
        /// <param name="rtn">接收到的回传值数组</param>
        /// <returns></returns>
        private bool WriteRs232CommandSB(byte[] cmd, int len, int cycleDelay, int rtnLen, out byte[] rtn)
        {
            textBoxAck.Text = "";   //清空 ACK 框内容
            rtn = null;
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;
            g_bPortErr = false;

            int i = new int();
            int j = new int();
            int retryCount = new int();
            byte[] cmdBuf = new byte[len];
            byte[] tmp = new byte[256];

            if (serialPortTV.IsOpen == false)
            {
                try
                {
                    serialPortTV.Open();
                }
                catch (Exception e)
                {
                    g_bCmdWorkFlag = false;
                    g_bPortErr = true;
                    MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            for (i = 0; i < len; i++)
            {
                cmdBuf[i] = cmd[i];
            }
            cmdBuf[len - 1] = 0;
            j = 0;
            //calc checksum
            for (i = 0; i < len - 1; i++)
            {
                j = j ^ (int)cmdBuf[i];
            }
            cmdBuf[len - 1] = (byte)j;

            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < len; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", cmdBuf[i]);
            }
            textBoxAck.Text = g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框;

            serialPortTV.DiscardInBuffer();
            serialPortTV.DiscardOutBuffer();
            serialPortTV.WriteTimeout = 5000;
            serialPortTV.ReadTimeout = 5000;
            try
            {
                serialPortTV.Write(cmdBuf, 0, len); //write cmd
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "写入串口指令失败\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            i = Convert.ToInt32(comboBoxCmdDelay.SelectedItem.ToString());
            Delay(i);
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(20);

            try
            {
                do
                {
                    retryCount += 1;
                    if (retryCount > 100 + cycleDelay)
                    {
                        if (serialPortTV.IsOpen == true) serialPortTV.Close();
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(40);
                    if (g_iCmdDelay != 0)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Delay(g_iCmdDelay);
                        g_iCmdDelay = 0;
                    }
                } while (serialPortTV.BytesToRead < rtnLen);
                serialPortTV.Read(tmp, 0, rtnLen);  //get ack
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "Get ACK Fail\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            rtn = tmp;
            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < rtnLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", tmp[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();
            textBoxAck.Text = g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        /// <summary>
        /// 如果网络上的程序比当前版本新,则自动更新程序
        /// </summary>
        private void AutoUpdate()
        {
            try
            {
                //网络位置定义
                const string sNetExePath = @"\\10.2.100.27\te\SETUP\Neat\FTTEST\FTTEST.exe";

                //获得当前程序信息
                Process cur = Process.GetCurrentProcess();
                FileInfo fi = new FileInfo(cur.MainModule.FileName);
                string MM=System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString() ;
                // MessageBox.Show(MM);
                MM=iniFile.Right(MM,10).ToUpper();
                //获得网络位置程序信息
                if (MM!= "FTTEST.EXE")
                {
                    return ;
                }

                FileInfo fi2 = new FileInfo(sNetExePath);
                System.TimeSpan ND = fi2.LastWriteTime - fi.LastWriteTime;
                //比较文件修改日期信息
                //如果网络上的文件比当前文件新,则...
                if (ND.Hours != 0 || ND.Minutes != 0)
                {
                    try
                    {
                        Process subPro = new Process();
                        subPro.StartInfo.UseShellExecute = true;
                        subPro.StartInfo.FileName = "cmd";
                        subPro.StartInfo.Arguments = " /q /c echo 请勿中断,正在更新程序... & ping -n 3 127.1 1>nul 2>nul & taskkill /pid "
                            + cur.Id + " 1>nul 2>nul & copy /y \""
                            + sNetExePath + "\" \""
                            + cur.MainModule.FileName
                            + "\" 1>nul 2>nul && start \"\" /max \""
                            + cur.MainModule.FileName + "\"";
                        subPro.StartInfo.CreateNoWindow = false;
                        subPro.Start();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Close();
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 重启程序
        /// </summary>
        private void Restart()
        {
            try
            {
                //获得当前程序信息
                Process cur = Process.GetCurrentProcess();
                FileInfo fi = new FileInfo(cur.MainModule.FileName);

                try
                {
                    Process subPro = new Process();
                    subPro.StartInfo.UseShellExecute = true;
                    subPro.StartInfo.FileName = "cmd";
                    subPro.StartInfo.Arguments = " /q /c echo 请勿中断,正在重启程序... & ping -n 3 127.1 1>nul 2>nul & taskkill /pid "
                        + cur.Id + " 1>nul 2>nul & start \"\" /max \""
                        + cur.MainModule.FileName + "\"";
                    subPro.StartInfo.CreateNoWindow = false;
                    subPro.Start();
                }
                catch
                {

                }
                finally
                {
                    Close();
                }
            }
            catch
            {

            }
        }
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
       * 串口设置函数
       * 例如：InitPort (TvPort, 2, "115200,N,8,1")
       * --------------------------------------------------*/
        /// <summary>
        /// 串口初始化函数
        /// </summary>
        /// <param name="SerPort">SerialPort</param>
        /// <param name="com">串口号</param>
        /// <param name="setting">串口设置</param>
        private void InitPort(System.IO.Ports.SerialPort SerPort, int com, string setting)
        {
            string strBuf = string.Empty;

            SerPort.PortName = "COM" + com; //设置 COM 口
            strBuf = Regex.Match(setting, @"^\d+(?=,)").ToString().Trim();
            if (strBuf != string.Empty)
            {
                SerPort.BaudRate = Convert.ToInt32(strBuf); //设置 BaudRate
            }
            strBuf = Regex.Match(setting, @"(?<=^\d+,)[a-z A-Z]+").ToString().Trim();
            if (strBuf != string.Empty) //设置 Parity
            {
                switch (strBuf)
                {
                    case "E":
                        SerPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "M":
                        SerPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case "N":
                        SerPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "O":
                        SerPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case "S":
                        SerPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    default:
                        break;
                }
            }
            strBuf = Regex.Match(setting, @"(?<=[a-z A-Z]+,)\d+").ToString().Trim();
            if (strBuf != string.Empty)
            {
                SerPort.DataBits = Convert.ToInt32(strBuf); //设置 DataBits
            }
            strBuf = Regex.Match(setting, @"(?<=,)[0-9 \.]+$").ToString().Trim();
            if (strBuf != string.Empty) //设置 StopBits
            {
                switch (strBuf)
                {
                    case "0":
                        SerPort.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case "1":
                        SerPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case "1.5":
                        SerPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "2":
                        SerPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        SerPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                }
            }
        }
        /// <summary>
        /// 根据配置文件初始化串口设置
        /// </summary>
        void InitSerPort()
        {
            if (GlobalConfig.sTvEnable != "N")
            {
                InitPort(serialPortTV, GlobalConfig.iTvPort, GlobalConfig.sTvSettings);
                try
                {
                    if (!serialPortTV.IsOpen)
                    {
                        serialPortTV.Open();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (GlobalConfig.sGdmEnable != "N")
            {
                InitPort(serialPortGDM, GlobalConfig.iGdmPort, GlobalConfig.sGdmSettings);
            }
            if (GlobalConfig.sIOCardEnable != "N")
            {
                InitPort(serialPortIOCard, GlobalConfig.iIOCardPort, GlobalConfig.sIOCardSettings);
            }
            if (GlobalConfig.sCp310Enable != "N")
            {
                InitPort(serialPortCp310, GlobalConfig.iCp310Port, GlobalConfig.sCp310Settings);
            }
            if (GlobalConfig.sMhlEnable != "N")
            {
                InitPort(serialPortMHL, GlobalConfig.iMhlPort, "115200,N,8,1");
            }
            if (GlobalConfig.sDoorEnable != "N")
            {
                InitPort(serialPortDoor, GlobalConfig.iDoorPort, "9600,N,8,1");
            }
        }
        bool StrToSn(string strSrc, out string sn)
        {
            sn = string.Empty;

            if (ResultCheck(strSrc, @"\d+/\d+"))    //SN -> SN
            {
                sn = strSrc;
            }
            else if (ResultCheck(strSrc, @"S12.*"))  // MN -> SN
            {
                string sql = string.Empty;
                bool bErr = new bool();
                OracleDataReader reader;

                sql = string.Format("select productname from consumable t where t.consumabletype = 'MBDMN' and t.consumablename =  '{0}'",
                    strSrc
                    );
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        sn = reader[0].ToString();
                    }
                    else
                    {
                        //没有数据
                        SetMsg("MN索引失败，请扫 SN 作业.", UDF.COLOR.FAIL);
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

                if (sn.Length == 0)
                {
                    SetMsg("MN索引失败，请扫 SN 作业.", UDF.COLOR.FAIL);
                    return false;
                }
            }

            return true;
        }
        bool SnMnCheck()
        {
            int retryTime = 0;
            Match match = null;

        CheckMn:
            SetMsg("Read and check mn...", UDF.COLOR.WORK);
            if (WriteRs232CommandS12(true, "MSTC_MNRead", 0) == false)
            {
                if (retryTime > 1)
                {
                    SetMsg("Write mn fail，请检查通讯线连接状况！", UDF.COLOR.FAIL);
                    return false;
                }
                retryTime++;
                goto CheckMn;
            }
            match = Regex.Match(g_sCmdRtnStrText, @"(?<=MSTC_R_MN_)[A-Za-z0-9]{1,}(?=\r\n)"); //old regular:(?<=MSTC_R_MN_)(\d{5}/\d{9})
            if (match.Success)
            {
                SetMsg("Read mn:" + match.Value, UDF.COLOR.WORK);

                //查询标签SN对应MN
                string sql = string.Empty;
                bool bErr = new bool();
                OracleDataReader reader;

                SetMsg("正在从数据库索引SN绑定的MN信息...", UDF.COLOR.WORK);
                //先查询是否已申请
                sql = string.Format("select consumablename from consumable t where t.consumabletype = 'MBDMN' and t.productname = '{0}'",
                    SDriver.SDriverX.g_modelInfo.sSn
                    );
                bErr = Database.Oracle.ServerExecute(sql, out reader);
                if (bErr)
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //已有数据
                        SDriver.SDriverX.g_modelInfo.sMn = reader[0].ToString();
                    }
                    else
                    {
                        //没有数据
                        SetMsg("没有从数据库检索到MN，请通知TE.", UDF.COLOR.FAIL);
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

                //比对MN
                SetMsg("Check mn...", UDF.COLOR.WORK);
                if (match.Value != SDriver.SDriverX.g_modelInfo.sMn)
                {
                    SetMsg("MN 读取值与SN绑定的MN不符!可能是标签机台不符", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                SetMsg(@"Read mn fail,未匹配到MN,匹配规则：(?<=MSTC_R_MN_)(\d{5}/\d{9})", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool CheckTRID_ForNeatframeModel(string sn)
        {
            string sql = string.Empty;

            if (GlobalConfig.sMesEid == "1AFWUD9061")
            {
                GlobalConfig.sMesEid = "1AFWUD9061";
            }

            SDriverX.g_modelInfo = new UDF.ModelInfo
            {
                sSn = sn,
                sModelName = "Neatframe Model"
            };

            OracleDataReader rdr = null;
            bool bErr = new bool();
            SetMsg("检查FW_UPGRADE站过站记录...", UDFs.UDF.COLOR.WORK);
            if (SDriverX.STCK(GlobalConfig.sMesEid, sn, "TestUser") == false)
            {
                SetMsg(SDriverX.g_modelInfo.sErr_msg, UDF.COLOR.WORK);
                if (SDriverX.g_modelInfo.sErr_msg.Contains("ReSet"))
                {
                    SetMsg(SDriverX.g_modelInfo.sErr_msg + ",请返回[Reset]站。", UDF.COLOR.FAIL);
                    return false;
                }
            }
            SetMsg("检查SN信息...", UDFs.UDF.COLOR.WORK);
            //bErr = Database.Oracle.ServerExecute("select PRODUCTSPECNAME, PRODUCTREQUESTNAME from product where SET_SERIAL_NO = '" + sn + "'", out rdr);
            bErr = SDriverX.STCK(GlobalConfig.sMesEid, sn, "TestUser");
            if (bErr)
            {
                if (SDriverX.g_modelInfo.sPart_no == string.Empty)
                {
                    SetMsg("SN信息为空！KPPN未作业，请返回[KPPN]站。", UDF.COLOR.FAIL);
                    return false;
                }
                if (SDriverX.g_modelInfo.sWord == string.Empty)
                {
                    SetMsg("SN信息为空！KPPN未作业，请返回[KPPN]站。", UDF.COLOR.FAIL);
                    return false;
                }
                textBoxPn.Text = SDriverX.g_modelInfo.sPart_no;   
            }
            else
            {
                SetMsg("数据库操作失败！(检查网络连接或重启程序)", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 流程卡验证函数,验证当前机台序列号所在站别是否在本站
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        ///
        UDF.ModelInfo mod = new UDF.ModelInfo();
        bool PreCheckTRID(string sn)
        {
            if (sn.Length > 13)
            {
                SetMsg("SN长度超出范围 SN Độ dài vượt quá phạm vi！", UDF.COLOR.FAIL);
                return false;
            }
            
            if (g_runMode >= UDF.RunMode.FTA && g_runMode <= UDF.RunMode.RST)
            {
                SetMsg("流程卡验证...", UDF.COLOR.WORK);

                if (SDriverX.STCK(GlobalConfig.sMesEid, sn, "TestUser"))
                {
                    //SetMsg(SDriverX.g_modelInfo.sPart_no, UDF.COLOR.WORK);
                    //SetMsg(textBoxPn.Text, UDF.COLOR.WORK);
                    //SetMsg(g_runMode.ToString(),UDF.COLOR.WORK);
                    if ((SDriverX.g_modelInfo.sPart_no != textBoxPn.Text) && (textBoxPn.Text != string.Empty) && g_runMode.ToString() != "WBC")
                    {
                        DialogResult UserSelete = MessageBox.Show(null, "是否切换到工单[" + SDriver.SDriverX.g_modelInfo.sWord + "]？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        if (UserSelete != DialogResult.Yes)
                        {
                            SetMsg("流程卡验证失败！用户否定了切换工单", UDF.COLOR.FAIL);
                            return false;
                        }
                        else
                        {
                            g_iPtInitFlag = INIT_ZERO;
                        }
                    }
                    textBoxModel.Text = SDriverX.g_modelInfo.sModel;
                    textBoxPn.Text = SDriverX.g_modelInfo.sPart_no;
                    if (SDriver.SDriverX.g_modelInfo.sTv_offline == "Y")
                    {
                        SetMsg("流程卡验证失败！离线机台", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    //SetMsg(SDriverX.g_modelInfo.sPart_no, UDF.COLOR.WORK);
                    //SetMsg(textBoxPn.Text, UDF.COLOR.WORK);
                    SetMsg(SDriver.SDriverX.g_modelInfo.sErr_msg ?? "网络故障！未接收到 SDriver 服务器消息", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else if (g_runMode == UDF.RunMode.FWU && GlobalConfig.sMesEid.ToUpper().Contains("QRE") == false)
            {
                //SDriver.SDriverX.g_modelInfo = new UDF.ModelInfo
                //{
                //    sSn = sn,
                //    sModelName = "ARRAN/BARRA"
                //};
                if (CheckTRID_ForNeatframeModel(sn) == false) return false;
            }
            else if (g_runMode == UDF.RunMode.FWU && GlobalConfig.sMesEid.ToUpper().Contains("QRE"))
            {
                string sql = string.Empty;
                OracleDataReader rdr = null;
                //SDriverX.g_modelInfo = new UDF.ModelInfo
                //UDF.ModelInfo mod = new UDF.ModelInfo();
                //{
                //    sSn = sn,
                //    sModelName = "Neatframe Model"
                //};
                mod.sSn = sn;
                bool bErr = Database.Oracle.ServerExecute("select * from PRODUCTHISTORY t where T.PROCESSOPERATIONNAME='Q200' AND t.set_serial_no= '" + sn + "'", out rdr);
                if (bErr)
                {
                    rdr.Read();
                    if (rdr.HasRows)
                    {
                        mod.sPart_no = rdr["BASIC_MODEL"].ToString();
                        mod.sWord = rdr["BUYER_LABEL"].ToString();
                        textBoxPn.Text = mod.sPart_no;

                    }
                    else
                    {
                        SetMsg("qre SN信息为空！ 请返回入qre库。", UDF.COLOR.FAIL);
                        return false;

                    }
                }
                else
                {
                    SetMsg("数据库操作失败！(检查网络连接或重启程序)", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                SDriver.SDriverX.g_modelInfo = new UDF.ModelInfo
                {
                    sSn = sn,
                    sModelName = "DEFAULT"
                };
            }

            return true;
        }
        bool Cycle
        {
            get
            {
                return timerCycle.Enabled;
            }

            set
            {
                if (!value)
                {
                    g_bWorkFlag = false;
                    g_sCmdRtnStrText = string.Empty;
                    g_iRcvPortLogNum = 0;

                    g_iTimerCycle = 0; //重置计时
                    textBoxSn.Enabled = true;   //解锁 SN 框
                    //buttonSetting.Enabled = true;   //解锁 设置按钮
                    buttonExit.Enabled = true;  //解锁 退出 按钮
                    //buttonGdmSend.Enabled = true;   //解锁 GDMSend 按钮
                    //buttonAutoScan.Enabled = true; //解锁 自动扫描 按钮
                    //buttonManScan.Enabled = true; //解锁 手动扫描 按钮
                    //buttonSignal.Enabled = true;    //解锁 到位信号 按钮
                    comboBoxCmdDelay.Enabled = true;    //解锁 延时 列表框
                    //checkBoxAutoRF.Enabled = true;  //解锁 自动RF
                    timerCycle.Enabled = value; //停止 Timer 计时
                    return;
                }
                g_bWorkFlag = true;
                textBoxSn.Enabled = false;  //锁定 SN 框
                buttonSetting.Enabled = false;  //锁定 设置 按钮
                buttonExit.Enabled = false; //锁定 退出 按钮
                //buttonGdmSend.Enabled = false;   //锁定 GDMSend 按钮
                //buttonAutoScan.Enabled = false; //锁定 自动扫描 按钮
                //buttonManScan.Enabled = false; //锁定 手动扫描 按钮
                //buttonSignal.Enabled = false;    //锁定 到位信号 按钮
                comboBoxCmdDelay.Enabled = false;   //锁定 延时 列表框
                //checkBoxAutoRF.Enabled = false;     //锁定 自动 RF
                timerCycle.Enabled = value; //激活 Timer 计时
            }

        }
        /// <summary>
        /// 初始化界面显示
        /// </summary>
        void InitUI()
        {
            Cycle = true;   // 开始计时
            ClearMsg(); //清空步骤列表框
            this.textBoxCycle.Text = "0";   //重置界面计时为 0
        }
        /// <summary>
        /// 重置界面显示
        /// </summary>
        void ResetUI()
        {
            Cycle = false;  //停止计时
            //重置焦点输入
            this.textBoxSn.Focus();
            this.textBoxSn.SelectAll();
            return;
        }
        /// <summary>
        /// DataGridView 初始化
        /// Vizio Soundbar Style
        /// </summary>
        void VizioSoundbarDataStyleInit()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "序号");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col2", "机台 SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "C-T");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col4", "测试时间");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.45f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        /// <summary>
        /// 添加测试数据显示
        /// Vizio Soundbar Style
        /// </summary>
        /// <param name="sn">机台 SN</param>
        /// <param name="ct">测试耗时</param>
        /// 
        void AddDataVizioSoundbar(string sn, int ct)
        {
            int index = new int();

            if (this.dataGridViewTestData.RowCount > 200)
            {
                this.dataGridViewTestData.Rows.Clear();
            }
            index = this.dataGridViewTestData.Rows.Add();
            this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
            this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
            this.dataGridViewTestData.Rows[index].Cells[2].Value = ct;
            this.dataGridViewTestData.Rows[index].Cells[3].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
            this.dataGridViewTestData.Rows[index].Selected = true;
        }
        /// <summary>
        /// 查询程序工作状态
        /// </summary>
        bool IsWorking
        {
            get
            {
                return g_bCmdWorkFlag;
            }
        }
        void ShowTvPortMsg_Internal(string msg)
        {
            textBoxAck.AppendText(msg);
        }
        bool WriteRs232CommandS12(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (resetRtnFlag)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            //尝试打开端口
            try
            {
                if (!serialPortTV.IsOpen)
                {
                    serialPortTV.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            serialPortTV.DiscardInBuffer();
            serialPortTV.DiscardOutBuffer();

            serialPortTV.WriteLine(cmd);    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25 + cycleDelay)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"root\@\(none\):/#").Success == false); //root\@\(none\):/#|MSTC_R_SystemInitDone|root\@mico:/#
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool ExitFactMode_S12(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (resetRtnFlag)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            //尝试打开端口
            try
            {
                if (!serialPortTV.IsOpen)
                {
                    serialPortTV.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            serialPortTV.DiscardInBuffer();
            serialPortTV.DiscardOutBuffer();

            serialPortTV.WriteLine(cmd);    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25 + cycleDelay)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"name: mac_bt, size 17").Success == false);
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool EnterSleepModeS12()
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (true)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            g_bPortErr = false;
            //尝试打开端口
            try
            {
                if (!serialPortTV.IsOpen)
                {
                    serialPortTV.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            serialPortTV.DiscardInBuffer();
            serialPortTV.DiscardOutBuffer();

            serialPortTV.WriteLine("MSTC_SLP");    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"enter suspend").Success == false);
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        void ResetCmdRtnStrText()
        {
            g_sCmdRtnStrText = String.Empty;
        }
        bool ResultCheck(string pattern)
        {
            return Regex.Match(g_sCmdRtnStrText, pattern).Success;
        }
        bool ResultCheck(string strSrouce, string pattern)
        {
            return Regex.Match(strSrouce, pattern).Success;
        }
        bool ResultPick(string strSrouce, string pattern, out string target)
        {
            target = string.Empty;
            Match match = Regex.Match(strSrouce, pattern);
            if (match.Success)
                target = match.Value.ToString();
            else
                return false;

            return true;
        }
        bool WriteIOCard(string cmdStr, int delay)
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            String strBuf = cmdStr.Trim().Replace(" ", "");
            Byte[] byCmdBuf = new Byte[6];
            int i = 0, j = 0;

            if (strBuf.Length < 10)
            {
                //不合法的指令长度
                g_bCmdWorkFlag = false;
                return false;
            }

            j = 0;
            for (i = 0; i < 5; i++)
            {
                byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
                j += byCmdBuf[i];
            }
            byCmdBuf[5] = (Byte)(j & 0xff);

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                if (!serialPortIOCard.IsOpen)
                {
                    serialPortIOCard.Open();
                }
                serialPortIOCard.DiscardInBuffer();
                serialPortIOCard.DiscardOutBuffer();
                serialPortIOCard.ReadTimeout = 5000;
                serialPortIOCard.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            serialPortIOCard.Write(byCmdBuf, 0, 6);

            if (delay != 0) Delay(delay);
            g_bCmdWorkFlag = false;
            return true;
        }
        bool GetCp310Power(out float power)
        {
            power = 0.0f;
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            Byte[] byCmdBuf = new Byte[4] { 0x3F, 0x57, 0x3B, 0x0D };
            Byte[] byRtnBuf = new byte[16];
            String strBuf = string.Empty;
            int retryTime = new int();
            Match match;

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                if (!serialPortCp310.IsOpen)
                {
                    serialPortCp310.Open();
                }
                serialPortCp310.DiscardInBuffer();
                serialPortCp310.DiscardOutBuffer();
                serialPortCp310.ReadTimeout = 5000;
                serialPortCp310.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                g_bPortErr = true;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            serialPortCp310.Write(byCmdBuf, 0, 4);

            do
            {
                if (retryTime > 20)
                {
                    g_bCmdWorkFlag = false;
                    return false;
                }
                retryTime++;
                Delay(200);
            } while (serialPortCp310.BytesToRead < 6);

            serialPortCp310.Read(byRtnBuf, 0, 16);
            strBuf = Encoding.Default.GetString(byRtnBuf);
            strBuf = strBuf.TrimEnd('\0').Replace("00.", "0.");
            match = Regex.Match(strBuf, @"\d{1,}\.\d{1,}");
            if (match.Success)
            {
                power = Convert.ToSingle(match.Value);
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool WriteCommandToGdm(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            //Byte[] byCmdBuf = new Byte[cmd.Length/2];
            Byte[] byRtnBuf = new byte[32];
            string strRtn = string.Empty;
            String strBuf = string.Empty;
            int retryTime = new int();

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                if (!serialPortGDM.IsOpen)
                {
                    serialPortGDM.Open();
                }
                serialPortGDM.DiscardInBuffer();
                serialPortGDM.DiscardOutBuffer();
                serialPortGDM.ReadTimeout = 5000;
                serialPortGDM.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            if (resetRtnFlag) g_sGdmRtnText = String.Empty;

            serialPortGDM.WriteLine(cmd);
            do
            {
                if (retryTime > 20 + cycleDelay)
                {
                    g_bCmdWorkFlag = false;
                    return false;
                }
                retryTime++;
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(200);
                System.Windows.Forms.Application.DoEvents();
            } while (!Regex.Match(g_sGdmRtnText, @">\x0D\x0A").Success);

            g_bCmdWorkFlag = false;
            return true;
        }   //WriteCommandToGdm end
        bool OpenDoor()
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                if (!serialPortDoor.IsOpen) serialPortDoor.Open();
                serialPortDoor.DiscardInBuffer();
                serialPortDoor.DiscardOutBuffer();
                serialPortDoor.ReadTimeout = 5000;
                serialPortDoor.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            serialPortDoor.Write("OPEN\r\n");

            g_bCmdWorkFlag = false;
            return true;
        }
        bool CloseDoor()
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            int retryCount = new int();
            byte[] tmp = new byte[8];

            Array.Clear(tmp, 0, 8);
            g_bPortErr = false;
            //尝试打开串口
            try
            {
                if (!serialPortDoor.IsOpen) serialPortDoor.Open();
                serialPortDoor.DiscardInBuffer();
                serialPortDoor.DiscardOutBuffer();
                serialPortDoor.ReadTimeout = 5000;
                serialPortDoor.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            serialPortDoor.Write("CLOSE\r\n");

            try
            {
                do
                {
                    retryCount += 1;
                    if (retryCount > 100)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(50);
                    if (g_iCmdDelay != 0)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(g_iCmdDelay);
                        g_iCmdDelay = 0;
                    }
                } while (serialPortDoor.BytesToRead < 5);
                serialPortDoor.Read(tmp, 0, 5);  //get ack
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "Get ACK Fail\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Encoding.Default.GetString(tmp).TrimEnd('\0') != "READY")
            {
                g_bCmdWorkFlag = false;
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool UpdataSpecFromServer()
        {
            String sStrBuf = string.Empty;

            //FW
            sStrBuf = GlobalConfig.ReadIniFile("TEST", "FW", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\spec\config.ini");
            if (sStrBuf.Length == 0)
            {
                SetMsg(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\spec\config.ini 未找到 FW 规格，请向 TE 报告此问题", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                GlobalConfig.sFw = sStrBuf.ToUpper().Replace(" ", "");
            }

            //FW2
            sStrBuf = GlobalConfig.ReadIniFile("TEST", "FW2", @"\\10.2.100.27\te\SETUP\Neat\FTTEST\spec\config.ini");
            if (sStrBuf.Length == 0)
            {
                SetMsg(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\spec\config.ini 上未找到 FW2 规格，请向 TE 报告此问题", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                GlobalConfig.sFw2 = sStrBuf.ToUpper().Replace(" ", "");
            }

            g_iPtInitFlag |= INIT_GETTESTITEM;
            return true;
        }
        void InitGridFormatA()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "序号");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.10f);
            dataGridViewTestData.Columns.Add("Col2", "机台 SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "项目");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.20f);
            dataGridViewTestData.Columns.Add("Col4", "OK/NG");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.10f);
            dataGridViewTestData.Columns.Add("Col5", "测试时间");
            dataGridViewTestData.Columns["Col5"].Width = Convert.ToInt32(iDataGridWidth * 0.40f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        void AddDataA(string sn, string item, string flag)
        {
            int index = new int();

            if (this.dataGridViewTestData.RowCount > 200)
            {
                this.dataGridViewTestData.Rows.Clear();
            }
            index = this.dataGridViewTestData.Rows.Add();
            this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
            this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
            this.dataGridViewTestData.Rows[index].Cells[2].Value = item;
            this.dataGridViewTestData.Rows[index].Cells[3].Value = flag;
            this.dataGridViewTestData.Rows[index].Cells[4].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
            this.dataGridViewTestData.Rows[index].Selected = true;
        }
        void InitGridFormatB()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "序号");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col2", "机台 SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "C-T");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col4", "测试时间");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.45f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        void AddDataB(string sn, int ct)
        {
            int index = new int();

            //if (this.dataGridViewTestData.RowCount > 200)
            //{
            //    this.dataGridViewTestData.Rows.Clear();
            //}
            index = this.dataGridViewTestData.Rows.Add();
            this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
            this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
            this.dataGridViewTestData.Rows[index].Cells[2].Value = ct;
            this.dataGridViewTestData.Rows[index].Cells[3].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
            this.dataGridViewTestData.Rows[index].Selected = true;
        }
        bool RunAdbCmd(string cmd, out string ack, int skipOption = 0)
        {
            ack = string.Empty;

            try
            {
                textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n"); //cmd

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + cmd; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                 process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x01) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool RunAdbCmd_BYOD(string cmd, out string ack, int skipOption = 0)
        {
            ack = string.Empty;

            try
            {
                textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n");

                Process process = new Process();
                process.StartInfo.FileName = "adb"; // 直接使用 adb 可執行文件
                process.StartInfo.Arguments = cmd;  // 傳入完整的 adb 命令
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true; // 添加錯誤輸出重定向
                process.Start();

                // 讀取標準輸出和錯誤輸出
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if ((skipOption & 0x01) == 0)
                {
                    ack = output; // 只使用標準輸出
                    if (!string.IsNullOrEmpty(error))
                    {
                        ack += "\nError: " + error; // 將錯誤附加到 ack
                        textBoxAck.AppendText("Error:\r\n" + error + "\r\n");
                    }
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                textBoxAck.AppendText("Exception:\r\n" + e.Message + "\r\n");
                return false;
            }

            return true;
        }
        bool RunAdbCmdMIC(string cmd, out string ack, int skipOption = 0)
        {
            ack = string.Empty;

            try
            {
                textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n"); //cmd

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + cmd; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x01) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool RunAdbCmd(string cmd, string query_ack, out string ack, int skipOption = 0)
        {
            ack = string.Empty;

            //CMD
            try
            {
                textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n"); //cmd
                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + cmd; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                Delay(700);
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x01) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            Delay(700);

            //QUERY ACK
            try
            {
                textBoxAck.AppendText("CMD:\r\n" + query_ack + "\r\n"); //cmd

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + query_ack; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x02) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }


            return true;
        }




        private void Process_Exited(object sender, EventArgs e)
        {
            if (!g_bAdbProcessExited) g_bAdbProcessExited = true;
        }
    }
}
