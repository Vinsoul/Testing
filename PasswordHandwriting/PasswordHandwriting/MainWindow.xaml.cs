using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace PasswordHandwriting {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private const string correctPassword = "P@ssw0rd";
        private Stopwatch sw;

        private List<double> pressTime;
        private List<double> timeDifference;

        private List<List<double>> pt;
        private List<List<double>> td;

        private PasswordManager pm;

        public MainWindow() {
            InitializeComponent();

            UserInput.Focus();

            pm = new PasswordManager();

            pt = new List<List<double>>();
            td = new List<List<double>>();
            
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key.Equals(Key.Enter)) {
                ButtonCheck.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, ButtonCheck));
                return;
            }
            if (e.Key.Equals(Key.LeftShift)) {
                return;
            }
            else {
                if (UserInput.Text.Length == 0) {
                    pressTime = new List<double>();
                    timeDifference = new List<double>();

                    sw = new Stopwatch();
                    sw.Start();
                }
                else {
                    sw.Stop();
                    timeDifference.Add(sw.Elapsed.TotalMilliseconds);
                    sw.Restart();
                }
            }
        }

        private void UserInput_KeyUp(object sender, KeyEventArgs e) {
            if (!e.Key.Equals(Key.Enter) && !e.Key.Equals(Key.LeftShift)) {
                sw.Stop();
                pressTime.Add(sw.Elapsed.TotalMilliseconds);
                sw.Restart();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show(pm.CheckPassword(UserInput.Text, pressTime, timeDifference) ? "Access Granted" : "Access Denied!");

            UserInput.Clear();
            UserInput.Focus();
        }

        private void ButtonNewPassword_Click(object sender, RoutedEventArgs e) {
            NewPasswordWindow npw = new NewPasswordWindow();
            npw.ShowDialog();
            pm = new PasswordManager();
            UserInput.Focus();
        }
    }
}
