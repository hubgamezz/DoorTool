using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DoorTool.ViewModels
{
    /// <summary>
    /// Base class cho tất cả ViewModel.
    /// Cung cấp INotifyPropertyChanged với helper SetProperty&lt;T&gt;.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gán giá trị mới và raise PropertyChanged nếu giá trị thay đổi.
        /// </summary>
        /// <returns>true nếu giá trị thực sự thay đổi</returns>
        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}