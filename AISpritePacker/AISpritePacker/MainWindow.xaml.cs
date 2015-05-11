using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using System.ComponentModel;

namespace AISpritePacker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		int canvasItemsXPos = 0;
		int canvasItemsYPos = 0;
		int maxItemsWidth = 512;

		private int i_margin;
		public int i_Margin
		{
			get { return i_margin; }
			set
			{
				i_margin = value;
				NotifyPropertyChanged("i_Margin");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
			Canvas_Sprites.Width = 512;
			Canvas_Sprites.Height = 512;
			i_Margin = 10;
			maxItemsWidth = Convert.ToInt32(Canvas_Sprites.Width);
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

					//Set the canvas left to the width of total items
					cardImage.SetValue(Canvas.LeftProperty, (double)canvasItemsXPos);
					cardImage.SetValue(Canvas.TopProperty, (double)canvasItemsYPos);

					//add the image width to total item width + margin
					canvasItemsXPos += ((int)cardImage.Source.Width + i_Margin);
					
					
					//
					if ((canvasItemsXPos + cardImage.Source.Width) > maxItemsWidth)
					{	
						canvasItemsXPos = 0;
						canvasItemsYPos = (int)GetMaxY(Canvas_Sprites) + i_Margin;
					}

					//Add the image to the canvas
					Canvas_Sprites.Children.Add(cardImage);
				}
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
				temp = Canvas.GetTop(child) + child.Source.Height;
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

        private void Execute_Save(object sender, RoutedEventArgs e)
        {
			Canvas_Sprites.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
			SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PNG Files (*.png) | *.png";
			saveFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (saveFile.ShowDialog() == true)
            {
                //Export PNG
				string fileName = System.IO.Path.GetFileNameWithoutExtension(saveFile.FileName);
                string extension = System.IO.Path.GetExtension(saveFile.FileName);
                string folder = System.IO.Path.GetDirectoryName(saveFile.FileName);

                Console.WriteLine("Saving...");
                Console.WriteLine("File: " + fileName + extension + "\nFolder: " + folder + "\\");	

                RenderTargetBitmap rtb = GetImage(Canvas_Sprites);
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));
                using (Stream stm = new FileStream(folder + "\\" + fileName + extension, FileMode.Create))
                {
                    png.Save(stm);
                }

				//Export XML
				XDocument srcTree = new XDocument();

				//Root Node
				XElement root = new XElement("atlas");
				root.SetAttributeValue("width", Canvas_Sprites.Width);
				root.SetAttributeValue("height", Canvas_Sprites.Height);
				root.SetAttributeValue("sheet", "filePath");

				//Sprite Node
				foreach (Image child in Canvas_Sprites.Children)
				{
					XElement sprite = new XElement("sprite");
					sprite.SetAttributeValue("Name", System.IO.Path.GetFileNameWithoutExtension(child.Source.ToString()));
					sprite.SetAttributeValue("x0", Canvas.GetLeft(child));
					sprite.SetAttributeValue("x1", Canvas.GetLeft(child) + child.Source.Width);
					sprite.SetAttributeValue("y0", Canvas.GetTop(child));
					sprite.SetAttributeValue("y1", Canvas.GetTop(child) + child.Source.Height);
					root.Add(sprite);
				}
				srcTree.Add(root);

				Console.WriteLine(srcTree);

				extension = ".xml";
				Console.WriteLine("Saving...");
				Console.WriteLine("File: " + fileName + extension + "\nFolder: " + folder + "\\");

				using (Stream stm = new FileStream(folder + "\\" + fileName + extension, FileMode.Create))
				{
					srcTree.Save(stm);
				}

            }
			Canvas_Sprites.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        }

        //New/Clear Sprite Sheet
        private void MenuItem_Click_New(object sender, RoutedEventArgs e)
        {
            Canvas_Sprites.Children.Clear();
            canvasItemsXPos = 0;
            canvasItemsYPos = 0;
            try
            {
                Canvas_Sprites.Height = Convert.ToInt32(txt_CanvasHeight.Text);
                Canvas_Sprites.Width = Convert.ToInt32(txt_CanvasHeight.Text);
            }
            catch (Exception exz)
            {
                string messageBoxText = ("An error has occured: Canvas Height/Width invalid.");
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                //Display box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }
	}
}
