using System.Windows.Forms;
using System.Windows.Input;
using KiviTR.Common;
using KiviTR.Desktop.Client.ViewModels;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace KiviTR.Desktop.Client.Views
{

    public partial class SearchView
    {
        public SearchView()
        {
            InitializeComponent();
            HotKeyManager.RegisterHotKey(Keys.D1, ModifierKeys.Shift | ModifierKeys.Alt);
            HotKeyManager.HotKeyPressed += HotKeyManagerOnHotKeyPressed;
        }

        private void HotKeyManagerOnHotKeyPressed(object sender, HotKeyEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    if (DataContext is SearchViewModel vm)
                    {
                        string copiedText = Clipboard.GetText();
                        vm.SearchAsync(copiedText);
                    }
                });
        }
    }

}