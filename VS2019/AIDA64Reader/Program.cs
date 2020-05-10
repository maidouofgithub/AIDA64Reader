using AIDA64Reader.Model;
using AIDA64Reader.Utils;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIDA64Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    using (var mmf = MemoryMappedFile.OpenExisting("AIDA64_SensorValues"))
                    { 
                        using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                        {
                            var aida64Info = new StringBuilder();
                            aida64Info.Insert(0, "<AIDA64>"); 
                            for (var i =0;i<accessor.Capacity;i++)
                            {
                                aida64Info.Append(((char)accessor.ReadByte(i)).ToString()); 
                            } 
                            var aida64Str = aida64Info.ToString().Replace("\0", "");
                            aida64Str += "</AIDA64>";  
                            var aida64 = XmlSerializerHelper.Instance.Deserialize<AIDA64>(aida64Str);
                            //参数1对应COM端口，需到设备管理器里找CH340G对应的端口
                            using (SerialPort serialPort1 = new SerialPort("COM8", 115200, Parity.None, 8, StopBits.One))
                            {
                                serialPort1.Open();
                                var json = Newtonsoft.Json.JsonConvert.SerializeObject(aida64);
                                serialPort1.Write(json);
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine(json);
                                Console.WriteLine();
                                //var list = aida64.GetType()
                                //   .GetProperties().SelectMany(item =>
                                //   {
                                //       return item.GetType().GetProperties().Select(a =>
                                //       {
                                //           return new AIDA64Item
                                //           {
                                //               Id = a.MemberType.ToString(),
                                //               Value = a.GetValue(item).ToString()
                                //           };
                                //       });
                                //   }).ToList();
                                //list.ForEach(it =>
                                //{
                                //    try
                                //    {
                                //        serialPort1.Write("?" + it.Id + "=" + it.Value + "!");
                                //    }
                                //    catch (Exception err)
                                //    {
                                //    }
                                //});
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"has error :{ex.Message}");
                }
            }
        }
    }
}
