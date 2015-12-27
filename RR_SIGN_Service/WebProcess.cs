/*
 * Created by SharpDevelop.
 * User: yuan_xinyuan
 * Date: 2015/11/20
 * Time: 12:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Xml;

namespace RenRen_AutoSignService
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class WebProcess
	{
		public static HttpClient httpClient = new HttpClient();		
		private static XmlDocument xmlDoc;
		
		private static string headerUrl;
		private static string loginUrl;
		private static string signUrl;
		private static string doSignUrl;
		private static string username;
		private static string password;
		public static string lastTime;
		
		public static void iniProcess(){
			httpClient.DefaultRequestHeaders.Referrer = new Uri(headerUrl);			
		}
		
		public static void loginProcess (){
      		String remember = "1";
      		List<KeyValuePair<String, String>> parm = new List<KeyValuePair<String, String>>();
      		parm.Add(new KeyValuePair<String, String>("account",username));
      		parm.Add(new KeyValuePair<String, String>("password",password));
      		parm.Add(new KeyValuePair<String, String>("remember",remember));
      		parm.Add(new KeyValuePair<String, String>("signUrl",signUrl));
      		HttpResponseMessage response = httpClient.PostAsync(new Uri(signUrl), new FormUrlEncodedContent(parm)).Result;
      		System.Threading.Thread.Sleep(2000);
      		httpClient.GetAsync(new Uri(loginUrl));
		}
		
		public static string redirct(string path){
      		HttpResponseMessage response = httpClient.GetAsync(new Uri(doSignUrl)).Result;
      		JavaScriptSerializer ser = new JavaScriptSerializer();
      		Hashtable result = ser.Deserialize<Hashtable>(response.Content.ReadAsStringAsync().Result);
      		switch (result["status"].ToString()) {
      			case "1":
      				return "1";
      			case "4002":
      				return "0";
      			default:
      				return "2";
      		}
		}
		public static void loadXML(string path){
			xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNodeList nodeList = xmlDoc.SelectSingleNode("config").ChildNodes;
			foreach (XmlNode xn in nodeList)
			{
				XmlElement xe = (XmlElement)xn;
				switch (xe.Name) {
					case "url":
						headerUrl = xe.SelectSingleNode("headerUrl").InnerText;
						loginUrl = xe.SelectSingleNode("loginUrl").InnerText;
						signUrl = xe.SelectSingleNode("signUrl").InnerText;
						doSignUrl = xe.SelectSingleNode("doSignUrl").InnerText;
						break;
					case "userInfo":
						username = xe.SelectSingleNode("id").InnerText;
						password = xe.SelectSingleNode("password").InnerText;
					break;
					case "lastsignTime":
						lastTime = xe.InnerText;
					break;
					default:						
						break;
				}
			}
		}
		
		public static void saveXML(string path){
			xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNodeList nodeList = xmlDoc.SelectSingleNode("config").ChildNodes;
			foreach (XmlNode xn in nodeList)
			{
				XmlElement xe = (XmlElement)xn;
				switch (xe.Name) {
					case "url":
						break;
					case "userInfo":
					break;
					case "lastsignTime":
					xe.InnerText = DateTime.Now.Date.ToString("yyyy-MM-dd HH：mm：ss：ffff");
					break;
					default:						
						break;
				}
			}
			xmlDoc.Save(path);
		}
	}
}
