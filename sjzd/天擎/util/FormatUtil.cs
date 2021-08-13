using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
namespace sjzd.天擎.util
{
    public class FormatUtil
    {
        public void outputRstXml(string xml)
        {
         /*  StringBuilder sb = new StringBuilder();
            sb.Append(xml);
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = new XmlTextWriter(sw);
            xtw.Formatting = Formatting.Indented;
            Console.WriteLine(sb); */
            this.output(xml);
        }

        public void outputRstJson(string json)
        {
          /*string formatJson = null;
            Gson gson = new GsonBuilder().setPrettyPrinting().create();
            JsonParser jp = new JsonParser();
            JsonElement je = jp.parse(json);
            formatJson= gson.toJson(je); */
            this.output(json);
        }

        public void outputRstHtml(String html)
        {
            this.outputRstXml(html);
        }

        public void outputRstText(String text)
        {
            this.output(text);
        }

      public void output(String str) {
        if( str != null ) {
      //DEMO中，只输出前1000个字母
      if( str.Length < 2000 ) {
        Console.WriteLine( str ) ;
      } else {
        Console.WriteLine( str.Substring( 0, 2000 ) ) ;
        Console.WriteLine("..........");
      }      
    }    
  }
    }
}
