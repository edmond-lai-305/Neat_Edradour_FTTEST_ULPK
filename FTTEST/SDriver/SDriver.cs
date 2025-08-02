using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDriver;
using System.Text.RegularExpressions;
using FTTEST.UDFs;
using FTTEST.WebReference;
using FTTEST.AppConfig;

namespace FTTEST.SDriver
{
    public class SDriverX
    {
        //static clsWinSockClass g_winsock = new clsWinSockClass();   //全局 winsock 对象
        static bool g_bRecvFlag = false;     //服务器返回消息开关
        public static string g_sRecvMsg = "";    //服务器返回的消息
        public static UDF.ModelInfo g_modelInfo = new UDF.ModelInfo();   //查询的机种信息
        public static WebReference.AVTC_webservice webService = new AVTC_webservice();

        #region SDriver 相关
        ///*++++++++++++++++++++++++++++++++++++++++++++++++++++
        //* SDriver 机种测试流程卡相关,查询或传递 PASS 信息
        //* --------------------------------------------------*/
        /// <summary>
        /// 初始化 SDriver
        /// </summary>
        /// <returns></returns>
        public static bool SDriverInit()
        {
            /*
           if (webService.hello_word("") == "hello ")
            {
                return true;
            }*/
           
            return true; 

        }

        /// <summary>
        /// 站别查询函数,并返回机种的相关信息
        /// </summary>
        /// <param name="meseid">MESEID</param>
        /// <param name="sn">机台SN 序列号</param>
        /// <param name="user">用户名</param>
        /// <returns></returns>
        /// 
        public static bool STCK(string meseid, string sn, string user)
        {
            int iRetryTime = new int();
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;
            if ( GlobalConfig.iRunMode == 1 )
            {
                iRetryTime = 0;
                do
                {
                    g_sRecvMsg = webService.CHK_MES_ROUTE(meseid, sn);
                    if (g_sRecvMsg != null)
                    {

                        if (Regex.IsMatch(g_sRecvMsg, "result:true"))
                        {
                            g_bRecvFlag = true;
                        }
                        else
                        {
                            modelInfoTmp.sErr_msg = g_sRecvMsg;
                            g_modelInfo = modelInfoTmp;
                            return false;
                        }

                    }
                    if (iRetryTime > 100) return false;
                    iRetryTime++;
                    Delay(100);
                } while (!g_bRecvFlag);
                modelInfoTmp.sSn = sn;
                modelInfoTmp.sMode = Regex.Match(g_sRecvMsg, "\"sMode\":\"(\\S+)\",\"sTv_offline\"").Groups[1].Value;
                modelInfoTmp.sTv_offline = Regex.Match(g_sRecvMsg, "\"sTv_offline\":\"(\\S+)\",\"sFinish_type\"").Groups[1].Value;
                modelInfoTmp.sFinish_type = Regex.Match(g_sRecvMsg, "\"sFinish_type\":\"(\\S+)\",\"sProductname\"").Groups[1].Value;
                modelInfoTmp.sProductname = Regex.Match(g_sRecvMsg, "\"sProductname\":\"(\\S+)\",\"sWord\"").Groups[1].Value;
                modelInfoTmp.sWord = Regex.Match(g_sRecvMsg, "\"sWord\":\"(\\S+)\",\"sPart_no\"").Groups[1].Value;
                modelInfoTmp.sPart_no = Regex.Match(g_sRecvMsg, "\"sPart_no\":\"(\\S+)\",\"sModel\"").Groups[1].Value;
                modelInfoTmp.sModel = Regex.Match(g_sRecvMsg, "\"sModel\":\"(\\S+)\",\"sSimple_model\"").Groups[1].Value;
                modelInfoTmp.sSimple_model = Regex.Match(g_sRecvMsg, "\"sSimple_model\":\"(\\S+)\",\"model_code\"").Groups[1].Value;
                modelInfoTmp.sRtn_cd = Regex.Match(g_sRecvMsg, "\"sRtn_cd\":\"(\\S+)\",\"sErr_eng_msg\"").Groups[1].Value;
                modelInfoTmp.sErr_eng_msg = Regex.Match(g_sRecvMsg, "\"sErr_eng_msg\":\"(\\S+)\",\"sErr_msg\"").Groups[1].Value;
                modelInfoTmp.sErr_msg = Regex.Match(g_sRecvMsg, "\"sErr_msg\":\"(\\S+)\",\"sSn\"").Groups[1].Value;
                modelInfoTmp.sUseFlag = 0;
            }
            else
            {
                //以下都是run mode = 0測試使用
                modelInfoTmp.sSn = sn;
                modelInfoTmp.sMode = "AUTO"; 
                modelInfoTmp.sTv_offline = "N";
                modelInfoTmp.sFinish_type = "FGD";
                modelInfoTmp.sProductname = "NH12342000014"; 
                modelInfoTmp.sWord = "P53E0903-TV-VN1";
                 modelInfoTmp.sPart_no = "VXXWWNF2——.NFEBAR3";
                //modelInfoTmp.sPart_no = "V50WWNF22AO-.HARRIS3";
                //modelInfoTmp.sModel = "Neat Harris NeatBoard QCS8250_AMT5000UFEY39-01.0_Merry_Active pen_L/T Thermal_camera GNK_AVTC_by air";
                modelInfoTmp.sModel = "Neat Edradour NeatBoard QCS8250_AMT5000UFEY39";
                modelInfoTmp.sSimple_model = "NF-H1";
                modelInfoTmp.sRtn_cd = "0";
                modelInfoTmp.sErr_eng_msg = "";
                modelInfoTmp.sErr_msg = "";
                modelInfoTmp.sUseFlag = 0;
            }

            g_modelInfo = modelInfoTmp;

            if (modelInfoTmp.sPart_no == string.Empty) return false;
            return true;
        }
        //==============
        /// <summary>
        /// 机台序列号过站函数
        /// </summary>
        /// <param name="meseid">MESEID</param>
        /// <param name="sn">机台 SN 序列号</param>
        /// <param name="user">用户名</param>
        /// <param name="pf">'P' Or 'F'(PASS OR FAIL)</param>
        /// <param name="comment">注释</param>
        /// <returns></returns>
        /// 
        public static bool WBER(string meseid, string sn, string user, string pf, string errorCode)
        {
            int iRetryTime = new int();
            //int i = new int();
            string msg = string.Empty;

            g_bRecvFlag = false;
            g_sRecvMsg = "";
            
            
            iRetryTime = 0;
            /*
            do
            {
                g_sRecvMsg = webService.INS_MES_STATION(meseid,sn ,user, comment);//mesid/sn/user/error code
                if (g_sRecvMsg != null)
                {
                    g_bRecvFlag = true;
                }
                if (iRetryTime > 100) return false;
                iRetryTime++;
                Delay(100);
            } while (!g_bRecvFlag);*/
            g_sRecvMsg = webService.INS_MES_STATION(meseid, sn, user, errorCode);//mesid/sn/user/error code

            g_modelInfo.sMode =             Regex.Match(g_sRecvMsg, "\"sMode\":\"(\\S+)\",\"sTv_offline\"").Groups[1].Value;
            g_modelInfo.sProductname =      Regex.Match(g_sRecvMsg, "\"sProductname\":\"(\\S+)\",\"sWord\"").Groups[1].Value;
            g_modelInfo.sRtn_cd =           Regex.Match(g_sRecvMsg, "\"sRtn_cd\":\"(\\S+)\",\"sErr_eng_msg\"").Groups[1].Value;
            g_modelInfo.sErr_eng_msg =      Regex.Match(g_sRecvMsg, "\"sErr_eng_msg\":\"(\\S+)\",\"sErr_msg\"").Groups[1].Value;
            g_modelInfo.sErr_msg =          Regex.Match(g_sRecvMsg, "\"sErr_msg\":\"(\\S+)\",\"sSn\"").Groups[1].Value;
            //if (g_modelInfo.sRtn_cd != "0") return false;
            if (CheckResult(g_sRecvMsg, "result:true") == false)
            {
                g_sRecvMsg = g_sRecvMsg.Replace("result:false", "");
                g_modelInfo.sErr_msg = g_sRecvMsg;
                //errormsg = g_sRecvMsg;
              
                // modelInfoTmp.sErr_eng_msg = Regex.Match(g_sRecvMsg, @"(?<= sErr_msg\().*(?=\) sErr_msg)").ToString().Trim();
                return false;
            }

            return true;
        }
        public static string errormsg = "";
        public static bool CHK_MES_ROUTE_Webservice(string meseid, string sn, string user)
        {
            int iRetryTime = new int();
            string msg = string.Empty;
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;
            g_sRecvMsg = "";
            string RTN = string.Empty;

            // g_sRecvMsg = webService.SD_STCK(meseid, sn, user);
            g_sRecvMsg = webService.CHK_MES_ROUTE(meseid, sn);
            RTN = g_sRecvMsg.Replace(":", "");

            if (CheckResult(g_sRecvMsg, "result:false") == true)
            {
                g_sRecvMsg = g_sRecvMsg.Replace("result:false", "");
                modelInfoTmp.sErr_msg = g_sRecvMsg;
                errormsg = g_sRecvMsg;
                g_modelInfo = modelInfoTmp;
                // modelInfoTmp.sErr_eng_msg = Regex.Match(g_sRecvMsg, @"(?<= sErr_msg\().*(?=\) sErr_msg)").ToString().Trim();
                return false;
            }
            else
            {
                //RTN
                modelInfoTmp.sSn = sn;
                modelInfoTmp.sMode = MidStrEx(RTN, "sMode", ",");
                modelInfoTmp.sMode = modelInfoTmp.sMode.Replace("\"", "");

                modelInfoTmp.sTv_offline = MidStrEx(RTN, "sTv_offline", ",");
                modelInfoTmp.sTv_offline = modelInfoTmp.sTv_offline.Replace("\"", "");

                modelInfoTmp.sFinish_type = MidStrEx(RTN, "sFinish_type", ",");
                modelInfoTmp.sFinish_type = modelInfoTmp.sFinish_type.Replace("\"", "");

                modelInfoTmp.sProductname = MidStrEx(RTN, "sProductname", ",");
                modelInfoTmp.sProductname = modelInfoTmp.sProductname.Replace("\"", "");

                modelInfoTmp.sWord = MidStrEx(RTN, "sWord", ",");
                modelInfoTmp.sWord = modelInfoTmp.sWord.Replace("\"", "");

                modelInfoTmp.sPart_no = MidStrEx(RTN, "sPart_no", ",");
                modelInfoTmp.sPart_no = modelInfoTmp.sPart_no.Replace("\"", "");

                modelInfoTmp.sModel = MidStrEx(RTN, "sModel", ",");
                modelInfoTmp.sModel = modelInfoTmp.sModel.Replace("\"", "");

                modelInfoTmp.sSimple_model = MidStrEx(RTN, "sSimple_model", ",");
                modelInfoTmp.sSimple_model = modelInfoTmp.sSimple_model.Replace("\"", "");

                modelInfoTmp.sRtn_cd = MidStrEx(RTN, "sRtn_cd", ",");
                modelInfoTmp.sRtn_cd = modelInfoTmp.sRtn_cd.Replace("\"", "");

                modelInfoTmp.sErr_msg = MidStrEx(RTN, "sErr_msg", ",");
                modelInfoTmp.sErr_msg = modelInfoTmp.sErr_msg.Replace("\"", "");
            }
            modelInfoTmp.sUseFlag = 0;
            g_modelInfo = modelInfoTmp;
            if (modelInfoTmp.sPart_no == string.Empty) return false;

            return true;
        }
        public static bool INS_MES_STATION_Webservice(string meseid, string sn, string errorCode)
        {

            int iRetryTime = new int();
            string msg = string.Empty;
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;
            g_sRecvMsg = "";
            string RTN = string.Empty;

            // g_sRecvMsg = webService.SD_STCK(meseid, sn, user);
            g_sRecvMsg = webService.INS_MES_STATION(meseid, sn, "TestUser", errorCode);
            RTN = g_sRecvMsg.Replace(":", "");
            if (CheckResult(g_sRecvMsg, "result:true") == false)
            {
                g_sRecvMsg = g_sRecvMsg.Replace("result:false", "");
                modelInfoTmp.sErr_msg = g_sRecvMsg;
                errormsg = g_sRecvMsg;
                g_modelInfo = modelInfoTmp;
                // modelInfoTmp.sErr_eng_msg = Regex.Match(g_sRecvMsg, @"(?<= sErr_msg\().*(?=\) sErr_msg)").ToString().Trim();
                return false;
            }
            modelInfoTmp.sUseFlag = 0;
            g_modelInfo = modelInfoTmp;
            //if (modelInfoTmp.sPart_no == string.Empty) return false;

            return true;
        }
        public static bool INS_check_3T_Webservice(string meseid, string sn, string OKNG)
        {
            int iRetryTime = new int();
            string msg = string.Empty;
            ///// step 5.1set modelInfoTmp = new => modelInfoTmp.sn = null
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;
            g_sRecvMsg = "";
            string RTN = string.Empty;

            /* public string INS_check_3T(
             string x_sn,
             string x_ST_ID, 
            string x_result, 
            string x_error_code, 
            string x_error_contect, 
            string user_id)
            */

            // g_sRecvMsg = webService.SD_STCK(meseid, sn, user);
            try
            {
                g_sRecvMsg = webService.INS_check_3T(sn, meseid, OKNG, "", "", "TestUser");
            }
            catch (Exception ex)
            {

                return false;
            }
            //RTN = g_sRecvMsg.Replace(":", "");
            if (CheckResult(g_sRecvMsg, "result:true") == false)
            {
                g_sRecvMsg = g_sRecvMsg.Replace("result:false", "");
                modelInfoTmp.sErr_msg = g_sRecvMsg;
                errormsg = g_sRecvMsg;
                g_modelInfo = modelInfoTmp;
                // modelInfoTmp.sErr_eng_msg = Regex.Match(g_sRecvMsg, @"(?<= sErr_msg\().*(?=\) sErr_msg)").ToString().Trim();
                return false;
            }
            modelInfoTmp.sUseFlag = 0;
            g_modelInfo = modelInfoTmp;
            //if (modelInfoTmp.sPart_no == string.Empty) return false;

            return true;
        }

        public static bool CheckResult(string strSrouce, string pattern)
        {
            return Regex.Match(strSrouce, pattern).Success;
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
        //==============


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
                System.Threading.Thread.Sleep(40);
            } while (i < j);
        }
        #endregion
    }
}
