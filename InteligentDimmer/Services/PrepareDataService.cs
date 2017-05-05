using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteligentDimmer.Model;

namespace InteligentDimmer.Services
{
    public static class PrepareDataService
    {
        public static void PrepareData(byte firstByte, byte secondByte, byte thirdByte,
            byte fourthByte, byte fifthByte)
        {
            ControlData.FirstByte = firstByte;
            ControlData.SecondByte = secondByte;
            ControlData.ThirdByte = thirdByte;
            ControlData.FourthByte = fourthByte;
            ControlData.FifthByte = fifthByte;
        }
    }
}
