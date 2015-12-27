/*
 * Created by SharpDevelop.
 * User: yuan_xinyuan
 * Date: 2015/11/24
 * Time: 20:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Microsoft.Win32;

namespace RenRen_AutoSignService
{
	public class RenRen_AutoSignService : ServiceBase
	{
		public const string MyServiceName = "RenRen_AutoSignService";
		public static Timer timer24 = new Timer();
		public static Timer timer05 = new Timer();
		public static Timer timer0020 = new Timer();
		public static string configFile;
		public static EventLog log;
		public RenRen_AutoSignService()
		{
			InitializeComponent();
		}
		
		private void InitializeComponent()
		{
			this.ServiceName = MyServiceName;
			
			timer24.Interval = 1*1000*60*60*24;			
			timer05.Interval = 30*1000*60;			
			timer0020.Interval = 20*1000;
			
			timer24.Elapsed += new System.Timers.ElapsedEventHandler(Timer24Event);
			timer05.Elapsed += new System.Timers.ElapsedEventHandler(Timer05Event);
			timer0020.Elapsed += new System.Timers.ElapsedEventHandler(Timer0020Event);
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			// TODO: Add cleanup code here (if required)
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		protected override void OnStart(string[] args)
		{	
			configFile = GetWindowsServiceInstallPath() + "\\config.xml";
			printLog("configPath:" + configFile);			
			WebProcess.loadXML(configFile);
			printLog(WebProcess.lastTime);
			if(String.IsNullOrEmpty(WebProcess.lastTime) || (DateTime.Compare(Convert.ToDateTime(WebProcess.lastTime),DateTime.Now) < 0 && DateTime.Now.Subtract(Convert.ToDateTime(WebProcess.lastTime)).Days>=1)){
				WebProcess.iniProcess();
				WebProcess.loginProcess();
				timer0020.Start();
				printLog("OnStart > 24");
			}
			timer24.Start();
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			WebProcess.httpClient.Dispose();
			timer24.Stop();
			timer05.Stop();
			timer0020.Stop();
		}
		
		private static void Timer24Event(object source, ElapsedEventArgs e){
			printLog("Timer24Event start");
			WebProcess.httpClient = new System.Net.Http.HttpClient();
			WebProcess.iniProcess();
			WebProcess.loginProcess();
			timer0020.Start();
			printLog("Timer24Event stop");
		}
		private static void Timer05Event(object source, ElapsedEventArgs e){
			printLog("Timer05Event start");
			WebProcess.httpClient = new System.Net.Http.HttpClient();
			WebProcess.iniProcess();
			WebProcess.loginProcess();
			timer0020.Start();
			timer05.Stop();
			printLog("Timer05Event stop");
			
		}
		private static void Timer0020Event(object source, ElapsedEventArgs e){
			printLog("Timer0020Event start");
			string status = WebProcess.redirct(configFile);
			switch(status){
				case "1":
					WebProcess.saveXML(configFile);
					timer0020.Stop();
					timer05.Stop();
					break;
				case "0":
				printLog("Timer0020Event fail");
				timer0020.Stop();
				timer05.Start();
				break;
			default:
				timer0020.Stop();
				timer05.Stop();
				break;
			}
		}
		
		
        public static string GetWindowsServiceInstallPath()
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + MyServiceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
			printLog("GetWindowsServiceInstallPath key=" + key +"\r\npath=" + path);
            path = path.Replace("\"", string.Empty);
            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();
        }
        
        private static void printLog(string message){
        	log = new EventLog();
			log.Source = MyServiceName;
			log.WriteEntry(message);
        }
	}
}
