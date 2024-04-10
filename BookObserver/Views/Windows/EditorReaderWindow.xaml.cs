using System.Windows;
using System.Windows.Input;

namespace BookObserver.Views.Windows
{
    public partial class EditorReaderWindow : Window
    {
        public EditorReaderWindow() => InitializeComponent();

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9)
            && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
            && e.Key != Key.OemPlus
            && e.Key != Key.OemMinus
            && e.Key != Key.Subtract
            && e.Key != Key.Add
            )
                e.Handled = true;
        }
    }
}
