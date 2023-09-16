using MauiApp1.View;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //register Route page
            Routing.RegisterRoute(nameof(DetailPageView), typeof(DetailPageView));
        }
    }
}