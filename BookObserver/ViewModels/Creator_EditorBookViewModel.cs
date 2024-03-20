﻿using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookObserver.ViewModels
{
    internal class Creator_EditorBookViewModel : ViewModel
    {
		private readonly BooksUserControlViewModel _booksViewModel;

		private ObservableCollection<Book>? Books { get; } 

		#region Title : string? - Заголовок окна

		///<summary>Заголовок окна</summary>
		private string _title = "Создать книгу";

		///<summary>Заголовок окна</summary>
		public string Title { get => _title; set => Set(ref _title, value); }

		#endregion

		#region SelectedCodeAuthor : string? - Выбранный код автора

		///<summary>Выбранный код автора</summary>
		private string? _selectedCodeAuthor;

		///<summary>Выбранный код автора</summary>
		public string? SelectedCodeAuthor { get => _selectedCodeAuthor; set => Set(ref _selectedCodeAuthor, value); }

        #endregion

        #region SelectedBBK : double - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private double _selectedBBK;

		///<summary>Выбранный ББК</summary>
		public double SelectedBBK { get => _selectedBBK; set => Set(ref _selectedBBK, value); }

		#endregion

		#region SelectedAuthor : string? - Выбранный автор

		///<summary>Выбранный автор</summary>
		private string? _selectedAuthor;

		///<summary>Выбранный автор</summary>
		public string? SelectedAuthor { get => _selectedAuthor; set => Set(ref _selectedAuthor, value); }

		#endregion

		#region SelectedName : string? - Выбранное название

		///<summary>Выбранное название</summary>
		private string? _selectedName;

		///<summary>Выбранное название</summary>
		public string? SelectedName { get => _selectedName; set => Set(ref _selectedName, value); }

		#endregion

		#region SelectedPublish : string? - Выбранное издательство

		///<summary>Выбранное издательство</summary>
		private string? _selectedPublish;

		///<summary>Выбранное издательство</summary>
		public string? SelectedPublish { get => _selectedPublish; set => Set(ref _selectedPublish, value); }

		#endregion

		#region SelectedYearPublish : int - Выбранный год издания

		///<summary>Выбранный год издания</summary>
		private int _selectedYearPublish;

		///<summary>Выбранный год издания</summary>
		public int SelectedYearPublish { get => _selectedYearPublish; set => Set(ref _selectedYearPublish, value); }

		#endregion

		#region SelectedPages : int - Выбранное количество страниц

		///<summary>Выбранное количество страниц</summary>
		private int _selectedPages;

		///<summary>Выбранное количество страниц</summary>
		public int SelectedPages { get => _selectedPages; set => Set(ref _selectedPages, value); }

		#endregion

		#region SelectedISBN : string? - Выбранный ISBN

		///<summary>Выбранный ISBN</summary>
		private string? _selectedISBN;

		///<summary>Выбранный ISBN</summary>
		public string? SelectedISBN { get => _selectedISBN; set => Set(ref _selectedISBN, value); }

		#endregion

		#region Stock : bool - В наличии

		///<summary>В наличии</summary>
		private bool _stock = true;

		///<summary>В наличии</summary>
		public bool Stock { get => _stock; set => Set(ref _stock, value); }

		#endregion



		public Creator_EditorBookViewModel(BooksUserControlViewModel booksViewModel)
		{
            _booksViewModel = booksViewModel;
			if (_booksViewModel.Books is null) return;

			Books = new ObservableCollection<Book>(_booksViewModel.Books);
        }

	}
}
