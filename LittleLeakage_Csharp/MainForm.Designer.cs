/*
 * Created by SharpDevelop.
 * User: P1
 * Date: 24.09.2012
 * Time: 14:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 using System;
namespace Drive1
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
//		int Index=0;SerialPort port = new SerialPort( ″COM1″ , 9600, Parity.None, 8, StopBits.One);

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// webBrowser1
			// 
			resources.ApplyResources(this.webBrowser1, "webBrowser1");
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.ScrollBarsEnabled = false;
			this.webBrowser1.Url = new System.Uri("http://127.0.0.1/", System.UriKind.Absolute);
			this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowser1DocumentCompleted);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
			// 
			// serialPort1
			// 
			this.serialPort1.ReadTimeout = 1000;
			this.serialPort1.WriteTimeout = 1000;
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDialog1FileOk);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.webBrowser1);
			this.Name = "MainForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.ResumeLayout(false);

		}
		private System.IO.Ports.SerialPort serialPort1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		
		void Timer1Tick(object sender, System.EventArgs e)
		{
    
    this.Text= "Barocamera "+DateTime.Now.ToString();
		}
	}
}
