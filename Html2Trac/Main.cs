using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using Mono.Options;

namespace Html2Trac
{
	class MainClass
	{
		private static Stack<string> liMarkers = new Stack<string>();		
		
		public static void Main (string[] args)
		{
			string name = "", comment = "updated by Html2Trac";
			bool showHelp = false;
			bool useTrac = false;
			var p = new OptionSet() {
				{ "n|name=", "The page name to modify", s => name = s },
				{ "c|comment:", "associate a comment", s => comment = s },
				{ "t|trac", "use trac", v => useTrac = v != null },
				{ "h|help", v => showHelp = v != null }
			};
			try
			{
				p.Parse(args);
			}
			catch (OptionException e)
			{
				ShowHelp(p);
				return;
			}
			if (showHelp)
			{
				ShowHelp(p);
				return;
			}
			
			//string filename = args[0];
			XmlDocument doc = new XmlDocument();
			doc.Load(Console.In);
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("x", doc.DocumentElement.NamespaceURI);
			
			TextWriter writer;
			if (!useTrac)
			{
				writer = Console.Out;
			}
			else
			{
				writer = new StringWriter();
			}
			WriteNode( doc.DocumentElement.SelectSingleNode("//x:body",nsmgr), writer);
//			foreach(XmlNode n in doc.DocumentElement.ChildNodes)
//			{
//				Console.WriteLine ("{0}, {1}, {2}", nsmgr.DefaultNamespace,n.NamespaceURI, n.Name);
//			}
			if (useTrac)
			{
				TracPutter.Put(name, writer.ToString(), comment);
			}
		}
		
		private static void ShowHelp(OptionSet p)
		{
			Console.WriteLine ("Usage: Html2Trac [OPTIONS]");
			Console.WriteLine ("Converts html from stdin and posts to page");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions(Console.Out);
		}
		
		private static void WriteNode(XmlNode node, TextWriter writer )
		{
			if( node == null )
				return;
			bool newLine = true;
			bool closeTag = true;			
			string tag = string.Empty;
			bool popLiStack = false;
			if( node.NodeType == XmlNodeType.Text )
			{
				writer.Write(node.InnerText);
				return;
			}
			switch (node.LocalName.ToLower ())
			{
			case "h1":
				tag = "=";
				break;
			case "h2":
				tag = "==";
				break;
			case "h3":
				tag = "===";
				break;
			case "h4":
				tag = "====";
				break;
			case "h5":
				tag = "=====";
				break;
			case "b":
			case "strong":
				tag = "'''";
				newLine = false;
				break;
			case "i":
			case "em":
				tag = "''";
				newLine = false;
				break;
			case "ol":				
				liMarkers.Push("1. ");
				popLiStack = true;
				newLine = false;
				break;
			case "ul":
				liMarkers.Push("* ");
				popLiStack = true;
				newLine = false;
				break;
			case "li":
				tag = new string(' ', liMarkers.Count) + liMarkers.Peek();
				closeTag = false;
				newLine = false;
				break;
			case "p":
				newLine = true;
				break;
			default:
				newLine = false;
				break;
			}
			
			if (tag != string.Empty)
			{
				writer.Write (tag + (newLine ? " " : ""));
			}
			foreach (XmlNode n in node.ChildNodes)
			{
				WriteNode(n, writer);
			}
			if( closeTag )
				writer.Write((newLine ? " " : "") + tag);
			if( newLine )
				writer.WriteLine();
			if( popLiStack )
				liMarkers.Pop();
		}
	}
}

