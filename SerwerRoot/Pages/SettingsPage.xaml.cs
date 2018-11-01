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
  

        List<ModuleBody> ModulesList = App.Modules.Values.ToList();

        //public Dictionary<ModulesId, ModuleBody> Moduless = App.Modules

        public SettingsPage()
        {
            this.InitializeComponent();
            
        }

        private void lightToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if(toggleSwitch == null)
            {
                return;
            }

            ModuleBody ok = toggleSwitch.DataContext as ModuleBody;

            Modules[ok.Id].Change(toggleSwitch.IsOn);
            
        }

        private void lightToggle_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch == null)
            {
                return;
            }

            ModuleBody ok = toggleSwitch.DataContext as ModuleBody;

            toggleSwitch.IsOn = Modules[ok.Id].On;
        }
    }
}
