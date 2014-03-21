using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using MyVote.BusinessObjects;

namespace MyVote.UI.Droid
{
    class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
			Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
#if DEBUG
			Csla.ApplicationContext.DataPortalUrlString = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
			Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
#else
			Csla.ApplicationContext.DataPortalUrlString =  "http://yourserver.cloudapp.net/MobilePortal.svc";
#endif // DEBUG
			var container = new ContainerBuilder ();

			new UiContainerBuilderComposition ().Compose (container);
            new BusinessObjectsContainerBuilderComposition ().Compose (container);
            var returnValue = container.Build();
            Csla.ApplicationContext.DataPortalActivator = new ObjectActivator(returnValue);

            return returnValue;
        }
    }
}