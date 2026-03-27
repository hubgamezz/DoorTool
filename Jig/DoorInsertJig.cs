using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace DoorTool.Jig
{
    public class DoorInsertJig : EntityJig
    {
        private Point3d _position;
        private double  _rotation;
        private bool    _rotationPhase;

        public Point3d Position => _position;
        public double  Rotation => _rotation;

        public DoorInsertJig(ObjectId blockDefId)
            : base(new BlockReference(Point3d.Origin, blockDefId))
        {
            _position      = Point3d.Origin;
            _rotation      = 0;
            _rotationPhase = false;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            if (!_rotationPhase)
            {
                var opts = new JigPromptPointOptions(
                    "\nChọn điểm đặt cửa [ESC để hủy]: ")
                {
                    UserInputControls =
                        UserInputControls.Accept3dCoordinates |
                        UserInputControls.NullResponseAccepted
                };

                var res = prompts.AcquirePoint(opts);
                if (res.Status is PromptStatus.Cancel or PromptStatus.None)
                    return SamplerStatus.Cancel;
                if (res.Value.IsEqualTo(_position, Tolerance.Global))
                    return SamplerStatus.NoChange;

                _position = res.Value;
                return SamplerStatus.OK;
            }
            else
            {
                var opts = new JigPromptAngleOptions("\nGóc xoay <0°>: ")
                {
                    DefaultValue    = 0,
                    BasePoint       = _position,
                    UseBasePoint    = true,
                };

                var res = prompts.AcquireAngle(opts);
                if (res.Status == PromptStatus.Cancel)
                    return SamplerStatus.Cancel;
                if (Math.Abs(res.Value - _rotation) < 1e-6)
                    return SamplerStatus.NoChange;

                _rotation = res.Value;
                return SamplerStatus.OK;
            }
        }

        protected override bool Update()
        {
            var br = (BlockReference)Entity;
            br.Position = _position;
            br.Rotation = _rotation;
            return true;
        }

        public void StartRotationPhase() => _rotationPhase = true;
    }
}