using BookObserver.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookObserver.ViewModels
{
    internal class Creator_EditorBookViewModel : ViewModel
    {
		private readonly BooksUserControlViewModel? _booksViewModel;
		#region Title : string? - Заголовок окна

		///<summary>Заголовок окна</summary>
		private string _title = "Создать книгу";

		///<summary>Заголовок окна</summary>
		public string Title { get => _title; set => Set(ref _title, value); }

		#endregion

		public Creator_EditorBookViewModel(BooksUserControlViewModel booksViewModel)
		{
			_booksViewModel = booksViewModel;
		}

	}
}
