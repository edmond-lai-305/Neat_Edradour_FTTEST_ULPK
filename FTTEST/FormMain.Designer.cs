namespace FTTEST
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lableMsg = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCmdDelay = new System.Windows.Forms.ComboBox();
            this.textBoxUn = new System.Windows.Forms.TextBox();
            this.textBoxRC_SN = new System.Windows.Forms.TextBox();
            this.textBoxPn = new System.Windows.Forms.TextBox();
            this.textBoxCycle = new System.Windows.Forms.TextBox();
            this.textBoxDC = new System.Windows.Forms.TextBox();
            this.textBoxMeseid = new System.Windows.Forms.TextBox();
            this.textBoxModel = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStripVersion = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listBoxSetup = new System.Windows.Forms.ListBox();
            this.dataGridViewTestData = new System.Windows.Forms.DataGridView();
            this.textBoxAck = new System.Windows.Forms.TextBox();
            this.textBoxSn = new System.Windows.Forms.TextBox();
            this.labelMsgtip = new System.Windows.Forms.Label();
            this.labelProgMode = new System.Windows.Forms.Label();
            this.serialPortTV = new System.IO.Ports.SerialPort(this.components);
            this.serialPortGDM = new System.IO.Ports.SerialPort(this.components);
            this.serialPortIOCard = new System.IO.Ports.SerialPort(this.components);
            this.serialPortCp310 = new System.IO.Ports.SerialPort(this.components);
            this.serialPortMHL = new System.IO.Ports.SerialPort(this.components);
            this.timerCycle = new System.Windows.Forms.Timer(this.components);
            this.serialPortDoor = new System.IO.Ports.SerialPort(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStripVersion.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTestData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lableMsg
            // 
            this.lableMsg.AutoSize = true;
            this.lableMsg.ForeColor = System.Drawing.Color.Red;
            this.lableMsg.Location = new System.Drawing.Point(294, 150);
            this.lableMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lableMsg.Name = "lableMsg";
            this.lableMsg.Size = new System.Drawing.Size(371, 18);
            this.lableMsg.TabIndex = 18;
            this.lableMsg.Text = "数据库：Oracle,Local access,MES:ap.rakenpd.com";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonExit);
            this.groupBox2.Controls.Add(this.buttonSetting);
            this.groupBox2.Location = new System.Drawing.Point(1040, 170);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(289, 140);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Image = global::FTTEST.Properties.Resources.Exit;
            this.buttonExit.Location = new System.Drawing.Point(153, 24);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(123, 105);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.TabStop = false;
            this.buttonExit.Text = "退出(&E)";
            this.buttonExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonSetting
            // 
            this.buttonSetting.Enabled = false;
            this.buttonSetting.Image = global::FTTEST.Properties.Resources.toolgray;
            this.buttonSetting.Location = new System.Drawing.Point(17, 26);
            this.buttonSetting.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(123, 105);
            this.buttonSetting.TabIndex = 1;
            this.buttonSetting.TabStop = false;
            this.buttonSetting.Text = "设置(&S)";
            this.buttonSetting.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxCmdDelay);
            this.groupBox1.Controls.Add(this.textBoxUn);
            this.groupBox1.Controls.Add(this.textBoxRC_SN);
            this.groupBox1.Controls.Add(this.textBoxPn);
            this.groupBox1.Controls.Add(this.textBoxCycle);
            this.groupBox1.Controls.Add(this.textBoxDC);
            this.groupBox1.Controls.Add(this.textBoxMeseid);
            this.groupBox1.Controls.Add(this.textBoxModel);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.groupBox1.Location = new System.Drawing.Point(21, 168);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1009, 141);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "本站信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(824, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "Delay:";
            // 
            // comboBoxCmdDelay
            // 
            this.comboBoxCmdDelay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.comboBoxCmdDelay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCmdDelay.Items.AddRange(new object[] {
            "0",
            "50",
            "500",
            "1000"});
            this.comboBoxCmdDelay.Location = new System.Drawing.Point(897, 84);
            this.comboBoxCmdDelay.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxCmdDelay.Name = "comboBoxCmdDelay";
            this.comboBoxCmdDelay.Size = new System.Drawing.Size(84, 26);
            this.comboBoxCmdDelay.TabIndex = 3;
            this.comboBoxCmdDelay.TabStop = false;
            // 
            // textBoxUn
            // 
            this.textBoxUn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxUn.Location = new System.Drawing.Point(597, 84);
            this.textBoxUn.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxUn.Name = "textBoxUn";
            this.textBoxUn.ReadOnly = true;
            this.textBoxUn.Size = new System.Drawing.Size(216, 28);
            this.textBoxUn.TabIndex = 10;
            this.textBoxUn.TabStop = false;
            // 
            // textBoxRC_SN
            // 
            this.textBoxRC_SN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxRC_SN.Location = new System.Drawing.Point(420, 84);
            this.textBoxRC_SN.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxRC_SN.Name = "textBoxRC_SN";
            this.textBoxRC_SN.ReadOnly = true;
            this.textBoxRC_SN.Size = new System.Drawing.Size(116, 28);
            this.textBoxRC_SN.TabIndex = 9;
            this.textBoxRC_SN.TabStop = false;
            // 
            // textBoxPn
            // 
            this.textBoxPn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxPn.Location = new System.Drawing.Point(73, 84);
            this.textBoxPn.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPn.Name = "textBoxPn";
            this.textBoxPn.ReadOnly = true;
            this.textBoxPn.Size = new System.Drawing.Size(253, 28);
            this.textBoxPn.TabIndex = 8;
            this.textBoxPn.TabStop = false;
            // 
            // textBoxCycle
            // 
            this.textBoxCycle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxCycle.Location = new System.Drawing.Point(897, 30);
            this.textBoxCycle.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCycle.Name = "textBoxCycle";
            this.textBoxCycle.ReadOnly = true;
            this.textBoxCycle.Size = new System.Drawing.Size(84, 28);
            this.textBoxCycle.TabIndex = 7;
            this.textBoxCycle.TabStop = false;
            this.textBoxCycle.Text = "0";
            // 
            // textBoxDC
            // 
            this.textBoxDC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxDC.Location = new System.Drawing.Point(597, 30);
            this.textBoxDC.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDC.Name = "textBoxDC";
            this.textBoxDC.ReadOnly = true;
            this.textBoxDC.Size = new System.Drawing.Size(216, 28);
            this.textBoxDC.TabIndex = 6;
            this.textBoxDC.TabStop = false;
            // 
            // textBoxMeseid
            // 
            this.textBoxMeseid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxMeseid.Location = new System.Drawing.Point(420, 30);
            this.textBoxMeseid.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMeseid.Name = "textBoxMeseid";
            this.textBoxMeseid.ReadOnly = true;
            this.textBoxMeseid.Size = new System.Drawing.Size(116, 28);
            this.textBoxMeseid.TabIndex = 5;
            this.textBoxMeseid.TabStop = false;
            // 
            // textBoxModel
            // 
            this.textBoxModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxModel.Location = new System.Drawing.Point(73, 30);
            this.textBoxModel.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxModel.Name = "textBoxModel";
            this.textBoxModel.ReadOnly = true;
            this.textBoxModel.Size = new System.Drawing.Size(253, 28);
            this.textBoxModel.TabIndex = 4;
            this.textBoxModel.TabStop = false;
            this.textBoxModel.TextChanged += new System.EventHandler(this.textBoxModel_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(548, 90);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 18);
            this.label8.TabIndex = 6;
            this.label8.Text = "FW:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(338, 90);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "SSID:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 90);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 18);
            this.label6.TabIndex = 4;
            this.label6.Text = "料号:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(824, 36);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "Cycle:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(548, 36);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "MAC:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "MESEID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "机种:";
            // 
            // statusStripVersion
            // 
            this.statusStripVersion.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripVersion.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStripVersion.Location = new System.Drawing.Point(0, 1003);
            this.statusStripVersion.Name = "statusStripVersion";
            this.statusStripVersion.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStripVersion.Size = new System.Drawing.Size(1351, 31);
            this.statusStripVersion.TabIndex = 19;
            this.statusStripVersion.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(360, 24);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(158, 24);
            this.toolStripStatusLabel2.Text = "Ver: ????-??-??.??";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listBoxSetup);
            this.groupBox5.Location = new System.Drawing.Point(21, 318);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(469, 663);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "流程信息";
            // 
            // listBoxSetup
            // 
            this.listBoxSetup.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxSetup.FormattingEnabled = true;
            this.listBoxSetup.ItemHeight = 18;
            this.listBoxSetup.Location = new System.Drawing.Point(9, 22);
            this.listBoxSetup.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxSetup.Name = "listBoxSetup";
            this.listBoxSetup.Size = new System.Drawing.Size(458, 562);
            this.listBoxSetup.TabIndex = 0;
            this.listBoxSetup.TabStop = false;
            this.listBoxSetup.DoubleClick += new System.EventHandler(this.listBoxSetup_DoubleClick);
            // 
            // dataGridViewTestData
            // 
            this.dataGridViewTestData.AllowUserToResizeColumns = false;
            this.dataGridViewTestData.AllowUserToResizeRows = false;
            this.dataGridViewTestData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTestData.Location = new System.Drawing.Point(500, 735);
            this.dataGridViewTestData.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewTestData.MinimumSize = new System.Drawing.Size(798, 246);
            this.dataGridViewTestData.MultiSelect = false;
            this.dataGridViewTestData.Name = "dataGridViewTestData";
            this.dataGridViewTestData.ReadOnly = true;
            this.dataGridViewTestData.RowHeadersWidth = 30;
            this.dataGridViewTestData.RowTemplate.Height = 23;
            this.dataGridViewTestData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTestData.Size = new System.Drawing.Size(837, 246);
            this.dataGridViewTestData.TabIndex = 28;
            this.dataGridViewTestData.TabStop = false;
            // 
            // textBoxAck
            // 
            this.textBoxAck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxAck.ForeColor = System.Drawing.Color.Lime;
            this.textBoxAck.Location = new System.Drawing.Point(500, 485);
            this.textBoxAck.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxAck.Multiline = true;
            this.textBoxAck.Name = "textBoxAck";
            this.textBoxAck.ReadOnly = true;
            this.textBoxAck.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAck.Size = new System.Drawing.Size(831, 239);
            this.textBoxAck.TabIndex = 29;
            this.textBoxAck.TabStop = false;
            // 
            // textBoxSn
            // 
            this.textBoxSn.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxSn.Location = new System.Drawing.Point(523, 387);
            this.textBoxSn.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxSn.Name = "textBoxSn";
            this.textBoxSn.Size = new System.Drawing.Size(804, 67);
            this.textBoxSn.TabIndex = 26;
            this.textBoxSn.TextChanged += new System.EventHandler(this.textBoxSn_TextChanged);
            this.textBoxSn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSn_KeyPress);
            // 
            // labelMsgtip
            // 
            this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelMsgtip.Font = new System.Drawing.Font("宋体", 26F, System.Drawing.FontStyle.Bold);
            this.labelMsgtip.Location = new System.Drawing.Point(500, 323);
            this.labelMsgtip.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMsgtip.Name = "labelMsgtip";
            this.labelMsgtip.Size = new System.Drawing.Size(832, 156);
            this.labelMsgtip.TabIndex = 27;
            this.labelMsgtip.Text = "请输入TRID";
            this.labelMsgtip.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelProgMode
            // 
            this.labelProgMode.BackColor = System.Drawing.Color.Transparent;
            this.labelProgMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgMode.ForeColor = System.Drawing.Color.MediumSpringGreen;
            this.labelProgMode.Location = new System.Drawing.Point(240, 26);
            this.labelProgMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProgMode.Name = "labelProgMode";
            this.labelProgMode.Size = new System.Drawing.Size(788, 108);
            this.labelProgMode.TabIndex = 30;
            this.labelProgMode.Text = " FTTEST";
            this.labelProgMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerCycle
            // 
            this.timerCycle.Interval = 1000;
            this.timerCycle.Tick += new System.EventHandler(this.timerCycle_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(955, 35);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 36);
            this.button1.TabIndex = 31;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1083, 35);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 36);
            this.button2.TabIndex = 32;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(955, 80);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(118, 36);
            this.button3.TabIndex = 33;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1083, 80);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(118, 36);
            this.button4.TabIndex = 34;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::FTTEST.Properties.Resources.ft_logo;
            this.pictureBoxLogo.Location = new System.Drawing.Point(26, 18);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(1308, 123);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 12;
            this.pictureBoxLogo.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1351, 1034);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelProgMode);
            this.Controls.Add(this.dataGridViewTestData);
            this.Controls.Add(this.textBoxAck);
            this.Controls.Add(this.textBoxSn);
            this.Controls.Add(this.labelMsgtip);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.statusStripVersion);
            this.Controls.Add(this.lableMsg);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBoxLogo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FT TEST20230425";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStripVersion.ResumeLayout(false);
            this.statusStripVersion.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTestData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lableMsg;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCmdDelay;
        private System.Windows.Forms.TextBox textBoxUn;
        private System.Windows.Forms.TextBox textBoxRC_SN;
        private System.Windows.Forms.TextBox textBoxPn;
        private System.Windows.Forms.TextBox textBoxCycle;
        private System.Windows.Forms.TextBox textBoxDC;
        private System.Windows.Forms.TextBox textBoxMeseid;
        private System.Windows.Forms.TextBox textBoxModel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStripVersion;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListBox listBoxSetup;
        private System.Windows.Forms.DataGridView dataGridViewTestData;
        private System.Windows.Forms.TextBox textBoxAck;
        private System.Windows.Forms.TextBox textBoxSn;
        private System.Windows.Forms.Label labelMsgtip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Label labelProgMode;
        private System.IO.Ports.SerialPort serialPortTV;
        private System.IO.Ports.SerialPort serialPortGDM;
        private System.IO.Ports.SerialPort serialPortIOCard;
        private System.IO.Ports.SerialPort serialPortCp310;
        private System.IO.Ports.SerialPort serialPortMHL;
        private System.Windows.Forms.Timer timerCycle;
        private System.IO.Ports.SerialPort serialPortDoor;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Timer timer1;
    }
}

