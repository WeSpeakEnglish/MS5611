/*
 * Created by SharpDevelop.
 * User: P1
 * Date: 24.09.2012
 * Time: 14:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Security.Permissions;
//using System.Collections;
using System.Windows;	

namespace Drive1
{
[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
[System.Runtime.InteropServices.ComVisibleAttribute(true)]
	public partial class MainForm : Form
	{
     public string MessageInTextarea1=null;
 	 public SerialPort MyPort;
 	 //public 

		public MainForm()
		{
			
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			
			InitializeComponent();
   
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Label1Click(object sender, EventArgs e)
		{
			
		}
		

 private void MyPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
 	        System.Threading.Thread.Sleep(100);
 	        
 	        if(MyPort.IsOpen){
            int bytes = MyPort.BytesToRead;
            byte[]  read_buf = new byte[bytes];
            MyPort.Read(read_buf, 0, bytes);
               
            //string POT = MyPort.Read();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), read_buf);
 	        }
        }
 
      private delegate void LineReceivedEvent(byte[] read_buf);
 
        private void LineReceived(byte[] read_buf)
        {

      DataConverters DataConverter1 = new DataConverters();
      Int32[] ValuesOfSource =  new Int32[10];



      if(read_buf.Length != 32)
      	return;
     
      if(read_buf[0] == 0x40){

     	
     
        	ValuesOfSource[0]=(read_buf[1]*100+read_buf[2]); //
        	ValuesOfSource[1]=(read_buf[3]*10000 + read_buf[4]*100 + read_buf[5]); //
        	ValuesOfSource[2]=(read_buf[6]*100+read_buf[7]); //
        	ValuesOfSource[3]=(read_buf[8]*10000 + read_buf[9]*100 + read_buf[10]); //

     	
     	// Send data to form
     	DataConverter1.SetValuesToHTML(this.webBrowser1.Document, ref ValuesOfSource);
    
   
        string dataStringHEX = BitConverter.ToString(read_buf);
 
        dataStringHEX = dataStringHEX.Replace("-"," ");
        ComList("WritePorts",dataStringHEX);
        }    
 
        }
 
  
  
		// ComList("WritePorts","Connected!");
		 public void ComList(String COMfunction,String message)
    {
         System.Windows.Forms.HtmlDocument document = this.webBrowser1.Document;
 		 Object[] objArray = new Object[1];
 		 objArray[0] = (Object)(message);
 		 document.InvokeScript(COMfunction,objArray);
		 }
		
		void WebBrowser1DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
		 string[] ports = SerialPort.GetPortNames();
		 string MessageInnerHTML=null;
		 Array.Sort(ports);
        foreach(string port in ports){
        	MessageInnerHTML+="<option value="+port.ToString()+">"+port.ToString()+"</option>";
        }
        ComList("CreateCOMportsList",MessageInnerHTML);
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
    
//	String TitleContent=File.ReadAllText(MyPath+"\\interface\\title.html", System.Text.Encoding.GetEncoding(1251));
	string curDir = Directory.GetCurrentDirectory();
     webBrowser1.AllowWebBrowserDrop = false;
     webBrowser1.IsWebBrowserContextMenuEnabled = false;
     webBrowser1.WebBrowserShortcutsEnabled = false;
     webBrowser1.ObjectForScripting = this;	 	 	
     this.webBrowser1.Url = new Uri(String.Format("file:///{0}/interface/title.html", curDir));
  
     webBrowser1.Navigating += 
        new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
		}

private void webBrowser1_Navigating(object sender,
    WebBrowserNavigatingEventArgs e)
    {
        e.Cancel = true;
    }
 
 public void ConnectToCom (string COM_Name, int ConnetOrDisconnect){
	if(ConnetOrDisconnect == 1){
			MyPort= new SerialPort(COM_Name , 115200, Parity.None, 8, StopBits.One);
			MyPort.ReadTimeout = 100;
            MyPort.WriteTimeout = 100;
            MyPort.ReadBufferSize = 4096;
            MyPort.WriteBufferSize = 4096;
            MyPort.DataReceived += new SerialDataReceivedEventHandler(MyPort_DataReceived);
            MyPort.Encoding = System.Text.Encoding.GetEncoding("windows-1251");
            MyPort.Open();
			byte[] data = { 0, 1, 2, 1, 0 };
            MyPort.Write(data, 0, data.Length);		
			}
	else{
		  //  System.Threading.Thread.Sleep(300);
			MyPort.Close();
			}
		
		}
		
  public void SaveToCSV (string FieldStr, int ExtraParam){
    SaveFileDialog saveFileDialog1 = new SaveFileDialog();      
    saveFileDialog1.InitialDirectory = "C:\\"; 
    saveFileDialog1.FileName = "Leakage"+DateTime.Now.ToString().Replace('/','_').Replace(':','_')+".csv";
    saveFileDialog1.Title = "Save CSV File";      
    saveFileDialog1.CheckFileExists = false;      
    saveFileDialog1.CheckPathExists = true;      
    saveFileDialog1.DefaultExt = "csv";      
    saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";      
    saveFileDialog1.FilterIndex = 1;      
    saveFileDialog1.RestoreDirectory = true;      
    if (saveFileDialog1.ShowDialog() == DialogResult.OK) {      
        using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
        sw.WriteLine (FieldStr);     
    }      
		
		}
 public void SendSettings (int typeSettings, string FirstParam, string SecondParam){ // transfer settings to COM-port
 
  byte[] ParamsToSend={0,0,0,0};
  int M_1_byte=0; //first param master byte
  int S_1_byte=0;    //first param slave byte
  int M_2_byte=0; //first param master byte
  int S_2_byte=0;    //first param slave byte 
  byte CheckSum=0x00; 
 
 
     
    	M_1_byte = int.Parse(FirstParam);
    	S_1_byte = M_1_byte & 0x00FF;
  		M_1_byte >>=8;
  		M_2_byte = int.Parse(SecondParam);
  		S_2_byte = M_2_byte & 0x00FF;
  		M_2_byte >>=8;
  		CheckSum =  (byte)M_1_byte;
  		CheckSum ^= (byte)S_1_byte;
  		CheckSum ^= (byte)M_2_byte;
  		CheckSum ^= (byte)S_2_byte;
     //   CheckSum ^=	(byte)typeSettings;
        CheckSum ^= 0x04; //Length eq 4 XOR
  
	switch(typeSettings){
	    case 2:

  		     
  				byte[] data = { 0x40, 0x02, 0x04,(byte)M_1_byte,(byte)S_1_byte, (byte)M_2_byte,(byte)S_2_byte, CheckSum ,0x40 };
           MyPort.Write(data, 0, data.Length);			
            
			break;
		case 3:
			
			break;
			
			}
      			
		
		MessageBox.Show(FirstParam+"; "+SecondParam);
		}
 		
		
		void MainFormClosing(object sender, FormClosingEventArgs e)
		{
			//if(MyPort != null)MyPort.Close();
		}
		void SaveFileDialog1FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
	
		}
		
	}

}
