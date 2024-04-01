using System.Windows;
using System.Windows.Input;

namespace BookObserver.Views.Windows
{
    public partial class CreatorBookWindow : Window
    {
        public CreatorBookWindow() => InitializeComponent();

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyDigit(e)
                && e.Key != Key.Back
                && e.Key != Key.Oem2
                && e.Key != Key.Decimal)
                e.Handled = true;
        }

        private bool IsKeyDigit(KeyEventArgs e) =>
            (e.Key >= Key.D0 && e.Key <= Key.D9) 
            || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(!IsKeyDigit(e)
                && e.Key != Key.OemMinus
                && e.Key != Key.Subtract)
                e.Handled = true;
        }

        private void ComboBox_KeyDownDigits(object sender, KeyEventArgs e)
        {
            if (!IsKeyDigit(e)) e.Handled = true;
        }
    }
}
