using DoorTool.Models;
using DoorTool.Services;
using System.Windows.Input;

namespace DoorTool.ViewModels
{
    /// <summary>
    /// ViewModel cho một tile cửa trong danh sách.
    /// Wrap DoorDefinition và expose các property mà View cần binding.
    /// </summary>
    public class DoorTileViewModel : BaseViewModel
    {
        private readonly IDoorInserter _inserter;

        // ── Properties binding ────────────────────────────────────────────────

        /// <summary>Model gốc — View không dùng trực tiếp</summary>
        public DoorDefinition Model { get; }

        /// <summary>Tên hiển thị trên tile</summary>
        public string DisplayName => Model.DisplayName;

        /// <summary>Loại cửa — View dùng DataTrigger để chọn icon</summary>
        public DoorType DoorType  => Model.DoorType;

        // ── Commands ──────────────────────────────────────────────────────────

        /// <summary>
        /// Gọi khi user click tile — trigger insert vào bản vẽ.
        /// </summary>
        public ICommand InsertCommand { get; }

        // ── Constructor ───────────────────────────────────────────────────────

        public DoorTileViewModel(DoorDefinition model, IDoorInserter inserter)
        {
            Model     = model;
            _inserter = inserter;

            InsertCommand = new RelayCommand(OnInsert);
        }

        // ── Handlers ──────────────────────────────────────────────────────────

        private void OnInsert()
        {
            _inserter.RequestInsert(Model);
        }
    }
}