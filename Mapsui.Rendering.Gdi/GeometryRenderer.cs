// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Copyright 2010 - Paul den Dulk (Geodan) - Adapted SharpMap for Mapsui
//
// This file is part of Mapsui.
// Mapsui is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// Mapsui is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Mapsui; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Mapsui.Geometries;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Point = Mapsui.Geometries.Point;

namespace Mapsui.Rendering.Gdi
{
    /// <summary>
    /// This class renders individual geometry features to a graphics object using the settings of a map object.
    /// </summary>
    internal static class GeometryRenderer
    {
        internal static PointF[] ConvertPoints(IEnumerable<Point> points)
        {
            var result = new List<PointF>();
            foreach (var point in points) result.Add(new PointF((float)point.X, (float)point.Y));
            return result.ToArray();
        }

        internal static IEnumerable<Point> WorldToScreen(LineString linearRing, IViewport viewport)
        {
            var v = new Point[linearRing.Vertices.Count];
            for (int i = 0; i < linearRing.Vertices.Count; i++)
                v[i] = viewport.WorldToScreen(linearRing.Vertices[i]);
            return v;
        }

        internal static System.Drawing.PointF[] WorldToScreenGDI(LineString linearRing, IViewport viewport)
        {
            var v = new List<System.Drawing.PointF>(linearRing.Vertices.Count);
            for (int i = 0; i < linearRing.Vertices.Count; i++)
            {
                var point = viewport.WorldToScreen(linearRing.Vertices[i]);
                if (v.Count > 0 && i < linearRing.Vertices.Count - 1)
                {
                    var previousPoint = v.Last();
                    var xRange = Math.Round(previousPoint.X - point.X, 2);
                    var yRange = Math.Round(previousPoint.Y - point.Y, 2);

                    // Filter the point based on range.
                    if (!Filter(xRange) && !Filter(yRange))
                        continue;
                }
                v.Add(new System.Drawing.PointF((float)Math.Round(point.X, 2), (float)Math.Round(point.Y, 2)));
            }
            return v.ToArray();
        }

        internal static Point WorldToScreen(Point point, IViewport viewport)
        {
            return viewport.WorldToScreen(point);
        }

        private static bool Filter(double range)
        {
            return Math.Abs(range) >= 0.2;
        }
    }
}