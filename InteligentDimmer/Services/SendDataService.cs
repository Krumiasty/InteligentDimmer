using InteligentDimmer.Configuration;
using InteligentDimmer.Model;
using System.IO.Ports;

namespace InteligentDimmer.Services
{
    public static class SendDataService
    {
        public static void SendData(SerialPort serialPort)
        {
            serialPort.Write(new byte[]
            {
                ControlData.StartByte,
                ControlData.CommandByte,
                ControlData.SeparatorByte1,
                ControlData.DataByte1,
                ControlData.SeparatorByte2,
                ControlData.DataByte2,
                ControlData.EndByte
          },
          0,
          Constants.BytesNumber);
        }
    }
}
