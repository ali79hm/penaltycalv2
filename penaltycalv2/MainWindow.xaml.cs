using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Globalization;
using System.Collections.Generic;

namespace penaltycalv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double fullPenalty = 30000;
        public struct oneProject
        {
            public double fullproject { get; set; }

            public double solvedproject { get; set; }

        }
        List<oneProject> items;
        public MainWindow()
        {
            InitializeComponent();
            items = new List<oneProject>();
        }
        public void showerr(int errcode)
        {
            if (errcode == 1)
            {
                MessageBox.Show("لطفا بخش ویدئو را خالی نگذارید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (errcode == 2)
            {
                MessageBox.Show("لطفا در کادر ''میزان ویدئو دیده شده'' درصد را به طور درست وارد کنید ", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (errcode == 3)
            {
                MessageBox.Show("لطفا بخش پروژه را به درستی تکمیل کنید ", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (errcode == 4)
            {
                MessageBox.Show("خطایی ناشناخته رخ داده ", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void calcBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            outputText.Text = "";
            //check
            int err = checkBoxes();
            if (err > 0)
            {
                showerr(err);
            }
            else
            {
                //darsad if more than 85 noting
                try
                {

                    double sumX = double.Parse(videoNumber.Text.ToString(), CultureInfo.InvariantCulture.NumberFormat)/*the number of videos*/ ;
                    for (int i = 0; i < items.Count; i++)
                    {
                        sumX += items[i].fullproject * 5 / 100;
                    }

                    double compX = 0;
                    if (RdNumber.IsChecked == true){ compX = double.Parse(compVideoNumber.Text.ToString(), CultureInfo.InvariantCulture.NumberFormat); }
                    else { compX = double.Parse(compVideoNumber.Text.ToString(), CultureInfo.InvariantCulture.NumberFormat) / 100 * double.Parse(videoNumber.Text.ToString(), CultureInfo.InvariantCulture.NumberFormat); }
                    for (int i = 0; i < items.Count; i++)
                    {
                        compX += items[i].solvedproject * 5 / 100;
                    }

                    double compPercent = compX / sumX;

                    if (compPercent < 0.85)
                    {
                        compPercent = Math.Round(compPercent * 100, 4);
                        outputText.Text += String.Format("متاسفانه شما فقط {0} درصد از ددلاین خود را انجام داده اید و مشمول جریمه می شوید", compPercent);

                        double notcomppercent = 100 - compPercent;

                        double penalty = Math.Round(fullPenalty * notcomppercent/100, 2);
                        int roundpenalty = (int)Math.Round(penalty / 100, 0) * 100;
                        outputText.Text += String.Format("\n جریمه شما {0} تومان میشود (قابل پرداخت : {1} تومان) ", penalty, roundpenalty);
                    }
                    else
                    {
                        compPercent = Math.Round(compPercent * 100, 4);
                        outputText.Text += String.Format("تبریک شما {0} درصد از ددلاین خود را انجام داده اید و مشمول جریمه نمی شوید", compPercent);
                    }
                }
                catch (Exception)
                {
                    showerr(4);
                }
            }
        }
        private int checkBoxes()  //return 1 => video part is empty  | retun 2 => compVideoNumber in percent mode must be between 0-100 | return 0 => no err
        {
            try
            {
                if (videoNumber.Text.Length < 1 || compVideoNumber.Text.Length < 1)
                {
                    return 1;
                }
                else if (RDPersent.IsChecked == true)
                {
                    float temp = float.Parse(compVideoNumber.Text.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    if (temp < 0 || temp > 100)
                    {
                        return 2;
                    }
                }
                
            }
            catch (Exception errroe)
            {
                MessageBox.Show(errroe.ToString(), "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return 4;
            }
            return 0;
        }

        private void RdNumber_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (percentLable != null)
            {
                percentLable.Visibility = System.Windows.Visibility.Hidden;
                compVideoNumber.Padding = new Thickness(1, 1.5, 0, 1);
            }
        }

        private void RDPersent_Checked(object sender, RoutedEventArgs e)
        {
            if (percentLable != null)
            {
                percentLable.Visibility = System.Windows.Visibility.Visible;
                compVideoNumber.Padding = new Thickness(12, 1.5, 0, 1);
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (projectNumber.Text != null && compProjectNumber.Text != null && projectNumber.Text != "" && compProjectNumber.Text != "")
            {
                double fullproject = double.Parse(projectNumber.Text, CultureInfo.InvariantCulture.NumberFormat);
                double solvedproject = double.Parse(compProjectNumber.Text, CultureInfo.InvariantCulture.NumberFormat);

                if (fullproject>100 || fullproject<0)
                {
                    MessageBox.Show("درصد قابل انجام را به طور درست وارد کنید");
                }
                else if (solvedproject > 100 || solvedproject < 0 || solvedproject> fullproject)
                {
                    MessageBox.Show("درصد انجام شده را به طور درست وارد کنید");
                }
                else
                {
                    items.Add(new oneProject() { fullproject = fullproject, solvedproject = solvedproject });

                    list.ItemsSource = items;
                    list.Items.Refresh();
                    projectNumber.Text = "";
                    compProjectNumber.Text = "";
                }
                
            }
            if (items.Count>0)
            {
                noPjText.Visibility = Visibility.Hidden;
            }
        }


        private void videoNumber_GotFocus_1(object sender, RoutedEventArgs e)
        {
            videoNumber.Text = "";
        }

        private void projectNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            projectNumber.Text = "";
        }

        private void compVideoNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            compVideoNumber.Text = "";
        }

        private void compProjectNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            compProjectNumber.Text = "";
        }

        private void delbtn_Click(object sender, RoutedEventArgs e)
        {
            double key = 0;
            foreach (oneProject eachItem in list.SelectedItems)
            {
                key = eachItem.fullproject;
                items.Remove(eachItem);
                list.ItemsSource = items;
                list.Items.Refresh();
                break;
            };
            if (items.Count<1)
            {
                noPjText.Visibility = Visibility.Visible;
            }
        }

        private void eraseallbtn_Click(object sender, RoutedEventArgs e)
        {
            videoNumber.Text = "";
            projectNumber.Text = "";
            compVideoNumber.Text = "";
            compProjectNumber.Text = "";

            items = new List<oneProject>();
            list.ItemsSource = items;
            list.Items.Refresh();
        }

        private void aboteus_Click(object sender, RoutedEventArgs e)
        {
            string message = "این نرم افزار صرفا برای گروه ماشین لرنینگ ساخته شده است \n توجه : نسبت محاسبه ارزشی ویدئو به پروژه 1 به 5 است\n کاری از سید علی حسینی";
            MessageBox.Show(message,"نرم افزار محاسبه جریمه",  MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
