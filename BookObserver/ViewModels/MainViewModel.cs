using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    // https://stackoverflow.com/questions/36129372/how-to-cancel-window-closing-in-mvvm-wpf-application
    class MainViewModel(BooksViewModel booksVM, ReadersViewModel readersVM) : ViewModel
    {
        private readonly BooksViewModel _booksVM = booksVM;
        private readonly ReadersViewModel _readersVM = readersVM;

		#region CloseWindowCommand - Команда закрытие окна

		///<summary>Команда закрытие окна</summary>
		private ICommand? _closeWindowCommand;

		///<summary>Команда закрытие окна</summary>
		public ICommand CloseWindowCommand => _closeWindowCommand
			??= new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute);

		///<summary>Проверка возможности выполнения - Команда закрытие окна</summary>
		private bool CanCloseWindowCommandExecute(object? p) =>
			p is CancelEventArgs
			&& (!((Command)_booksVM.SaveBooksCommand).Executable
			|| !((Command)_readersVM.SaveReadersCommand).Executable)
			;

		///<summary>Логика выполнения - Команда закрытие окна</summary>
		private void OnCloseWindowCommandExecuted(object? p)
		{
			if(((Command)_booksVM.SaveBooksCommand).Executable)
				switch (MessageBox.Show("Список книг не сохранен. Сохранить?", "BookObserver", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
				{
					case MessageBoxResult.Cancel:
                        (p as CancelEventArgs)!.Cancel = true;
						return;
					case MessageBoxResult.Yes:
						_booksVM.SaveBooksCommand.Execute(_booksVM.Books);
						break;
					case MessageBoxResult.No:
						break;
					default:
						break;
				}
			if(((Command)_readersVM.SaveReadersCommand).Executable)
				switch (MessageBox.Show("Список читателей не сохранен. Сохранить?", "BookObserver", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
				{
					case MessageBoxResult.Cancel:
                        (p as CancelEventArgs)!.Cancel = true;
                        return;
                    case MessageBoxResult.Yes:
                        _booksVM.SaveBooksCommand.Execute(_booksVM.Books);
                        break;
					case MessageBoxResult.No:
						break;
					default:
						break;
				}
		}

		#endregion
	}
}
