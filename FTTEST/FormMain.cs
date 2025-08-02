using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTTEST.UDFs;
using FTTEST.AppConfig;
using FTTEST.Database;
using FTTEST.SDriver;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FTTEST.UDFs.UDF;
//using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Web.Services;
using FTTEST.WebReference;

namespace FTTEST
{
    public partial class FormMain : Form
    {
        public static WebReference.AVTC_webservice webService = new AVTC_webservice();
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 窗口自动调节相关
        * --------------------------------------------------*/
        public static int g_iFormWidth = new int();  //窗口宽度
        public static int g_iFormHeight = new int(); //窗口高度
        public static int g_iStatusStripHeight = new int(); //状态栏
        public static float g_fWidthScaling = 1.0f; //宽度缩放比例
        public static float g_fHeightScaling = 1.0f; //高度缩放比例
        public static UDF.ComplexConSetting g_complexConSetting = new UDF.ComplexConSetting();
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 指令相关
        * --------------------------------------------------*/
        public static bool g_bRecvTvPortData = true;   //接收 Port 数据
        public static int g_iCmdDelay = new int();  //指令延时
        public static byte[] g_byCmdBuf = new byte[2048]; //命令缓冲区
        public static byte[] g_byCmdRtn = new byte[4096]; //命令回传值
        public static byte[] g_byGdmRtn = new byte[512];   //GDM
        public static string g_sCmdRtnHexText = string.Empty;    //命令回传值16进制文本
        public static string g_sCmdRtnStrText = string.Empty;    //命令回传值-普通字符串文本
        public static string g_sCmdRtnCurStrText = string.Empty;   //命令回传－实时文本
        public static string g_sGdmRtnText = string.Empty; //GDM回传值
        public static bool g_bCmdWorkFlag = new bool(); //命令运行标志
        public static bool g_bWorkFlag = new bool(); //程序运行flag
        public static bool g_bPortLogStart = new bool();   //端口log Start
        public static bool g_bPortLogStop = new bool();    //端口log Stop
        public static uint g_iRcvPortLogNum = new uint();  //端口接收数据次数
        public delegate void ShowTvPortMsgFunc(string msg);
        public static ShowTvPortMsgFunc ShowTvPortMsg;
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 其他
        * --------------------------------------------------*/
        public static bool g_bAdbProcessExited = new bool();  //ADB 进程退出标志
        static UDF.RunMode g_runMode = new UDF.RunMode();       //程序工作模式, 例如 RunMode.RST 或 RunMode.MHL
        public static bool g_bRecvFlag = false;     //服务器返回消息开关
        public static string g_sRecvMsg = "";    //服务器返回的消息
        public static string g_sFwVer1 = string.Empty;  //数据库中的 FW ver1
        public static string g_sFwVer2 = string.Empty;  //数据库中的 FW ver1
        public static int g_iTimerCycle = new int();   //程序测试过程计时器
        public static bool g_bSubProFlag = new bool();  //子程序运行 flag
        public static bool g_bPortErr = new bool();    //端口错误
        public static bool g_bPrventPortErr = true;     //端口错误阻止标志,立即停止不再继续
        //public const string g_sVersion = "Ver: 2025-02-19.01";  //程序状态栏显示的版本
        public const string g_sVersion = "Ver: 2025-03-31.02";  //程序状态栏显示的版本
        public static string g_sAdcProgInfo = string.Empty;
        public static string g_sLogFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";   //日志文件后缀名，例 *20171101.txt
        //NFA1.20200706.0027_163302100
        //NFA1.20210213.0033_255688573
        public string g_sArranFWUVer = "NFA1.20211012.0955_386672040";    //FW烧录工具VERSION
        public string g_sArranFWUVer2 = "NFA1.20211012.0955_386672040";    //FW烧录工具SECURE
        public string g_sArranFWUVer3 = "NFA1.20211012.0955_386672040";    //FW烧录工具NONSECURE
        //NFB1.20200706.0036_163305256
        //NFB1.20210213.0007_255688428
        public string g_sBarraFWUVer = "NFB1.20211012.1033_386672698";    //FW烧录工具VERSION
        public string FW_TEMP = "";
        //NFC1.20200909.0117_187561005
        //NFC1.20201119.0019_218193544
        //NFC1.20210213.0020_255688428
        public string g_sCardFWUVer = "NFC1.20210213.0020_255688428";    //FW烧录工具VERSION
        public string g_so = ""; 
        //original
        //readonly static double[] g_dLightLuxSpec = new double[2] { 1d, 1000d };
        readonly static double[] g_dLightLuxSpec = new double[2] { 1d, 10000d };
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 工单初始化相关
        * 
        * 通常情况下一个工单内的卡关的信息是一致的,
        * 所以应该只在工单切换初始时查询一次这些信息,而无需每次都进行查询
        * (例如,无需每次都要从数据库或其他网络上查询当前机种应该卡关的FW版本,测试规格,测试项目)
        * --------------------------------------------------*/
        static uint g_iPtInitFlag = new uint();    //工单初始化标志。工单切换后
        const uint INIT_ZERO = 0;   //工单初始化标志－重置
        const uint INIT_FW = 1; //FW 已下载
        const uint INIT_BTRSSI = 2; //BTRSSI 已下载
        const uint INIT_GETTESTITEM = 4;    //测试项 已下载
        const uint INIT_DATA_STYLE = 8;     //DataGridView 控件样式已重置

