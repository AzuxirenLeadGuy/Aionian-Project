using System;
namespace Progress_bar
{
	public enum ProgressFormat : byte
	{
		BarOnly = 1,
		PercOnly = 2,
		RevolverOnly = 4
	}
	public struct ConsoleProgressBar
	{
		public static readonly char block = '■';
		private readonly bool _fixedLength;
		private byte _length;
		public byte Progress { get; private set; }
		private bool _lastRevolveState;
		public byte Percentage
		{
			get => _perc;
			set
			{
				_perc = value;
				Progress = (byte)(_perc * _length / 100);
			}
		}
		public ConsoleProgressBar(byte l)
		{
			_length = l;
			_fixedLength = l >= 2;
			_perc = 0;
			Progress = 0;
			_written = 0;
			_lastRevolveState = false;
		}
		private byte _perc;
		private byte _written;
		private void Erase()
		{
			int x = _written;
			while (--x >= 0) Console.Write('\b');
			_written = 0;

		}
		private void UpdateBar()
		{
			Console.Write("[");
			_written += 2;
			for (byte i = 0; i < _length; i++)
			{
				if (100 * i < Percentage * _length) Console.Write(block);
				else Console.Write(" ");
			}
			_written += _length;
			Console.Write("]");
		}
		private void UpdatePerc()
		{
			Console.Write(_perc.ToString().PadLeft(3, ' ') + '%');
			_written += 4;
		}
		private void UpdateRevolver()
		{
			if (_lastRevolveState) Console.Write("+");
			else Console.Write("x");
			_lastRevolveState = !_lastRevolveState;
			_written++;
		}
		public void Write(ProgressFormat format = ProgressFormat.BarOnly | ProgressFormat.PercOnly | ProgressFormat.RevolverOnly)
		{
			if (_written != 0) Erase();
			if (_fixedLength == false) _length = (byte)((Console.WindowWidth > 255 ? 255 : Console.WindowWidth) * 4 / 5);
			if ((format & ProgressFormat.BarOnly) != 0) UpdateBar();
			if ((format & ProgressFormat.PercOnly) != 0) UpdatePerc();
			if ((format & ProgressFormat.RevolverOnly) != 0) UpdateRevolver();
		}
	}
}