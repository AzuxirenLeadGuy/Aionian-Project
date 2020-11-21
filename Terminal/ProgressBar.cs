using System;
namespace Aionian.Terminal
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
		private bool FixedLength;
        private byte Length;
        public byte Progress { get; private set; }
        bool LastRevolveState;
        public byte Percentage
        {
            get => perc;
            set
            {
                perc = value;
                Progress = (byte)(perc * Length / 100);
            }
        }
        public ConsoleProgressBar(byte l)
        {
            Length = l;
			FixedLength=l<2;
            perc = 0;
            Progress = 0;
            Written = 0;
            LastRevolveState = false;
        }
        byte perc;
        byte Written;
        void Erase(byte x)
        {
            while (x-- > 0) Console.Write("\b");
            Written = 0;
        }
        void UpdateBar()
        {
            Console.Write("[");
            Written += 2;
            for (byte i = 0; i < Length; i++)
            {
                if (100 * i < Percentage * Length) Console.Write(block);
                else Console.Write(" ");
            }
            Written += Length;
            Console.Write("]");
        }
        void UpdatePerc()
        {
            Console.Write(perc.ToString().PadLeft(3, ' ') + '%');
            Written += 4;
        }
        void UpdateRevolver()
        {
            if (LastRevolveState) Console.Write("+");
            else Console.Write("x");
            LastRevolveState = !LastRevolveState;
            Written++;
        }
        public void Write(ProgressFormat format = (ProgressFormat.BarOnly | ProgressFormat.PercOnly | ProgressFormat.RevolverOnly))
        {
            if (Written != 0) Erase(Written);
			if(FixedLength==false)Length=(byte)(((System.Console.WindowWidth>255?255:System.Console.WindowWidth) *4)/5);
            if ((format & ProgressFormat.BarOnly) != 0) UpdateBar();
            if ((format & ProgressFormat.PercOnly) != 0) UpdatePerc();
            if ((format & ProgressFormat.RevolverOnly) != 0) UpdateRevolver();
        }
    }
}