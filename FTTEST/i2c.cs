using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
 
using System.IO;
using System.IO.Ports;
using System.Threading;
 
namespace FTTEST
{
    class i2c
    {
        public const byte OFFSET_IO = 0X0;
        public const byte OFFSET_I2C = 0X4;
        public const byte I2CSTA = 0X4; //'已经包含了Offet的值；
        public const byte INDPTR = 0X4;
        public const byte I2CDAT = 0X5;
        public const byte I2CCON = 0X7;
        public const byte INDIRECT = 0X6;

        public const byte STA_START = 0X8;
        public const byte STA_RESTART = 0X10;
        public const byte STA_SLA_W = 0X18;
        public const byte STA_DAT_W = 0X28;
        public const byte STA_SLA_R = 0X40;
        public const byte STA_DAT_R = 0X50;
        public const byte STA_DAT_R_NA = 0X58;
        public bool g_bEdidToOSD;
        public String g_AcerStartSN;
        public Int32 m_nInput, m_nBasePort, m_nINTNo, m_nBuf;
        public Int32 g_nMicSec, m_nTryTimes;
        byte m_btOutput;
        bool m_bI2CRunning, bAcerCI;
        String m_strBuf;
        byte[] m_btArray = new byte[256];
        byte m_btI2CCON, m_btI2CSTA, m_btI2CDAT;
        Int16 iIsDDCLoadInTime, iIsDDCLoadInTimes;
        [DllImport("dlportio.dll")]
        public static extern byte DlPortReadPortUchar(Int32 port);
        [DllImport("dlportio.dll")]
        public static extern void DlPortWritePortUchar(Int32 port, byte value);
        [DllImport("kernel32")]
        public static extern void Sleep(Int32 dwMilliseconds);

