using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTTEST.SDriver;
using FTTEST.UDFs;
using FTTEST.AppConfig;
using FTTEST.Database;
//using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace FTTEST.TestProcess
{
    class TestProcess
    {
    }
}
namespace FTTEST
{
    public partial class FormMain
    {
        bool PRE_MainTestProcess()
        {
            //检查料号
            //AXXUSNFB----.NBARB11
            if (SDriverX.g_modelInfo.sPart_no == "356506020300" ||
                SDriverX.g_modelInfo.sPart_no.Substring(13, 6).ToUpper() == "CARDHU")
            {
                if (PRE_Cardhu() == false) return false;
            }
            //else if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "NF")
            //{
            //    if (PRE_Barra() == false) return false;
            //}
            else if (SDriverX.g_modelInfo.sPart_no.Substring(14, 2).ToUpper() == "BA")
            {
                if (PRE_Barra() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(10, 2).ToUpper() == "FE")//20221209
            {  //@@@
                SetMsg("PRE_Edradour", UDFs.UDF.COLOR.WORK);
                if (PRE_Edradour() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 2).ToUpper() == "HA")//20221209
            {
                SetMsg("PRE_HARRIS", UDFs.UDF.COLOR.WORK);
                if (PRE_Harr() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 2).ToUpper() == "KA")//20221209
            {
                SetMsg("PRE_kavala", UDFs.UDF.COLOR.WORK);
                if (PRE_Kava() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "OL")//20221209
            {
                SetMsg("PRE_Owllabs", UDFs.UDF.COLOR.WORK);
                if (PRE_OwlLbas() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "QC")  //VXXWWQCB----.QC82501
            {
                //if (PRE_QCS_TEST() == false) return false;
                if (PRE_QCS8550() == false) return false;    
            }
            else
            {
                SetMsg("当前料号信息未维护", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            

            return true;
        }

        string curLine_Usercode = "", curBarcode_So="", curBarcode_PartNo="", curLine_MLine="", s_changeSo="";
        Int32 i_changeNum = 0;
        bool b_changeTime;
        DateTime curBarcode_sTime  ;
        void Insert_Mes_qty(string str)
        {
            string sql = string.Empty;
            OracleDataReader reader;
            bool bErr = new bool();
            string m_Line = string.Empty;
            bool b_Rework = false;
            string buf = string.Empty;

            curLine_Usercode = "NA";
            curBarcode_So = SDriverX.g_modelInfo.sWord;
            curBarcode_PartNo = SDriverX.g_modelInfo.sPart_no;
            curBarcode_sTime = DateTime.Now ;
            if (isReworkSN(SDriverX.g_modelInfo.sSn) == true)
            {
                b_Rework = true;
            }
            else
            {
                b_Rework = false;
            }

            sql = string.Format("select AREANAME from machinespec where machinename ='" + GlobalConfig.sMesEid + "'");
            bErr = Database.Oracle.ServerExecute(sql, out reader);
            if (bErr)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    curLine_MLine = reader[0].ToString();
                }
            }

            sql = string.Format(" select productrequestname from producthistory where SET_SERIAL_NO='" + SDriverX.g_modelInfo.sSn + "' AND EVENTNAME ='MOVE'");
            bErr = Database.Oracle.ServerExecute(sql, out reader);
            if (bErr)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    buf = reader["productrequestname"].ToString();
                    if (buf.Substring(0, 1).ToUpper() == "R")
                    {
                        b_Rework = true;
                    }
                }
            }
            if (b_Rework == false)
            {
                Sql.Close();
                if (Sql.Open("Data Source=192.168.158.29;Initial Catalog=MES; User ID=mes;Password=mesuser") == false)
                {
                    Application.Exit();
                }
                else
                {
                    Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
                }
                InsertQty_count();
                m_Line = curLine_MLine;
                Mpq(curBarcode_So, "FGD", curBarcode_PartNo, curBarcode_sTime, m_Line, curLine_Usercode);

                i_changeNum = i_changeNum + 1;
                if (s_changeSo != curBarcode_So || b_changeTime == true || i_changeNum == 10)
                {
                    if (s_changeSo != curBarcode_So || b_changeTime == true)
                    {
                        i_changeNum = 1;
                    }
                    s_changeSo = curBarcode_So;

                    Insert_Rate();
                }
                 
                Sql.Close();
                if (Sql.Open("Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te") == false)
                {
                    Application.Exit();
                }
                else
                {
                    Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
                }
            }
        }
        Int64 Plan_Qty = 0;
        void Insert_Rate()
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            string Update_Item = string.Empty;
            string Update_Rate = string.Empty;
            string iPeriod = string.Empty;
            string sDateid = string.Empty;
            string sCurDateid = string.Empty;
            string sShift = string.Empty;
            string diffqty = string.Empty;
            string sPart_no = string.Empty;
            string sStarttime = string.Empty;
            string Stand_Time = string.Empty;
            string iHour = string.Empty;
            string ipreHour = string.Empty;
            string sLine = string.Empty;
            string tDate = string.Empty;
            string sDate = string.Empty;
            DateTime sDatetime , g_dSql_time;
            int retrytime = 0;
            InitT();
            GetSqltime(out g_dSql_time);
           
            sDatetime = g_dSql_time;
            getShiftPeriod(g_dSql_time, out sDate, out sShift);
            getQtyHour(curLine_MLine, curBarcode_sTime, out Update_Item, out Update_Rate, sShift, out iPeriod);

            //sDateid = Trim(curLine.MLine) & Format(getDayTimeStart(curBarcode.sTime, sShift), "yyyymmdd")
            //sCurDateid = Trim(curLine.MLine) & Format(curBarcode.sTime, "yyyymmdd")
            getDayTimeStart(g_dSql_time, sShift, out Stand_Time);
            sDateid = curLine_MLine + Convert.ToDateTime(Stand_Time).ToString("yyyyMMdd");
            sCurDateid = curLine_MLine + curBarcode_sTime.ToString("yyyyMMdd");
            iHour = sCurDateid + curBarcode_sTime.ToString("HH");
            if (curBarcode_sTime.ToString("HH") == "00")
            {
                ipreHour = curLine_MLine + curBarcode_sTime.AddHours(-1).ToString("yyyyMMdd") + curBarcode_sTime.AddHours(-1).ToString("HH");
            }
            else
            {
                ipreHour = sCurDateid + curBarcode_sTime.AddHours(-1).ToString("HH");
            }
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select DATE_ID from Qty_Rate with(nolock) where DATE_ID= '" + sDateid + "' and Mline= 'T300'", out rdr) == true)
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                   rdr.Close();
                   if( Sql.UpdateServer("insert into Qty_Rate(DATE_ID,MLINE) values ('" + sDateid + "','T300') " )==false)
                    {
                        SetMsg("Qty_Rate insert数据失败！", UDF.COLOR.FAIL);
                        return;
                    }                     
                }
            }
            rdr.Close();
            RateShow(iHour, ipreHour);
            up1:
            if (Sql.UpdateServer("UPDATE Qty_Rate SET " + Update_Rate + "=" + Plan_Qty + "   WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'" ) == false)
            {
                if(retrytime<3)
                {
                    System.Threading.Thread.Sleep(50);
                    retrytime++;
                    goto up1;
                }
                SetMsg("Qty_Rate UPDATE数据失败！", UDF.COLOR.FAIL);
                return;
            }
            rdr.Close();
        }
        void GetSqltime(out DateTime sdate)
        {
             
            string strSql = "select getdate() as date";
            string dt =  Sql.queryData(strSql, "date");
            sdate = Convert.ToDateTime(dt);
            
        }
        void RateShow(string sHour, string spreHour)
        {
            string sql = string.Empty;
            int sqlCount = 0;
            SqlDataReader rdr = null;

            string dsr = string.Empty;
            string[] startTime = new string[1000];
            DateTime[] startTimeRs = new DateTime[1000];
            string[] Part_No = new string[1000];
            string[] PartNoNum = new string[1000];
            long TimeSpace, TimeSSpace;
            int SpaceCount = 0;
            int j = 0;
            int k = 0, kk = 0;
            int iShift = 0;
            int StdQty = 0;
            int iModelCount = 0;
            long ValidTime;
            Single Day_qty, Day_SumQty, temp_QTY;
            string temp_Station = string.Empty;
            string sLine = string.Empty;
            DateTime dt;
            string timestr = string.Empty;
            string datestr = string.Empty;
            string CurTime = string.Empty;
            string dDiffTime = string.Empty;

            sLine = curLine_MLine;
            datestr = curBarcode_sTime.ToString("yyyy-MM-dd");
            CurTime = datestr + " " + curBarcode_sTime.ToString("HH") + ":59:59";
            j = 0;
            kk = 1;
            Day_qty = 0;
            startTime[0] = CurTime;
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select * from mpq_new with(nolock) where (dayhour='" + sHour.Substring(2) + "' or dayhour='" + spreHour.Substring(2) + "') and line='" + sLine + "' and eline='" + curLine_Usercode + "' and station='T300' order by starttime Desc", out rdr) == true)
            {
                j = 1;
                while (rdr.Read())
                {
                    Part_No[j] = rdr["PartNo"].ToString().Trim();
                    PartNoNum[j] = rdr["quantity"].ToString().Trim();
                    startTimeRs[j] = Convert.ToDateTime(rdr["startTime"].ToString().Trim());
                    sqlCount = kk;
                    j += 1;
                    kk += 1;
                    if (j >= 1000)
                    {
                        break;
                    }
                }
                for (k = 1; k < kk; k++)
                {
                    if (sqlCount <= 1)
                    {
                        startTime[k] = curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + ":00:00";
                    }
                    else
                    {
                        if (sqlCount == k && Convert.ToDateTime(startTimeRs[k].ToString("yyyy-MM-dd HH:mm:ss")) > Convert.ToDateTime(curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + " :00:00"))
                        {
                            startTime[k] = curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + ":00:00";
                        }
                        else
                        {
                            startTime[k] = startTimeRs[k].ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
            }
            rdr.Close();
            SpaceCount = j - 1;
            for (j = 1; j < SpaceCount + 1; j++)
            {
                if (Convert.ToDateTime(startTime[j - 1]) <= Convert.ToDateTime(datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00"))
                {
                    break;
                }
                temp_QTY = 0;
                if (j > 1)
                {
                    //M32ARGGB2EQH.G32QCA1
                    if (Part_No[j].Substring(13, 6) != Part_No[j - 1].Substring(13, 6))
                    {
                        if (Convert.ToInt32(PartNoNum[j]) >= 10 && Convert.ToInt32(PartNoNum[j - 1]) >= 10)
                        {
                            GetModelCount(Part_No[j - 1], iShift, out iModelCount);
                            temp_QTY = iModelCount / (480 * 60) * 1800;
                        }
                    }
                }
                GetModelCount(Part_No[j], iShift, out iModelCount); //'获取各机种的标准产量
                StdQty = iModelCount;
                GetValidTime(startTime[j], startTime[j - 1], out ValidTime);
                TimeSpace = ValidTime;

                //DbStdQty = StdQty;
                //Day_qty = Day_qty + (DbStdQty / (480 * 60) * TimeSpace) - mm.StrToInt(dDiffQTY);
                Day_SumQty = StdQty;
                Day_qty = Day_qty + (Day_SumQty / (480 * 60) * TimeSpace) - temp_QTY;
            }
            Plan_Qty = Convert.ToInt16(Day_qty);
            //数组的清空，以备下一次 iShift 循环
            for (k = 0; k < j + 1; k++)
            {
                Part_No[k] = "";
                startTime[k] = "";
            }

        }
        void GetValidTime(string FirstTime, string LastTime, out long ValidTime)
        {
            long sumT = 0;
            string datestr = string.Empty;

            datestr = curBarcode_sTime.ToString("yyyy-MM-dd");
            if (Convert.ToDateTime(FirstTime) < Convert.ToDateTime(datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00"))
            {
                FirstTime = datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00";
            }
            sumT = Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(LastTime)));
            // 'Remove the First FreeTime.
            if (timeGroup_FreeTimeFirstStart != "" && timeGroup_FreeTimeFirstEnd == "")
            {
                if (Convert.ToDateTime(timeGroup_FreeTimeFirstStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeFirstStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_FreeTimeFirstCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_FreeTimeFirstStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_FreeTimeFirstStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_FreeTimeFirstEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            //'Remove the second FreeTime.
            if (timeGroup_FreeTimeSecondStart != "" && timeGroup_FreeTimeSecondEnd != "")
            {
                if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeSecondStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_FreeTimeSecondCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_FreeTimeSecondStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_FreeTimeSecondEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            //'Remove First EatTime.
            if (timeGroup_EatTimeFirstStart != "" && timeGroup_EatTimeFirstEnd != "")
            {
                if (Convert.ToDateTime(timeGroup_EatTimeFirstStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeFirstStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_EatTimeFirstCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_EatTimeFirstStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_EatTimeFirstStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeFirstEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_EatTimeFirstEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            //'Remove Second EatTime
            if (timeGroup_EatTimeSecondStart != "" && timeGroup_EatTimeSecondEnd != "")
            {
                if (Convert.ToDateTime(timeGroup_EatTimeSecondStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeSecondStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_EatTimeSecondCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_EatTimeSecondStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_EatTimeSecondStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeSecondEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_EatTimeSecondEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            ValidTime = sumT;
        }
        public double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return secondSpan.TotalSeconds;
        }
        public double DiffMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return minuteSpan.TotalMinutes;
        }
        public double DiffHours(DateTime startTime, DateTime endTime)
        {
            TimeSpan HourSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return HourSpan.TotalHours;
        }
        void GetModelCount(string Part_No, int iShift, out int ModelCount)
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            int dsr = 0;
            string sLine = string.Empty;

            sLine = curLine_MLine;

            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select * from UPH with(nolock) where partno='" + Part_No + "' and linetype='" + sLine + "'", out rdr) == true)
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    if (Sql.ServerExecute("select * from UPH with(nolock) where partno='" + Part_No + "' and linetype='Normal'", out rdr) == true)
                    {
                        rdr.Read();
                        if (rdr.HasRows == false)
                        {
                            MessageBox.Show(null, "该线别未设定此机种的UPH,请确认线别是否设定正确？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            dsr = Convert.ToInt16(rdr[2].ToString());
                        }
                    }
                }
                else
                {
                    dsr = Convert.ToInt16(rdr[2].ToString());
                }
                rdr.Close();
            }
            ModelCount = dsr;
        }

        bool timeGroup_DayWork;
        Int32 timeGroup_FreeTimeFirstCount,
              timeGroup_FreeTimeSecondCount,
              timeGroup_EatTimeFirstCount,
              timeGroup_EatTimeSecondCount 
              ;

        string timeGroup_EatTimeFirstStart,
               timeGroup_EatTimeFirstEnd,
               timeGroup_EatTimeSecondStart,
               timeGroup_EatTimeSecondEnd,
               timeGroup_FreeTimeSecondStart,
               timeGroup_FreeTimeSecondEnd,
               timeGroup_FreeTimeFirstStart,
               timeGroup_FreeTimeFirstEnd
            ;
        void InitT()
        {
            string timestr = string.Empty;
            string datestr = string.Empty;
            string CurTime = string.Empty;
            string sLine = string.Empty;

            string sql = string.Empty;
            SqlDataReader rdr = null;

            timeGroup_DayWork = false;
            timestr = curBarcode_sTime.ToString("HHmmss");
            datestr = curBarcode_sTime.ToString("yyyy-MM-dd");
            CurTime = curBarcode_sTime.ToString("HH:mm:ss");

            timeGroup_FreeTimeFirstCount = 0;
            timeGroup_FreeTimeSecondCount = 0;
            timeGroup_EatTimeFirstCount = 0;
            timeGroup_EatTimeSecondCount = 0;

            timeGroup_EatTimeFirstStart = "";
            timeGroup_EatTimeFirstEnd = "";
            timeGroup_EatTimeSecondStart = "";
            timeGroup_EatTimeSecondEnd = "";

            sLine = curLine_MLine;
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Convert.ToInt32(timestr) >= 80000 && Convert.ToInt32(timestr) < 200000)        //'8.00 Am - 8.00 Pm
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and starttime < '200000' and starttime > '080000' order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "3":  //10:00休息
                                timeGroup_FreeTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "4":  //11:00 第一次吃饭开始
                                timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "5":
                                if (timeGroup_EatTimeFirstStart == "")
                                {
                                    timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_FreeTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "8": // 15:00 下午休息
                                timeGroup_FreeTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount = Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "9": //16:00 晚饭开始
                                timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "10":
                                if (timeGroup_EatTimeSecondStart == "")
                                {
                                    timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "11":
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult();
                    }
                }
            }
            else if (Convert.ToInt32(timestr) >= 200000 && Convert.ToInt32(timestr) < 235959)        //'8.00 Pm -11.59.59
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and (starttime > '200000' or starttime < '080000') order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "15": //22:00 夜班第一次休息
                                timeGroup_FreeTimeFirstStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "16": //23:00 夜班第一次吃饭 

                                timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                if (Convert.ToInt32(rdr["endTime"].ToString().Trim()) - Convert.ToInt32(rdr["startTime"].ToString().Trim()) > 0)
                                {
                                    timeGroup_EatTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                else
                                {
                                    timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "17": //00:00 夜班第一次吃饭
                                if (timeGroup_EatTimeFirstStart == "")
                                {
                                    timeGroup_EatTimeFirstStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "20":  //3:00夜班第二次休息
                                timeGroup_FreeTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount = Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "21":  //4:00夜班早饭 
                                timeGroup_EatTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "22": // 5:00夜班早饭
                                if (timeGroup_EatTimeSecondStart == "")
                                {
                                    timeGroup_EatTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                }

                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "23": //6:00夜班早饭
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult(); 
                    }
                }
            }
            else if (Convert.ToInt32(timestr) >= 0 && Convert.ToInt32(timestr) < 80000)        //'00.00 Am - 8.00 Am
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and (starttime > '200000' or starttime < '080000') order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "15":
                                timeGroup_FreeTimeFirstStart = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "16":
                                timeGroup_EatTimeFirstStart = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                if (Convert.ToInt32(rdr["endTime"].ToString().Trim()) - Convert.ToInt32(rdr["startTime"].ToString().Trim()) > 0)
                                {
                                    timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                }
                                else
                                {
                                    timeGroup_EatTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "17":  // 00:00 夜班第一次吃饭 
                                if (timeGroup_EatTimeFirstStart == "")
                                {
                                    timeGroup_EatTimeFirstStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "20": // 3:00夜班第二次休息
                                timeGroup_FreeTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount = Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "21": // 4:00夜班早饭
                                timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "22": // 5:00夜班早饭
                                if (timeGroup_EatTimeSecondStart == "")
                                {
                                    timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "23": //6:00夜班早饭
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult();
                    }
                }
            }
            rdr.Close();
        }
        bool isReworkSN(string sSN)
        {
            string sql = string.Empty;
            OracleDataReader reader;
            bool bErr = new bool();
            string db = string.Empty;

            sql = string.Format("SELECT PRODUCTNAME,SET_SERIAL_NO FROM PRODUCTHISTORY WHERE SET_SERIAL_NO = '" + sSN + "' AND OLDPROCESSOPERATIONNAME ='T300' AND EVENTNAME ='MOVE'");
            bErr = Database.Oracle.ServerExecute(sql, out reader);
            if (bErr)
            {
                reader.Read();
                if (reader.HasRows == false)
                {
                    return false;
                }
            }
            return true;
        }
        void InsertQty_count()
        {
            string sql = string.Empty;
            string Update_Item = string.Empty;
            string Update_Rate = string.Empty;
            string iPeriod = string.Empty;
            string sDateid = string.Empty;
            string sShift = string.Empty;

            string diffqty = string.Empty;
            string sPart_no = string.Empty;
            string sStarttime = string.Empty;
            string sLine = string.Empty;
            string sDate = string.Empty;
            string tDate = string.Empty;
            string Stand_Time = string.Empty;
            DateTime g_dSql_time ;
            SqlDataReader rdr = null;
            int retrytime = 0;
            bool DBSQL;
            GetSqltime(out g_dSql_time);
            curBarcode_sTime = g_dSql_time;
            getShiftPeriod(g_dSql_time, out sDate, out sShift);
            getQtyHour(curLine_MLine, curBarcode_sTime, out Update_Item, out Update_Rate, sShift, out iPeriod);
            getDayTimeStart(g_dSql_time, sShift, out Stand_Time);
            sDateid = curLine_MLine + Convert.ToDateTime(Stand_Time).ToString("yyyyMMdd");

            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
             
            if (Sql.ServerExecute("select DATE_ID from QTY_Count with(nolock) where DATE_ID= '" + sDateid + "' and Mline= 'T300'", out rdr) == true)
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                re:
                    if (Sql.UpdateServer("insert into QTY_Count(DATE_ID,MLINE) values ('" + sDateid + "','T300')") == false)
                    {
                        if (retrytime <3)
                        {
                            System.Threading.Thread.Sleep(50);
                            retrytime++;
                            goto re;
                        }
                        SetMsg("QTY_Count 数据库插入数据错误" , UDF.COLOR.FAIL);
                        return;
                    }
                    rdr.Close();
                }
                else
                {
                    rdr.Close();
                }
            }
            try
            {
                retrytime = 0;
                up1:
                DBSQL =Sql.UpdateServer("UPDATE QTY_Count SET " + Update_Item + "=" + Update_Item + " + 1   WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'" );
                if(DBSQL ==false)
                {
                    if (retrytime < 3)
                    {
                        System .Threading.Thread.Sleep(50);
                        retrytime++;
                        goto up1;
                    }
                    SetMsg("QTY_Count 数据库UPDATE数据错误", UDF.COLOR.FAIL);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            rdr.Close();

            try
            {
                if (Convert.ToInt16(iPeriod) >= 1 && Convert.ToInt16(iPeriod) <= 12)
                {
                    retrytime = 0;
                    up2:
                    DBSQL = Sql.UpdateServer("UPDATE QTY_Count SET TOTAL_A= TOTAL_A+1,MODEL_A='" + curBarcode_PartNo.Substring(13, 7) + "' WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'");
                    if (DBSQL == false)
                    {
                        if (retrytime < 3)
                        {
                            System.Threading.Thread.Sleep(50);
                            retrytime++;
                            goto up2;
                        }
                        SetMsg("QTY_Count 数据库UPDATE数据错误", UDF.COLOR.FAIL);
                        return;
                    }
                }
                else
                {
                    retrytime = 0;
                    up3:
                    DBSQL = Sql.UpdateServer("UPDATE QTY_Count SET TOTAL_B= TOTAL_B+1,MODEL_B='" + curBarcode_PartNo.Substring(13, 7) + "' WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'");
                    if (DBSQL == false)
                    {
                        if (retrytime < 3)
                        {
                            System.Threading.Thread.Sleep(50);
                            retrytime++;
                            goto up3;
                        }
                        SetMsg("QTY_Count 数据库UPDATE数据错误", UDF.COLOR.FAIL);
                        return;
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        void Mpq(string sSO, string sType, string sPartNo, DateTime sDatetime, string sLine, string sEline)
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            SqlDataReader rdr2 = null;
            string sUserModel = string.Empty;
            string sShiftPeriod = string.Empty;
            string sRework = string.Empty;
            string smark = string.Empty;
            string sDate = string.Empty;
            string sHour = string.Empty;
            string sHourid = string.Empty;
            string sDateid = string.Empty;
            string sMonthID = string.Empty;
            string sWeekID = string.Empty;
            string sQuarterID = string.Empty;
            string tempTime = string.Empty;
            int nQty = 0;
            int retrytime = 0;
            b_changeTime = false;

            GetShift(out sShiftPeriod);
            getSOMark(sShiftPeriod, out sDate, out sHour, out smark);
            if (SDriverX.g_modelInfo.sWord.Substring(0, 1) == "R")
            {
                sRework = "R";
            }
            else
            {
                sRework = "N";
            }
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            Sql.ServerExecute("select * from mpq_new with(nolock) where Dayhour='" + sHour + "' and ELine='" + sEline + "' and  Line='" + sLine + "' and  ShiftAB='" + sShiftPeriod + "' and Station ='T300' order by starttime desc", out rdr);
            rdr.Read();
            if (rdr.HasRows == true)
            {
                if (rdr[3].ToString().Trim() == curBarcode_So)
                {
                    tempTime = rdr[8].ToString().Trim();
                    nQty = Convert.ToInt16(rdr[1].ToString().Trim());
                    nQty = nQty + 1;
                    rdr.Close();
                    up1:
                    sql = "update mpq_new set Quantity='" + nQty + "', Endtime= '" + sDatetime + "' where  starttime='" + tempTime + "' and  so ='" + curBarcode_So + "'  and  partno='" + curBarcode_PartNo + "' and  Line='" + sLine + "'and Station ='T300'";
                    if (Sql.UpdateServer(sql) == false)
                    {
                        if (retrytime < 3)
                        {
                            System.Threading.Thread.Sleep(50);
                            retrytime++;
                            goto up1;
                        }
                        SetMsg("mpq_new 更新数据失败！", UDF.COLOR.FAIL);
                        return;
                    }
                    //rdr2.Close();
                }
                else
                {
                    try
                    {
                        rdr.Close();
                        retrytime = 0;
                        b_changeTime = true;
                        GetDateID(sDatetime, out sHourid, out sDateid, out sWeekID, out sMonthID, out sQuarterID);
                        up2:
                        if (Sql.UpdateServer("Insert mpq_new(Dayhour,Quantity,Line,ELine, PartNo, So, DateID, WeekID, MonthID, QuarterID,starttime,endtime,ShiftAB, Onot, Rework,station,type,Mark) values ( '" + sHourid + "', '1', '" + sLine + "','" + sEline + "','" + sPartNo + "','" + curBarcode_So + "'," +
                            "'" + sDate + "', '" + sWeekID + "','" + sMonthID + "','" + sQuarterID + "','" + sDatetime + "','" + sDatetime + "', '" + sShiftPeriod + "',  '" + smark + "', '" + sRework + "','T300', '" + sType + "','Y')" )==false)
                        {
                            if (retrytime < 3)
                            {
                                System.Threading.Thread.Sleep(50);
                                retrytime++;
                                goto up2;
                            }
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                
                try
                {
                    retrytime = 0;
                    rdr.Close();
                    b_changeTime = true;
                    GetDateID(sDatetime, out sHourid, out sDateid, out sWeekID, out sMonthID, out sQuarterID);
                    up3:
                    if (Sql.UpdateServer("Insert mpq_new(Dayhour,Quantity,Line,ELine, PartNo, So, DateID, WeekID, MonthID, QuarterID,starttime,endtime,ShiftAB, Onot, Rework,station,type,Mark) values ( '" + sHourid + "', '1', '" + sLine + "','" + sEline + "','" + sPartNo + "','" + curBarcode_So + "'," +
                        "'" + sDate + "', '" + sWeekID + "','" + sMonthID + "','" + sQuarterID + "','" + sDatetime + "','" + sDatetime + "', '" + sShiftPeriod + "',  '" + smark + "', '" + sRework + "','T300', '" + sType + "','Y')")==false)
                    {
                        if (retrytime < 3)
                        {
                            System.Threading.Thread.Sleep(50);
                            retrytime++;
                            goto up3;
                        }
                    }
                     
                     
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

            }
            rdr.Close();
        }

        void GetDateID(DateTime sDatetime, out string sHourid, out string sDateid, out string sWeekID, out string sMonthID, out string sQuarterID)
        {
            string sTime = string.Empty;
            string strDate = string.Empty;
            string sWeekID1 = string.Empty;
            string sTemp = string.Empty;
            int iQ = 0;
            DateTime tDate, tDate1;
            sTime = sDatetime.ToString("HH");
            sHourid = sDatetime.ToString("yyyyMMdd") + sTime;
            if (Convert.ToInt16(sTime) >= 8)
            {
                tDate = sDatetime;
            }
            else
            {
                tDate = Convert.ToDateTime(sDatetime.AddDays(-1).ToString("yyyy-MM-dd"));
            }
            sDateid = tDate.ToString("yyyyMMdd");
            sWeekID = ((tDate.DayOfYear + Convert.ToInt32(Convert.ToDateTime(tDate.ToString("yyyy") + "/01" + "/01").DayOfWeek) - 1) / 7 + 1).ToString();
            if (Convert.ToInt32(tDate.DayOfWeek) + 1 >= 5)
            {
                sWeekID = Convert.ToString(Convert.ToInt16(sWeekID) + 1);
            }

            if (sWeekID.Length > 1)
            {
                sWeekID = sDatetime.ToString("yyyy") + sWeekID;
            }
            else
            {
                sWeekID = sDatetime.ToString("yyyy") + "0" + sWeekID;
            }

            tDate1 = Convert.ToDateTime(sDatetime.AddMonths(1).ToString("yyyy-MM"));
            sWeekID1 = ((tDate1.DayOfYear + Convert.ToInt32(Convert.ToDateTime(tDate1.ToString("yyyy") + "/01" + "/01").DayOfWeek) - 1) / 7 + 1).ToString();
            if (Convert.ToInt32(tDate1.DayOfWeek) + 1 >= 5)
            {
                sWeekID1 = Convert.ToString(Convert.ToInt16(sWeekID1) + 1);
            }
            sWeekID1 = tDate1.ToString("yyyy") + sWeekID1;
            if (sWeekID == sWeekID1)
            {
                sMonthID = tDate1.ToString("yyyyMM");
            }
            else
            {
                sMonthID = tDate.ToString("yyyyMM");
            }
            sTemp = sMonthID.Substring(sMonthID.Length - 2, 2);//Right 取2位
            //iQ = Convert.ToInt16(sMonthID.Substring(sMonthID.Length -2),2);  
            iQ = Convert.ToInt16(Convert.ToInt16(sTemp) - 1) / 3;
            iQ = iQ + 1;
            sQuarterID = sMonthID.Substring(0, 4).ToString() + iQ.ToString();
        }

        public static int Weekday(DateTime dt, DayOfWeek startOfWeek)
        {
            return (dt.DayOfWeek - startOfWeek + 7) % 7;
        }
        void getShiftPeriod(DateTime sDatetime, out string sDate, out string sShift)
        {
            string sTime = string.Empty;
            string hh = string.Empty;
            string mm = string.Empty;

            sDate = sDatetime.ToString("yyyyMMdd");
            sTime = sDatetime.ToString("HHmmss");
            hh = sDatetime.ToString("HH");
            mm = sDatetime.ToString("mm");
            if (Convert.ToInt16(hh) >= 8 && Convert.ToInt16(hh) < 20)
            {
                sShift = "A";
            }
            else
            {
                sShift = "B";
            }
        }

        void getQtyHour(string sLine, DateTime sDatetime, out string Update_Item, out string Update_Rate, string sShiftAB, out string iPeriod)
        {
            string Stand_Time = string.Empty;
            long iHourdiff = 0;
            long iiPeriod = 0;

            Update_Item = "";
            Update_Rate = "";
            getDayTimeStart(sDatetime, sShiftAB, out Stand_Time);

            //TimeSpan timeSpan= sDatetime- Convert.ToDateTime (Stand_Time);
            iHourdiff = Convert.ToInt32(DiffMinutes(Convert.ToDateTime(Stand_Time), Convert.ToDateTime(sDatetime)));
            if (iHourdiff < 0)
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO1_QTY";
                    Update_Rate = "NO1_PF";
                    iiPeriod = 1;
                }
                else
                {
                    Update_Item = "NO13_QTY";
                    Update_Rate = "NO13_PF";
                    iiPeriod = 13;
                }
            }
            else if (iHourdiff < 60)   // '8:00~8:59
            {
                Update_Item = "NO1_QTY";
                Update_Rate = "NO1_PF";
                iiPeriod = 1;
            }
            else if (iHourdiff < 120 && iHourdiff >= 60)  //9:00~09:59
            {
                Update_Item = "NO2_QTY";
                Update_Rate = "NO2_PF";
                iiPeriod = 2;
            }
            else if (iHourdiff < 180 && iHourdiff >= 120)  //10:00~10:59
            {
                Update_Item = "NO3_QTY";
                Update_Rate = "NO3_PF";
                iiPeriod = 3;
            }
            else if (iHourdiff < 240 && iHourdiff >= 180)  // 10:00~11:59
            {
                Update_Item = "NO4_QTY";
                Update_Rate = "NO4_PF";
                iiPeriod = 4;
            }
            else if (iHourdiff < 300 && iHourdiff >= 240)  // 12:00~12:59
            {
                Update_Item = "NO5_QTY";
                Update_Rate = "NO5_PF";
                iiPeriod = 5;
            }
            else if (iHourdiff < 360 && iHourdiff >= 300)  // 13:00~13:59
            {
                Update_Item = "NO6_QTY";
                Update_Rate = "NO6_PF";
                iiPeriod = 6;
            }
            else if (iHourdiff < 420 && iHourdiff >= 360)  // 14:00~14:59
            {
                Update_Item = "NO7_QTY";
                Update_Rate = "NO7_PF";
                iiPeriod = 7;
            }
            else if (iHourdiff < 480 && iHourdiff >= 420)  // 15:00~15:59
            {
                Update_Item = "NO8_QTY";
                Update_Rate = "NO8_PF";
                iiPeriod = 8;
            }
            else if (iHourdiff < 540 && iHourdiff >= 480)  // 16:00~16:59
            {
                Update_Item = "NO9_QTY";
                Update_Rate = "NO9_PF";
                iiPeriod = 9;
            }
            else if (iHourdiff < 600 && iHourdiff >= 540)  // 17:00~17:59
            {
                Update_Item = "NO10_QTY";
                Update_Rate = "NO10_PF";
                iiPeriod = 10;
            }
            else if (iHourdiff < 660 && iHourdiff >= 600)  // 18:00~18:59
            {
                Update_Item = "NO11_QTY";
                Update_Rate = "NO11_PF";
                iiPeriod = 11;
            }
            else if (iHourdiff < 720 && iHourdiff >= 660)   //19:00~19:59
            {
                Update_Item = "NO12_QTY";
                Update_Rate = "NO12_PF";
                iiPeriod = 12;
            }
            else if (iHourdiff < 780 && iHourdiff >= 720)  //  20:00~20:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO13_QTY";
                    Update_Rate = "NO13_PF";
                    iiPeriod = 13;
                }
            }
            else if (iHourdiff < 840 && iHourdiff >= 780)  //  21:00~21:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO14_QTY";
                    Update_Rate = "NO14_PF";
                    iiPeriod = 14;
                }
            }
            else if (iHourdiff < 900 && iHourdiff >= 840)  //  22:00~22:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO15_QTY";
                    Update_Rate = "NO15_PF";
                    iiPeriod = 15;
                }
            }
            else if (iHourdiff < 960 && iHourdiff >= 900)  //  23:00~23:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO16_QTY";
                    Update_Rate = "NO16_PF";
                    iiPeriod = 16;
                }
            }
            else if (iHourdiff < 1020 && iHourdiff >= 960)   // 00:00~00:59
            {
                Update_Item = "NO17_QTY";
                Update_Rate = "NO17_PF";
                iiPeriod = 17;
            }
            else if (iHourdiff < 1080 && iHourdiff >= 1020)   // 01:00~01:59
            {
                Update_Item = "NO18_QTY";
                Update_Rate = "NO18_PF";
                iiPeriod = 18;
            }
            else if (iHourdiff < 1140 && iHourdiff >= 1080)   // 02:00~02:59
            {
                Update_Item = "NO19_QTY";
                Update_Rate = "NO19_PF";
                iiPeriod = 19;
            }
            else if (iHourdiff < 1200 && iHourdiff >= 1140)   // 03:00~03:59
            {
                Update_Item = "NO20_QTY";
                Update_Rate = "NO20_PF";
                iiPeriod = 20;
            }
            else if (iHourdiff < 1260 && iHourdiff >= 1200)   // 04:00~04:59
            {
                Update_Item = "NO21_QTY";
                Update_Rate = "NO21_PF";
                iiPeriod = 21;
            }
            else if (iHourdiff < 1320 && iHourdiff >= 1260)   // 05:00~05:59
            {
                Update_Item = "NO22_QTY";
                Update_Rate = "NO22_PF";
                iiPeriod = 22;
            }
            else if (iHourdiff < 1380 && iHourdiff >= 1320)   // 06:00~06:59
            {
                Update_Item = "NO23_QTY";
                Update_Rate = "NO23_PF";
                iiPeriod = 23;
            }
            else if (iHourdiff < 1440 && iHourdiff >= 1380)   // 07:00~07:59
            {
                Update_Item = "NO24_QTY";
                Update_Rate = "NO24_PF";
                iiPeriod = 24;
            }
            else if (iHourdiff > 1440)
            {
                Update_Item = "NO24_QTY";
                Update_Rate = "NO24_PF";
                iiPeriod = 24;
            }
            iPeriod = Convert.ToString(iiPeriod);
        }
        void getDayTimeStart(DateTime sDatetime, string sShiftAB, out string Stand_Time)
        {
            string sDate = string.Empty;
            string sHour = string.Empty;
            sHour = sDatetime.ToString("HH");
            if (Convert.ToInt16(sHour) >= 0 && Convert.ToInt16(sHour) < 8)
            {
                if (sShiftAB == "A")
                {
                    sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                }
                else
                {
                    sDate = sDatetime.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
                }
            }
            else if (Convert.ToInt16(sHour) >= 8 && Convert.ToInt16(sHour) < 20)
            {
                if (sShiftAB == "B")
                {
                    if (Convert.ToInt16(sHour) <= 12)
                    {
                        sDate = sDatetime.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
                    }
                    else
                    {
                        sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                    }
                }
                else
                {
                    sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                }
            }
            else
            {
                sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
            }
            Stand_Time = sDate;
        }
        void GetShift(out string shift)
        {
            string sDatetime = string.Empty;
            string sDate = string.Empty;
            string iHour = string.Empty;
            DateTime g_dSql_time;
            GetSqltime(out g_dSql_time);
            iHour = g_dSql_time.ToString("HH");
            if (Convert.ToInt16(iHour) >= 8 && Convert.ToInt16(iHour) < 20)
            {
                shift = "A";
                sDate = g_dSql_time.ToString("yyyyMMdd");
            }
            else
            {
                shift = "B";
            }
        }
        void getSOMark(string sShift, out string sDate, out string sHour, out string smark)
        {
            DateTime sDatetime;
            string sCurDate = string.Empty;
            string sNormalShift = string.Empty;
            string iHour = string.Empty;
            sDatetime = curBarcode_sTime;
            sCurDate = sDatetime.ToString("yyyyMMdd");
            iHour = sDatetime.ToString("HH");
            if (sShift == "A")
            {
                if (Convert.ToInt16(iHour) >= 0 && Convert.ToInt16(iHour) < 17)
                {
                    smark = "ON";
                }
                else
                {
                    smark = "OFF";
                }
                sDate = sDatetime.ToString("yyyyMMdd");
            }
            else
            {
                if (Convert.ToInt16(iHour) >= 0 && Convert.ToInt16(iHour) < 5)
                {
                    smark = "ON";
                    sDate = sDatetime.AddDays(-1).ToString("yyyyMMdd");
                }
                else
                {
                    if (Convert.ToInt16(iHour) >= 5 && Convert.ToInt16(iHour) < 17)
                    {
                        smark = "OFF";
                        sDate = sDatetime.AddDays(-1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        smark = "ON";
                        sDate = sDatetime.ToString("yyyyMMdd");
                    }
                }
            }
            iHour = sDatetime.ToString("HH");
            sHour = sCurDate + iHour;
        }
        bool FWU_MainTestProcess()
        {
            if (SDriverX.g_modelInfo.sSn.Substring(0, 2).ToUpper() == "NA")
            {
                if (FWU_Arran() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sSn.Substring(0, 2).ToUpper() == "NB")
            {
                if (FWU_Barra() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sSn.Substring(0, 2).ToUpper() == "NC")
            {
                if (FWU_Cardhu() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sSn.Substring(0, 2).ToUpper() == "NE")
            {
                if (FWU_Edradour() == false) return false;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 2).ToUpper() == "HA")
            {
               
                if (FWU_Harr() == false) return false;
                if (Harris_usb() == false) return false;

            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 2).ToUpper() == "KA")
            {
                if (FWU_Kava() == false) return false;
            }
            else
            {
                SetMsg("当前料号信息未维护, Thông tin mã vật tư hiện tại chưa được bảo trì", UDFs.UDF.COLOR.FAIL);
                return false;
            }

            if (GlobalConfig.sMesEid.ToUpper().Contains ("QRE"))
            {
                if (Database.Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + mod.sSn + "','" + GlobalConfig.sMesEid + "','times:" + g_iTimerCycle.ToString() + "','QRE', 'OK')") == false)
                {
                    SetMsg("上传测试记录失败, Tải lên bản ghi kiểm tra thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            // 上传 Oracle
            else
            {
                if (GlobalConfig.iRunMode == 1)
                {
                    if (Database.Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + SDriverX.g_modelInfo.sSn + "','" + GlobalConfig.sMesEid.Substring(5, 3) + "','times:" + g_iTimerCycle.ToString() + "','FWU', 'OK')") == false)
                    {
                        SetMsg("上传测试记录失败, Tải lên bản ghi kiểm tra thất bại", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        SetMsg("上传测试记录成功, Tải lên bản ghi kiểm tra thành công", UDF.COLOR.WORK);
                    }
                }
                else
                {
                    SetMsg("Run mode = 0 不上傳測試紀錄, Không tải lên bản ghi kiểm tra", UDF.COLOR.WORK);
                }
            }

            int retrytime = 0;
            this.Refresh();
            isrt:
            DateTime dt1 = DateTime.Now;
            SetMsg("保存分位开始, Bắt đầu lưu phiên bản phần mềm " + FW_TEMP, UDF.COLOR.WORK);

            OracleDataReader reader;
            string sql = "select FW_DATA from RKNMGR.Insp_Xiaomi_Fw where  SSN='" + SDriverX.g_modelInfo.sSn + "'  order by SYSDAT DESC";
            if (Database.Oracle.ServerExecute(sql, out reader) == false)
            {
                retrytime++;
                if (retrytime < 6)
                {
                    goto isrt;
                }
                SetMsg("数据库err", UDF.COLOR.FAIL);
                return false ;
            }
            else
            {
                if (reader.HasRows)
                {
                    sql = "update RKNMGR.Insp_Xiaomi_Fw set FW_DATA='"+ FW_TEMP + "'where SSN='"+ SDriverX.g_modelInfo.sSn + "'";
                    if (Database.Oracle.UpdateServer(sql) == false)
                    {
                        retrytime++;
                        if (retrytime < 6)
                        {
                            goto isrt;
                        }
                        SetMsg("数据库更新分位失败", UDF.COLOR.FAIL);
                        MessageBox.Show("数据库更新分位失败");
                        return false;
                    }
                }
                else
                {
                    sql = "insert into RKNMGR.Insp_Xiaomi_Fw(SSN,MODEL,WORKORDER,FW_DATA,SYSDAT,REMARK) values ('" + SDriverX.g_modelInfo.sSn + "','" + SDriverX.g_modelInfo.sPart_no + "','" + SDriver.SDriverX.g_modelInfo.sWord + " ','" + FW_TEMP + "', to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'), 'OK')";
                    if (Database.Oracle.UpdateServer(sql) == false)
                    {
                        retrytime++;
                        if (retrytime < 6)
                        {
                            goto isrt;
                        }
                        SetMsg("数据库插入分位失败", UDF.COLOR.FAIL);
                        MessageBox.Show("数据库插入分位失败");
                        return false;
                    }
                }
            }
            SetMsg("fw — save —ok", UDF.COLOR.WORK);
            return true;
        }
        bool FWU_MainTestProcess1()
        {
            if (mod.sSn.Substring(0, 2).ToUpper() == "NA")
            {
                if (FWU_Arran() == false) return false;
            }
            else if (mod.sSn.Substring(0, 2).ToUpper() == "NB")
            {
                if (FWU_Barra() == false) return false;
            }
            else if (mod.sSn.Substring(0, 2).ToUpper() == "NC")
            {
                if (FWU_Cardhu() == false) return false;
            }
            else if (mod.sSn.Substring(0, 2).ToUpper() == "NE")
            {
                if (FWU_Edradour() == false) return false;
            }
            else if (mod.sPart_no.Substring(13, 2).ToUpper() == "HA")
            {
                if (FWU_Harr() == false) return false;
                if (Harris_usb() == false) return false;
            }
            else if (mod.sPart_no.Substring(13, 2).ToUpper() == "KA")
            {
                if (FWU_Kava() == false) return false;
            }
            else
            {
                SetMsg("当前料号信息未维护", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            pass:
            if (GlobalConfig.sMesEid.ToUpper().Contains("QRE"))
            {
                if (Database.Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + mod.sSn + "','" + GlobalConfig.sMesEid + "','times:" + g_iTimerCycle.ToString() + "','QRE', 'OK')") == false)
                {
                    SetMsg("上传测试记录失败", UDF.COLOR.FAIL);
                    return false;
                }
            }
            // 上传 Oracle
            else
            {
                if (Database.Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + mod.sSn + "','" + GlobalConfig.sMesEid.Substring(5, 3) + "','times:" + g_iTimerCycle.ToString() + "','FWU', 'OK')") == false)
                {
                    SetMsg("上传测试记录失败", UDF.COLOR.FAIL);
                    return false;
                }
            }


            int retrytime = 0;
            this.Refresh();
            isrt:
            DateTime dt1 = DateTime.Now;
            SetMsg("保存分位开始" + FW_TEMP, UDF.COLOR.WORK);
            //if (FW_TEMP=="")
            //{
            //    MessageBox.Show("分位为空");
            //    return;
            //}            
            if (GlobalConfig.sMesEid.ToUpper().Contains("FWU"))
            { 
                OracleDataReader reader;
                string sql = "select FW_DATA from RKNMGR.Insp_Xiaomi_Fw where  SSN='" + mod.sSn + "'  order by SYSDAT DESC";
                if (Database.Oracle.ServerExecute(sql, out reader) == false)
                {
                    retrytime++;
                    if (retrytime < 6)
                    {
                        goto isrt;
                    }
                    SetMsg("数据库err", UDF.COLOR.FAIL);
                    //MessageBox.Show("没有读到存储分位");                    
                    return false;
                }
                else
                {
                    if (reader.HasRows)
                    {
                        //string m = "";
                        //while (reader.Read())
                        //{
                        //      m = reader["FW_DATA"].ToString();
                        //}

                        //if (m=="OK")
                        //{
                        //sql = ("insert RKNMGR.Insp_Xiaomi_Fw(SSN,MODEL,WORKORDER,FW_DATA,SYSDAT,REMARK) values ('" + SDriverX.g_modelInfo.sSn + "','" + SDriverX.g_modelInfo.sPart_no + "','" + SDriver.SDriverX.g_modelInfo.sWord + " ','" + FW_TEMP + "', to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'), 'OK')");
                        sql = "update RKNMGR.Insp_Xiaomi_Fw set FW_DATA='" + FW_TEMP + "'where SSN='" + mod.sSn + "'";
                        if (Database.Oracle.UpdateServer(sql) == false)
                        {
                            retrytime++;
                            if (retrytime < 6)
                            {
                                goto isrt;
                            }
                            SetMsg("数据库更新分位失败", UDF.COLOR.FAIL);
                            MessageBox.Show("数据库更新分位失败");
                            return false;
                        }
                        //}

                    }
                    else
                    {
                        sql = "insert into RKNMGR.Insp_Xiaomi_Fw(SSN,MODEL,WORKORDER,FW_DATA,SYSDAT,REMARK) values ('" + mod.sSn + "','" + mod.sPart_no + "','" + mod.sWord + " ','" + FW_TEMP + "', to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'), 'OK')";
                        if (Database.Oracle.UpdateServer(sql) == false)
                        {
                            retrytime++;
                            if (retrytime < 6)
                            {
                                goto isrt;
                            }
                            SetMsg("数据库插入分位失败", UDF.COLOR.FAIL);
                            MessageBox.Show("数据库插入分位失败");
                            return false;
                        }
                    }
                }

            }
            SetMsg("fw — save —ok", UDF.COLOR.WORK);
            return true;
        }
        bool FTA_MainTestProcess()
        {
            //检查料号
            //AXXUSNFB----.NBARB11
            if (SDriverX.g_modelInfo.sPart_no.Substring(9, 3).ToUpper() == "AO1")
            {
                if (FTA_Cardhu() == false) return false;
            }

            
            

            return true;
        }
    }
}
