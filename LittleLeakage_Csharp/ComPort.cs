/*
 * Created by SharpDevelop.
 * User: P1
 * Date: 30.10.2012
 * Time: 12:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows;	

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;
//using System.Collections;
	

namespace Drive1
{
public class DataConverters
	{
	public	DataConverters(){
		
		}
public UInt16 TakeValuesFromCom(ref byte HighByte, ref byte LowByte){
UInt16 GetValueOfData=0;
     	GetValueOfData = HighByte; //<< 8;) + data[4];            // << 8) + data.GetValue(4);
        GetValueOfData <<=8;
        GetValueOfData += LowByte;
return GetValueOfData;
}
	
public Int16 SetValuesToHTML(System.Windows.Forms.HtmlDocument document,ref Int32[] ValuesOfSource){
	     
		 Object[] objArray = new Object[10];
		 for (int i=0;i<10;i++)objArray[i] = (Object)(ValuesOfSource[i]);
		 
 		 document.InvokeScript("SetValuesToHTML",objArray);
	return 0;
	}
	}
}


