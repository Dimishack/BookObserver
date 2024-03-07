using BookObserver.Infrastructure.Commands;
using BookObserver.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
		#region Title : string? - Заголовок окна

		///<summary>Заголовок окна</summary>
		private string? _title = "BookObserver";

		///<summary>Заголовок окна</summary>
		public string? Title { get => _title; set => Set(ref _title, value); }

		#endregion

		#region CloseWindowCommand - Закрытие окна

		///<summary>Закрытие окна</summary>
		private ICommand? _closeWindowCommand;

		///<summary>Закрытие окна</summary>
		public ICommand CloseWindowCommand => _closeWindowCommand
			??= new LambdaCommand(OnCloseWindowCommandExecuted);

		///<summary>Логика выполнения - Закрытие окна</summary>
		private void OnCloseWindowCommandExecuted(object? p)
		{
			MessageBox.Show("Закрытие окна прошло успешно!", _title);
			App.Current.Shutdown();
		}

		#endregion
	}
}
