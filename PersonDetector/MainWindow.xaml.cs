﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PersonDetector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // const int SPEECH_AMOUNT = 4;
        private int currentSpeech = 1;
        private SingleInput currentInput;
        private DispatcherTimer debugTimer= new DispatcherTimer();
       
        private int CurrentSpeech
        {
            get { return currentSpeech; }
            set
            {
                if(value>=Config.SPEECH_AMOUNT)
                {
                    //koniec testu
                    buttonNextSpeech.Content = "Zakończ test";
                    buttonNextSpeech.Click -= buttonNextSpeech_Click;
                    buttonNextSpeech.Click += finishTest;
                }
                currentSpeech = value;
                labelSpeechNumber.Content = value;
              // richTextBox.Document.Blocks.Clear();
            }
        }
        public MainWindow(bool final=false)
        {
            InitializeComponent();
            

            debugTimer.Tick  += new EventHandler(RefreshDebug);
            debugTimer.Interval = TimeSpan.FromMilliseconds( Config.DEBUG_REFRESH_INTERVAL);
            debugTimer.Start();

            NewSpeech();

            if(Config.IS_DEBUG_ENABLED)
            {
                expanderDebug.IsExpanded = true;
            }
            else
            {
                expanderDebug.IsExpanded = false;
            }
            CurrentSpeech = 1;
            if (final)
            {
                Config.currentUserData.userName = "FINAL";
                Config.SPEECH_AMOUNT = 1;
                CurrentSpeech = 1; //musi być drugi raz
                labelInfoText.Content = "Tekst końcowy";
                labelSpeechNumber.Visibility = Visibility.Hidden;
                buttonNextSpeech.Click -= finishTest;
                buttonNextSpeech.Click -= buttonNextSpeech_Click;
                buttonNextSpeech.Click += EndFinalTest;
            }
         

        }
        private void NewSpeech()
        {
            currentInput = new SingleInput();
            textBoxInput.Text = "";
            textBoxInput.Focus();
        }
        private  void RefreshDebug(object source, EventArgs e)
        {
            LnewLinesPerText.Content = currentInput.newLinesPerText;
            LspacesAfterPunctuation.Content = currentInput.spacesAfterPunctuation;
            LspacesBeforePunctuation.Content = currentInput.spacesBeforePunctuation;
            LpolishChars.Content = currentInput.polishChars;
            LavgLetterTime.Content = currentInput.avgLetterTime;
            LavgCapitalLetterTime.Content = currentInput.avgCapitalLetterTime;
            LIsWordTypedNow.Content = WritingAnalytics.isWordTypedNow;
            
        }
        private void buttonNextSpeech_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput != null)
                Config.currentUserData.inputs.Add(currentInput);
            CurrentSpeech++;
            NewSpeech();
            
        }
        private void finishTest(object sender, RoutedEventArgs e)
        {
            //TODO: okienko zamykania
            if (currentInput != null)
                Config.currentUserData.inputs.Add(currentInput);
            UserEndScreen s = new UserEndScreen();
            s.Show();
            this.Hide();
        }

        private void expanderDebug_Expanded(object sender, RoutedEventArgs e)
        {
            MainWindowForm.Height = 520;
        }

        private void expanderDebug_Collapsed(object sender, RoutedEventArgs e)
        {
            MainWindowForm.Height = 365;
        }

        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            WritingAnalytics.AnalizeReadyText( currentInput, textBoxInput.Text);
            WritingAnalytics.AnalizeFreshInput(currentInput, textBoxInput.Text);
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.LeftShift || e.Key == Key.RightShift)
            {
                WritingAnalytics.ShiftPressed(true);
            }
        }

        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                WritingAnalytics.ShiftPressed(false);
            }
        }

        private void textBoxInput_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxInput.Focus();
        }
        private void EndFinalTest(object o, EventArgs e)
        {
            if (currentInput != null)
            { 
                Config.currentUserData.inputs.Add(currentInput);
                Config.allUsersData.Add(Config.currentUserData);
            }

            this.Close();
        }
    
}
}
