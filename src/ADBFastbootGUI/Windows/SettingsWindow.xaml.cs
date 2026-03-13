using ADBFastbootGUI.Themes;
using MaterialDesignThemes.Wpf;
using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// SettingsWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent(); 
            
            ThemeManagerHelper.ThemeChanged += OnThemeChanged;

            // 2. Hafıza sızıntılarını önlemek için pencere kapandığında dinlemeyi bırak.
            //    Bu çok önemlidir!
            this.Unloaded += (s, e) => ThemeManagerHelper.ThemeChanged -= OnThemeChanged;

            // 3. Pencere ilk açılırken mevcut temayı hemen uygula.
            //    Bunu yapmazsak, tema değişikliği olmadan pencere varsayılan renkte açılır.
            ChangeTheme(ThemeManagerHelper.IsDarkTheme);
        }
        private void OnThemeChanged(bool isDark)
        {
            // Gelen bilgiye göre renkleri değiştiren metodu çağır.
            ChangeTheme(isDark);
        }
        public void ChangeTheme(bool isDark)
        {
            System.Windows.Media.Brush foregroundBrush; // Tek bir değişkenle kodu kısaltalım.

            if (isDark)
            {
                // Koyu Tema
                foregroundBrush = System.Windows.Media.Brushes.White;
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
            }
            else
            {
                // Açık Tema
                foregroundBrush = System.Windows.Media.Brushes.Black;
                this.Background = System.Windows.Media.Brushes.WhiteSmoke; // Örnek arkaplan
            }

            Header.Foreground = foregroundBrush;
            CTTitle.Foreground = foregroundBrush;
            ThemeCheckBox.Foreground = foregroundBrush;
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheckBox.IsChecked = ThemeManagerHelper.IsDarkTheme;

            this.Opacity = 0;
            this.Top -= 20;

            var opacityAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top + 20, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var sb = new Storyboard();
            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Begin();
        }

        private void ThemeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();

            ThemeManagerHelper.IsDarkTheme = true;

            Header.Foreground = Brushes.White;
            ThemeCheckBox.Foreground = Brushes.White;
            CTTitle.Foreground = Brushes.White;
        }
        private void ThemeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();

            ThemeManagerHelper.IsDarkTheme = false;

            Header.Foreground = Brushes.Black;
            ThemeCheckBox.Foreground = Brushes.Black;
            CTTitle.Foreground = Brushes.Black;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var sb = new Storyboard();

            var opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top - 20, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Completed += (s, _) =>
            {
                this.Hide();
            };
            sb.Begin();
        }

        private void SetColorThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();

            ITheme theme = paletteHelper.GetTheme();

            Color accentColor = new Color();
            if (ColorThemeComboBox.SelectedIndex == 0)
                accentColor = Color.FromRgb(255, 235, 60);
            else if (ColorThemeComboBox.SelectedIndex == 1)
                accentColor = Color.FromRgb(0, 150, 136);
            else if (ColorThemeComboBox.SelectedIndex == 2)
                accentColor = Color.FromRgb(103, 58, 183);
            else if (ColorThemeComboBox.SelectedIndex == 3)
                accentColor = Color.FromRgb(205, 220, 57);
            else if (ColorThemeComboBox.SelectedIndex == 4)
                accentColor = Color.FromRgb(96, 125, 139);
            else if (ColorThemeComboBox.SelectedIndex == 5)
                accentColor = Color.FromRgb(255, 193, 7);
            else if (ColorThemeComboBox.SelectedIndex == 6)
                accentColor = Color.FromRgb(0, 188, 212);
            else if (ColorThemeComboBox.SelectedIndex == 7)
                accentColor = Color.FromRgb(63, 81, 181);
            else if (ColorThemeComboBox.SelectedIndex == 8)
                accentColor = Color.FromRgb(245, 67, 54);
            else if (ColorThemeComboBox.SelectedIndex == 9)
                accentColor = Color.FromRgb(158, 158, 158);
            else if (ColorThemeComboBox.SelectedIndex == 10)
                accentColor = Color.FromRgb(255, 87, 34);
            else if (ColorThemeComboBox.SelectedIndex == 11)
                accentColor = Color.FromRgb(234, 31, 100);
            else if (ColorThemeComboBox.SelectedIndex == 12)
                accentColor = Color.FromRgb(139, 195, 74);
            else if (ColorThemeComboBox.SelectedIndex == 13)
                accentColor = Color.FromRgb(255, 152, 0);
            else if (ColorThemeComboBox.SelectedIndex == 14)
                accentColor = Color.FromRgb(121, 85, 72);
            else if (ColorThemeComboBox.SelectedIndex == 15)
                accentColor = Color.FromRgb(3, 169, 244);
            else if (ColorThemeComboBox.SelectedIndex == 16)
                accentColor = Color.FromRgb(76, 175, 80);
            else if (ColorThemeComboBox.SelectedIndex == 17)
                accentColor = Color.FromRgb(33, 150, 243);
            else if (ColorThemeComboBox.SelectedIndex == 18)
                accentColor = Color.FromRgb(156, 39, 176);


            theme.SetPrimaryColor(accentColor);

            paletteHelper.SetTheme(theme);
        }
    }
}
