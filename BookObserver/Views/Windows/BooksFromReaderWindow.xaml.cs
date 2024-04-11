using BookObserver.Models.Books;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookObserver.Views.Windows
{
    public partial class BooksFromReaderWindow : Window
    {
        public BooksFromReaderWindow() => InitializeComponent();

        #region Books : ObservableCollection<Book> - Книги

        public static readonly DependencyProperty BooksProperty =
            DependencyProperty.Register(nameof(Books), typeof(ObservableCollection<Book>), typeof(BooksFromReaderWindow), new PropertyMetadata(default));

        public ObservableCollection<Book> Books
        {
            get => (ObservableCollection<Book>)GetValue(BooksProperty);
            set => SetValue(BooksProperty, value);
        }

        #endregion
    }
}
