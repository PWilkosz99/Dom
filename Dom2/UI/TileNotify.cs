using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Dom
{
    class TileNotify
    {

        static String[] dnitygodnia = new String[] { "Nd", "Pn","Wt", "Śr", "Czw", "Pt", "Sb"};



        /// <summary>
        /// <para>To Read: object AppData[String Value Name], to string you must use Convert.ToString(object)</para>
        /// <para>To write AppData[String Value Name] = object value to save</para>
        /// </summary>
        static Windows.Foundation.Collections.IPropertySet AppData = Windows.Storage.ApplicationData.Current.RoamingSettings.Values;  // Access roaming chce

        static public void Clear()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            for (int i = 0; i < 4; i++)
            {

                AppData["Content" + i] = null;
                AppData["TContent" + i] = null;
            }
        }

        static public void Send(String Text)
        {
            String[] content = new String[4];
            String[] tcontent = new String[4];

            string tmp;

            tmp = Convert.ToString(AppData["Content2"]);// 3 to 4
            if (tmp != String.Empty)
            {
                content[3] = tmp; 
                tmp = null;
            }

            tmp = Convert.ToString(AppData["Content1"]);// 2 to 3
            if (tmp != String.Empty)
            {
                content[2] = tmp;  
                tmp = null;
            }


            tmp = Convert.ToString(AppData["Content0"]);// 1 to 2
            if (tmp != String.Empty)
            {
                content[1] = tmp;  
                tmp = null;
            }
            content[0] = Text;        // nowy txt to 3

            //--------------Czasy--------

            tmp = Convert.ToString(AppData["TContent2"]);// 3 to 4
            if (tmp != String.Empty)
            {
                tcontent[3] = tmp;
                tmp = null;
            }

            tmp = Convert.ToString(AppData["TContent1"]);// 2 to 3
            if (tmp != String.Empty)
            {
                tcontent[2] = tmp;
                tmp = null;
            }


            tmp = Convert.ToString(AppData["TContent0"]);// 1 to 2
            if (tmp != String.Empty)
            {
                tcontent[1] = tmp;
                tmp = null;
            }

            tcontent[0] = dnitygodnia[(int)DateTime.Now.DayOfWeek] + " " + DateTime.UtcNow.ToString("HH:mm:ss");     // nowy txt to 3    



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


            // Save to roaming chce



            for (int i = 0; i < 4; i++)
            {
                if (String.IsNullOrEmpty(content[i]))
                {
                    break;
                }
                AppData["Content" + i] = content[i];
                AppData["TContent" + i] = tcontent[i];
            }

        }
       
    }
}
