using System;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace MyVote.AppServer
{
	public class WebRole
		: RoleEntryPoint
	{
		public override bool OnStart()
		{
			// To enable the AzureLocalStorageTraceListner, uncomment relevent section in the web.config  
			var diagnosticConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
			diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
			diagnosticConfig.Directories.DataSources.Add(AzureLocalStorageTraceListener.GetLogDirectory());

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}
	}
}
