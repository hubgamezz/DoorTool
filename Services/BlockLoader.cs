using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DoorTool.Models;
using System;
using System.Collections.Generic;

namespace DoorTool.Services
{
    public class BlockLoader : IBlockLoader
    {
        public List<DoorDefinition> Load(string prefix)
        {
            var result = new List<DoorDefinition>();

            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return result;

            var db = doc.Database;

            using var tr = db.TransactionManager.StartOpenCloseTransaction();
            var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

            foreach (ObjectId id in bt)
            {
                var btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);

                if (btr.IsLayout || btr.IsAnonymous ||
                    btr.IsFromExternalReference || btr.IsFromOverlayReference)
                    continue;

                if (!btr.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add(new DoorDefinition(btr.Name, id, prefix));
            }

            result.Sort((a, b) =>
                string.Compare(a.BlockName, b.BlockName,
                               StringComparison.OrdinalIgnoreCase));
            return result;
        }
    }
}