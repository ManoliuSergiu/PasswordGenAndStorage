﻿using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
namespace Password_Gen_and_Storage
{
	public partial class MainWindow : Window
	{
		string lowercase = "abcdefghijklmnopqrstuvwxyz";
		string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string numbers = "0123456789";
		string symbols = ",<.>/?;:'[{]}\\|~!@#$%^&*()_\"";
		string loginpath = @"../../LogIn.txt";
		string passwordspath = @"../../Passwords.txt";
		char[] split = {'☺'};
		Random random = new Random();

		public MainWindow()
		{
			InitializeComponent();
			genGrid.Visibility = Visibility.Visible;
			loginGrid.Visibility = Visibility.Hidden;
			pwviewGrid.Visibility = Visibility.Hidden;
			pwaddGrid.Visibility = Visibility.Hidden;

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string characters = null;

			if (lowercaseCheckbox.IsChecked == true) //IsChecked is a nullable bool
				characters += lowercase;

			if (uppercaseCheckbox.IsChecked == true)
				characters += uppercase;

			if (numbersCheckbox.IsChecked == true)
				characters += numbers;

			if (symbolsCheckbox.IsChecked == true)
				characters += symbols;
			
			if (characters == null)
			{
				outputTextBox.Text = "You must select a character set";
			}
			else
			{

				if (int.TryParse(lengthTextBox.Text, out int passwordLength))
				{
					string password = null;

					for (int i = 0; i < passwordLength; i++)
					{
						password += characters[random.Next(characters.Length)];
					}

					outputTextBox.Text = password;
				}
				else
				{
					outputTextBox.Text = "You must insert a number";
				}
			}
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			LoginTransition();
		}

		private void LoginTransition()
		{
			genGrid.Visibility = Visibility.Hidden;
			loginGrid.Visibility = Visibility.Visible;
		}

		private void Login_Click(object sender, RoutedEventArgs e)
		{
			StreamReader a = new StreamReader(loginpath);
			string buffer;
			while ((buffer = a.ReadLine()) != null && buffer != "")
			{
				string[] aux = buffer.Split(split,StringSplitOptions.RemoveEmptyEntries);
				if (usernameTextBox.Text == aux[0])
				{
					byte[] encpw = new byte[aux.Length-1];
					for (int i = 0; i < aux.Length-1; i++)
					{
						encpw[i] = byte.Parse(aux[i + 1]);
					}
					if (pwBox.Password == Crypt.DecryptString(encpw))
					{
						Login(usernameTextBox.Text);
					}
					else
					{
						errorLabel.Content = "Wrong username/password";
					}
				}
			}
			a.Close();
		}

		private void Login(string v)
		{
			loginGrid.Visibility = Visibility.Hidden;
			pwaddGrid.Visibility = Visibility.Hidden;
			pwviewGrid.Visibility = Visibility.Visible;
			dataGrid.Items.Clear();
			welcomeLabel.Content = "Welcome back, " + v;
			addButton.Tag = v;
			List<Password> passwords = new List<Password>();
			errorLabel.Content = "";
			StreamReader a = new StreamReader(passwordspath);
			string buffer;
			while ((buffer = a.ReadLine()) != null && buffer != "")
			{
				string[] aux = buffer.Split(split, StringSplitOptions.RemoveEmptyEntries);
				if (v == aux[0])
				{
					byte[] encpw = new byte[aux.Length - 2];
					for (int i = 0; i < aux.Length - 2; i++)
					{
						encpw[i] = byte.Parse(aux[i + 2]);
					}
					string pass = Crypt.DecryptString(encpw);
					Password auxpass = new Password()
					{
						platform = aux[1],
						password = pass
					};
					passwords.Add(auxpass);
				}
			}
			a.Close();
			foreach (Password password in passwords)
			{
				dataGrid.Items.Add(password);
			}
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			StreamReader a = new StreamReader(loginpath);
			string buffer;
			bool ok = true;
			while ((buffer = a.ReadLine()) != null && buffer !="")
			{
				string[] aux1 = buffer.Split(split, StringSplitOptions.RemoveEmptyEntries);
				if (usernameTextBox.Text == aux1[0])
				{
					errorLabel.Content = "Username already taken";
					ok = false;
				}
			}
			a.Close();
			if (ok)
			{

				string aux = null;
				byte[] pw = Crypt.EncryptString(pwBox.Password);
				aux += usernameTextBox.Text + "☺";
				for (int i = 0; i < pw.Length; i++)
				{
					aux += pw[i] + "" + split[0];
				}
				File.AppendAllText(loginpath, aux + Environment.NewLine);
			}
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			pwviewGrid.Visibility = Visibility.Hidden;
			pwaddGrid.Visibility = Visibility.Visible;

		}

		private void AddpwButton_Click(object sender, RoutedEventArgs e)
		{
			string aux = null;
			byte[] pw = Crypt.EncryptString(itempwBox.Password);
			aux += Convert.ToString(addButton.Tag) + "☺" + platformTextBox.Text + "☺";
			for (int i = 0; i < pw.Length; i++)
			{
				aux += pw[i] + "" + split[0];
			}
			File.AppendAllText(passwordspath, aux + Environment.NewLine);
			Login(Convert.ToString(addButton.Tag));
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			genGrid.Visibility = Visibility.Hidden;
			loginGrid.Visibility = Visibility.Visible;
			pwviewGrid.Visibility = Visibility.Hidden;
			pwaddGrid.Visibility = Visibility.Hidden;
			usernameTextBox.Text = "";
			pwBox.Password = "";
		}
	}
}
