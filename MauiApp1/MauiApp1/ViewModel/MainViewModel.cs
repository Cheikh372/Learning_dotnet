using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
//using System.Windows.Input;


namespace MauiApp1.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public ICommand AddTaskCommand { get; set; }   // l'anotation n'a pas marché avec ICommand
        public ICommand DeleteTaskCommande {  get; set; }  
        public ICommand GoToDetailTaskCommande {  get; set; }

        IConnectivity connectivity;

        public MainViewModel(IConnectivity connectivity) 
        {
            items = new ObservableCollection<string>();
            AddTaskCommand = new Command(AddTask) ;
            DeleteTaskCommande = new Command(DeleteTask);
            GoToDetailTaskCommande = new Command(GoToDetailTask);
            this.connectivity = connectivity;
        }

       

        [ObservableProperty]
        ObservableCollection<string> items;


        [ObservableProperty]
        string text;

      
        async void AddTask()
        {
            if (string.IsNullOrEmpty(Text))
                return;

            if(connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Ouffff", "No Internet ","Ok");
                return;
            }
            Items.Add(Text);
            Text = string.Empty;
        }

        private void DeleteTask(object text)
        {
            var s = text as string;

            if (Items.Contains(s))
            {
                Items.Remove(s);

            }
        }
       private async void GoToDetailTask(object text)
        {
            var s = text as string;
            await Shell.Current.GoToAsync($"{nameof(DetailPageView)}?Text={s}");
        }
      
    }
}
