//#define MULTIPLEBBOXONE

using System;
using System.Linq;
using BBoxAPI;

namespace ConsoleApp1
{
    public class Class1 : MarshalByRefObject
    {
        public void callConstructor()
        {
            BBoxOneAPI b = new BBoxOneAPI();
        }
    }
    enum TRMODE
    {
        STANDBY = 0,
        TX = 1,
        RX = 2,
        SLEEP = 3,
    };
    class Program
    {
        static void Main(string[] args)
        {
            BBoxOneAPI b = new BBoxOneAPI();
            string[] dev_info = b.ScanningDevice();

            for (int i = 0; i < dev_info.Count(); i++)
            {
                string p = dev_info[i];
                if (p != "")
                    Console.WriteLine("device info from API side : " + p);
            }

            Console.WriteLine("dll location : ", b.GetCurrentPath());

            /* It will send the init command to BBox. */
            /* first BBoxOne */
            /*****************************
                dev_type = 0, IDT device
                dev_type = 1, KDDI device
                dev_type = 2, NGK device
            *****************************/
            string s_info_1 = b.Init("B19312300-24", 2, 0);
            Console.WriteLine(s_info_1);
            Console.WriteLine("Init first one");

            //double dev_1_spacing = b.selectAntenna(Device.AntennaType.FOURBYFOUR, 0);
            //Console.WriteLine("1st device antenna spacing : " + dev_1_spacing);
            b.SwitchTxRxMode((int)TRMODE.TX, 0);

#if MULTIPLEBBOXONE
            /* second BBoxOne */
            String s_info_2 = b.Init("B19178000-24", 1);
            Console.WriteLine(s_info_2);
            Console.WriteLine("Init second one");
            double dev_2_spacing = b.selectAntenna(Device.AntennaType.FOURBYFOUR, 1);
            Console.WriteLine("2nd device antenna spacing : " + dev_2_spacing);
            b.SwitchTxRxMode((int)TRMODE.TX, 1);
#endif 

            long start_milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
          
            int deg = 0;
            int dir = -1;
            for(int count = 0; count < 10; count++)
            {
                try
                {
                    //setRxGainPhaseStep(int icth/*1-16*/, int PH1, int PH2, int PH3, int PH4, int GA1, int GA2, int GA3, int GA4, int GAC, int idx)
                    //b.setRxGainPhaseStep(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    b.setBeamXY(100.0, 0, 0, 0);
#if IDT
                    s_info_1 = b.setBeamX(5/*dB*/, deg/*degree*/, 0);
                    s_info_1 = b.setBeamY(5/*dB*/, deg/*degree*/, 0);
                    s_info_1 = b.setBeamXY(5/*dB*/, deg/*degree*/, deg/*degree*/, 0);
                    Console.WriteLine("First Device control : " + s_info_1);
#if MULTIPLEBBOXONE
          
                    s_info_2 = b.setBeamX(0, deg, 1);
                    s_info_2 = b.setBeamY(0, deg, 1);
                    s_info_2 = b.setBeamXY(0, deg, deg, 1);
                    Console.WriteLine("Second Device control : " + s_info_2);
#endif
      
                    if (deg <= -25)
                        dir = 1;
                    else if (deg >= 25)
                        dir = -1;
                    deg += dir;
#endif
                }
                catch (Exception e)
                {
                    Console.WriteLine("EEROR : " + e.ToString());
                    break;
                }
            }
            long end_milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Console.WriteLine("Time : " + (end_milliseconds - start_milliseconds) + " ms");
        
            Console.Read();
        }
    }
}
