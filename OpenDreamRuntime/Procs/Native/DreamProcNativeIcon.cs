using OpenDreamRuntime.Objects;
using OpenDreamRuntime.Objects.Types;
using OpenDreamRuntime.Resources;
using OpenDreamShared.Dream;
using BlendType = OpenDreamRuntime.Objects.DreamIconOperationBlend.BlendType;
using DreamValueTypeFlag = OpenDreamRuntime.DreamValue.DreamValueTypeFlag;

namespace OpenDreamRuntime.Procs.Native {
    internal static class DreamProcNativeIcon {
        [DreamProc("Width")]
        public static DreamValue NativeProc_Width(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            return new DreamValue(((DreamObjectIcon)src!).Icon.Width);
        }

        [DreamProc("Height")]
        public static DreamValue NativeProc_Height(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            return new DreamValue(((DreamObjectIcon)src!).Icon.Height);
        }

        [DreamProc("Insert")]
        [DreamProcParameter("new_icon", Type = DreamValueTypeFlag.DreamObject)]
        [DreamProcParameter("icon_state", Type = DreamValueTypeFlag.String)]
        [DreamProcParameter("dir", Type = DreamValueTypeFlag.Float)]
        [DreamProcParameter("frame", Type = DreamValueTypeFlag.Float)]
        [DreamProcParameter("moving", Type = DreamValueTypeFlag.Float)]
        [DreamProcParameter("delay", Type = DreamValueTypeFlag.Float)]
        public static DreamValue NativeProc_Insert(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            //TODO Figure out what happens when you pass the wrong types as args

            DreamValue newIcon = state.GetArgument(0, "new_icon");
            DreamValue iconState = state.GetArgument(1, "icon_state");
            DreamValue dir = state.GetArgument(2, "dir");
            DreamValue frame = state.GetArgument(3, "frame");
            DreamValue moving = state.GetArgument(4, "moving");
            DreamValue delay = state.GetArgument(5, "delay");

            // TODO: moving & delay

            var resourceManager = IoCManager.Resolve<DreamResourceManager>();
            if (!resourceManager.TryLoadIcon(newIcon, out var iconRsc))
                throw new Exception($"Cannot insert {newIcon}");

            ((DreamObjectIcon)src!).Icon.InsertStates(iconRsc, iconState, dir, frame); // TODO: moving & delay
            return DreamValue.Null;
        }

        public static void Blend(DreamIcon icon, DreamValue blend, BlendType function, int x, int y) {
            if (blend.TryGetValueAsString(out var colorStr)) {
                if (!ColorHelpers.TryParseColor(colorStr, out var color))
                    throw new Exception($"Invalid color {colorStr}");

                icon.ApplyOperation(new DreamIconOperationBlendColor(function, x, y, color));
            } else {
                icon.ApplyOperation(new DreamIconOperationBlendImage(function, x, y, blend));
            }
        }

        [DreamProc("Blend")]
        [DreamProcParameter("icon", Type = DreamValueTypeFlag.DreamObject)]
        [DreamProcParameter("function", Type = DreamValueTypeFlag.Float, DefaultValue = (int)BlendType.Add)] // ICON_ADD
        [DreamProcParameter("x", Type = DreamValueTypeFlag.Float, DefaultValue = 1)]
        [DreamProcParameter("y", Type = DreamValueTypeFlag.Float, DefaultValue = 1)]
        public static DreamValue NativeProc_Blend(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            //TODO Figure out what happens when you pass the wrong types as args

            DreamValue icon = state.GetArgument(0, "icon");
            DreamValue function = state.GetArgument(1, "function");

            state.GetArgument(2, "x").TryGetValueAsInteger(out var x);
            state.GetArgument(3, "y").TryGetValueAsInteger(out var y);

            if (!function.TryGetValueAsInteger(out var functionValue))
                throw new Exception($"Invalid 'function' argument {function}");

            Blend(((DreamObjectIcon)src!).Icon, icon, (BlendType)functionValue, x, y);
            return DreamValue.Null;
        }

        [DreamProc("Scale")]
        [DreamProcParameter("width", Type = DreamValueTypeFlag.Float)]
        [DreamProcParameter("height", Type = DreamValueTypeFlag.Float)]
        public static DreamValue NativeProc_Scale(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            //TODO Figure out what happens when you pass the wrong types as args

            state.GetArgument(0, "width").TryGetValueAsInteger(out var width);
            state.GetArgument(1, "height").TryGetValueAsInteger(out var height);

            DreamIcon iconObj = ((DreamObjectIcon)src!).Icon;
            iconObj.Width = width;
            iconObj.Height = height;
            return DreamValue.Null;
        }

        [DreamProc("Turn")]
        [DreamProcParameter("angle", Type = DreamValueTypeFlag.Float)]
        public static DreamValue NativeProc_Turn(FastNativeProc.FastNativeProcBundle state, DreamObject? src, DreamObject? usr) {
            DreamValue angleArg = state.GetArgument(0, "angle");
            if (!angleArg.TryGetValueAsFloat(out float angle)) {
                return new DreamValue(src!); // Defaults to input on invalid angle
            }

            _NativeProc_TurnInternal((DreamObjectIcon)src!, angle);
            return DreamValue.Null;
        }

        /// <summary> Turns a given icon a given amount of degrees clockwise. </summary>
        public static void _NativeProc_TurnInternal(DreamObjectIcon src, float angle) {
            src.Turn(angle);
        }
    }
}
