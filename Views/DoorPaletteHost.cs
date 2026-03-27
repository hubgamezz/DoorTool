using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace DoorTool.Views
{
    /// <summary>
    /// WinForms wrapper bọc WPF UserControl.
    /// PaletteSet.Add() yêu cầu System.Windows.Forms.Control.
    /// </summary>
    public class DoorPaletteHost : UserControl
    {
        public DoorPaletteHost()
        {
            var host = new ElementHost
            {
                Dock  = DockStyle.Fill,
                Child = new DoorPaletteView(),
            };
            Controls.Add(host);
        }
    }
}