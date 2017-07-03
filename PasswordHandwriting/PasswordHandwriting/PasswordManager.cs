using System;
using System.Collections.Generic;
using System.IO;

namespace PasswordHandwriting {
    class PasswordManager {
        private const string fileName = "password.txt";
        
        private string correctPassword;
        private double[] M;
        private double[] D;

        public PasswordManager() {
            if (File.Exists(fileName)) {
                StreamReader reader = new StreamReader(fileName);
                correctPassword = reader.ReadLine();
                int len = correctPassword.Length;
                len = len * 2 - 1;
                M = new double[len];
                D = new double[len];
                for (int i = 0; i < len; i++) {
                    string[] buffer = reader.ReadLine().Trim().Split(' ');
                    if (buffer.Length == 2) {
                        M[i] = double.Parse(buffer[0]);
                        D[i] = double.Parse(buffer[1]);
                    }
                }
                reader.Close();
            }
        }

        private bool InRange(double number, double a, double b) {
            return (number <= a + b) && (number >= a - b);
        }

        public bool CheckPassword(string password, List<double> pressTime, List<double> timeDifference) {
            if (correctPassword.Equals(password)) {
                float k = 3;
                int matches = 0;
                int count = 0;
                int len = password.Length;
                for (int i = 0; i < pressTime.Count; i++) {
                    if (InRange(pressTime[i], M[i], k * Math.Sqrt(D[i]))) {
                        matches++;
                    }
                    count++;

                }
                for (int i = 0; i < timeDifference.Count; i++) {
                    if (InRange(timeDifference[i], M[i + pressTime.Count], k * Math.Sqrt(D[i + pressTime.Count]))) {
                        matches++;
                    }
                    count++;
                }
                return (0.8 * (double)count <= (double)matches);
            }
            return false;
        }

        public bool NewPassword(string newPassword, List<List<double>> pressTime, List<List<double>> timeDifference) {
            
            int passwordLen = newPassword.Length;
            int c = passwordLen * 2 - 1;
            double[] nM = new double[c];
            double[] nD = new double[c];
                
            //first of all - calculate expectation for each press time and time difference
            double[] sumForM = new double[c];
            double[] sumForD = new double[c];
            for (int i = 0; i < c; i++) {
                sumForM[i] = 0.0;
                sumForD[i] = 0.0;
            }
            for (int i = 0; i < pressTime.Count; i++) {
                int l1 = pressTime[i].Count;
                for (int j = 0; j < l1; j++) {
                    sumForM[j] += pressTime[i][j];
                }
                int l2 = timeDifference[i].Count;
                for (int j = 0; j < l2; j++) {
                    sumForM[j + l1] += timeDifference[i][j];
                }
            }

            for (int i = 0; i < c; i++) {
                nM[i] = sumForM[i] / pressTime.Count;
            }
                
            //secondly - calculate dispersion for each press time and time difference
            for (int i = 0; i < pressTime.Count; i++) {
                int l1 = pressTime[i].Count;
                for (int j = 0; j < l1; j++) {
                    sumForD[j] += Math.Pow((pressTime[i][j] - nM[j]), 2.0);
                }

                int l2 = timeDifference[i].Count;
                for (int j = 0; j < l2; j++) {
                    sumForD[j + l1] += Math.Pow((timeDifference[i][j] - nM[j + l1]), 2.0);
                }
            }
            for (int i = 0; i < c; i++) {
                nD[i] = sumForD[i] / (pressTime.Count);
            }

            M = nM;
            D = nD;
            correctPassword = newPassword;
            Save();

            return true;
        }
        
        private void Save() {
            File.Create(fileName).Close();
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine(correctPassword);
            for (int i = 0; i < M.Length; i++) {
                writer.WriteLine($"{M[i]} {D[i]}");
            }
            writer.Close();
        }
    }
}