        public bool ByteStringToArray(string strArr, ref byte[] byteArr)
        {
            try
            {
                string[] tArr = strArr.Split(',');
                byteArr = new byte[tArr.Length];
                for (int i = 0; i < tArr.Length; i++)
                {
                    byteArr[i] = (byte)Convert.ToInt32(tArr[i], 16);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("1ERR:" + ex);
                return false;
            }
        }
        public void  checksum(ref byte[] byteArr)
        {
            for (int i = 0; i < byteArr.Length - 1; i++)
            {
                byteArr[byteArr.Length - 1] = (byte)(byteArr[byteArr.Length - 1] ^ byteArr[i]);
            }
        }
    public void IIC_InitIOcard()
        {
            m_nInput = 0;
            m_btOutput = 0;
            m_btI2CCON = 0;
            m_btI2CSTA = 0;
        }
    public bool IIC_SetBasePortInFile(string strfile)
        {
            string strbuf = "0";
            try
            {
                strbuf = wbc.iniFile.GetPrivateStringValue("I2CCard", "BaseAddr", strfile);
                if (wbc.iniFile.Left(strbuf, 2).ToUpper() == "&H") { strbuf = strbuf.ToUpper().Replace("&H", "0x"); }
                if (strbuf == "" || strbuf == "0")
                {
                    strbuf = "0X340";
                    //return false;
                }
                m_nBasePort = Convert.ToInt16(strbuf, 16);
                if ((m_nBasePort > 0X358) || (m_nBasePort < 0X340))
                {
                    m_nBasePort = 0X340;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("异常" + e);
                return false;
            }

            return true;
        }
    public void IIC_SetBasePort(Int16 nPort)
        {
            if (nPort > 358 || nPort < 340)
            {
                MessageBox.Show("BASEPORT配置不在范围340-358");
                return;
            }
            m_nBasePort = nPort;
        }
    public void Dlyus(int nCount)
        {
            long i;
            for (i = 0; i <= (g_nMicSec) * nCount; i++)
            {
                Application.DoEvents();
            }
        }
        public bool Start()
        {
             
            m_nTryTimes = 0;
            m_btI2CCON = DlPortReadPortUchar((m_nBasePort + I2CCON));
            // m_btI2CCON = (Byte)((m_btI2CCON & temp[1]) | temp[2]);//'启动STA =1,清除SI；
            m_btI2CCON = (byte)((m_btI2CCON & 0X66) | 0X20);//'启动STA =1,清除SI；
           // MessageBox.Show("s132:" + m_btI2CCON);
            DlPortWritePortUchar((m_nBasePort + I2CCON), m_btI2CCON);
            do
            {

                Dlyus(1);
                Application.DoEvents();
                m_btI2CCON = (byte)DlPortReadPortUchar((m_nBasePort + I2CCON));

                m_nTryTimes = (Int16)(m_nTryTimes + 1);
                if (m_nTryTimes > 30)
                { return false; }
            }
            while ((m_btI2CCON & 0X8) == 0);
            //MessageBox.Show("s2:" + m_btI2CCON);
            m_btI2CSTA = DlPortReadPortUchar((m_nBasePort + I2CSTA));
            //MessageBox.Show("s3:" + m_btI2CCON);
            if (m_btI2CSTA != STA_START && m_btI2CSTA != STA_RESTART)
            {
                 return false;
            }
            return true;
        }
        public bool IIC_ResetI2C()
        {
           // byte[] temp = new byte[3];

            try
            {
                DlPortWritePortUchar((m_nBasePort + INDPTR), 0x05);
                DlPortWritePortUchar((m_nBasePort + INDIRECT), 0xA5);
                DlPortWritePortUchar((m_nBasePort + INDIRECT), 0X5A);
                Sleep(5);
                IIC_InitialByteMode();
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }
            return true;
        }
        public bool IIC_InitialByteMode()
        {
            try
            {
                m_btI2CCON = DlPortReadPortUchar( (m_nBasePort + I2CCON));
                //'AA设置为0，不能进入Slave mode.
                m_btI2CCON = Convert.ToByte((m_btI2CCON & 0x6) | 0X40);
                //MessageBox.Show("InitialByteMode：" + m_btI2CCON);
                DlPortWritePortUchar((m_nBasePort + I2CCON), m_btI2CCON);
                Sleep(1);// '至少delay 550uS才可以启动9665N的动作；
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }
            return true;
        }
        public bool RepeatStart()
        {

            byte[] temp = new byte[3];
            
            m_nTryTimes = 0;
            m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
            m_btI2CCON = (Byte)((m_btI2CCON & 0X66) | 0X20);//'启动STA =1,清除SI；
            DlPortWritePortUchar(m_nBasePort + I2CCON, m_btI2CCON);
            do
            {
                Dlyus(1);
                Application.DoEvents();
                m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
                m_nTryTimes = (m_nTryTimes + 1);
                if (m_nTryTimes > 30) { return false; }
            }
            while ((m_btI2CCON & 0x8) == 0);
            m_btI2CSTA = DlPortReadPortUchar(m_nBasePort + I2CSTA);
            if (m_btI2CSTA != STA_RESTART)
            {
                return false;
            }
            return true;

        }
        public bool StopD()
        {

            m_nTryTimes = 0;
            m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
            m_btI2CCON = Convert.ToByte((m_btI2CCON & 0x46) | 0x10);// '启动STO =1,清除SI；
            DlPortWritePortUchar(m_nBasePort + I2CCON, m_btI2CCON);
            Dlyus(10);
            return true;
        }
        public bool WriteD(byte btData, byte btSTA)
        {
            m_nTryTimes = 0;
            DlPortWritePortUchar((m_nBasePort + I2CDAT), btData);
            //清除SI位,清除STA，STO；
            m_btI2CCON = DlPortReadPortUchar((m_nBasePort + I2CCON));
            m_btI2CCON = Convert.ToByte(m_btI2CCON & 0XC6);
             DlPortWritePortUchar(m_nBasePort +I2CCON, m_btI2CCON);
            Dlyus(120);
            do
            {
                Dlyus(5);
                //Application.DoEvents();
                m_btI2CCON = DlPortReadPortUchar((m_nBasePort + I2CCON));
                m_nTryTimes = (m_nTryTimes + 1);
                if (m_nTryTimes > 300)
                { return false; }
            } while ((m_btI2CCON & 0x8) == 0);
            Dlyus(5);
            m_btI2CSTA = DlPortReadPortUchar(m_nBasePort + I2CSTA);
            if (m_btI2CSTA != btSTA) { return false; }
            return true;
        }
        public Byte ReadD(byte btSTA)
        {

            byte ReadD = 0xFF;
            int m_nTryTimes = 0;
            //'清除SI位；需要回ACK;
            m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
            m_btI2CCON = Convert.ToByte((m_btI2CCON & 0xf7) | 0x80);
            DlPortWritePortUchar(m_nBasePort + I2CCON, m_btI2CCON);
            //'Read I2CCON if SI is set then read I2CSTA and I2CDAT
            Dlyus(105);
            do
            {
                Dlyus(1);
                Application.DoEvents();
                m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
                m_nTryTimes = m_nTryTimes + 1;
                if (m_nTryTimes > 300) { return 0; }
            } while ((m_btI2CCON & 0x8) == 0);
            m_btI2CSTA = DlPortReadPortUchar(m_nBasePort + I2CSTA);
            if (m_btI2CSTA != btSTA) { return ReadD; }
            Dlyus(1);
            m_btI2CDAT = DlPortReadPortUchar(m_nBasePort + I2CDAT);
            return m_btI2CDAT;

        }
        public Byte ReadE(byte btSTA)
        {

            byte ReadE = 0xff;
            int m_nTryTimes = 0;
            //'清除SI位；需要回ACK;
            m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
            m_btI2CCON = Convert.ToByte(m_btI2CCON & 0x77);
            DlPortWritePortUchar(m_nBasePort + I2CCON, m_btI2CCON);
            //'Read I2CCON if SI is set then read I2CSTA and I2CDAT
            Dlyus(45);
            do
            {
                Dlyus(1);
                Application.DoEvents();
                m_btI2CCON = DlPortReadPortUchar(m_nBasePort + I2CCON);
                m_nTryTimes = m_nTryTimes + 1;
                if (m_nTryTimes > 3) { return ReadE; }
            } while ((m_btI2CCON & 0x8) == 0);
            m_btI2CSTA = DlPortReadPortUchar(m_nBasePort + I2CSTA);
            if (m_btI2CSTA != btSTA) { return ReadE; }
            Dlyus(1);
            m_btI2CDAT = DlPortReadPortUchar(m_nBasePort + I2CDAT);
            return m_btI2CDAT;

        }
        public bool DDC_Read(Int16 nBegin, Int16 nEnd, ref byte[] btReturn)
        {
            int nIndex = 0;
            btReturn = new byte[nEnd+1];
            if (Start() == false) { return false; }
            if (WriteD(0xA0, STA_SLA_W) == false) { return false; }
            if (WriteD((byte)(nBegin), STA_DAT_W) == false) { return false; }
            StopD();
            if (Start() == false) { return false; }
            if (WriteD(0xA1, STA_SLA_R) == false) { return false; }
            for (nIndex = nBegin; nIndex <= nEnd - 1; nIndex++)
            {
                Application.DoEvents();
                btReturn[nIndex] = ReadD(STA_DAT_R);
            }
            btReturn[nEnd] = ReadE(STA_DAT_R_NA);
            StopD();
            return true;
        }
        public bool DDC_WriteBank(bool bIs256Bytes, byte[] btWrite)
        {
            Int32 nIndexR, nIndexB;
            byte btBuf;
            int nBankCount;
            btWrite = new byte[0];
            nBankCount = 1;
            if (bIs256Bytes == true)
            {
                nBankCount = 2;
            }
            for (nIndexR = 0; nIndexR < nBankCount * 16; nIndexR++)
            {
                Application.DoEvents();
                if (Start() == false) { return false; }
                if (WriteD(0xA0, STA_SLA_W) == false) { return false; }
                btBuf = (byte)(nIndexR * 8);
                if (WriteD(0xA0, STA_SLA_W) == false) { return false; }
                for (nIndexB = 0; nIndexB <= 7; nIndexB++)
                {
                    Application.DoEvents();
                    if (WriteD(btWrite[nIndexB + nIndexR * 8], STA_DAT_W) == false) { return false; }
                }
                StopD();
                Sleep(20);
            }
            return true;
        }
        public bool DDC_WriteByte(Int16 nAddr, byte btData)
        {
            byte btBuf;
            if (Start() == false) { return false; }
            if (WriteD(0xA0, STA_SLA_W) == false) { return false; }
            btBuf = (byte)nAddr;
            if (WriteD(btBuf, STA_DAT_W) == false) { return false; }
            if (WriteD(btData, STA_DAT_W) == false) { return false; }
            StopD();
            return true;
        }
        public bool checkI2Cres(ref byte[] ppr, ref string textppr, string tempres)
        {
            textppr = "";
            for (int nIndex = 0; nIndex < ppr.Length; nIndex++)
            {
                textppr += Convert.ToString(ppr[nIndex], 16).ToUpper() + ",";
            }

            switch (tempres)
            {
                case "RXCK_SET":
                    if (ppr[7] != 0xE0)
                    { return false; }
                    break;
                case "RXCK_POWER":
                    if (ppr[2] != 0xBE)
                    { return false; }
                    break;
                case "RXCK_POWEROFF":
                    if (ppr[6] != 0x31)
                    { return false; }
                    break;
                case "RXCK_GetUSB":
                    if (ppr[6] != 0x1)
                    { return false; }
                    break;
                case "RXCK_RT22":
                    if (ppr[5] != 0x6F)
                    { return false; }
                    break;
                case "RXCK_RT14":
                    if (ppr[5] != 0x6E)
                    { return false; }
                    break;
                case "RXCK_NONE":
                    break;
                case "RXCK_GET":
                    //Definition.PJ_Data.ComVer=ppr[7].ToString();
                    
                    break;
                case "RXCK_GET_PKEY":
                    if (ppr[6] != 0x1)
                    { return false; }
                    break;
                case "RXCK_KEY_ALL":
                    if (ppr[6] != 0x1 && ppr[7] != 0x3E)
                    { return false; }
                    break;
                case "RXCK_GET_ALL":
                    if (ppr[7] != 0x7E)
                    { return false; }
                    break;
                case "RXCK_GET_COUNT":
                     
                    break;
                case "RXCK_GET_KEY":
                    if (ppr[7] != 0xE0)
                    { return false; }
                    break;
                case "RXCK_GET_OVERLOCK":
                    if (ppr[7] != 0xE0 || ppr[6] != 0x0)
                    { return false; }
                    break;
                case "RXCK_GET_NUM":
                    //Definition.PJ_Data.ComVer = ppr[6].ToString();
                    break;

                default:
                    MessageBox.Show("No predefined RXCK type.");
                    break;

            }
            return true;
        }
        public bool   IIC_WriteCmd(byte[] btArray, int nStart, int nSendCount, ref byte[] btReturn, int nRetCount, int nDelayBeforeRead )
        {
            int nIndex;
            int retrytime = 0;
            try
            {
                //btReturn = new byte[0];
           re1:
                if (nDelayBeforeRead < 30) { nDelayBeforeRead = 30; }
                if (Start() == false) { return false; }
                //MessageBox.Show("11");
                if (WriteD(btArray[nStart], STA_SLA_W) == false)
                {
                    //wrlog
                    //MessageBox.Show("log");
                    wbc.iniFile.WriteTestLog("write SLA_W error" + DateTime.Now);
                    return false;
                }
                //MessageBox.Show("6");
                for (nIndex = nStart + 1; nIndex <= nStart + nSendCount-1; nIndex++)
                {
                    if (WriteD(btArray[nIndex], STA_DAT_W) == false)
                    {
                        //wrlog
                        wbc.iniFile.WriteTestLog("write DAT_W error" + DateTime.Now);
                        return false;
                    }
                }
               // MessageBox.Show("8");
                StopD();
                //Application.DoEvents();
                if (nDelayBeforeRead > 1000)
                {
                    for (int h = 0; h < 4; h++)
                    {
                        Application.DoEvents();
                        Sleep(1000);
                    }
                }
                else
                {
                    Sleep(nDelayBeforeRead);
                }
                   // SDriverX.Delay1(nDelayBeforeRead);
                
                if (nRetCount > 0)
                {
                    if (Start() == false) { return false; }
                    if (WriteD((byte)(btArray[nStart] + 1), STA_SLA_R) == false)
                    {
                        //wrlog
                        wbc.iniFile.WriteTestLog("write SLA_R error" + DateTime.Now);
                       return false;
                    }
                    Sleep(10);
                    btReturn = new byte[nRetCount];
                    for (nIndex = 0; nIndex <= nRetCount - 2; nIndex++)
                    {

                        int m = btReturn.Length;
                        btReturn[nIndex] =  ReadD(STA_DAT_R);
                        //Application.DoEvents();
                    }
                    btReturn[nIndex] = ReadE(STA_DAT_R_NA);
                    //if(Definition.PJ_Data.USB=="1")
                    //{
                    //    if (btReturn[7] != 0xE0)
                    //    {
                    //        retrytime++;
                    //        Sleep(nDelayBeforeRead+30* retrytime);
                    //        if (retrytime < 6)
                    //          { goto re1; }
                    //    }
                    //}
                    //MessageBox.Show("9");
                    StopD();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("IIC_WRITECMD_err:" + e);
                return false;
            }
            return true;
        }
        public bool ConfirmOSDSNtoEdidSN(ref string strSN, string strCustomer)
        {
            return true;

        }
        public bool IO_IsKeyIn(string strWaiting, bool NeedLoadIn, bool NeedLoadout, ref Int16 nKeyin)
        {
            ReRead:
            if (NeedLoadIn == true || NeedLoadout == true)
            {
                Sleep(50);
                Application.DoEvents();
                if (NeedLoadIn == true)
                {
                    nKeyin = 1;
                    return true;
                }
                else if (NeedLoadout == true)
                {
                    nKeyin = 0;
                    return true;
                }
                else
                {
                    if (strWaiting == "1")
                    {
                        goto ReRead;
                    }
                }
            }
            else
            {
                Sleep(50);
                Application.DoEvents();
                if (strWaiting == "1")
                {
                    goto ReRead;
                }
            }
            return true;
        }
        public bool IO_IsLoadIn(string strWaiting, bool NeedLoadIn)
        {
            int i = 0;
            try
            {
                reread:
                i++;
                //NeedLoadIn = Definition.PJ_Data.LampSFC;
                Application.DoEvents();
                m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                m_nInput = (Int16)(m_nInput % 2);
                if (m_nInput == 0 || NeedLoadIn == true)
                {
                    //SDriverX.Delay1(50); 
                    //Application.DoEvents();
                    m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                    m_nInput = (Int16)(m_nInput % 2);
                    if (m_nInput == 0 || NeedLoadIn == true)
                    {
                        return true;
                    }
                    else
                    {
                        if (strWaiting == "1")
                        {
                           // if (i<50)
                            //{
                                goto reread;
                           // }
                           // return false;
                        }
                    }
                }
                else
                {
                    Sleep(50);
                    //Application.DoEvents();
                    if (strWaiting == "1")
                    {
                        // if (i < 50)
                       // {
                            goto reread;
                        //}
                         return false;
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }
            finally
            {

            }
            return true;
        }
        public bool IsLoadIn(string strWaiting, Boolean g_bNeedLoadin)
        {
            try
            {
                reread:
                m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                m_nInput = (Int16)(m_nInput % 2);
                if (m_nInput == 0 || g_bNeedLoadin == true)
                {
                    Sleep(50);
                    Application.DoEvents();
                    m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                    m_nInput = (Int16)(m_nInput % 2);
                    if (m_nInput == 0 || g_bNeedLoadin == true)
                    {
                        return true;
                    }
                    else
                    {
                        if (strWaiting == "1") { goto reread; }
                    }
                }
                else
                {
                    Sleep(50);
                    Application.DoEvents();
                    if (strWaiting == "1") { goto reread; }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }
            finally
            {

            }
            return true;
        }
        public bool IsLoadOut(string strWaiting)
        {
            reread:
            try
            {
                m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                m_nInput = (Int16)(m_nInput % 2);
                if (m_nInput == 1)
                {
                    Sleep(50);
                    Application.DoEvents();
                    m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                    m_nInput = (Int16)(m_nInput % 2);
                    if (m_nInput == 1)
                    {
                        return true;
                    }
                    else
                    {
                        if (strWaiting == "1") { goto reread; }
                    }
                }
                else
                {
                    Sleep(50);
                    Application.DoEvents();
                    if (strWaiting == "1") { goto reread; }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }
            return true;
        }
        //**********************************
        public bool SetLoadOut(Int16 nMicroSec)
        {
            try
            {

                m_btOutput = (byte)(m_btOutput | 0X80);

                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                //FormReset.DOP0(0) = FormReset.Image0
                Sleep(nMicroSec);
                Application.DoEvents();
                m_btOutput = (byte)(m_btOutput ^ 0X80);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                // FormReset.DOP0(0) = FormReset.Image2
            }
            catch (Exception e)
            { MessageBox.Show("err:" + e); }
            return true;
        }
        public bool DDC_IO_IsLoadIn(string strWaiting, bool NeedLoadIn)
        {
            int iPLCTimes = 0;
            try
            {
                iPLCTimes = 0;
                if (iIsDDCLoadInTime == 0) { iIsDDCLoadInTime = 500; }
                if (iIsDDCLoadInTime == 0) { iIsDDCLoadInTime = 2; }
                reread:
                m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                m_nInput = (Int16)(m_nInput % 2);
                if (m_nInput == 0 || NeedLoadIn == true)
                {
                    Sleep(50);
                    Application.DoEvents();
                    m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                    m_nInput = (Int16)(m_nInput % 2);
                    if (m_nInput == 0 || NeedLoadIn == true)
                    {
                        return true;
                    }
                    else
                    {
                        if (strWaiting == "1") { goto reread; }
                    }
                }
                else
                {
                    Sleep(iIsDDCLoadInTime);
                    Application.DoEvents();
                    //iPLCTimes = iPLCTimes + 1;
                    if (iPLCTimes > iIsDDCLoadInTimes) { return false; }
                    if (strWaiting == "1") { goto reread; }
                }

            }
            catch (Exception e)
            { MessageBox.Show("err:" + e); }
            return true;
        }
        public bool IO_IsLoadOut(string strWaiting)
        {
            try
            {
                reread:
                m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                m_nInput = (Int16)(m_nInput % 2);
                if (m_nInput == 1)
                {
                    Sleep(50);
                    Application.DoEvents();
                    m_nInput = DlPortReadPortUchar(m_nBasePort + OFFSET_IO);
                    m_nInput = (Int16)(m_nInput % 2);
                    if (m_nInput == 1)
                    {
                        return true;
                    }
                    else
                    {
                        if (strWaiting == "1") { goto reread; }
                    }
                }
                else
                {
                    Sleep(50);
                    Application.DoEvents();
                    if (strWaiting == "1") { goto reread; }
                }
            }
            catch (Exception e)
            { MessageBox.Show("err:" + e); }
            return false;
        }


        //------导通DDC卡的第1与第14脚***************
        public bool IO_SetLoadOut(Int32 nMicroSec)
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0x80);

                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(nMicroSec);
                Application.DoEvents();
                //'断开DDC卡的第1与第14脚
                m_btOutput = (byte)(m_btOutput ^ 0x80);           // DlPortWritePortUchar m_nBasePort, Not (m_btOutput)
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }

            return true;
        }
        //导通DDC卡的第2与第15脚
        public bool IO_RemoteRFID(Int32 nMicroSec)
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0x40);

                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(nMicroSec);
                Application.DoEvents();
                //'断开DDC卡的第2与第15脚
                m_btOutput = (byte)(m_btOutput ^ 0x40);           // DlPortWritePortUchar m_nBasePort, Not (m_btOutput)
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }

            return true;
        }
        //导通DDC卡的第2与第15脚
        public bool IO_RemoteRFIDON()
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0x40);

                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(100);
                Application.DoEvents();
            }

            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }

            return true;
        }
        //'断开DDC卡的第2与第15脚
        public bool IO_RemoteRFIDOFF()
        {
            try
            {
                m_btOutput = (byte)(m_btOutput ^ 0x40);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));

                Sleep(100);
                Application.DoEvents();
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }

            return true;
        }
        public bool IO_RemoteTry(int nMicroSec)
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0x40);

                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(nMicroSec);
                Application.DoEvents();

                m_btOutput = (byte)(m_btOutput ^ 0x40);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));

                Sleep(100);
                Application.DoEvents();
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }

            return true;
        }

        //'导通DDC卡的第3与第16脚
        public bool IO_RemoteFAILON()
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0x20);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(100);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }
            return true;

        }
        //'断开DDC卡的第3与第16脚
        public bool IO_RemoteFAILOFF()
        {
            try
            {
                m_btOutput = (byte)(m_btOutput ^ 0x20);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(100);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }
            return true;

        }

        //'导通DDC卡的第4与第17脚
        public bool IO_RemoteLightON()
        {
            try
            {
                m_btOutput = (byte)(m_btOutput | 0X10);
                Sleep(100);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(300);

            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }
            return true;

        }
        //'断开DDC卡的第4与第17脚
        public bool IO_RemoteLightOFF()
        {

            try
            {
                m_btOutput = (byte)(m_btOutput ^ 0X10);
                Sleep(100);
                DlPortWritePortUchar(m_nBasePort, (byte)~(m_btOutput));
                Sleep(300);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                //return false;
            }
            return true;
        }
        public bool IO_RemoteOut(Int16 nCode, Int32 nKeepTime)
        {
            try
            {
                nCode = (Int16)((nCode & 0x7F) | 0x40);
                m_btOutput = (byte)((m_btOutput & 0x80) | nCode);
                DlPortWritePortUchar(m_nBasePort, (byte)(~m_btOutput));
                Sleep(nKeepTime);
                m_btOutput = (byte)(m_btOutput ^ 0x40);// 'reset the switch.
                DlPortWritePortUchar(m_nBasePort, (byte)(~m_btOutput));
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("err:" + e);
                return false;
            }

        }

        public byte DDC_CheckSum128(byte[] X)
        {
            int DDC_sum, i, ibuffer;
            byte chksum;
            DDC_sum = 0;
            for (i = 0; i < 127; i++)
            {
                DDC_sum = DDC_sum + X[i];
                Application.DoEvents();
            }
            ibuffer = (~(DDC_sum));
            chksum = (byte)((ibuffer + 1) & 0xFF);
            return chksum;

        }

        public bool IO_CARD_OUPUT1(int nMicroSec)
        {
            // FormReset.TimerIO.Enabled = False;
            int crc;
            byte[] w = new byte[7];
            w[1] = 0xAA;
            w[2] = 0xBB;
            w[3] = 0x1;
            w[4] = 0x1;
            w[5] = 0x1;

            crc = 0;
            crc = crc + w[1] + w[2] + w[3] + w[4] + w[5];
            crc = crc % 0x100;
            w[6] = (byte)crc;
            //FormReset.ComIO_CARD.Output = w
            //FormReset.DOP0(0) = FormReset.Image0
            Sleep(nMicroSec);
            Application.DoEvents();
            w[1] = 0xAA;
            w[2] = 0xBB;
            w[3] = 0x1;
            w[4] = 0x1;
            w[5] = 0x0;

            crc = 0;
            crc = crc + w[1] + w[2] + w[3] + w[4] + w[5];
            crc = crc % 0x100;
            w[6] = (byte)crc;
            //FormReset.ComIO_CARD.Output = w
            //FormReset.DOP0(0) = FormReset.Image2;
            return true;
        }
        public bool IO_CARD_OUPUT2(int nMicroSec)
        {
            // FormReset.TimerIO.Enabled = False;
            int crc;
            byte[] w = new byte[7];
            w[1] = 0xAA;
            w[2] = 0xBB;
            w[3] = 0x1;
            w[4] = 0x2;
            w[5] = 0x1;

            crc = 0;
            crc = crc + w[1] + w[2] + w[3] + w[4] + w[5];
            crc = crc % 0x100;
            w[6] = (byte)crc;
            //FormReset.ComIO_CARD.Output = w

            Sleep(nMicroSec);
            Application.DoEvents();
            w[1] = 0xAA;
            w[2] = 0xBB;
            w[3] = 0x1;
            w[4] = 0x2;
            w[5] = 0x0;

            crc = 0;
            crc = crc + w[1] + w[2] + w[3] + w[4] + w[5];
            crc = crc % 0x100;
            w[6] = (byte)crc;
            //FormReset.ComIO_CARD.Output = w

            return true;
        }

    }
}
