using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using DoorTool.Services;
using DoorTool.Views;

[assembly: ExtensionApplication(typeof(DoorTool.DoorApplication))]
[assembly: CommandClass(typeof(DoorTool.DoorApplication))]

namespace DoorTool
{
    public class DoorApplication : IExtensionApplication
    {
        private static DoorPaletteSet _paletteSet = null;

      
        internal static readonly DoorInserter Inserter = new DoorInserter();

        

        public void Initialize()
        {
            try
            {
                Application.Idle += OnFirstIdle;
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?
                    .Editor.WriteMessage($"\n[DoorTool] Initialize error: {ex.Message}");
            }
        }

        public void Terminate() { }

    

        private static bool _initialized = false;
        private static void OnFirstIdle(object sender, EventArgs e)
        {
            if (_initialized) return;
            _initialized = true;
            Application.Idle -= OnFirstIdle;

            try
            {
                _paletteSet = new DoorPaletteSet();
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?
                    .Editor.WriteMessage($"\n[DoorTool] Palette error: {ex.Message}\n{ex.StackTrace}");
            }
        }

     

        [CommandMethod("DOORPANEL", CommandFlags.Modal)]
        public void TogglePalette()
        {
            try
            {
                if (_paletteSet == null)
                    _paletteSet = new DoorPaletteSet();
                else
                    _paletteSet.Visible = !_paletteSet.Visible;
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?
                    .Editor.WriteMessage($"\n[DoorTool] DOORPANEL error: {ex.Message}");
            }
        }

        [CommandMethod("DOOR_INSERT",
            CommandFlags.Modal | CommandFlags.NoHistory | CommandFlags.NoPaperSpace)]
        public void InsertDoor()
        {
            Inserter.Execute();
        }
    }
}
