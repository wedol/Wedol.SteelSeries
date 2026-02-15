using System;
using System.IO;

namespace Wedol.SteelSeries
{
    public sealed class SteelSeriesGG
    {
        public string SteelSeriesGGProgramDataPath { get; private set; }

        public SteelSeriesGG()
        {
            SteelSeriesGGProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SteelSeries");
        }

        public SteelSeriesGG(string SteelSeriesGGProgramDataPath)
        {
            this.SteelSeriesGGProgramDataPath = SteelSeriesGGProgramDataPath;
        }
    }
}