        public static bool logFlag = true;
        public string strLogFileName = "";

        public string SoftwareVersion
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                //return string.Format(" V{0}.{1}.{2}.G{3}", version.Major, version.Minor, version.Build, version.MinorRevision);
                return string.Format(" V{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }

        }
        #region 窗口调节相关
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 此块用于窗口自动调节相关
        * --------------------------------------------------*/
        /// <summary>
        /// 记录控件集初始信息
        /// </summary>
        /// <param name="cons">控件集</param>
        private void InitConTag(Control cons)
        {
            foreach (Control con in cons.Controls) //遍历控件集
            {
                con.Tag = con.Left + "," + con.Top + "," + con.Width + "," + con.Height + "," + con.Font.Size + "," + con.Font.Style;
                if (con.Controls.Count > 0) //处理子控件
                {
                    InitConTag(con);
                }
            }
        }
        /// <summary>
        /// 重新调整控件集显示
        /// </summary>
        /// <param name="widthScaling">窗口宽度缩放比例</param>
        /// <param name="heightScaling">窗口高度缩放比例</param>
        /// <param name="cons">控件集</param>
        private void ResizeCon(float widthScaling, float heightScaling, Control cons)
        {
            float fTmp = new float();

            foreach (Control con in cons.Controls) //遍历控件集
            {
                string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
                fTmp = Convert.ToSingle(conTag[0]) * widthScaling;
                con.Left = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[1]) * heightScaling;
                con.Top = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[2]) * widthScaling;
                con.Width = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[3]) * heightScaling;
                con.Height = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[4]) * heightScaling;
                con.Font = new System.Drawing.Font("", (fTmp == 0) ? 0.1f : fTmp, (FontStyle)Enum.Parse(typeof(FontStyle), conTag[5]));
                if (con.Controls.Count > 0) //处理子控件
                {
                    ResizeCon(widthScaling, heightScaling, con);
                }
            }
        }
        /// <summary>
        /// 记录复杂控件初始信息
        /// </summary>
        private void InitComplexCon()
        {
            g_complexConSetting.sStatusStripVersion = this.statusStripVersion.Height + "," + this.toolStripStatusLabel1.Width + "," + this.toolStripStatusLabel2.Width;
        }
        /// <summary>
        /// 重新调整复杂控件显示
        /// </summary>
        /// <param name="widthScaling">窗口宽度绽放比例</param>
        private void ResetComplexCon(float widthScaling, float heightScaling)
        {
            float fTmp = new float();
            string[] conTag;

            if ((g_iPtInitFlag & INIT_DATA_STYLE) == INIT_DATA_STYLE)
            {
                conTag = g_complexConSetting.sDataGradView.Split(new char[] { ',' });
                fTmp = Convert.ToSingle(conTag[0]) * widthScaling;
                this.dataGridViewTestData.RowHeadersWidth = (int)fTmp;
                for (int i = 0; i < this.dataGridViewTestData.ColumnCount; i++)
                {
                    fTmp = Convert.ToSingle(conTag[i + 1]) * widthScaling;
                    this.dataGridViewTestData.Columns[i].Width = (int)fTmp;
                }
            }

            conTag = g_complexConSetting.sStatusStripVersion.Split(new char[] { ',' });
            fTmp = Convert.ToSingle(conTag[0]) * heightScaling;
            this.statusStripVersion.Height = (int)fTmp;
            fTmp = Convert.ToSingle(conTag[1]) * widthScaling;
            this.toolStripStatusLabel1.Width = (int)fTmp;
            fTmp = Convert.ToSingle(conTag[2]) * widthScaling;
            this.toolStripStatusLabel2.Width = (int)fTmp;
        }
        #endregion
        public FormMain()
        {
            InitializeComponent();
            g_iFormWidth = this.ClientSize.Width;
            g_iFormHeight = this.ClientSize.Height;
            g_iStatusStripHeight = statusStripVersion.Height;
            InitConTag(this);
            InitComplexCon();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            savelogs($"[script version] :{SoftwareVersion}");

            string nul;
            //解决 adb.exe bug
            RunAdbCmd("taskkill /f /im adb.exe /t", out nul, 1);

            //初始化界面一
            toolStripStatusLabel2.Text = g_sVersion;
            dataGridViewTestData.RowHeadersVisible = false;
            comboBoxCmdDelay.SelectedIndex = 0;

            //AutoUpdate();

            if ( GlobalConfig.LoadIniSettings() == false)
            {
                MessageBox.Show("配置档错误 \nLỗi tập tin cấu hình");
                Application.Exit();
            }
            
            //更新程序模式
            if (GlobalConfig.sMesEid .Contains ( "QRE"))
            {
                g_runMode = UDF.RunMode.FWU;
                labelProgMode.Text = Text = "FW-QRE UPGRADE";
                toolStripStatusLabel1.Text = "Welcome to use FW UPGRADE project";
            }
            else if(GlobalConfig.sMesEid.Substring(2, 3) == "HIP")
            {
                g_runMode = UDF.RunMode.HIP;
                labelProgMode.Text = Text = "Hi-Pot TEST";
                toolStripStatusLabel1.Text = "Welcome to use Hi-Pot project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "FTA")
            {
                g_runMode = UDF.RunMode.FTA;
                labelProgMode.Text = Text = "FTA TEST";
                toolStripStatusLabel1.Text = "Welcome to use FTA-Test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "PRE")
            {
                g_runMode = UDF.RunMode.PRE;
                labelProgMode.Text = Text = "PRE TEST";
                toolStripStatusLabel1.Text = "Welcome to use Pre-Test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "WBC")
            {
                g_runMode = UDF.RunMode.WBC;
                labelProgMode.Text = Text = "WBC TEST";
                toolStripStatusLabel1.Text = "Welcome to use WBC test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "WBA")
            {
                g_runMode = UDF.RunMode.WBC;
                labelProgMode.Text = Text = "WBC TEST";
                toolStripStatusLabel1.Text = "Welcome to use WBC test project";
                I2C.IIC_SetBasePortInFile(Application.StartupPath + "\\" + "ULPK.ini");
                I2C.IIC_ResetI2C();
                CountUS();
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "VOC")
            {
                g_runMode = UDF.RunMode.VOC;
                labelProgMode.Text = Text = "VOC TEST";
                toolStripStatusLabel1.Text = "Welcome to use VOC test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "CTK")
            {
                g_runMode = UDF.RunMode.CTK;
                labelProgMode.Text = Text = "CTK TEST";
                toolStripStatusLabel1.Text = "Welcome to use CTK test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "FRQ")
            {
                g_runMode = UDF.RunMode.FRQ;
                labelProgMode.Text = Text = "FRQ TEST";
                toolStripStatusLabel1.Text = "Welcome to use FRQ test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "ULP")
            {
                g_runMode = UDF.RunMode.ULP;
                labelProgMode.Text = Text = "ULPK TEST";
                toolStripStatusLabel1.Text = "Welcome to use ULPK test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "RST")
            {
                g_runMode = UDF.RunMode.RST;
                labelProgMode.Text = Text = "RESET♪TEST";
                toolStripStatusLabel1.Text = "Welcome to use RESET test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "FWU")
            {
                g_runMode = UDF.RunMode.FWU;
                labelProgMode.Text = Text = "FW☀UPGRADE";
                toolStripStatusLabel1.Text = "Welcome to use FW UPGRADE project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "T_S")
            {
                g_runMode = UDF.RunMode.T_S;
                labelProgMode.Text = Text = "TS";
                toolStripStatusLabel1.Text = "Welcome to use TS project";
            }
            else
            {
                MessageBox.Show("错误的 MESEID！ Không chính xác MESEID");
                Application.Exit();
            }
            if (GlobalConfig.sMesEid.Contains("QRE"))
            {
                goto qre;
            }
        
        qre:

            if (GlobalConfig.iRunMode != 1)
            {
                DialogResult UserSelect = MessageBox.Show(null, "用户选择了测试模式，不会上传过站信息，确定继续？\nNgười dùng đã chọn chế độ kiểm tra, sẽ không tải lên thông tin trạm, bạn có chắc chắn muốn tiếp tục?\n\nRunMode:0\t测试模式，不上传过站信息\nRunMode:0\tChế độ kiểm tra, không tải lên thông tin trạm.\nRunMode:1\t正常测试，上传过站信息\nRunMode:1\tKiểm tra bình thường, tải lên thông tin trạm.",
                            "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (UserSelect == DialogResult.No)
                {
                    Application.Exit();
                }
            }

            SetMsg("连接数据库, Kết nối cơ sở dữ liệu", UDF.COLOR.WORK);
         
            if (Sql.Open("Data Source=10.2.100.21;Initial Catalog=RAKEN_TE; User ID=mes;Password=mesuser") == false)
            {
                SetMsg("Connect SQLServer 10.2.100.21 Fail", UDF.COLOR.FAIL);
                Application.Exit();
            }
            else
            {
                SetMsg("Connect SQLServer 10.2.100.21 PASS", UDF.COLOR.WORK);
            }
            if (Database.Oracle.Open("DATA SOURCE = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.2.235.119)(PORT = 1521))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = AVTCMES))); PERSIST SECURITY INFO = True; USER ID = RKNMGR; PASSWORD = 6v7tqe-v;") == false)
            {
                SetMsg("Connect Oracle Server 10.2.235.119 Fail", UDF.COLOR.FAIL);
                Application.Exit();
            }
            else
            {
                SetMsg("Connect Oracle Server 10.2.235.119 PASS", UDF.COLOR.WORK);
            }
            SetMsg("连接数据库ok, Đã kết nối cơ sở dữ liệu thành công", UDF.COLOR.WORK);
            SetMsg("请输入TRID, Vui lòng nhập TRID.", UDF.COLOR.WORK);

            #region 初始化窗口界面二
            textBoxMeseid.Text = GlobalConfig.sMesEid;
            textBoxDC.Text = string.Empty;
            textBoxRC_SN.Text = string.Empty;
            textBoxUn.Text = string.Empty;
            FormMain.ActiveForm.Text = "FTTEST | RunMode = "+GlobalConfig.iRunMode ;
            #endregion
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            string nul;
            Database.Oracle.Close();
            Sql.Close();
            RunAdbCmd("taskkill /f /im adb.exe /t", out nul, 1);
            Process.GetCurrentProcess().Kill();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            
            //调整窗口大小
            if (g_iFormWidth == 0 || g_iFormHeight == 0) return;
            g_fWidthScaling = (float)this.ClientSize.Width / (float)g_iFormWidth;
            g_fHeightScaling = ((float)(this.ClientSize.Height)) / ((float)(g_iFormHeight));
            if (g_fWidthScaling < 0.2f || g_fHeightScaling < 0.2f) return;
            ResizeCon(g_fWidthScaling, g_fHeightScaling, this);
            ResetComplexCon(g_fWidthScaling, g_fHeightScaling);
            
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxSn_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sn = string.Empty;

            if (e.KeyChar == 13)    //回车键
            {
                sn = textBoxSn.Text.Trim().ToUpper();
                if (sn.Length == 0) return;
                textBoxSn.Enabled = false;
                logFlag = true;

              
                
                if (PreCheckTRID(sn) == false)
                {
                    //重置焦点输入
                    this.textBoxSn.Enabled = true;
                    this.textBoxSn.Focus();
                    this.textBoxSn.SelectAll();
                    return;
                }
                
                FW_TEMP = "";
                InitUI();

                //TODO:测试流程开始
                SDriverX.g_modelInfo.sSn = sn;
                //SDriverX.g_modelInfo.sModelName = SDriverX.g_modelInfo.sPart_no.Substring(13, 4);

                //if ((g_iPtInitFlag & INIT_GETTESTITEM) != INIT_GETTESTITEM)    //
                //{
                //    if (UpdataSpecFromServer() == false)
                //    {
                //        ResetUI();
                //        return;
                //    }
                //}

                //if ((g_iPtInitFlag & INIT_GETTESTITEM) != INIT_GETTESTITEM)    //
                //{
                //    if (UpdataSpecFromServer() == false)
                //    {
                //        ResetUI();
                //        return;
                //    }
                //}
            HH:
                if (g_runMode == UDF.RunMode.PRE)
                {
                    //初始化数据表显示
                    if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                    {
                        //TODO: 添加 dataGridView 控件初始化代码
                        InitGridFormatB();
                        g_iPtInitFlag |= INIT_DATA_STYLE;
                    }

                    if (PRE_MainTestProcess() == false)
                    {
                        ResetUI();
                        return;
                    }

                    AddDataB(SDriverX.g_modelInfo.sSn, g_iTimerCycle);
                    Insert_Mes_qty(SDriverX.g_modelInfo.sSn);
                }
                else if (g_runMode == UDF.RunMode.ULP)//@@@###
                {
                    //初始化数据表显示
                    if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                    {
                        //TODO: 添加 dataGridView 控件初始化代码
                        InitGridFormatB();
                        g_iPtInitFlag |= INIT_DATA_STYLE;
                    }

                    if (ULP_Edradour() == false)
                    {
                        ResetUI();
                        return;
                    }

                    AddDataB(SDriverX.g_modelInfo.sSn, g_iTimerCycle);
                    Insert_Mes_qty(SDriverX.g_modelInfo.sSn);
                }
                else if (g_runMode == UDF.RunMode.FWU)
                {
                    //初始化数据表显示
                    if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                    {
                        //TODO: 添加 dataGridView 控件初始化代码
                        InitGridFormatB();
                        g_iPtInitFlag |= INIT_DATA_STYLE;
                    }
                    if (GlobalConfig.sMesEid.ToUpper().Contains("QRE"))
                    {
                        if (FWU_MainTestProcess1() == false)
                        {
                            ResetUI();
                            return;
                        }
                        if (GlobalConfig.sDoorEnable == "Y") OpenDoor();
                    }
                    else if (FWU_MainTestProcess() == false)
                    {
                        ResetUI();
                        return;
                    }
                    AddDataB(textBoxSn.Text.Trim(), g_iTimerCycle);
                    SetMsg("PASS！请输入下一台TRID, Vui lòng nhập TRID tiếp theo", UDF.COLOR.PASS);
                    ResetUI();
                    return;
                }
                else if (g_runMode == UDF.RunMode.FTA)
                {
                    //初始化数据表显示
                    if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                    {
                        //TODO: 添加 dataGridView 控件初始化代码
                        InitGridFormatB();
                        g_iPtInitFlag |= INIT_DATA_STYLE;
                    }

                    if (FTA_MainTestProcess() == false)
                    {
                        ResetUI();
                        SetMsg("test " + "Fail", UDF.COLOR.FAIL);
                        return;
                    }
                    AddDataB(SDriverX.g_modelInfo.sSn, g_iTimerCycle);
                }
                else if (g_runMode == UDF.RunMode.WBC)
                {
                    //初始化数据表显示
                    if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                    {
                        //TODO: 添加 dataGridView 控件初始化代码
                        InitGridFormatB();
                        g_iPtInitFlag |= INIT_DATA_STYLE;
                    }

                    if (FGA_TEST2(SDriverX.g_modelInfo.sSn) == false)
                    {
                        SetMsg("wba fail", UDFs.UDF.COLOR.FAIL);
                        ResetUI();
                        return;
                    }

                    AddDataB(SDriverX.g_modelInfo.sSn, g_iTimerCycle);
                }
                else
                {
                    SetMsg("未维护的运行模式, Chế độ vận hành không bảo trì", UDF.COLOR.FAIL);
                    ResetUI();
                    return;
                }

                if (g_runMode >= UDF.RunMode.FTA && g_runMode <= UDF.RunMode.FWU)
                {
                    if (GlobalConfig.iRunMode == 1)   //上传过站信息
                    {
                        if (g_runMode != UDF.RunMode.FWU)
                        {
                            SetMsg("上传信息至数据库 , Tải thông tin lên cơ sở dữ liệu", UDF.COLOR.WORK);
                            if (SDriverX.WBER(GlobalConfig.sMesEid, SDriverX.g_modelInfo.sSn, "TestUser", "P", "") == false)
                            {
                                SetMsg("上传过站信息失败, Gửi thông tin qua trạm không thành công" + SDriverX.g_modelInfo.sErr_msg, UDF.COLOR.FAIL);
                                ResetUI();
                                return;
                            }
                        }
                    }
                    else if (GlobalConfig.iRunMode == 0)
                    {
                        SetMsg("测试模式不会上传过站信息", UDF.COLOR.WORK);
                    }
                    else
                    {
                        SetMsg("错误的运行模式。上传过站信息请设定 RunMode:1\r\n否则请设置为 RunMode:0，", UDF.COLOR.FAIL);
                        ResetUI();
                        return;
                    }
                }

                if (GlobalConfig.sDoorEnable == "Y") OpenDoor();
                SetMsg("PASS！请输入下一台TRID", UDF.COLOR.PASS);
                ResetUI();
                return;
            }
        }
        
         
        i2c I2C = new i2c();
        public bool ByteStringToArray(string strArr, ref byte[] byteArr)
        {

            //string kk=Convert.ToString(Convert.ToInt32("C", 16))+","+ Convert.ToString(Convert.ToInt32("O", 16)) + "," + Convert.ToString(Convert.ToInt32("M", 16));
            try
            {
                if (strArr.Contains(","))
                {
                    string[] tArr = strArr.Split(',');
                    byteArr = new byte[tArr.Length];
                    for (int i = 0; i < tArr.Length; i++)
                    {
                        // int m= (byte)Convert.ToInt32("K", 16);
                        byteArr[i] = (byte)Convert.ToInt32(tArr[i], 16);
                    }
                }
                else
                {
                    string[] tArr1 = strArr.Split(' ');
                    byteArr = new byte[tArr1.Length];
                    for (int i = 0; i < tArr1.Length; i++)
                    {
                        // int m= (byte)Convert.ToInt32("K", 16);
                        byteArr[i] = (byte)Convert.ToInt32(tArr1[i], 16);
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool m_bTimeCount;
        private void CountUS()
        {
            Int32 i, nTimeCycle;
            recount:
            m_bTimeCount = false;
            timer1.Enabled = true;
            timer1.Interval = 1000;
            nTimeCycle = 0;
            for (i = 0; i <= 0x7FFFFFFF; i++)
            {
                Application.DoEvents();
                if (m_bTimeCount == true || i > 90000000)
                {
                    break;
                }
                nTimeCycle = nTimeCycle + 1;
            }
            I2C.g_nMicSec = nTimeCycle / 1000;// '这里取1ms
            if (I2C.g_nMicSec < 3)
            {
                goto recount;
            }
            I2C.g_nMicSec = I2C.g_nMicSec / 200;// '取5us
            if (I2C.g_nMicSec < 2) { I2C.g_nMicSec = 2; }
           
        }
        public bool sendI2C(string cmd, string Cmdres, string type)
        {

            string strTemp, textppr;
            int strleng = 0;
            int times = 1;
            int sleept = 0;
            int TIMES = 6;

            //Application.DoEvents();
            byte[] Dialdata = new byte[0];

            byte[] ppr = new byte[0];
            if (cmd.Contains(","))
            { strTemp = cmd + ",0"; }
            else
            {
                strTemp = cmd + " 0";
            }
            bool rt =  ByteStringToArray(strTemp, ref Dialdata);
            I2C.checksum(ref Dialdata);

            //listBox1.Items.Add("cmd:" + cmd + "," + Convert.ToString(Dialdata[Dialdata.Length - 1], 16).ToUpper());
            // this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight) + 3;
            this.Refresh();
            mm:
            Application.DoEvents();
            I2C.IIC_ResetI2C();

            if (type == "set")
            {
                strleng = Dialdata.Length - 1;
            }

            sleept = 300;

            if (I2C.IIC_WriteCmd(Dialdata, 0, Dialdata.Length, ref ppr, strleng, sleept * times) == false)
            {
                //lbtxt.Text = "cmd send fail，检查通讯线";
                //lbtxt.ForeColor = Color.Red;
                // System.Threading.Thread.Sleep(20);
                Application.DoEvents();
                //lbtxt.Text = "发送命令失败，检查通讯线" + times;
                //lbtxt.ForeColor = Color.Yellow;
                System.Threading.Thread.Sleep(20);
                //if (Definition.PJ_Data.Brand == "0") { return false; }
                times++;
                if (times < TIMES)
                {
                    //listBox1.Items.Add("cmd:" + times);
                    goto mm;
                }
                //if (Definition.PJ_Data.Qty == 3) { return true; }
                return false;
            }
            else
            {
                //Definition.PJ_Data.Value = 0;
                //Definition.PJ_Data.Qty = 0;
                textppr = "";
                if (I2C.checkI2Cres(ref ppr, ref textppr, Cmdres) == false)
                {
                    times++;
                    if (times < TIMES)
                    { //listBox1.Items.Add("cmd:" + times); goto mm; }
                      //lbtxt.Text = "返回值错误";
                        return false;
                    }
                    else
                    {
                        //listBox1.Items.Add("revalue:" + textppr);
                    }
                }
                return true;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //string ack, ack2;
            //Harris_usb();
            //check_result("NK12343000015");
            //RunAdbCmd(@"D:\platform-tools" + "\\" + "fastboot devices", out ack);
            //SetMsg(ack, UDF.COLOR.PASS);
            //RunAdbCmd( "fastboot devices", out ack);
            //SetMsg(ack, UDF.COLOR.PASS);
            //RunAdbCmd("adb devices", out ack);
            //SetMsg(ack, UDF.COLOR.PASS);
            //PRE_Harr();
            //harris_CheckFw();
            //PRE_Kava();
            //Neat_Install_Apk();
            //FGA_bs("404695947321010092");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string ack;
            //RunAdbCmd("adb shell cat /sdcard/Download/command_ack.txt", out ack);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string ack;
            //RunAdbCmd("adb shell am broadcast -a com.amtran.factory.GetFactoryMode", out ack);

            //UpdateBiTime();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string ack;
            //RunAdbCmd("adb shell cat /sdcard/Download/command_ack.txt", out ack);
        }

        private void timerCycle_Tick(object sender, EventArgs e)
        {
            g_iTimerCycle++;
            textBoxCycle.Text = g_iTimerCycle.ToString();
        }

        private void listBoxSetup_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxSetup.SelectedIndex < 0) return;
            MessageBox.Show(listBoxSetup.Items[listBoxSetup.SelectedIndex].ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_bTimeCount = true;
            timer1.Enabled = false;
        }

        private bool Harris_usb()
        {
            string ack, ack2;
            int retrytime = 0;
            
            try
            {
                if (File.Exists(Application.StartupPath + @"\Neat_cmd\HARRIS.txt"))
                {
                    File.Delete(Application.StartupPath + @"\Neat_cmd\HARRIS.txt");
                    Delay(200);
                }

                if (Directory.Exists(Application.StartupPath + @"\Neat_cmd\") == false)
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Neat_cmd");
                    Delay(200);
                }
                
                if (File.Exists(Application.StartupPath + @"\Neat_cmd\Harris.bat") == false)
                {
                    SetMsg("copy Harris.bat", UDF.COLOR.WORK);
                    File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\Harris.bat", Application.StartupPath + @"\Neat_cmd\Harris.bat", true);
                }
                if (File.Exists(Application.StartupPath + @"\Neat_cmd\devcon.exe") == false)
                {
                        SetMsg("copy devcon.exe", UDF.COLOR.WORK);
                        File.Copy(@"\\10.2.100.27\te\SETUP\Neat\FTTEST\Neat_cmd\devcon.exe", Application.StartupPath + @"\Neat_cmd\devcon.exe",true);
                }
                for (int i=0;i<20;i++)
                {
                    Application.DoEvents();
                    ack2= "等待机台启动中 Đang chờ khởi động máy" + i.ToString() +"\r\n";
                    textBoxAck.AppendText(ack2);
                    Delay(1000);
                }
            re1:
                Application.DoEvents();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Application.StartupPath + @"\Neat_cmd\Harris.bat";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
                Delay(1000);
                Application.DoEvents();

                if (File.Exists(Application.StartupPath + @"\Neat_cmd\HARRIS.txt") == false)
                {
                    if (retrytime < 60)
                    {
                        ack2 = "未找到HARRIS.txt , Không tìm thấy tệp tin HARRIS.txt" + retrytime.ToString() + "\r\n";
                        textBoxAck.AppendText(ack2);
                        Delay(100);
                        retrytime++;
                        goto re1;
                    }
                    return false;
                }
                else
                {
                    textBoxAck.AppendText(Application.StartupPath + @"\Neat_cmd\HARRIS.txt" + "\r\n");
                }

                Delay(500);
                retrytime = 0;
                ack = "";
                if (File.Exists(Application.StartupPath + @"\Neat_cmd\HARRIS.txt") == true)
                {
                    ack = File.ReadAllText(Application.StartupPath + @"\Neat_cmd\HARRIS.txt");
                }

                if (ResultCheck(ack, "1 matching") == false)
                {
                    ack2 = "查找adb:" + ack + retrytime.ToString() + "\r\n";
                    textBoxAck.AppendText(ack2);

                    if (retrytime < 60)
                    {
                        retrytime++;
                        goto re1;
                    }
                   
                    SetMsg(ack, UDF.COLOR.FAIL);
                    return false;
                }
                textBoxAck.AppendText(ack);
            }

            catch (Exception ex)
            {
                if (ex.ToString().Contains(" find"))
                {
                    textBoxAck.AppendText(ex.ToString());
                    SetMsg("没找到文档, Không tìm thấy tệp tin", UDF.COLOR.FAIL);
                }
                return false;
            }

            return true;
        }

        private void textBoxSn_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxModel_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
