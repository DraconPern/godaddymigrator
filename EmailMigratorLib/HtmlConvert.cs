using System;
using System.IO;

namespace HtmlAgilityPack.Samples
{
	public class HtmlToText
	{
		public HtmlToText()
		{
		}

		public string Convert(string path)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(path);

			StringWriter sw = new StringWriter();
			ConvertTo(doc.DocumentNode, sw);
			sw.Flush();
			return sw.ToString();
		}

		public string ConvertHtml(string html)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			StringWriter sw = new StringWriter();
			ConvertTo(doc.DocumentNode, sw);
			sw.Flush();
			return sw.ToString();
		}

		private void ConvertContentTo(HtmlNode node, TextWriter outText)
		{
			foreach(HtmlNode subnode in node.ChildNodes)
			{
				ConvertTo(subnode, outText);
			}
		}

		public void ConvertTo(HtmlNode node, TextWriter outText)
		{
			string html;
			switch(node.NodeType)
			{
				case HtmlNodeType.Comment:
					// don't output comments
					break;

				case HtmlNodeType.Document:
					ConvertContentTo(node, outText);
					break;

				case HtmlNodeType.Text:					
					// get text
					html = ((HtmlTextNode)node).Text.Trim();

					// is it in fact a special closing node output as text?
					if (HtmlNode.IsOverlappedClosingElement(html))
						break;

                    html = html.Replace("&nbsp;", " ");

					outText.Write(HtmlEntity.DeEntitize(html));
					
					break;
				case HtmlNodeType.Element:
					switch(node.Name)
					{
                        case "font":
                            break;
                        case "br":        
							// treat paragraphs as crlf
							outText.Write("\r\n");
							break;
					}
                    
					if (node.HasChildNodes)
					{
						ConvertContentTo(node, outText);
					}
					break;
			}
		}
	}
}
