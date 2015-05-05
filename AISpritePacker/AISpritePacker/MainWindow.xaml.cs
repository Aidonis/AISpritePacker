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
		int canvasItemsXPos = 0;
		int canvasItemsYPos = 0;
		int maxItemsWidth = 512;
		double newHeight = 0;

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

		//Import Image from drag/drop
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
					Canvas_Sprites.Width = maxItemsWidth;
					newHeight = canvasItemsYPos + cardImage.Source.Height;
					if (newHeight < Canvas_Sprites.Height)
					{
						newHeight = canvasItemsYPos + cardImage.Source.Height + 10;
					}
					Canvas_Sprites.Height = newHeight;

					//Set the canvas left to the width of total items
					cardImage.SetValue(Canvas.LeftProperty, (double)canvasItemsXPos);
					cardImage.SetValue(Canvas.TopProperty, (double)canvasItemsYPos);

					//add the image width to total item width + margin
					canvasItemsXPos += ((int)cardImage.Source.Width + 10);
					
					
					//
					if ((canvasItemsXPos + cardImage.Source.Width) > maxItemsWidth)
					{	
						canvasItemsXPos = 0;
						canvasItemsYPos = (int)GetMaxY(Canvas_Sprites) + 10;
					}

					//Add the image to the canvas
					Canvas_Sprites.Children.Add(cardImage);
					testBox.Items.Add(GetMaxY(Canvas_Sprites));
				}
			}
		}

		//Export image controls
		private void Button_Click_Export(object sender, RoutedEventArgs e)
		{
			RenderTargetBitmap rtb = GetImage(Canvas_Sprites);
			PngBitmapEncoder png = new PngBitmapEncoder();
			png.Frames.Add(BitmapFrame.Create(rtb));
			using (Stream stm = new FileStream("C:/Users/aidan.nabass/Documents/PackerSaves/new.png", FileMode.Create))
			{
				png.Save(stm);
			}
			
			
		}

		//Return view as RenderTargetBitmap
		public static RenderTargetBitmap GetImage(Canvas view)
		{
			Size size = new Size(view.ActualWidth, view.ActualHeight);
			if (size.IsEmpty)
			{
				return null;
			}
			RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

			DrawingVisual drawingvisual = new DrawingVisual();
			using (DrawingContext context = drawingvisual.RenderOpen())
			{
				context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
				context.Close();
			}
			result.Render(drawingvisual);
			return result;
		}

		//Get next row height
		public static double GetMaxY(Canvas view)
		{
			double maxY = 0;
			double temp = 0;
			foreach (Image child in view.Children)
			{
				temp = Canvas.GetTop(child);
				if(maxY <= temp)
				{
					maxY = Canvas.GetTop(child) + child.Source.Height;
				}

					
			}
			return maxY;
		}

		//Get furthest right
		public static double GetMaxX(Canvas view)
		{
			double maxX = 0;
			foreach (Image child in view.Children)
			{
				if (Canvas.GetRight(child) > maxX)
				{
					maxX = Canvas.GetRight(child);
				}
			}

			return maxX;
		}
	}
}
