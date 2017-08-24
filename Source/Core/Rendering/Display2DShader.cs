
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Display2DShader : D3DShader
	{
		#region ================== Variables

		// Property handlers
		private EffectHandle texture1;
		private EffectHandle rendersettings;
		private EffectHandle transformsettings;
		private EffectHandle filtersettings;
		
		#endregion

		#region ================== Properties

		public Texture Texture1 { set { if(manager.Enabled) effect.SetTexture(texture1, value); } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Display2DShader(ShaderManager manager) : base(manager)
		{
			// Load effect from file
			effect = LoadEffect("display2d.fx");

			// Get the property handlers from effect
			if(effect != null)
			{
				texture1 = effect.GetParameter(null, "texture1");
				rendersettings = effect.GetParameter(null, "rendersettings");
				transformsettings = effect.GetParameter(null, "transformsettings");
				filtersettings = effect.GetParameter(null, "filtersettings");
			}
			
			// Initialize world vertex declaration
			VertexElement[] elements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				new VertexElement(0, 16, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				VertexElement.VertexDeclarationEnd
			};
			vertexdecl = new VertexDeclaration(General.Map.Graphics.Device, elements);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(texture1 != null) texture1.Dispose();
				if(rendersettings != null) rendersettings.Dispose();
				if(transformsettings != null) transformsettings.Dispose();
				if(filtersettings != null) filtersettings.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This sets the settings
		public void SetSettings(float texelx, float texely, float fsaafactor, float alpha, bool bilinear)
		{
			if(manager.Enabled)
			{
				Vector4 values = new Vector4(texelx, texely, fsaafactor, alpha);
				effect.SetValue(rendersettings, values);
				Matrix world = manager.D3DDevice.Device.GetTransform(TransformState.World);
				Matrix view = manager.D3DDevice.Device.GetTransform(TransformState.View);
				effect.SetValue(transformsettings, Matrix.Multiply(world, view));
				TextureFilter filter = TextureFilter.Point;
				if(bilinear) filter = TextureFilter.Linear;
				effect.SetValue<int>(filtersettings, (int)filter);
			}
		}

		// This sets up the render pipeline
		public override void BeginPass(int index)
		{
			Device device = manager.D3DDevice.Device;

			if(!manager.Enabled)
			{
				// Sampler settings
				if(General.Settings.ClassicBilinear)
				{
					device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
					device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
					device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
					device.SetSamplerState(0, SamplerState.MipMapLodBias, 0f);
				}
				else
				{
					device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
					device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
					device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);
					device.SetSamplerState(0, SamplerState.MipMapLodBias, 0f);
				}

				// Texture addressing
				device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
				device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
				device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Wrap);

				// First texture stage
				device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
				device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
				device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
				device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Current);
				device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);

				// Second texture stage
				device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Modulate);
				device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Current);
				device.SetTextureStageState(1, TextureStage.ColorArg2, TextureArgument.TFactor);
				device.SetTextureStageState(1, TextureStage.ResultArg, TextureArgument.Current);
				device.SetTextureStageState(1, TextureStage.TexCoordIndex, 0);

				// No more further stages
				device.SetTextureStageState(2, TextureStage.ColorOperation, TextureOperation.Disable);

				// First alpha stage
				device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
				device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
				device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

				// Second alpha stage
				device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Modulate);
				device.SetTextureStageState(1, TextureStage.AlphaArg1, TextureArgument.Current);
				device.SetTextureStageState(1, TextureStage.AlphaArg2, TextureArgument.TFactor);

				// No more further stages
				device.SetTextureStageState(2, TextureStage.AlphaOperation, TextureOperation.Disable);
			}

			base.BeginPass(index);
		}
		
		#endregion
	}
}
