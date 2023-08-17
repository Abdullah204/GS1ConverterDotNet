using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
    public void injectAdditionalGCPLengthMappings(Dictionary<string, int> mappings)
    {
       Dictionary<string, int> injected = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> mapping in mappings) {
            injected[mapping.Key] = mapping.Value;
        }
        gcpLengthMapping.Concat(injected);

    }

    public void injectTestGCPPrefixMappings(Dictionary<string, int> mappings)
    {
        injectAdditionalGCPLengthMappings(mappings);
    }
    
    public string version(bool print = true)
    {
        string version = "EPCIS SDK v0.0.1";
        if (print) Console.WriteLine(version);
        return version;
    }
    public string epcToSscc(string epc)
    {
        epc = epc.Trim();
        if ("urn:epc:id:sgtin:" == epc.Substring( 0, 17)) {
            Dictionary<string, string> sgtin = epcToSgtin(epc);
            return sgtin["gtin"]+"."+sgtin["serial"];
        }
        if ("urn:epc:id:sscc:" != epc.Substring(0, 16))
            throw new Exception("Invalid SSCC EPC");
        string[] epcArray = epc.Split(':');
        string lastElement = epcArray.Last();
        string gcp = lastElement.Split('.')[0];
        string container = lastElement.Split('.')[1];
        string extensionDigit = container.Substring(0, 1);
        container = container.Substring(1);
        int checkDigit = calculateCheckDigit(extensionDigit + gcp + container);

        return extensionDigit +gcp+container +checkDigit;
    }
    public Dictionary<string, string> epcToGln(string epc)
    {
        string ext,gln;
        epc = epc.Trim();
        Match match = Regex.Match(epc, @"^(?<gln>\d{13})\.(?<ext>\w*)$");
        
        if (Regex.IsMatch(epc, @"^\d{13}$")) {
            gln = epc;
            ext = "0";
        }
        else if (match.Success) {
            gln = match.Groups["gln"].Value;
            ext = match.Groups["ext"].Value;
        }
        else if ("urn:epc:id:sgln:" == epc.Substring( 0, 16)) {
            string[] epcArray = epc.Split(':');
            string lastElement = epcArray.Last();
            string gcp = lastElement.Split('.')[0];
            string location = lastElement.Split('.')[1];
            ext = lastElement.Split('.')[2];
            gln = gcp +location + calculateCheckDigit(gcp + location);
        }else {
            throw new Exception("Invalid SGLN EPC");
        }
        return new Dictionary<string, string>
        {
            { "414", gln },
            { "254", ext },
            { "gln", gln },
            { "ext", ext }
        };
        
    }
    public Dictionary<string, string> epcToSgtin(string epc)
    {
        epc = epc.Trim();
        if ("urn:epc:id:sgtin:" != epc.Substring(0, 17))
            throw new Exception("Invalid SGTIN EPC");

        string[] epcArray = epc.Split(':');
        string lastElement = epcArray.Last();
        string gcp = lastElement.Split('.')[0];
        string gtin = lastElement.Split('.')[1];
        string serial = lastElement.Split('.')[2];
        
        string extensionDigit = gtin.Substring(0, 1);
        gtin = gtin.Substring(1);
        gtin = extensionDigit+gcp+gtin+calculateCheckDigit(extensionDigit +gcp +gtin);

        
        return new Dictionary<string, string>
        {
            {"01" , gtin},
            {"21" , serial},
            {"gtin" , gtin},
            {"serial" , serial}
        };
    }
    
    public Dictionary<string, string>  sgtinArrayToEpc(string gtin, string[] serials, int? gcpLength = null)
    {
        string[] fragments = extractGTINFragments(gtin, gcpLength);

        Dictionary<string, string> sgtins = new Dictionary<string, string>();
        String value = "urn:epc:id:sgtin:" + fragments[1] +"."+fragments[0]+fragments[2]+".";
        foreach (string serial in serials)
        {
            sgtins.Add(serial, value + serial);
        }
        
        return sgtins;
    }
    public string sgtinBarcodeToEpc(string sgtinBarcode, int? gcpLength = null)
    {
        string sgtin = Regex.Replace(sgtinBarcode,@"^(?<gtin>\d{14})\.?21(\w*)$",@"\$1\$2");
        return sgtinToEpc(sgtin, gcpLength);
    }
    
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
    public string ssccBarcodeToEpc(string ssccBarcode, int? gcpLength = null)
    {
        if (ssccBarcode.Substring(0, 2) == '00'){
            string sscc = substr(ssccBarcode, 2);
            return ssccToEpc(sscc, gcpLength);
        }

        reportGenericError("ERROR : the SSCC barcode ({$ssccBarcode}) is not valid !");
    }
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
    public string glnAndDocNumToBizTrx(string gln, string documentNumber)
    {
        return "urn:epcglobal:cbv:bt:"+gln+":"+documentNumber;
    }
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
    private void reportGcpLengthError(string gs1Element, string elementType)
    {
        throw new Exception("ERROR: can’t find GCP length for" +elementType+" " + gs1Element);

    }
    private void reportGenericError(string errorDescription)
    {
        throw new Exception(errorDescription);
    }
    public bool hasValidCheckDigit(string gln)
    {
        int actualDigit = gln[gln.Length - 1] - '0';
        int calculated = calculateCheckDigit(gln.Substring(0, gln.Length - 1));
        
        return actualDigit == calculated;
    }


   
    public string sgtinToEpc(string sgtin , int? gcpLength = null)
    {
        string[] fragments = extractGTINFragments(sgtin, gcpLength);

        if (fragments[3] == "")
            reportGenericError("ERROR: the SGTIN ({$sgtin}) is not valid, the serial part is empty");
        return "urn:epc:id:sgtin:" + fragments[1] +"."+ fragments[0] + fragments[2] +"."+fragments[3];

    }
    /
        
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
