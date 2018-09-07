using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Dom_Background
{
    class TileNotify
    {
        static String[] content = new String[4];
        static String[] tcontent = new String[4];
        static String[] dnitygodnia = new String[] { "Nd", "Pn","Wt", "Śr", "Czw", "Pt", "Sb"};

        static public void Clear()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        static public void Send(String Text)
        {

            content[3] = content[2];  // 3 to 4
            content[2] = content[1];  // 2 to 3
            content[1] = content[0];  // 1 to 2
            content[0] = Text;        // nowy txt to 3


            tcontent[3] = tcontent[2];   // 3 to 4
            tcontent[2] = tcontent[1];  // 2 to 3
            tcontent[1] = tcontent[0];  // 1 to 2
            tcontent[0] = dnitygodnia[(int)DateTime.Now.DayOfWeek ] + " " +  DateTime.UtcNow.ToString("HH:mm:ss");     // nowy txt to 3         

            

            int step = 0;

            for(int a = 0; a < content.Length; a++)
            {
                if(!String.IsNullOrEmpty(content[a]))
                {
                    step++;
                }
            }



            string xml = $@"
                <tile version='3'>
                    <visual branding='nameAndLogo'>

                        <binding template='TileMedium'>";

            for(int a = 0; a < step; a++)
            {
                xml += @"<text hint-wrap='true'>" + content[a]  + "</text>";
            }

            xml += @"
                            <text hint-wrap='true' hint-style='captionSubtle'/>
                        </binding>

                        <binding template='TileWide'>";
            for (int a = 0; a < step; a++)
            {
                xml += @"<text hint-wrap='true'>" + tcontent[a] + ",  " +  content[a] + "</text>";
            }

            xml += @"
                            <text hint-wrap='true' hint-style='captionSubtle'/>
                        </binding>

                        <binding template='TileLarge'>";
            for (int a = 0; a < step; a++)
            {
                xml += @"<text hint-wrap='true'>" + tcontent[a] + ",  " + content[a] + "</text>";
            }

            xml += @"
                            <text hint-wrap='true' hint-style='captionSubtle'/>
                        </binding>

                </visual>
            </tile>";
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);


            TileNotification notification = new TileNotification(doc);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
