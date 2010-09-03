using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Html2Trac
{
	class MainClass
	{
		private static Stack<string> liMarkers = new Stack<string>();		
		
		public static void Main (string[] args)
		{
			//string filename = args[0];
			XmlDocument doc = new XmlDocument();
			doc.Load(Console.In);
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("x", doc.DocumentElement.NamespaceURI);
			WriteNode( doc.DocumentElement.SelectSingleNode("//x:body",nsmgr), Console.Out);
//			foreach(XmlNode n in doc.DocumentElement.ChildNodes)
//			{
//				Console.WriteLine ("{0}, {1}, {2}", nsmgr.DefaultNamespace,n.NamespaceURI, n.Name);
//			}
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

