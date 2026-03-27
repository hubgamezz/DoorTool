using System;
using System.Windows.Input;

namespace DoorTool.ViewModels
{
    /// <summary>
    /// ICommand implementation dùng delegate — không phụ thuộc framework nào.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>    _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(
            Action<object?> execute,
            Func<object?, bool>? canExecute = null)
        {
            _execute    = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Shorthand cho command không cần tham số
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(),
                   canExecute == null ? null : _ => canExecute())
        { }

        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
            => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter)
            => _execute(parameter);

        /// <summary>
        /// Gọi khi muốn ép WPF re-evaluate CanExecute thủ công.
        /// </summary>
        public void RaiseCanExecuteChanged()
            => CommandManager.InvalidateRequerySuggested();
    }
}