/*
 * Created by SharpDevelop.
 * User: yuan_xinyuan
 * Date: 2015/11/24
 * Time: 20:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace RenRen_AutoSignService
{
	static class Program
	{
		/// <summary>
		/// This method starts the service.
		/// </summary>
		static void Main()
		{
			// To run more than one service you have to add them here
			ServiceBase.Run(new ServiceBase[] { new RenRen_AutoSignService() });
		}
	}
}
