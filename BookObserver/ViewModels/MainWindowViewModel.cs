using BookObserver.ViewModels.Base;

namespace BookObserver.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
		#region Title : string? - Заголовок окна

		///<summary>Заголовок окна</summary>
		private string? _title = "Проверка работы ViewModel. Проверка прошла успшено!";

		///<summary>Заголовок окна</summary>
		public string? Title { get => _title; set => Set(ref _title, value); }

		#endregion

	}
}
