using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace AISpritePacker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		int canvasItemsWidth = 0;
		int canvasItemsHeight = 0;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
		{

		}
		private void MenuItem_Click_Import(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == true)
			{
				foreach (string filename in openFileDialog.FileNames)
				{
					Image cardImage = new Image();
					cardImage.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(filename)));
					Canvas_Sprites.Children.Add(cardImage);
					Canvas_Sprites.Children[0].AllowDrop = true;

				}
			}
		}

		private void Canvas_Sprites_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] filenames = e.Data.GetData(DataFormats.FileDrop) as string[];
				foreach (string filename in filenames)
				{
					//Construct image placeholder / set source
					Image cardImage = new Image();
					cardImage.Source = new BitmapImage(new Uri(filename));

					//Add filepath to left listbox
					//lbx_Left.Items.Add(filename);

					//Set the canvas left to the width of total items
					cardImage.SetValue(Canvas.LeftProperty, (double)canvasItemsWidth);
					cardImage.SetValue(Canvas.TopProperty, (double)canvasItemsHeight);

					//add the image width to total item width
					
					//canvasItemsHeight += (int)cardImage.Source.Width;
					if (canvasItemsWidth == 0 && canvasItemsHeight != 0)
					{
						Canvas_Sprites.Height = canvasItemsHeight + (int)cardImage.Source.Height;
					}
					canvasItemsWidth += (int)cardImage.Source.Width;
					
					
					//
					if (canvasItemsWidth > 300)
					{
						Canvas_Sprites.Width = canvasItemsWidth;
						canvasItemsWidth = 0;
						canvasItemsHeight += (int)cardImage.Source.Height;
					}

					//output total items width to list box
					//lbx_Bottom.Items.Add(canvasItemsWidth);

					//Set drop event true
					cardImage.AllowDrop = true;

					//Add the image to the canvas
					 Canvas_Sprites.Children.Add(cardImage);
				}
			}
		}
	}
}
