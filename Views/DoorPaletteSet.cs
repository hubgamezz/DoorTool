using Autodesk.AutoCAD.Windows;
using System;
using System.Drawing;

namespace DoorTool.Views
{
    public class DoorPaletteSet : PaletteSet
    {
        private static readonly Guid PaletteGuid =
            new("D3A7B2C1-F4E5-4D6C-9A8B-1234567890AB");

        public DoorPaletteSet()
            : base("Door Tool", "DOORPANEL", PaletteGuid)
        {
            Size        = new Size(260, 500);
            MinimumSize = new Size(200, 200);

            Style = PaletteSetStyles.ShowAutoHideButton
                  | PaletteSetStyles.ShowCloseButton
                  | PaletteSetStyles.Snappable;

            DockEnabled = DockSides.Left | DockSides.Right;
            Dock        = DockSides.Right;

            Add("Cửa", new DoorPaletteHost());

            Visible = true;
        }
    }
}