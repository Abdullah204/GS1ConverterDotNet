using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Main.Services;
public class EPCConverter
{
    public Dictionary<string, int> gcpLengthMapping;

    
    public const string KEY_PREFIX = "P-";
    public EPCConverter()
    {
        var jsonArray = File.ReadAllText("data.json");
        var list = JsonSerializer.Deserialize<List<Prefix>>(jsonArray);
        gcpLengthMapping = new Dictionary<string, int>();
        foreach (Prefix x in list)
        {
            gcpLengthMapping.Add(x.prefix, x.gcpLength);
        }

    }
    //1-
    public void injectAdditionalGCPLengthMappings(int [] mappings)
    {

    }

    //2-
    public void injectTestGCPPrefixMappings(int [] mappings)
    {

    }
    ////3-
    //public string version(bool print = true)
    //{

    //}
    ////4-
    //public string epcToSscc(string epc)
    //{

    //}
    ////5-
    //public string[] epcToGln(string epc)
    //{

    //}
    ////6-
    //public string[] epcToSgtin(string epc)
    //{

    //}
    ////7-
    //public string sgtinToEpc(string sgtin, int gcpLength = null)
    //{

    //}
    ////8-
    //public string[] sgtinArrayToEpc(string gtin, string[] serials, int gcpLength = null)
    //{

    //}
    ////9-
    //public string sgtinBarcodeToEpc(string sgtinBarcode, int gcpLength = null)
    //{

    //}
    ////10-
    //public string gtinToEpc(string gtin, int gcpLength = null)
    //{

    //}
    ////11-
    public string ssccToEpc(string sscc, int? gcpLength = null)
    {
        if(gcpLength == null)
            gcpLength =getGcpLength(sscc.Substring(1));
        if (gcpLength == 0) 
            reportGcpLengthError(sscc, "SSCC");

        string extensionDigit = sscc.Substring(0,1);
        string gcp = sscc.Substring(1,(int)gcpLength);
        // containerRef   = substr(substr($sscc, $gcpLength + 1), 0, -1);
        string sub = sscc.Substring((int)gcpLength + 1);
        string containerRef = sub.Substring(0,sub.Length-1);
        

        return "urn:epc:id:sscc:"+gcp+"." + extensionDigit + containerRef;
    }
    ////12-
    //public string ssccBarcodeToEpc(string ssccBarcode, int gcpLength = null)
    //{

    //}
    ////13-
    public string glnToEpc(string gln, int? gcpLength = null)
    {
        
        
        if (gcpLength == null) 
            gcpLength = getGcpLength(gln);

        
        
        if (gcpLength == 0)
            gcpLength = 12;
        string gcp = gln.Substring(0,(int) gcpLength);
        string cur = gln.Substring((int)gcpLength);
        string locationRef = cur.Substring( 0, cur.Length-1);

        return "urn:epc:id:sgln:"+ gcp + "."+locationRef+".0";
    }
    ////14-
    //public string glnAndDocNumToBizTrx(string gln, string documentNumber)
    //{

    //}
    //15-
    public int getGcpLength(string gs1Element)
    {
        for (int i = 3; i < 12; i++)
        {
            string prefix = gs1Element.Substring(0, i - 1);
            if (gcpLengthMapping.ContainsKey(prefix))
                return gcpLengthMapping[prefix];
        }
        return 0;
    }
    //16-
    private string[] extractGTINFragments(string sgtin, int? gcpLength = null)
    {
        
        if(gcpLength == null)
            gcpLength = getGcpLength(sgtin.Substring(1));

        
        if (gcpLength == 0)
            gcpLength = 7;
        int itemRefLength = 12 - (int)gcpLength;

        string extensionDigit = sgtin.Substring(0,1);
        string gcp = sgtin.Substring(1, (int)gcpLength);
        string itemRef = sgtin.Substring((int)gcpLength + 1, itemRefLength);
        string serial = sgtin.Substring(14);
        string[] result = new string[] { extensionDigit, gcp,itemRef , serial };
        return result;
    }
    //17-
    private void reportGcpLengthError(string gs1Element, string elementType)
    {
        throw new Exception("ERROR: can’t find GCP length for" +elementType+" " + gs1Element);

    }
    //18-
    private void reportGenericError(string errorDescription)
    {
        throw new Exception(errorDescription);
    }
    //19-
    public bool hasValidCheckDigit(string gln)
    {
        int actualDigit = gln[gln.Length - 1] - '0';
        int calculated = calculateCheckDigit(gln.Substring(0, gln.Length - 1));
        
        return actualDigit == calculated;
    }


   
    //20-
    public string sgtinToEpc(string sgtin , int? gcpLength = null)
    {
        string[] fragments = extractGTINFragments(sgtin, gcpLength);

        if (fragments[3] == "")
            reportGenericError("ERROR: the SGTIN ({$sgtin}) is not valid, the serial part is empty");
        return "urn:epc:id:sgtin:" + fragments[1] +"."+ fragments[0] + fragments[2] +"."+fragments[3];

    }
    //21-
    
        //22-
    
        //23-
    public string gtinToEpc(string sgtin, int? gcpLength = null)
    {
        string[] fragments = extractGTINFragments(sgtin,gcpLength);
        
        return "urn:epc:id:sgtin:" + fragments[1] +"."+ fragments[0] + fragments[2] + ".*";
    }


    public static int calculateCheckDigit(string gs1Key)
    {
        string z = "a";
        
        if (gs1Key.Length == 12)
            gs1Key = "0$gs1Key";
        int even = 0;
        int odd = 0;
        for (int i = 0; i<gs1Key.Length; i++) {
            if (i % 2 == 0) {
                even += (gs1Key[i]-'0');
            } else {
                odd += (gs1Key[i]-'0');
            }
        }
        int total = (even * 3) + odd;
        return (total % 10 > 0) ? (10 - (total % 10)) : 0;
    }


    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        EPCConverter a = new EPCConverter();
        Console.WriteLine(a.gcpLengthMapping);
        Console.WriteLine();
    }

}


public class Prefix
{
    public string prefix { get; set; }
    public int gcpLength { get; set; }
}
