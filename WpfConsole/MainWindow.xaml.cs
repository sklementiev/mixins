using System;
using System.Windows;
using Mixins;

namespace WpfConsole
{
    /// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal Person Person;
        internal PersonDynamic PersonDynamic;
        
		public MainWindow()
		{
			InitializeComponent();

            Person = new Person
            {
                FirstName = "Ron",
                LastName = "Sponge",
            };
			
			Person.OnPropertyChanged(c => c.IsChanged, 
				isChanged =>
			    {
			        Title = isChanged ? Title + '*' : Title.TrimEnd('*');
			    	Save.IsEnabled = Reset.IsEnabled = isChanged;
			    });

            PersonDynamic = new PersonDynamic();

            PersonDynamic.OnPropertyChanged(c => c.IsChanged,
                isChanged =>
                {
                    Title = isChanged ? Title + '*' : Title.TrimEnd('*');
                    Save.IsEnabled = Reset.IsEnabled = isChanged;
                });

			Person.BeginEdit();
			DataContext = Person;
		}

		private void ResetClick(object sender, RoutedEventArgs e)
		{
            if (dynamicVM.IsChecked.Value)
            {
                PersonDynamic.RejectChanges();
                PersonDynamic.BeginEdit();
            }
            else
            {
                Person.RejectChanges();
                Person.BeginEdit();
            }
		}

		private void SaveClick(object sender, RoutedEventArgs e)
		{
            Person.AcceptChanges();
            Person.BeginEdit();
		}

        private void dynamicVM_Unchecked(object sender, RoutedEventArgs e)
        {
            DataContext = Person;
            PersonDynamic.BeginEdit();
        }

        private void dynamicVM_Checked(object sender, RoutedEventArgs e)
        {
            DataContext = PersonDynamic;
            PersonDynamic.BeginEdit();
        }
	}
}
