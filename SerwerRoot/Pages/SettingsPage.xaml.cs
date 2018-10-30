using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static SerwerRoot.App;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace SerwerRoot.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
  

        List<ModuleBody> StuList = new List<ModuleBody>();


        //public Dictionary<ModulesId, ModuleBody> Moduless = App.Modules

        public SettingsPage()
        {
            this.InitializeComponent();

            StuList = App.Modules.Values.ToList();

            //StuList.Add(new Students()
            //{
            //    ID = 1,
            //    Name = "Ammar1"
            //});
            //StuList.Add(new Students()
            //{
            //    ID = 2,
            //    Name = "Ammar2"
            //});
            //StuList.Add(new Students()
            //{
            //    ID = 3,
            //    Name = "Ammar3"
            //});
            //StuList.Add(new Students()
            //{
            //    ID = 4,
            //    Name = "Ammar4"
            //});
            //StuList.Add(new Students()
            //{
            //    ID = 5,
            //    Name = "Ammar5"
            //});


            

        }

    }

    public class Students
    {
        public int ID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
