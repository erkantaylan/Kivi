using System.Windows;
using KiviTR.Desktop.Client.Views;
using Prism.Ioc;
using Prism.Unity;

namespace KiviTR.Desktop.Client
{

    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        { }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }
    }

}