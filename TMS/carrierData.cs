//  NAME    :   carrierData
//  PURPOSE :   This class models the attributes and behaviours for the carrierData. There are properties
//              included for CarrierName,frate,lrate and reefCharge

using System.Collections.Generic;

namespace TMS
{
    public class carrierData
    {
        public string CarrierName { get; set; }

        public double frate { get; set; }

        public double lrate { get; set; }

        public double reefCharge {get;set;}


        public static List<carrierData> CarrierTable()
        {
            List<carrierData> carrierRawData = new List<carrierData>();

            carrierRawData.Add(new carrierData() { CarrierName = "Planet Express", frate = 5.21, lrate = 0.3621, reefCharge = 0.08 });
            carrierRawData.Add(new carrierData() { CarrierName = "Schooner's", frate = 5.05, lrate = 0.3434,reefCharge = 0.07 });
            carrierRawData.Add(new carrierData() { CarrierName = "Tillman Transport", frate = 5.11, lrate = 0.3012, reefCharge = 0.09 });
            carrierRawData.Add(new carrierData() { CarrierName = "We Haul", frate = 5.2, lrate = 0, reefCharge = 0.065 });
            return carrierRawData;
        }
    }
}
