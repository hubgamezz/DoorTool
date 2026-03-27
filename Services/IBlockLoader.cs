using DoorTool.Models;
using System.Collections.Generic;

namespace DoorTool.Services
{
    public interface IBlockLoader
    {
        /// <summary>
        /// Quét BlockTable hiện tại và trả về các block có prefix phù hợp.
        /// </summary>
        List<DoorDefinition> Load(string prefix);
    }
}