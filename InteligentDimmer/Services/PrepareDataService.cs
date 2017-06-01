using InteligentDimmer.Model;

namespace InteligentDimmer.Services
{
    public static class PrepareDataService
    {
        public static void PrepareData(byte command1, byte data1, byte data2)
        {
            ControlData.StartByte = 0xAA;
            ControlData.CommandByte = command1;
            ControlData.SeparatorByte1 = 0xBB;
            ControlData.DataByte1 = data1;
            ControlData.SeparatorByte2 = 0xBB;
            ControlData.DataByte2 = data2;
            ControlData.EndByte = 0xCC;
        }
    }
}
