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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapsui.Rendering.Gdi.Extensions;
using Color = System.Drawing.Color;

namespace Mapsui.Rendering.Gdi
{
    /// <summary>
    /// Style context containing objects able to draw in GDI.
    /// This implements IDisposable to release drawing resources.
    /// </summary>
    internal sealed class StyleContext : IDisposable
    {
        private Dictionary<Styles.Pen, System.Drawing.Pen> pens;
        private Dictionary<Styles.Brush, System.Drawing.Brush> brushes;
        private Dictionary<Styles.Font, System.Drawing.Font> fonts;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleContext"/> class.
        /// </summary>
        public StyleContext()
        {
            pens = new Dictionary<Styles.Pen, System.Drawing.Pen>();
            brushes = new Dictionary<Styles.Brush, System.Drawing.Brush>();
            fonts = new Dictionary<Styles.Font, System.Drawing.Font>();
        }

        /// <summary>
        /// Gets the GDI pen.
        /// </summary>
        /// <param name="pen">The GDI pen.</param>
        /// <returns></returns>
        public System.Drawing.Pen GetPen(Styles.Pen pen)
        {
            System.Drawing.Pen gdiPen;
            if (!pens.TryGetValue(pen, out gdiPen))
            {
                gdiPen = new System.Drawing.Pen(pen.Color.ToGdi(), (float)pen.Width);
            }
            return gdiPen;
        }

        /// <summary>
        /// Gets the GDI brush.
        /// </summary>
        /// <param name="brush">The GDI brush.</param>
        /// <returns></returns>
        public System.Drawing.Brush GetBrush(Styles.Brush brush)
        {
            System.Drawing.Brush gdiBrush;
            if (!brushes.TryGetValue(brush, out gdiBrush))
            {
                gdiBrush = new System.Drawing.SolidBrush(brush.Color.ToGdi());
            }
            return gdiBrush;
        }

        public System.Drawing.Font GetFont(Styles.Font font)
        {
            System.Drawing.Font gdiFont;
            if (!fonts.TryGetValue(font, out gdiFont))
            {
                gdiFont = new System.Drawing.Font(font.FontFamily, (float)font.Size);
            }
            return gdiFont;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StyleContext"/> class.
        /// </summary>
        ~StyleContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (pens != null)
                {
                    foreach (var pen in pens.Values)
                    {
                        pen.Dispose();
                    }
                }

                if (brushes != null)
                {
                    foreach (var brush in brushes.Values)
                    {
                        brush.Dispose();
                    }
                }

                if (fonts != null)
                {
                    foreach (var font in fonts.Values)
                    {
                        font.Dispose();
                    }
                }
                pens = null;
                brushes = null;
                fonts = null;
            }
        }
    }
}