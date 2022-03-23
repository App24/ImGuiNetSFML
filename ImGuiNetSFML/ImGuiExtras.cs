using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiNETSFML
{
    public static class ImGuiExtras
    {
        public static bool ColorPicker(string label, ref SFML.System.Vector3f rgb)
        {
            Vector3 color = new Vector3(rgb.X, rgb.Y, rgb.Z);
            bool value = ColorPicker(label, ref color);

            rgb.X = color.X;
            rgb.Y = color.Y;
            rgb.Z = color.Z;

            return value;
        }

        public static bool ColorPicker(string label, ref Vector3 rgb)
        {
            float HUE_PICKER_WIDTH = 20f;
            float CROSSHAIR_SIZE = 7f;
            Vector2 SV_PICKER_SIZE = new Vector2(200, 200);

            ImColor color = new ImColor();
            color.Value = new Vector4(rgb, 255);
            bool valueChanged = false;

            var drawList = ImGui.GetWindowDrawList();

            var pickerPos = ImGui.GetCursorScreenPos();

            List<ImColor> colors = new List<ImColor>();

            ImColor customColor = new ImColor();
            customColor.Value = new Vector4(255, 0, 0, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(255, 255, 0, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(0, 255, 0, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(0, 255, 255, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(0, 0, 255, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(255, 0, 255, 255);
            colors.Add(customColor);

            customColor = new ImColor();
            customColor.Value = new Vector4(255, 0, 0, 255);
            colors.Add(customColor);

            for (int i = 0; i < 6; i++)
            {
                drawList.AddRectFilledMultiColor(
                    new Vector2(pickerPos.X + SV_PICKER_SIZE.X + 10, pickerPos.Y + i * (SV_PICKER_SIZE.Y / 6f)),
                    new Vector2(pickerPos.X + SV_PICKER_SIZE.X + 10 + HUE_PICKER_WIDTH, pickerPos.Y + (i + 1) * (SV_PICKER_SIZE.Y / 6)),
                    ImGui.ColorConvertFloat4ToU32(colors[i].Value),
                    ImGui.ColorConvertFloat4ToU32(colors[i].Value),
                    ImGui.ColorConvertFloat4ToU32(colors[i + 1].Value),
                    ImGui.ColorConvertFloat4ToU32(colors[i + 1].Value)
                    );
            }

            ImGui.ColorConvertRGBtoHSV(
                color.Value.X, color.Value.Y, color.Value.Z, out float hue, out float saturation, out float value);

            drawList.AddLine(
                new Vector2(pickerPos.X + SV_PICKER_SIZE.X + 8, pickerPos.Y + hue * SV_PICKER_SIZE.Y),
                new Vector2(pickerPos.X + SV_PICKER_SIZE.X + 12 + HUE_PICKER_WIDTH, pickerPos.Y + hue * SV_PICKER_SIZE.Y),
                ImGui.ColorConvertFloat4ToU32(new Vector4(255))
                );

            {
                int step = 5;
                Vector2 pos = new Vector2(0);

                Vector4 c00 = new Vector4(1, 1, 1, 1);
                Vector4 c10 = new Vector4(1, 1, 1, 1);
                Vector4 c01 = new Vector4(1, 1, 1, 1);
                Vector4 c11 = new Vector4(1, 1, 1, 1);
                for (int y1 = 0; y1 < step; y1++)
                {
                    for (int x1 = 0; x1 < step; x1++)
                    {
                        float s0 = x1 / (float)step;
                        float s1 = (x1 + 1) / (float)step;
                        float v0 = 1 - y1 / (float)step;
                        float v1 = 1 - (y1 + 1) / (float)step;

                        ImGui.ColorConvertHSVtoRGB(hue, s0, v0, out c00.X, out c00.Y, out c00.Z);
                        ImGui.ColorConvertHSVtoRGB(hue, s1, v0, out c10.X, out c10.Y, out c10.Z);
                        ImGui.ColorConvertHSVtoRGB(hue, s0, v1, out c01.X, out c01.Y, out c01.Z);
                        ImGui.ColorConvertHSVtoRGB(hue, s1, v1, out c11.X, out c11.Y, out c11.Z);

                        drawList.AddRectFilledMultiColor(
                            new Vector2(pickerPos.X + pos.X, pickerPos.Y + pos.Y),
                            new Vector2(pickerPos.X + pos.X + SV_PICKER_SIZE.X / step, pickerPos.Y + pos.Y + SV_PICKER_SIZE.Y / step),
                            ImGui.ColorConvertFloat4ToU32(c00),
                            ImGui.ColorConvertFloat4ToU32(c10),
                            ImGui.ColorConvertFloat4ToU32(c11),
                            ImGui.ColorConvertFloat4ToU32(c01)
                            );

                        pos.X += SV_PICKER_SIZE.X / step;
                    }
                    pos.X = 0;
                    pos.Y += SV_PICKER_SIZE.Y / step;
                }
            }

            float x = saturation * SV_PICKER_SIZE.X;
            float y = (1 - value) * SV_PICKER_SIZE.Y;
            Vector2 p = new Vector2(pickerPos.X + x, pickerPos.Y + y);
            drawList.AddLine(new Vector2(p.X - CROSSHAIR_SIZE, p.Y), new Vector2(p.X - 2, p.Y), ImGui.ColorConvertFloat4ToU32(new Vector4(255)));
            drawList.AddLine(new Vector2(p.X + CROSSHAIR_SIZE, p.Y), new Vector2(p.X + 2, p.Y), ImGui.ColorConvertFloat4ToU32(new Vector4(255)));
            drawList.AddLine(new Vector2(p.X, p.Y - CROSSHAIR_SIZE), new Vector2(p.X, p.Y - 2), ImGui.ColorConvertFloat4ToU32(new Vector4(255)));
            drawList.AddLine(new Vector2(p.X, p.Y + CROSSHAIR_SIZE), new Vector2(p.X, p.Y + 2), ImGui.ColorConvertFloat4ToU32(new Vector4(255)));

            ImGui.InvisibleButton("saturation_value_selector", SV_PICKER_SIZE);

            if (ImGui.IsItemActive() && ImGui.GetIO().MouseDown[0])
            {
                Vector2 mousePosInCanvas = new Vector2(ImGui.GetIO().MousePos.X - pickerPos.X, ImGui.GetIO().MousePos.Y - pickerPos.Y);

                if (!(mousePosInCanvas.X < -10 || mousePosInCanvas.X >= SV_PICKER_SIZE.X + 10 || mousePosInCanvas.Y < -10 || mousePosInCanvas.Y >= SV_PICKER_SIZE.Y + 10))
                {
                    if (mousePosInCanvas.X < 0) mousePosInCanvas.X = 0;
                    else if (mousePosInCanvas.X >= SV_PICKER_SIZE.X - 1) mousePosInCanvas.X = SV_PICKER_SIZE.X - 1;

                    if (mousePosInCanvas.Y < 0) mousePosInCanvas.Y = 0;
                    else if (mousePosInCanvas.Y >= SV_PICKER_SIZE.Y - 1) mousePosInCanvas.Y = SV_PICKER_SIZE.Y - 1;

                    value = 1 - (mousePosInCanvas.Y / (SV_PICKER_SIZE.Y - 1));
                    saturation = mousePosInCanvas.X / (SV_PICKER_SIZE.X - 1);

                    valueChanged = true;
                }
            }

            ImGui.SetCursorScreenPos(new Vector2(pickerPos.X + SV_PICKER_SIZE.X + 10, pickerPos.Y));
            ImGui.InvisibleButton("hue_selector", new Vector2(HUE_PICKER_WIDTH, SV_PICKER_SIZE.Y));

            if ((ImGui.IsItemHovered() || ImGui.IsItemActive()) && ImGui.GetIO().MouseDown[0])
            {
                Vector2 mousePosInCanvas = new Vector2(ImGui.GetIO().MousePos.X - pickerPos.X, ImGui.GetIO().MousePos.Y - pickerPos.Y);

                if (!(mousePosInCanvas.Y < -10 || mousePosInCanvas.Y >= SV_PICKER_SIZE.Y + 10))
                {
                    if (mousePosInCanvas.Y < 0) mousePosInCanvas.Y = 0;
                    else if (mousePosInCanvas.Y >= SV_PICKER_SIZE.Y - 2) mousePosInCanvas.Y = SV_PICKER_SIZE.Y - 2;

                    hue = mousePosInCanvas.Y / (SV_PICKER_SIZE.Y - 1);

                    valueChanged = true;
                }
            }

            ImGui.ColorConvertHSVtoRGB(hue, saturation, value, out rgb.X, out rgb.Y, out rgb.Z);

            return valueChanged | ImGui.ColorEdit3(label, ref rgb);
        }

        public static bool Dropdown(string label, string[] items, ref int index)
        {
            if (ImGui.BeginCombo(label, items[index]))
            {
                for (int i = 0; i < items.Length; i++)
                {
                    string name = items[i];
                    bool selected = name == items[index];
                    if (ImGui.Selectable(name, selected))
                    {
                        index = i;
                    }

                    if (selected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
                return true;
            }
            return false;
        }
    }
}
