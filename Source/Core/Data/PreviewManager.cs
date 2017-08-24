
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public class PreviewManager
	{
		#region ================== Constants

		// Image format
		private const PixelFormat IMAGE_FORMAT = PixelFormat.Format32bppArgb;

		// Dimensions of a single preview image
		public static readonly int[] PREVIEW_SIZES = new int[] { 48, 64, 80, 96, 112, 128 };

		#endregion

		#region ================== Variables
		
		// Dimensions of a single preview image
		private int maxpreviewwidth = 64;
		private int maxpreviewheight = 64;
		
		// Images
		private List<Bitmap> images;
		
		// Processing
		private Queue<ImageData> imageque;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Constants
		public int MaxImageWidth { get { return maxpreviewwidth; } }
		public int MaxImageHeight { get { return maxpreviewheight; } }
		
		// Disposing
		internal bool IsDisposed { get { return isdisposed; } }
		
		// Loading
		internal bool IsLoading
		{
			get
			{
				return (imageque.Count > 0);
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal PreviewManager()
		{
			// Initialize
			images = new List<Bitmap>();
			imageque = new Queue<ImageData>();
			maxpreviewwidth = PREVIEW_SIZES[General.Settings.PreviewImageSize];
			maxpreviewheight = PREVIEW_SIZES[General.Settings.PreviewImageSize];
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(Bitmap b in images) b.Dispose();
				images = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion
		
		#region ================== Private Methods
		
		// This makes a preview for the given image and updates the image settings
		private void MakeImagePreview(ImageData img)
		{
			int previewwidth, previewheight;
			int imagewidth, imageheight;
			Bitmap preview;
			Graphics g;
			
			lock(img)
			{
				// Load image if needed
				if(!img.IsImageLoaded) img.LoadImage();
				if(!img.LoadFailed)
				{
					imagewidth = img.Width;
					imageheight = img.Height;
				}
				else
				{
					imagewidth = img.GetBitmap().Size.Width;
					imageheight = img.GetBitmap().Size.Height;
				}
				
				// Determine preview size
				float scalex = (img.Width > maxpreviewwidth) ? ((float)maxpreviewwidth / (float)imagewidth) : 1.0f;
				float scaley = (img.Height > maxpreviewheight) ? ((float)maxpreviewheight / (float)imageheight) : 1.0f;
				float scale = Math.Min(scalex, scaley);
				previewwidth = (int)((float)imagewidth * scale);
				previewheight = (int)((float)imageheight * scale);
				if(previewwidth < 1) previewwidth = 1;
				if(previewheight < 1) previewheight = 1;

				// Make new image
				preview = new Bitmap(previewwidth, previewheight, IMAGE_FORMAT);
				g = Graphics.FromImage(preview);
				g.PageUnit = GraphicsUnit.Pixel;
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.None;
				g.Clear(Color.Transparent);
				
				// Draw image onto atlas
				Rectangle atlasrect = new Rectangle(0, 0, previewwidth, previewheight);
				RectangleF imgrect = General.MakeZoomedRect(new Size(imagewidth, imageheight), atlasrect);
				if(imgrect.Width < 1.0f)
				{
					imgrect.X -= 0.5f - imgrect.Width * 0.5f;
					imgrect.Width = 1.0f;
				}
				if(imgrect.Height < 1.0f)
				{
					imgrect.Y -= 0.5f - imgrect.Height * 0.5f;
					imgrect.Height = 1.0f;
				}
				g.DrawImage(img.GetBitmap(), imgrect);
				g.Dispose();
				
				// Unload image if no longer needed
				if(!img.IsReferenced) img.UnloadImage();
				
				lock(images)
				{
					// Set numbers
					img.PreviewIndex = images.Count;
					img.PreviewState = ImageLoadState.Ready;
					
					// Add to previews list
					images.Add(preview);
				}
			}
		}

		#endregion
		
		#region ================== Public Methods

		// This draws a preview centered in a target
		internal void DrawPreview(int previewindex, Graphics target, Point targetpos)
		{
			Bitmap image;

			// Get the preview we need
			lock(images) { image = images[previewindex]; }

			// Adjust offset for the size of the preview image
			targetpos.X += (maxpreviewwidth - image.Width) >> 1;
			targetpos.Y += (maxpreviewheight - image.Height) >> 1;
			
			// Draw from atlas to target
			lock(image)
			{
				target.DrawImageUnscaled(image, targetpos.X, targetpos.Y);
			}
		}

		// This returns a copy of the preview
		internal Bitmap GetPreviewCopy(int previewindex)
		{
			Bitmap image;

			// Get the preview we need
			lock(images) { image = images[previewindex]; }

			// Make a copy
			lock(image)
			{
				return new Bitmap(image);
			}
		}

		// Background loading
		// Return true when we have more work to do, so that the
		// thread will not wait too long before calling again
		internal bool BackgroundLoad()
		{
			// Get next item
			ImageData image = null;
			lock(imageque)
			{
				// Fetch next image to process
				if(imageque.Count > 0) image = imageque.Dequeue();
			}

			// Any image to process?
			if(image != null)
			{
				// Make image preview?
				if(!image.IsPreviewLoaded) MakeImagePreview(image);
			}

			return (image != null);
		}
		
		// This adds an image for preview creation
		internal void AddImage(ImageData image)
		{
			lock(imageque)
			{
				// Add to list
				image.PreviewState = ImageLoadState.Loading;
				imageque.Enqueue(image);
			}
		}


		#if DEBUG
		internal void DumpAtlases()
		{
			lock(images)
			{
				int index = 0;
				foreach(Bitmap a in images)
				{
					lock(a)
					{
						string file = Path.Combine(General.AppPath, "atlas" + index++ + ".png");
						a.Save(file, ImageFormat.Png);
					}
				}
			}
		}
		#endif
		
		#endregion
	}
}
