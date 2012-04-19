using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfConsole
{
	using Mixins;

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
				FirstName = "John",
				LastName = "Doe",
				DateOfBirth = DateTime.Today
			};
			
			Person.OnPropertyChanged(c => c.IsChanged, 
				isChanged =>
			    {
			        Title = isChanged ? Title + '*' : Title.TrimEnd('*');
			    	Save.IsEnabled = Reset.IsEnabled = isChanged;
			    });

			Person.StartTrackingChanges();
			DataContext = Person;
		}

		private void ResetClick(object sender, RoutedEventArgs e)
		{
			Person.RejectChanges();
			Person.StartTrackingChanges();
		}

		private void SaveClick(object sender, RoutedEventArgs e)
		{
			Person.AcceptChanges();
			Person.StartTrackingChanges();
		}

	}
}
