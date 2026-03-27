using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using DoorTool.Jig;
using DoorTool.Models;
using System;

namespace DoorTool.Services
{
    public class DoorInserter : IDoorInserter
    {
        // Block đang chờ insert — set bởi RequestInsert(), đọc bởi Execute()
        private DoorDefinition? _pending;

        // ── IDoorInserter ─────────────────────────────────────────────────────

        public void RequestInsert(DoorDefinition door)
        {
            _pending = door;

            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            // Kích hoạt cửa sổ AutoCAD rồi gửi command
            doc.SendStringToExecute("DOOR_INSERT\n", true, false, false);
        }

        public void Execute()
        {
            var door = _pending;
            _pending = null;

            if (door == null) return;

            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var db = doc.Database;
            var ed = doc.Editor;

            if (door.ObjectId.IsNull || !door.ObjectId.IsValid)
            {
                ed.WriteMessage($"\n[DoorTool] Block '{door.BlockName}' không hợp lệ.");
                return;
            }

            // ── Phase 1: chọn điểm đặt ───────────────────────────────────────
            var jig = new DoorInsertJig(door.ObjectId);

            var dragResult = ed.Drag(jig);
            if (dragResult.Status != PromptStatus.OK) return;

            // ── Phase 2: nhập góc xoay ────────────────────────────────────────
            jig.StartRotationPhase();
            var rotResult = ed.Drag(jig);
            if (rotResult.Status != PromptStatus.OK &&
                rotResult.Status != PromptStatus.None) return;

            // ── Commit vào bản vẽ ─────────────────────────────────────────────
            using var tr = db.TransactionManager.StartTransaction();

            var space = (BlockTableRecord)tr.GetObject(
                db.CurrentSpaceId, OpenMode.ForWrite);

            var blockRef = new BlockReference(jig.Position, door.ObjectId)
            {
                Rotation     = jig.Rotation,
                ScaleFactors = new Scale3d(1, 1, 1),
            };

            space.AppendEntity(blockRef);
            tr.AddNewlyCreatedDBObject(blockRef, true);

            AppendAttributes(blockRef, tr);

            tr.Commit();

            ed.WriteMessage(
                $"\n[DoorTool] '{door.DisplayName}' → " +
                $"({jig.Position.X:F2}, {jig.Position.Y:F2}), " +
                $"góc {jig.Rotation * 180 / Math.PI:F1}°");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void AppendAttributes(BlockReference blockRef, Transaction tr)
        {
            var btr = (BlockTableRecord)tr.GetObject(
                blockRef.BlockTableRecord, OpenMode.ForRead);

            if (!btr.HasAttributeDefinitions) return;

            foreach (ObjectId id in btr)
            {
                if (tr.GetObject(id, OpenMode.ForRead) is not AttributeDefinition attDef)
                    continue;
                if (attDef.Constant) continue;

                var attRef = new AttributeReference();
                attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
                attRef.TextString = attDef.TextString;

                blockRef.AttributeCollection.AppendAttribute(attRef);
                tr.AddNewlyCreatedDBObject(attRef, true);
            }
        }
    }
}