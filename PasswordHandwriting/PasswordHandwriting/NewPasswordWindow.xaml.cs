using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace PasswordHandwriting {
    /// <summary>
    /// Логика взаимодействия для NewPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : Window {

        private Stopwatch sw;

        private List<double> pressTime;
        private List<double> timeDifference;

        private List<List<double>> pt;
        private List<List<double>> td;

        private int counter;
        private const int max = 20;

        private string password;

        public NewPasswordWindow() {
            InitializeComponent();


            pt = new List<List<double>>();
            td = new List<List<double>>();

            counter = max;
            labelInfo.Content = $"Нужно ввести новый пароль {counter} раз";
            labelCount.Content = $"Осталось: {counter}";
            textBoxPassword.Focus();
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key.Equals(Key.Enter)) {
                AddInfo();
                return;
            }
            if (e.Key.Equals(Key.LeftShift)) {
                return;
            }
            else {
                if (textBoxPassword.Text.Length == 0) {
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

        private void textBoxPassword_KeyUp(object sender, KeyEventArgs e) {
            if (!e.Key.Equals(Key.Enter) && !e.Key.Equals(Key.LeftShift)) {
                sw.Stop();
                pressTime.Add(sw.Elapsed.TotalMilliseconds);
                sw.Restart();
            }
        }

        private void AddInfo() {
            if (counter == max) {
                password = textBoxPassword.Text;
                pt.Add(pressTime);
                td.Add(timeDifference);
                counter--;
            }
            else {
                if (password.Equals(textBoxPassword.Text)) {
                    pt.Add(pressTime);
                    td.Add(timeDifference);
                    counter--;
                }
            }
            if (counter == 0) {
                PasswordManager pm = new PasswordManager();
                pm.NewPassword(password, pt, td);
                this.Close();
            }
            labelCount.Content = $"Осалось: {counter}";
            textBoxPassword.Text = "";
            textBoxPassword.Focus();
        }
    }
}
