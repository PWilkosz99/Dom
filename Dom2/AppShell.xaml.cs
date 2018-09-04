using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Dom.Views;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Dom
{
    /// <summary>
    /// The "chrome" layer of the app that provides top-level navigation with
    /// proper keyboarding navigation.
    /// </summary>
    public sealed partial class AppShell : Page
    {
      
        public static AppShell Current = null;
        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public AppShell()
        {

            this.InitializeComponent();
            //ApplicationView.PreferredLaunchViewSize = new Size(1000, 800);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            this.Loaded += (sender, args) =>
            {


                Current = this;
                Client.ConnectAsnc();

                //this.TogglePaneButton.Focus(FocusState.Programmatic);
            };

            

           // NavMenuList.ItemsSource = NavLow;
        }



       //// public Frame AppFrame { get { return this.frame; } }

       // /// <summary>
       // /// Default keyboard focus movement for any unhandled keyboarding
       // /// </summary>
       // /// <param name="sender"></param>
       // /// <param name="e"></param>
       // private void AppShell_KeyDown(object sender, KeyRoutedEventArgs e)
       // {
       //     FocusNavigationDirection direction = FocusNavigationDirection.None;
       //     switch (e.Key)
       //     {
       //         case Windows.System.VirtualKey.Left:
       //         case Windows.System.VirtualKey.GamepadDPadLeft:
       //         case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
       //         case Windows.System.VirtualKey.NavigationLeft:
       //             direction = FocusNavigationDirection.Left;
       //             break;
       //         case Windows.System.VirtualKey.Right:
       //         case Windows.System.VirtualKey.GamepadDPadRight:
       //         case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
       //         case Windows.System.VirtualKey.NavigationRight:
       //             direction = FocusNavigationDirection.Right;
       //             break;

       //         case Windows.System.VirtualKey.Up:
       //         case Windows.System.VirtualKey.GamepadDPadUp:
       //         case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
       //         case Windows.System.VirtualKey.NavigationUp:
       //             direction = FocusNavigationDirection.Up;
       //             break;

       //         case Windows.System.VirtualKey.Down:
       //         case Windows.System.VirtualKey.GamepadDPadDown:
       //         case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
       //         case Windows.System.VirtualKey.NavigationDown:
       //             direction = FocusNavigationDirection.Down;
       //             break;
       //     }

       //     if (direction != FocusNavigationDirection.None)
       //     {
       //         var control = FocusManager.FindNextFocusableElement(direction) as Control;
       //         if (control != null)
       //         {
       //             control.Focus(FocusState.Programmatic);
       //             e.Handled = true;
       //         }
       //     }
       // }

       // #region BackRequested Handlers

       // private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
       // {
       //     bool handled = e.Handled;
       //     this.BackRequested(ref handled);
       //     e.Handled = handled;
       // }

       // private void BackButton_Click(object sender, RoutedEventArgs e)
       // {
       //     bool ignored = false;
       //     this.BackRequested(ref ignored);
       // }

       // private void BackRequested(ref bool handled)
       // {
       //     // Get a hold of the current frame so that we can inspect the app back stack.

       //     //if (this.AppFrame == null)
       //     //    return;

       //     //// Check to see if this is the top-most page on the app back stack.
       //     //if (this.AppFrame.CanGoBack && !handled)
       //     //{
       //     //    // If not, set the event to handled and go back to the previous page in the app.
       //     //    handled = true;
       //     //    this.AppFrame.GoBack();
       //     //}
       // }

       // #endregion

       // #region Settings

       // private void SettingsButton_Click(object sender, RoutedEventArgs e)
       // {
       //     //if (this.AppFrame.CurrentSourcePageType != typeof(SettingsPage))
       //     //{
       //     //    this.AppFrame.Navigate(typeof(SettingsPage), null);
       //     //}
       // }

       // #endregion

       // #region Navigation

       // /// <summary>
       // /// Navigate to the Page for the selected <paramref name="listViewItem"/>.
       // /// </summary>
       // /// <param name="sender"></param>
       // /// <param name="listViewItem"></param>
       // //private async void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
       // //{
       // //    var item = (NavMenuItem)((NavMenuListView)sender).ItemFromContainer(listViewItem);

       // //    if (item != null)
       // //    {
       // //        if (item.DestinationPage != null)
       // //        {
       // //            if (item.DestinationPage == typeof(Uri))
       // //            {
       // //                // Grab the URL from the argument
       // //                Uri url = null;
       // //                if (Uri.TryCreate(item.Arguments as string, UriKind.Absolute, out url))
       // //                {
       // //                    await Launcher.LaunchUriAsync(url);
       // //                }
       // //            }
       // //            //else if (item.DestinationPage != this.AppFrame.CurrentSourcePageType)
       // //            //{
       // //            //    this.AppFrame.Navigate(item.DestinationPage, item.Arguments);
       // //            //}
       // //        }
       // //    }
       // //}

       // /// <summary>
       // /// Ensures the nav menu reflects reality when navigation is triggered outside of
       // /// the nav menu buttons.
       // /// </summary>
       // /// <param name="sender"></param>
       // /// <param name="e"></param>
       // private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
       // {
       //     if (e.NavigationMode == NavigationMode.Back)
       //     {
       //         var item = (from p in this.NavLow where p.DestinationPage == e.SourcePageType select p).SingleOrDefault();
       //         //if (item == null && this.AppFrame.BackStackDepth > 0)
       //         //{
       //         //    // In cases where a page drills into sub-pages then we'll highlight the most recent
       //         //    // navigation menu item that appears in the BackStack
       //         //    foreach (var entry in this.AppFrame.BackStack.Reverse())
       //         //    {
       //         //        item = (from p in this.NavLow where p.DestinationPage == entry.SourcePageType select p).SingleOrDefault();
       //         //        if (item != null)
       //         //            break;
       //         //    }
       //         //}

       //         //var container = (ListViewItem)NavMenuList.ContainerFromItem(item);

       //         // While updating the selection state of the item prevent it from taking keyboard focus.  If a
       //         // user is invoking the back button via the keyboard causing the selected nav menu item to change
       //         // then focus will remain on the back button.
       //        // if (container != null) container.IsTabStop = false;
       //         //NavMenuList.SetSelectedItem(container);
       //       //  if (container != null) container.IsTabStop = true;
       //     }
       // }

       // private void OnNavigatedToPage(object sender, NavigationEventArgs e)
       // {
       //     // After a successful navigation set keyboard focus to the loaded page
       //     if (e.Content is Page && e.Content != null)
       //     {
       //         var control = (Page)e.Content;
       //         control.Loaded += Page_Loaded;
       //     }
       // }

       // private void Page_Loaded(object sender, RoutedEventArgs e)
       // {
       //     ((Page)sender).Focus(FocusState.Programmatic);
       //     ((Page)sender).Loaded -= Page_Loaded;
       //     this.CheckTogglePaneButtonSizeChanged();
       // }

       // #endregion

       // public Rect TogglePaneButtonRect
       // {
       //     get;
       //     private set;
       // }

       // /// <summary>
       // /// An event to notify listeners when the hamburger button may occlude other content in the app.
       // /// The custom "PageHeader" user control is using this.
       // /// </summary>
       // public event TypedEventHandler<AppShell, Rect> TogglePaneButtonRectChanged;

       // /// <summary>
       // /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
       // /// then the floating hamburger may be occluding other content in the app unless it is aware.
       // /// </summary>
       // /// <param name="sender"></param>
       // /// <param name="e"></param>
       // private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
       // {
       //     this.CheckTogglePaneButtonSizeChanged();
       // }

       // /// <summary>
       // /// Check for the conditions where the navigation pane does not occupy the space under the floating
       // /// hamburger button and trigger the event.
       // /// </summary>
       // private void CheckTogglePaneButtonSizeChanged()
       // {
       //     //if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
       //     //    this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
       //     //{
       //     //    var transform = this.TogglePaneButton.TransformToVisual(this);
       //     //    var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
       //     //    this.TogglePaneButtonRect = rect;
       //     //}
       //     //else
       //     //{
       //     //    this.TogglePaneButtonRect = new Rect();
       //     //}

       //     var handler = this.TogglePaneButtonRectChanged;
       //     if (handler != null)
       //     {
       //         // handler(this, this.TogglePaneButtonRect);
       //         handler.DynamicInvoke(this, this.TogglePaneButtonRect);
       //     }
       // }

        /// <summary>
        /// Enable accessibility on each nav menu item by setting the AutomationProperties.Name on each container
        /// using the associated Label of each item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavMenuItemContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item != null && args.Item is NavMenuItem)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((NavMenuItem)args.Item).Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }



        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {            
            //draw into the title bar
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            //remove the solid-colored backgrounds behind the caption controls and system back button
            //ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            //titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;



            // you can also add items in code behind
            //NavView.MenuItems.Add(new NavigationViewItemSeparator());
            //NavView.MenuItems.Add(new NavigationViewItem()
            //{ Content = "My content", Icon = new SymbolIcon(Symbol.Folder), Tag = "content" });

            //// set the initial SelectedItem 
            //foreach (NavigationViewItemBase item in NavView.MenuItems)
            //{
            //    if (item is NavigationViewItem && item.Tag.ToString() == "apps")
            //    {
            //        NavView.SelectedItem = item;
            //        break;
            //    }
            //}
        }       

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                switch (args.InvokedItem)
                {
                    case "Brama":
                        ContentFrame.Navigate(typeof(Brama));
                        break;

                    case "Biurko":
                        ContentFrame.Navigate(typeof(Biurko));
                        break;

                    case "SettingsPage":
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;

                }
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {

                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch (item.Tag)
                {
                    case "Brama":
                        ContentFrame.Navigate(typeof(Brama));
                        break;

                    case "Biurko":
                        ContentFrame.Navigate(typeof(Biurko));
                        break;

                    case "SettingsPage":
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;
                }
            }
        }














        public async Task GetuserAsync()
        {

            IReadOnlyList<User> users = await User.FindAllAsync(UserType.LocalUser);

            var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated && p.Type == UserType.LocalUser).FirstOrDefault();


            var data = (string)await current.GetPropertyAsync(KnownUserProperties.DisplayName);

            if (String.IsNullOrEmpty(data.ToString()))
            {
                data = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
            }
        }
    }
}
