﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static StbRectPackSharp.StbRectPack;

namespace StbRectPackSharp
{
	/// <summary>
	/// Simple Packer class that doubles size of the atlas if the place runs out
	/// </summary>
	unsafe class Packer : IDisposable
	{
		private readonly stbrp_context _context;
		private readonly List<Rectangle> _rectangles = new List<Rectangle>();

		public int Width => _context.width;
		public int Height => _context.height;

		public List<Rectangle> PackRectangles => _rectangles;


		public Packer(int width = 256, int height = 256)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			// Initialize the context
			var num_nodes = width;
			_context = new stbrp_context(num_nodes);

			fixed (stbrp_context* contextPtr = &_context)
			{
				stbrp_init_target(contextPtr, width, height, _context.all_nodes, num_nodes);
			}
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		/// <summary>
		/// Packs a rect. Returns null, if there's no more place left.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public bool PackRect(int width, int height, out Rectangle packRectangle)
		{
			var rect = new stbrp_rect
			{
				id = _rectangles.Count,
				w = width,
				h = height
			};

			int result;
			fixed (stbrp_context* contextPtr = &_context)
			{
				result = stbrp_pack_rects(contextPtr, &rect, 1);
			}

			if (result == 0)
			{
				packRectangle = Rectangle.Empty;
				return false;
			}

			packRectangle = new Rectangle(rect.x, rect.y, rect.w, rect.h);
			_rectangles.Add(packRectangle);

			return true;
		}
	}
}
