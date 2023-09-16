using MauiApp1.ViewModel;

namespace MauiApp1.View;

public partial class DetailPageView : ContentPage
{
	public DetailPageView(DetailPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}