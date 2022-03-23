using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiNETSFML
{
    public static class Utils
    {
        public static ImColor CreateImColor(Vector3 color)
        {
            return CreateImColor(new Vector4(color, 255));
        }

        public static ImColor CreateImColor(Vector4 color)
        {
            ImColor imColor = new ImColor();
            imColor.Value = color;
            return imColor;
        }
    }
}
