/*
MIT License

Copyright (c) 2024 Tolin Simpson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class Message : PropertyAttribute
	{
		public string message;
		public LogFormats messageType = LogFormats.None;
		public bool wordWrap = false;
		public int heightOffset = 0;

		public Message(string message)
		{
			this.message = message;
		}
		public Message(string message, bool wordWrap)
		{
			this.message = message;
			this.wordWrap = wordWrap;
		}

		public Message(string message, bool wordWrap, int heightOffset)
		{
			this.message = message;
			this.wordWrap = wordWrap;
			this.heightOffset = heightOffset;
		}

		public Message(string message, LogFormats messageType)
		{
			this.message = message;
			this.messageType = messageType;
		}

		public Message(string message, LogFormats messageType, int heightOffset)
		{
			this.message = message;
			this.messageType = messageType;
			this.heightOffset = heightOffset;
		}

		public Message(string message, LogFormats messageType, bool wordWrap)
		{
			this.message = message;
			this.messageType = messageType;
			this.wordWrap = wordWrap;
		}

		public Message(string message, LogFormats messageType, bool wordWrap, int heightOffset)
		{
			this.message = message;
			this.messageType = messageType;
			this.wordWrap = wordWrap;
			this.heightOffset = heightOffset;
		}
	}
}