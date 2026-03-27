using Autodesk.AutoCAD.ApplicationServices;
using DoorTool.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace DoorTool.ViewModels
{
    /// <summary>
    /// ViewModel chính của Palette.
    ///
    /// Trách nhiệm:
    ///   - Giữ danh sách DoorTileViewModel (ObservableCollection)
    ///   - Expose RefreshCommand
    ///   - Hiển thị trạng thái (StatusText, IsBusy)
    ///   - Lắng nghe sự kiện AutoCAD (document activated) để tự reload
    ///   - Lắng nghe Document.CommandEnded để tự thêm block mới vào UI
    /// </summary>
    public class DoorPaletteViewModel : BaseViewModel
    {
        private readonly IBlockLoader  _loader;
        private readonly IDoorInserter _inserter;
        private readonly string        _prefix;
        private readonly Dispatcher    _uiDispatcher;

        private Document? _currentDoc;

        // Lưu danh sách block name hiện tại để so sánh
        private int _lastBlockCount = 0;

        // ── Backing fields ────────────────────────────────────────────────────

        private string _statusText = "Sẵn sàng";
        private bool   _isBusy     = false;

        // ── Properties ────────────────────────────────────────────────────────

        /// <summary>Danh sách tile — binding tới ItemsControl trong View</summary>
        public ObservableCollection<DoorTileViewModel> Tiles { get; } = new();

        /// <summary>Text hiển thị ở thanh trạng thái phía dưới</summary>
        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// True khi đang load — có thể dùng để hiện spinner hoặc disable UI.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public ICommand RefreshCommand { get; }

        // ── Constructor ───────────────────────────────────────────────────────

        public DoorPaletteViewModel(
            IBlockLoader  loader,
            IDoorInserter inserter,
            string        prefix = "DOOR_")
        {
            _loader       = loader;
            _inserter     = inserter;
            _prefix       = prefix;
            _uiDispatcher = Dispatcher.CurrentDispatcher;

            RefreshCommand = new RelayCommand(Refresh);

            // Lắng nghe CommandEnded trên document hiện tại
            SubscribeDocumentEvents();

            // Tự động reload + re-subscribe khi đổi sang bản vẽ khác
            Application.DocumentManager.DocumentActivated += OnDocumentActivated;

            // Load lần đầu
            Refresh();
        }

        // ── Document event subscription ───────────────────────────────────────

        private void SubscribeDocumentEvents()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            _currentDoc = doc;
            _currentDoc.CommandEnded += OnCommandEnded;
        }

        private void UnsubscribeDocumentEvents()
        {
            if (_currentDoc != null)
            {
                _currentDoc.CommandEnded -= OnCommandEnded;
                _currentDoc = null;
            }
        }

        private void OnDocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            // Hủy đăng ký document cũ, đăng ký document mới
            UnsubscribeDocumentEvents();
            SubscribeDocumentEvents();
            _uiDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(Refresh));
        }

        private void OnCommandEnded(object sender, CommandEventArgs e)
        {
            // Sau khi bất kỳ command nào kết thúc, kiểm tra xem có block mới không
            // Chỉ refresh nếu số lượng block thay đổi (tránh refresh không cần thiết)
            try
            {
                var definitions = _loader.Load(_prefix);
                if (definitions.Count != _lastBlockCount)
                {
                    _uiDispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(Refresh));
                }
            }
            catch
            {
                // Bỏ qua lỗi trong event handler
            }
        }

        // ── Logic ─────────────────────────────────────────────────────────────

        private void Refresh()
        {
            IsBusy = true;
            StatusText = "Đang tải...";
            Tiles.Clear();

            try
            {
                var definitions = _loader.Load(_prefix);
                _lastBlockCount = definitions.Count;

                foreach (var def in definitions)
                    Tiles.Add(new DoorTileViewModel(def, _inserter));

                StatusText = Tiles.Count == 0
                    ? $"Không tìm thấy block '{_prefix}*'"
                    : $"{Tiles.Count} block";
            }
            catch (System.Exception ex)
            {
                StatusText = $"Lỗi: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}