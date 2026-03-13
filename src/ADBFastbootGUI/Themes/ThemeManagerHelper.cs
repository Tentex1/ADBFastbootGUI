using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using ADBFastbootGUI.Windows;

namespace ADBFastbootGUI.Themes
{
    public static class ThemeManagerHelper
    {
        public static event Action<bool> ThemeChanged;

        // 2. Mevcut tema durumunu tutan özel (private) bir değişken.
        private static bool isDarkTheme;

        // 3. Tema durumunu okumak ve değiştirmek için kullanılacak genel (public) özellik (property).
        public static bool IsDarkTheme
        {
            get { return isDarkTheme; }
            set
            {
                // Eğer yeni değer eskisinden farklıysa...
                if (isDarkTheme != value)
                {
                    // ...yeni değeri ata.
                    isDarkTheme = value;

                    // ...ve "ThemeChanged" olayını tetikle.
                    // Bu, "Eğer bu olaya abone olan varsa, onlara yeni 'isDarkTheme' değerini gönder" demektir.
                    // Soru işareti (?), kimse abone olmadıysa hata vermesini engeller.
                    ThemeChanged?.Invoke(isDarkTheme);
                }
            }
        }
    }
}
