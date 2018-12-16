using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeImp.DoomBuilder.Controls
{
    // in high contrast mode we have to handle drawing the background image ourselves.
    // ideally the background image isn't what we would use for this at all, but larger
    // architectural issues (potential plugin compatibility) force this approach.
    // see https://github.com/anotak/doombuilderx/issues/9

    // also thanks to cubrr from here for making me realize what the problem is
    // https://stackoverflow.com/questions/28565453/c-sharp-override-high-contrast-for-background-image
    public class ImagePanel : Panel
    {
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;

                if (BackgroundImage == null)
                {
                    bg = null;
                }
                else
                {
                    bg = new Bitmap(BackgroundImage);
                }
            }
        }
        private Bitmap bg;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (bg == null || !SystemInformation.HighContrast)
            {
                base.OnPaintBackground(e);
                return;
            }

            // filtering looks awful for old sprites
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            e.Graphics.Clear(BackColor);

            Rectangle clip = e.ClipRectangle;
            float centerx = clip.Width  / 2.0f;
            float centery = clip.Height / 2.0f;

            float outputw = bg.Width;
            float outputh = bg.Height;

            float scale = Math.Min(clip.Width / outputw, clip.Height / outputh);

            outputw *= scale;
            outputh *= scale;

            float targetx = centerx - (outputw / 2.0f);
            float targety = centery - (outputh / 2.0f);


            e.Graphics.DrawImage(bg,targetx,targety, outputw, outputh);
        }
    } // class
} // ns
