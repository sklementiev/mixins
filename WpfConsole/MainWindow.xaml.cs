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
		
		public MainWindow()
		{
			InitializeComponent();

			Person = new Person
			{
				FirstName = "Bob",
				LastName = "Marley",
				DateOfBirth = DateTime.Today
			};
			
			Person.OnPropertyChanged(c => c.IsChanged, 
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
            Person.RejectChanges();
            Person.BeginEdit();
		}

		private void SaveClick(object sender, RoutedEventArgs e)
		{
            Person.AcceptChanges();
            Person.BeginEdit();
		}
	}
}
