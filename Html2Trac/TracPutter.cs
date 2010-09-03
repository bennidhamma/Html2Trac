using System;
using CookComputing.XmlRpc;
using System.Configuration;
using Mono.Options;

namespace Html2Trac
{
	[XmlRpcUrl("https://your.server.org/trac/login/xmlrpc")]
    public interface Trac : IXmlRpcProxy
    {
        [XmlRpcMethod("wiki.getAllPages")]
        string[] getAllPages();

        [XmlRpcMethod("wiki.putPage")]
        bool putPage(string pagename, string content, PageAttributes attr);
    }

    // define the structure needed by the putPage method
    public struct PageAttributes {
        public string comment;
    }
	
	public class TracPutter
	{
		static TracPutter ()
		{
		}
		
		public static void Put(string pageName, string content, string comment)
		{
			Trac proxy;

		   // Fill these in appropriately
		   string user = ConfigurationManager.AppSettings["tracLogin"];
		   string password = ConfigurationManager.AppSettings["tracPassword"];
		
		   /// Create an instance of the Trac interface
		   proxy = XmlRpcProxyGen.Create<Trac>();
		
		   // If desired, point this to your URL. If you do not do this,
		   // it will use the one specified in the service declaration.
		   proxy.Url = ConfigurationManager.AppSettings["tracUrl"];
		
		   // Attach your credentials
		   proxy.Credentials = new System.Net.NetworkCredential(user, password);
		
		   PageAttributes attr;
		   attr.comment = comment;
		   bool rc = proxy.putPage(pageName, // new page name
				content, // new page contents
				attr // new page attributes
		   );
		}
	}
}

