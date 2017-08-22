using System;

namespace TapTrack.Demo
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Collections.ObjectModel;
    using System.Threading;
    using WpfAnimatedGif;
    using System.Diagnostics;
    using Tcmp.Communication;
    using Tcmp.CommandFamilies;
    using Tcmp.CommandFamilies.BasicNfc;
    using Tcmp.CommandFamilies.Type4;
    using Ndef;
    using Tcmp.Communication.Exceptions;
    using Tcmp;
    using NdefLibrary.Ndef;
    using System.Text;
    using Tcmp.CommandFamilies.System;
    using System.Management;
    using System.Text.RegularExpressions;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        TappyReader tappy;
        private ObservableCollection<Row> table;
        GridLength zeroHeight = new GridLength(0);
		bool keyboardModeLineBreak = false;
		bool keyboardModeTab = false;
		bool keyboardModeTabLineBreakLast = false;

        public MainWindow()
        {
            InitializeComponent();
            tappy = new TappyReader(CommunicationProtocol.Usb);
            table = new ObservableCollection<Row>();
            records.ItemsSource = table;
            this.Closed += MainWindow_Closed;
	

		}

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            tappy.Disconnect();
        }

        //
        // Read Ndef Message tab
        //

        private void ReadNdefButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("Waiting for tap");

            ndefData.Text = "";
            DetectSingleNdef detect = new DetectSingleNdef((byte)timeout.Value, DetectTagSetting.Type2Type4AandMifare);
            tappy.SendCommand(detect, AddNdefContent);
        }

        private void AddNdefContent(ResponseFrame frame, Exception e)
        {
            if (CheckForErrorsOrTimeout(frame, e))
                return;

            byte[] data = frame.Data;

            byte[] temp = new byte[data.Length - data[1] - 2];

            if (temp.Length > 0)
            {
                Array.Copy(data, 2 + data[1], temp, 0, temp.Length);

                NdefMessage message = NdefMessage.FromByteArray(temp);

                Action update = () =>
                {
                    foreach (NdefRecord record in message)
                    {
                        ndefData.AppendText("Ndef Record:\n\n");

						if (record.TypeNameFormat == NdefRecord.TypeNameFormatType.Empty)
						{
							ndefData.AppendText("Empty NDEF Record");
						}
						else
						{

							string type = Encoding.UTF8.GetString(record.Type);
							ndefData.AppendText($"TNF: {record.TypeNameFormat.ToString()} ({(byte)record.TypeNameFormat})\n");
							ndefData.AppendText($"Type: {type}\n");

							if (record.Id != null)
								ndefData.AppendText($"Type: {BitConverter.ToString(record.Id)}\n");

							if (type.Equals("U"))
							{
								NdefUriRecord uriRecord = new NdefUriRecord(record);
								ndefData.AppendText($"Payload: {uriRecord.Uri}\n");
							}
							else if (type.Equals("T"))
							{
								NdefTextRecord textRecord = new NdefTextRecord(record);
								ndefData.AppendText($"Encoding: {textRecord.TextEncoding.ToString()}\n");
								ndefData.AppendText($"Language: {textRecord.LanguageCode}\n");
								ndefData.AppendText($"Payload: {textRecord.Text}\n");
							}
							else if (type.Contains("text"))
							{
								ndefData.AppendText($"Payload: {Encoding.UTF8.GetString(record.Payload)}\n");
							}
							else
							{
								ndefData.AppendText($"Payload: {BitConverter.ToString(record.Payload)}");
							}

							ndefData.AppendText($"----------\n");
						}
                    }
                };

                Dispatcher.BeginInvoke(update);
            }

            ShowSuccessStatus();
        }

        //
        // Read UID Tab
        //

        private void ReadUIDButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("Waiting for tap");
            Command cmd = new DetectSingleTagUid((byte)timeout.Value, DetectTagSetting.Type2Type4AandMifare);
            tappy.SendCommand(cmd, AddUID);
        }

        private void AddUID(ResponseFrame frame, Exception e)
        {
            if (CheckForErrorsOrTimeout(frame, e))
                return;

            Tag tag = new Tag(frame.Data);

            Action update = () =>
            {
                uidTextBox.Text = "";
                uidTextBox.Text += BitConverter.ToString(tag.UID);
                typeTextBox.Text = Tcmp.Tag.TypeLookUp(tag.TypeOfTag);
            };
            ShowSuccessStatus();
            Dispatcher.Invoke(update);
        }

        //
        // Write URI Tab
        //

        private void WriteURLButton_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Copy(urlTextBox.Text);
            Command cmd = new WriteUri((byte)timeout.Value, (bool)lockCheckBox.IsChecked, new NdefUri(url));
            Callback repeatCommand = null;
            bool repeat = (bool)repeatUrlWrite.IsChecked;
            Action sendCommand = () => tappy.SendCommand(cmd, ResponseCallback + repeatCommand);

            ShowPendingStatus("Waiting for tap");

            repeatCommand = (ResponseFrame frame, Exception exc) =>
            {
                if (repeat)
                {
                    if (CheckForErrorsOrTimeout(frame, exc))
                        return;
                    Thread.Sleep(800);
                    ShowPendingStatus("Waiting for tap");
                    Dispatcher.BeginInvoke(sendCommand);
                }
            };

            tappy.SendCommand(cmd, ResponseCallback + repeatCommand);
        }

        private void WriteUrlWithTagMirror_Click(object sender, RoutedEventArgs e)
        {
            string temp = string.Copy(urlTextBox.Text);
            bool willLock = (bool)lockCheckBox.IsChecked;
            bool repeat = (bool)repeatUrlWrite.IsChecked;
            byte timeoutValue = (byte)timeout.Value;
            Command detectTag = new DetectSingleTagUid(timeoutValue, DetectTagSetting.Type2Type4AandMifare);
            ShowPendingStatus("Waiting for tap");
            Callback detectTagCallback = null;

            Callback writeCallback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;
                ShowSuccessStatus();

                if (repeat)
                {
                    Thread.Sleep(1000);
                    ShowPendingStatus("Waiting for tap");
                    Task.Run(() => tappy.SendCommand(detectTag, detectTagCallback));
                }
            };

            detectTagCallback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                Tag tag = new Tag(frame.Data);
                Command write = new WriteUri(timeoutValue, willLock, temp.Replace("[uid]", tag.UidToString()));
                ShowPendingStatus("Tag detected, please hold steady while tag is written");
                Task.Run(() => tappy.SendCommand(write, writeCallback));
            };

            tappy.SendCommand(detectTag, detectTagCallback);
        }

        //
        // Write Text Tab
        //

        private void WriteTextButton_Click(object sender, RoutedEventArgs e)
        {
            Command cmd = new WriteText((byte)timeout.Value, (bool)lockCheckBox.IsChecked, TextBox.Text ?? "");
            ShowPendingStatus("Waiting for tap");

            Callback repeatCommand = null;
            bool repeat = (bool)repeatTextWrite.IsChecked;
            Action sendCommand = () => tappy.SendCommand(cmd, ResponseCallback + repeatCommand);

            repeatCommand = (ResponseFrame frame, Exception exc) =>
            {
                if (repeat)
                {
                    if (CheckForErrorsOrTimeout(frame, exc))
                        return;
                    Thread.Sleep(1000);
                    ShowPendingStatus("Waiting for tap");
                    Dispatcher.BeginInvoke(sendCommand);
                }
            };

            tappy.SendCommand(cmd, ResponseCallback + repeatCommand);
        }

        //
        // Write Multi Ndef Tab
        //

        private void WriteMultNdef(object send, RoutedEventArgs e)
        {
            NdefMessage message = new NdefMessage();

            foreach (Row row in table)
            {
                if (row.Selected.Equals("Text"))
                {
                    NdefTextRecord temp = new NdefTextRecord()
                    {
                        TextEncoding = NdefTextRecord.TextEncodingType.Utf8,
                        LanguageCode = "en",
                        Text = row.Content ?? ""
                    };
                    message.Add(temp);
                }
                else
                {
                    message.Add(new NdefUriRecord() { Uri = row.Content ?? "" });
                }
            }

            ShowPendingStatus("Waiting for tap");

            Command cmd = new WriteCustomNdef((byte)timeout.Value, (bool)lockCheckBox.IsChecked, message);
            Callback repeatCommand = null;
            bool repeat = (bool)repeatMultiNdefWrite.IsChecked;
            Action sendCommand = () => tappy.SendCommand(cmd, ResponseCallback + repeatCommand);

            repeatCommand = (ResponseFrame frame, Exception exc) =>
            {
                if (repeat)
                {
                    if (CheckForErrorsOrTimeout(frame, exc))
                        return;
                    Thread.Sleep(1000);
                    ShowPendingStatus("Waiting for tap");
                    Dispatcher.BeginInvoke(sendCommand);
                }
            };

            tappy.SendCommand(cmd, ResponseCallback + repeatCommand);
        }

        private void AddTextRowButton_Click(object sender, RoutedEventArgs e)
        {
            Row row = new Row(table.Count);
            row.Selected = "Text";
            table.Add(row);
        }

        private void AddUriRowButton_Click(object sender, RoutedEventArgs e)
        {
            Row row = new Row(table.Count);
            row.Selected = "URI";
            table.Add(row);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            table.Clear();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = sender as Button;
            int row = (int)removeButton.Tag;
            for (int i = row + 1; i < table.Count; i++)
                table[i].Index = i - 1;
            table.RemoveAt(row);
        }

        //
        // vCard Tab
        //

        private void WriteVCardButton_Click(object sender, RoutedEventArgs e)
        {
            //VCard info = new VCard(nameTextBox.Text, cellPhoneTextBox.Text, workPhoneTextBox.Text,
            //    homePhoneTextBox.Text, emailTextBox.Text, businessEmailTextBox.Text, homeAddrTextBox.Text,
            //    businessAddrTextBox.Text, companyTextBox.Text, titleTextBox.Text, websiteTextBox.Text);

            //ShowPendingStatus("Waiting for tap");

            //CustomRecordPayload record = new CustomRecordPayload(info.ToByteArray(), "text/x-vcard", TypeNameField.NfcForumWellKnown);
            //NdefMessage message = new NdefMessage(record);

            //Command cmd = new WriteCustomNdef((byte)timeout.Value, (bool)lockCheckBox.IsChecked, message);

            //tappyDriver.SendCommand(cmd, ResponseCallback);
        }

        private void ClearVCardButton_Click(object sender, RoutedEventArgs e)
        {
            nameTextBox.Text = "";
            emailTextBox.Text = "";
            cellPhoneTextBox.Text = "";
            homePhoneTextBox.Text = "";
            homeAddrTextBox.Text = "";
            websiteTextBox.Text = "";
            companyTextBox.Text = "";
            titleTextBox.Text = "";
            businessEmailTextBox.Text = "";
            workPhoneTextBox.Text = "";
            businessAddrTextBox.Text = "";
        }

        //
        // Detect Type 4B
        //

        private void ReadType4B(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("Waiting for tap");
            Command cmd = new DetectType4B((byte)timeout.Value);

            Callback responseCallback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                if (frame.ResponseCode != 0x07)
                    return;

                byte[] data = frame.Data;

                Action update = () =>
                {
                    byte atqbLen = data[0];
                    byte attribLen = data[1];
                    atqbTextBox.Text = "";
                    attribTextBox.Text = "";


                    for (int i = 2; i < 2 + data[0]; i++)
                        atqbTextBox.Text += string.Format("{0:X}", data[i]).PadLeft(2, '0') + " ";

                    for (int i = data[0] + 2; i < data.Length; i++)
                        attribTextBox.Text += string.Format("{0:X}", data[i]).PadLeft(2, '0') + " ";
                };

                ShowSuccessStatus();
                Dispatcher.BeginInvoke(update);
            };

            tappy.SendCommand(cmd, responseCallback);
        }

        private void UpdateDetTypeBForm(byte[] data)
        {
            Action update = () =>
            {
                byte atqbLen = data[0];
                byte attribLen = data[1];
                atqbTextBox.Text = "";
                attribTextBox.Text = "";


                for (int i = 2; i < 2 + data[0]; i++)
                    atqbTextBox.Text += string.Format("{0:X}", data[i]).PadLeft(2, '0') + " ";

                for (int i = data[0] + 2; i < data.Length; i++)
                    attribTextBox.Text += string.Format("{0:X}", data[i]).PadLeft(2, '0') + " ";
            };

            ShowSuccessStatus();
            Dispatcher.BeginInvoke(update);
        }

        //
        // Lock Tag Tab
        //

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("Waiting for tap");
            Command readCommand = new DetectSingleTagUid((byte)timeout.Value, DetectTagSetting.Type2Type4AandMifare);

            tappy.SendCommand(readCommand, delegate (ResponseFrame frame, Exception exc)
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                Tag tag = new Tag(frame.Data);

                Action Lock = () =>
                {
                    Command lockCommand = new LockTag((byte)timeout.Value, tag.UID);

                    tappy.SendCommand(lockCommand, ResponseCallback);
                };

                Dispatcher.BeginInvoke(Lock);
            });
        }

        //
        // Other
        //

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            HideStatus();
            tappy.SendCommand<Stop>();
        }

        private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectWindow window = new ConnectWindow();

            if (window.ShowDialog() == true)
            {
                if (window.Protocol == CommunicationProtocol.Usb)
                    batteryTab.Visibility = Visibility.Hidden;
                else if (window.Protocol == CommunicationProtocol.Bluetooth)
                {
                    batteryTab.Visibility = Visibility.Visible;
					if (GetBluegigaDevice() == null)
					{
						ShowFailStatus("Please insert BLED112 dongle");
						return;
					}

                }
				
                tappy.SwitchProtocol(window.Protocol);

					ShowPendingStatus("Searching for a Tappy");

                Task.Run(() =>
                {
					if (tappy.AutoDetect())
					{
						ShowSuccessStatus($"Connected to {tappy.DeviceName}");
						if (window.Protocol == CommunicationProtocol.Bluetooth)
						{
							try
							{
								Command cmd = new EnableDataThrottling(10, 5);
								tappy.SendCommand(cmd);
							}
							catch
							{

							}
						}
					}
					else
					{
						ShowFailStatus("No Tappy found");
						if (window.Protocol == CommunicationProtocol.Bluetooth)
						{
							try
							{
								tappy.DisconnectBlueGiga();
							}
							catch 
							{								
								return;

							}

						}
				
					}
                });
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingsContainer.Height.IsAuto)
                settingsContainer.Height = zeroHeight;
            else
                settingsContainer.Height = GridLength.Auto;
        }

        private void ShowPendingStatus(string message)
        {
            Action show = () =>
            {
                statusPopup.IsOpen = true;
                statusText.Content = "Pending";
                statusMessage.Content = message;
                ImageBehavior.SetAnimatedSource(statusImage, (BitmapImage)FindResource("Pending"));
            };

            Dispatcher.BeginInvoke(show);
        }

        private void ShowSuccessStatus(string message = "")
        {
            Action show = () =>
            {
                statusPopup.IsOpen = true;
                statusText.Content = "Success";
                statusMessage.Content = message;
                ImageBehavior.SetAnimatedSource(statusImage, (BitmapImage)FindResource("Success"));

                Task.Run(() =>
                {
                    Thread.Sleep(750);
                    HideStatus();
                });
            };

            Dispatcher.BeginInvoke(show);
        }

        private void ShowFailStatus(string message)
        {
            Action show = () =>
            {
                dismissButtonContainer.Height = new GridLength(50);
                dismissButton.Visibility = Visibility.Visible;
                statusPopup.IsOpen = true;
                statusText.Content = "Fail";
                statusMessage.Content = message;
                ImageBehavior.SetAnimatedSource(statusImage, (BitmapImage)FindResource("Error"));
            };

            Dispatcher.BeginInvoke(show);
        }

        private void HideStatus()
        {
            Action hide = () =>
            {
                statusPopup.IsOpen = false;
            };

            Dispatcher.Invoke(hide);
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            HideStatus();
            dismissButton.Visibility = Visibility.Hidden;
            dismissButtonContainer.Height = zeroHeight;
			tgbtnLaunchKeyboardFeature.IsChecked = false;
        }

        private void ResponseCallback(ResponseFrame frame, Exception e)
        {
            if (CheckForErrorsOrTimeout(frame, e))
                return;
            ShowSuccessStatus();
        }

        private bool CheckForErrorsOrTimeout(ResponseFrame frame, Exception e)
        {
            if (e != null)
            {
                if (e.GetType() == typeof(HardwareException))
                    ShowFailStatus("Tappy is not connected");
                else
                    ShowFailStatus("An error occured");

                return true;
            }
            else if (!TcmpFrame.IsValidFrame(frame))
            {
                ShowFailStatus("An error occured");

                return true;
            }
            else if (frame.IsApplicationErrorFrame())
            {
                ApplicationErrorFrame errorFrame = (ApplicationErrorFrame)frame;
                ShowFailStatus(errorFrame.ErrorString);
                return true;
            }
            else if (frame.CommandFamily0 == 0 && frame.CommandFamily1 == 0 && frame.ResponseCode < 0x05)
            {
                ShowFailStatus(TappyError.LookUp(frame.CommandFamily, frame.ResponseCode));
                return true;
            }
            else if (frame.ResponseCode == 0x03)
            {
                ShowFailStatus("No tag detected");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void launchUrlButton_Click(object sender, RoutedEventArgs e)
        {
            DetectandLaunch();
        }

        private void DetectandLaunch()
        {
            Command cmd = new DetectSingleNdef(0, DetectTagSetting.Type2Type4AandMifare);
            tappy.SendCommand(cmd, LaunchCallback);
        }

        private void LaunchCallback(ResponseFrame frame, Exception e)
        {
            if (CheckForErrorsOrTimeout(frame, e))
                return;

            byte[] data = frame.Data;
            byte[] temp = new byte[data.Length - data[1] - 2];

            Array.Copy(data, 2 + data[1], temp, 0, temp.Length);

            NdefMessage message = NdefMessage.FromByteArray(temp);

            if (message.Count > 0)
            {
                if (Encoding.UTF8.GetString(message[0].Type).Equals("U"))
                {
                    NdefUriRecord uriRecord = new NdefUriRecord(message[0]);
                    NdefUri uri = new NdefUri(uriRecord.Uri);
                    if (uri.Scheme == 0)
                        return;
                    Process.Start(uriRecord.Uri);
                }
            }

            Task.Run(() =>
            {
                Thread.Sleep(500);
                DetectandLaunch();
            });
        }

        private void configureTagForPlatform_Click(object sender, RoutedEventArgs e)
        {
            Command readCommand = new DetectSingleTagUid(0, DetectTagSetting.Type2Type4AandMifare);

            tappy.SendCommand(readCommand, delegate (ResponseFrame frame, Exception exc)
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                Tag tag = new Tag(frame.Data);
                string uid = BitConverter.ToString(tag.UID).Replace("-", "");
                string url = $"https://members.taptrack.com/m?id={uid}";
                Command write = new WriteUri(0, false, new NdefUri(url));

                Task.Run(() =>
                {
                    tappy.SendCommand(write, ConfigSuccess);
                });
            });
        }

        private void ConfigSuccess(ResponseFrame frame, Exception e)
        {
            if (e != null)
            {
                return;
            }
            else if (!TcmpFrame.IsValidFrame(frame))
            {
                ShowFailStatus("Error occured");
                return;
            }
            else if (frame.IsApplicationErrorFrame())
            {
                ShowFailStatus(((ApplicationErrorFrame)frame).ErrorString);
                return;
            }

            Tag tag = new Tag(frame.Data);

            string uid = BitConverter.ToString(tag.UID).Replace("-", "");
            Process.Start(string.Format($"https://members.taptrack.com/x.php?tag_code={uid}"));
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tappy.Disconnect();
				tappy.DisconnectBlueGiga();
                ShowSuccessStatus("Disconnect was successful");
            }
            catch
            {
                ShowFailStatus("Disconnect was unsuccessful");
            }
        }

        private void firmwareVersionButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("");
            Command cmd = new GetFirmwareVersion();

            Action<string> update = (string text) =>
            {
                firmwareTextBox.Text = text;
            };

            Callback callback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                if (frame.ResponseCode == 0x06)
                {
                    byte[] data = frame.Data;

                    Dispatcher.BeginInvoke(update, $"{data[0]}.{data[1]}");
                }
                ShowSuccessStatus();
            };

            tappy.SendCommand(cmd, callback);
        }

        private void hardwareVersionButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("");
            Command cmd = new GetHardwareVersion();

            Action<string> update = (string text) =>
            {
                hardwareTextBox.Text = text;
            };

            Callback callback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;

                if (frame.ResponseCode == 0x05)
                {
                    byte[] data = frame.Data;

                    Dispatcher.BeginInvoke(update, $"{data[0]}.{data[1]}");
                }
                ShowSuccessStatus();
            };

            tappy.SendCommand(cmd, callback);
        }

        private void batteryButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPendingStatus("");
            Command cmd = new GetBatteryLevel();

            Action<string> update = (string text) =>
            {
                batteryTextBox.Text = text;
            };

            Callback callback = (ResponseFrame frame, Exception exc) =>
            {
                if (CheckForErrorsOrTimeout(frame, exc))
                    return;


                if (frame.ResponseCode == 0x08)
                {
                    byte[] data = frame.Data;

                    Dispatcher.BeginInvoke(update, $"{data[0]}%");
                }
                ShowSuccessStatus();
            };

            tappy.SendCommand(cmd, callback);
        }

        private void Type2Callback(ResponseFrame frame, Exception e)
        {
            if (CheckForErrorsOrTimeout(frame, e))
                return;

            if (frame.ResponseCode == 0x07)
            {
                ShowSuccessStatus();
            }
        }

        private void enableType2Button_Click(object sender, RoutedEventArgs e)
        {
            Command cmd = new SetType2TagIdentification(true);

            tappy.SendCommand(cmd, Type2Callback);
        }

        private void disableType2Button_Click(object sender, RoutedEventArgs e)
        {
            Command cmd = new SetType2TagIdentification(false);

            tappy.SendCommand(cmd, Type2Callback);
        }

        private string Search(string searchLocation)
        {
            ManagementObjectCollection comPortDevices;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"Select * From {searchLocation}");
            comPortDevices = searcher.Get();

            foreach (ManagementObject device in comPortDevices)
            {
                string name = device["Name"] as string;

                if (name?.Contains("Bluegiga Bluetooth Low Energy") ?? false)
                {
                    Debug.WriteLine($"Found {device["Name"]}");
                    Match match = Regex.Match(name, @"\(([^)]*)\)");
                    if (match.Groups.Count > 1)
                        return match.Groups[1].Value;
                }
            }

            return null;
        }

        private string GetBluegigaDevice()
        {
            return Search("Win32_SerialPort") ?? Search("Win32_pnpEntity");
        }

		//
		//Keyboard entry mode feature
		//

		#region Keyboard Feature

		private void chbxAddlineBreak_Checked(object sender, RoutedEventArgs e)
		{
			keyboardModeLineBreak = true;
		}

		private void chbxAddlineBreak_Unchecked(object sender, RoutedEventArgs e)
		{
			keyboardModeLineBreak = false;
		}

		private void tgbtnLaunchKeyboardFeature_Checked(object sender, RoutedEventArgs e)
		{

			StreamNdef stream = new StreamNdef(0, DetectTagSetting.Type2Type4AandMifare);
			tappy.SendCommand(stream, InvokeKeyboardFeature);

		}

		private void InvokeKeyboardFeature(ResponseFrame frame, Exception e)
		{
			if (CheckForErrorsOrTimeout(frame, e))
			{
				return;
			}
			else
			{

				byte[] data = frame.Data;

				byte[] temp = new byte[data.Length - data[1] - 2];

				if (temp.Length > 0)
				{
					Array.Copy(data, 2 + data[1], temp, 0, temp.Length);

					NdefMessage message = NdefMessage.FromByteArray(temp);

					int numRecords = message.Count;
					int recordNum = 1;

					Action EnterKeystrokes = () =>
					{
						foreach (NdefRecord record in message)
						{
							string type = Encoding.UTF8.GetString(record.Type);
							if (type.Equals("T"))
							{
								NdefTextRecord textRecord = new NdefTextRecord(record);
								System.Windows.Forms.SendKeys.SendWait(textRecord.Text);
								if (keyboardModeLineBreak)
									System.Windows.Forms.SendKeys.SendWait("{ENTER}");
								if (keyboardModeTab)
									System.Windows.Forms.SendKeys.SendWait("{TAB}");
								if (keyboardModeTabLineBreakLast)
								{
									if (recordNum == numRecords)
										System.Windows.Forms.SendKeys.SendWait("{ENTER}");
									else
										System.Windows.Forms.SendKeys.SendWait("{TAB}");

								}
									
								recordNum++;
							}

						}
					};

					Dispatcher.BeginInvoke(EnterKeystrokes);
				}
			}

		}

		private void tgbtnLaunchKeyboardFeature_Unchecked(object sender, RoutedEventArgs e)
		{			
			tappy.SendCommand<Stop>();
		}

		void TextBox_KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key.ToString() == "Tab")
			{
				ShowSuccessStatus("Tab key entered into text record");
			}

		}


		#endregion

		private void chbxAddTab_Unchecked(object sender, RoutedEventArgs e)
		{
			keyboardModeTab = false;
		}

		private void chbxAddTab_Checked(object sender, RoutedEventArgs e)
		{
			keyboardModeTab = true;
		}

		private void chbxAddTabLineBreakLast_Unchecked(object sender, RoutedEventArgs e)
		{
			keyboardModeTabLineBreakLast = false;
		}

		private void chbxAddTabLineBreakLast_Checked(object sender, RoutedEventArgs e)
		{
			keyboardModeTabLineBreakLast = true;
		}
	}
}
