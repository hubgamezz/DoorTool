using DoorTool.Services;
using DoorTool.ViewModels;
using System.Windows.Controls;

namespace DoorTool.Views
{
    public partial class DoorPaletteView : UserControl
    {
        public DoorPaletteView()
        {
            InitializeComponent();

            // Dùng singleton Inserter từ DoorApplication để chia sẻ _pending state
            var loader = new BlockLoader();

            DataContext = new DoorPaletteViewModel(
                loader:   loader,
                inserter: DoorApplication.Inserter,
                prefix:   "DOOR_"
            );
        }
    }
}