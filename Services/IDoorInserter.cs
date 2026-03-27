using DoorTool.Models;

namespace DoorTool.Services
{
    public interface IDoorInserter
    {
        /// <summary>
        /// Lưu block cần insert và trigger AutoCAD command.
        /// </summary>
        void RequestInsert(DoorDefinition door);

        /// <summary>
        /// Thực thi Jig + commit BlockReference — gọi từ [CommandMethod].
        /// </summary>
        void Execute();
    }
}