namespace WindowsFormsApplication
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
   
        private System.Windows.Forms.TextBox textBox_input;

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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        	this.textBox_input = new System.Windows.Forms.TextBox();
        	this.listBox_result = new System.Windows.Forms.ListBox();
        	this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
        	this.SuspendLayout();
        	// 
        	// textBox_input
        	// 
        	this.textBox_input.Dock = System.Windows.Forms.DockStyle.Top;
        	this.textBox_input.Location = new System.Drawing.Point(0, 0);
        	this.textBox_input.Name = "textBox_input";
        	this.textBox_input.Size = new System.Drawing.Size(292, 21);
        	this.textBox_input.TabIndex = 1;
        	this.textBox_input.TextChanged += new System.EventHandler(this.TextBox_inputTextChanged);
        	this.textBox_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Handle_ToResultKey);
        	this.textBox_input.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBox_inputPreviewKeyDown);
        	// 
        	// listBox_result
        	// 
        	this.listBox_result.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.listBox_result.FormattingEnabled = true;
        	this.listBox_result.HorizontalScrollbar = true;
        	this.listBox_result.ItemHeight = 12;
        	this.listBox_result.Location = new System.Drawing.Point(0, 29);
        	this.listBox_result.Name = "listBox_result";
        	this.listBox_result.Size = new System.Drawing.Size(292, 244);
        	this.listBox_result.TabIndex = 2;
        	this.listBox_result.SelectedIndexChanged += new System.EventHandler(this.listBox_result_SelectedIndexChanged);
        	this.listBox_result.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Handle_EscKey);
        	// 
        	// notifyIcon
        	// 
        	this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
        	this.notifyIcon.BalloonTipText = "Swswitch in systray";
        	this.notifyIcon.BalloonTipTitle = "Swswitch";
        	this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
        	this.notifyIcon.Text = "SwSwitch";
        	this.notifyIcon.Visible = true;
        	this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
        	// 
        	// MainForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(292, 273);
        	this.Controls.Add(this.listBox_result);
        	this.Controls.Add(this.textBox_input);
        	this.Name = "MainForm";
        	this.Text = "Form1";
        	this.Activated += new System.EventHandler(this.MainForm_Activated);
        	this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Handle_EscKey);
        	this.Resize += new System.EventHandler(this.MainForm_Resize);
        	this.ResumeLayout(false);
        	this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ListBox listBox_result;
        private System.Windows.Forms.NotifyIcon notifyIcon;

    }  
}

