using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Copy_File.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		Thread _thread { get; set; }

		private long progressBarValue = 0;

		public long ProgressBarValue
		{
			get { return progressBarValue; }
			set { progressBarValue = value; OnPropertyChanged(); }
		}


		private long progressBarMax = 100;

		public long ProgressBarMax
		{
			get { return progressBarMax; }
			set { progressBarMax = value; OnPropertyChanged(); }
		}

		private string sourceFilePath;

		public string SourceFilePath
		{
			get { return sourceFilePath; }
			set { sourceFilePath = value; OnPropertyChanged(); }
		}

		private string destinationFilePath;


		public string DestinationFilePath
		{
			get { return destinationFilePath; }
			set { destinationFilePath = value; OnPropertyChanged(); }
		}

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}



		public MainViewModel()
		{

			_thread = new Thread(() =>
			{
				CopyFile();
			});
		}


		public void CopyFile()
		{
			if (!File.Exists(SourceFilePath))
			{
				MessageBox.Show("Wrong", "", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if (!File.Exists(DestinationFilePath))
			{
				MessageBox.Show("Wrong", "", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if (SourceFilePath == DestinationFilePath)
			{
				MessageBox.Show("Wrong", "", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			using (var filestream = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
			{
				using (var filestream2 = new FileStream(DestinationFilePath, FileMode.Open, FileAccess.Write))
				{
					filestream2.Seek(filestream2.Length, SeekOrigin.Current);
					ProgressBarMax = filestream.Length;
					byte lengthArray = 10;
					byte[] byteArray = null;
					for (long i = 0; i < ProgressBarMax; i += lengthArray)
					{
						byteArray = new byte[lengthArray];
						filestream2.Write(byteArray, 0, lengthArray);
						ProgressBarValue += lengthArray;
						filestream.Read(byteArray, 0, lengthArray);
						Thread.Sleep(2);

					}
					MessageBox.Show("Succesfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
					_thread = new Thread(() =>
					{
						CopyFile();
					});
					SourceFilePath = null;
					DestinationFilePath = null;
					ProgressBarValue = default;
					ProgressBarMax = 100;
				}
			}
		}

		public RelayCommand SourceCommand
		{
			get => new RelayCommand(() =>
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog() == true)
				{
					SourceFilePath = openFileDialog.FileName;
				}
			});
		}

		public RelayCommand DestinationCommand
		{
			get => new RelayCommand(() =>
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog() == true)
				{
					DestinationFilePath = openFileDialog.FileName;
				}
			});
		}

		public RelayCommand CopyCommand
		{
			get => new RelayCommand(() =>
			{
				if (_thread.ThreadState != ThreadState.Running)
				{
					_thread?.Start();

				}
				else MessageBox.Show("Wrong");

			});
		}


		public RelayCommand ResumeCommand
		{
			get => new RelayCommand(() =>
			{
				if (_thread.ThreadState == ThreadState.Suspended)
				{
					_thread?.Resume();
				}
			});
		}

		public RelayCommand AbortCommand
		{
			get => new RelayCommand(() =>
			{
				if (_thread.ThreadState == ThreadState.Suspended)
				{
					_thread.Resume();

				}
				_thread?.Abort();
				_thread = new Thread(() =>
				{
					CopyFile();
				});

				SourceFilePath = null;
				DestinationFilePath = null;
				ProgressBarValue = default;
				ProgressBarMax = 100;

			});
		}

		public RelayCommand SuspendCommand
		{
			get => new RelayCommand(() =>
			{
				_thread.Suspend();
			});
		}

	}

}
