using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Services.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class SelectReaderForEditorBookViewModel : ReadersViewModel
    {
        private readonly EditorBookViewModel _editorBookVM;

        #region Commands

        #region SelectReaderCommand - Команда выбрать читателя

        ///<summary>Команда выбрать читателя</summary>
        private ICommand? _selectReaderCommand;

        ///<summary>Команда выбрать читателя</summary>
        public ICommand SelectReaderCommand => _selectReaderCommand
            ??= new LambdaCommand(OnSelectReaderCommandExecuted, CanSelectReaderCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда выбрать читателя</summary>
        private bool CanSelectReaderCommandExecute(object? p) => 
            p is Window
            && SelectedReader is not null;

        ///<summary>Логика выполнения - Команда выбрать читателя</summary>
        private void OnSelectReaderCommandExecuted(object? p)
        {
            _editorBookVM.IdReader = Readers.IndexOf(SelectedReader!);
            _editorBookVM.FullNameReader = SelectedReader!.FullName;
            (p as Window)!.Close();
        }

        #endregion

        #endregion

        public SelectReaderForEditorBookViewModel(EditorBookViewModel editorBookVM) : base(null)
        {
            _editorBookVM = editorBookVM;
            ((Command)ResetToZeroSearchCommand).Executable = false;
        }
    }
}
