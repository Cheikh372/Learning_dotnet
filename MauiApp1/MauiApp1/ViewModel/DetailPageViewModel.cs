using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp1.ViewModel
{
    [QueryProperty("Text", "Text")]
    public partial class DetailPageViewModel : ObservableObject
    {
        public ICommand GoBackCommand { get; set; }

        public DetailPageViewModel()
        {
            GoBackCommand = new Command(GoBackAsync);

        }

        [ObservableProperty]
        string  text;


        //[ICommand]
        async void GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
