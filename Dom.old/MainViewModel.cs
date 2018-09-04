using GalaSoft.MvvmLight;
using Dom.Models;
using Dom.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Dom.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>(GetMenuItems());
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        private MenuItem selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set { selectedMenuItem = value; RaisePropertyChanged(); }
        }

        private List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem() { Title = "Brama Wjazdowa", SymbolIcon = Symbol.Home, NavigateTo = typeof(Brama) });
            menuItems.Add(new MenuItem() { Title = "Work in progress...", SymbolIcon = Symbol.OutlineStar, NavigateTo = typeof(Home) });
            menuItems.Add(new MenuItem() { Title = "Work in progress...", SymbolIcon = Symbol.Map, NavigateTo = typeof(Map) });
            menuItems.Add(new MenuItem() { Title = "Work in progress...", SymbolIcon = Symbol.Video, NavigateTo = typeof(Video) });
            menuItems.Add(new MenuItem() { Title = "Work in progress...", SymbolIcon = Symbol.Download, NavigateTo = typeof(Download) });

            return menuItems;
        }
    }
}
