using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;

namespace Main.Services;
public class EPCConverter
{
    public IDictionary<string, int> gcpLengthMapping;

    
    public const string KEY_PREFIX = "P-";
    public EPCConverter()
    {
        var jsonArray = File.ReadAllText("data.json");
        var list = JsonSerializer.Deserialize<IEnumerable<KeyValuePair<string, int>>>(jsonArray);
        var gcpLengthMapping = list.ToDictionary(x => x.Key, x => x.Value);
    }
    //1-
    public void injectAdditionalGCPLengthMappings(int [] mappings)
    {

    }

    //2-
    public void injectTestGCPPrefixMappings(int [] mappings)
    {

    }
    //3-
    public string version(bool print = true)
    {

    }
    //4-
    public string epcToSscc(string epc)
    {

    }
    //5-
    public string[] epcToGln(string epc)
    {

    }
    //6-
    public string[] epcToSgtin(string epc)
    {

    }
    //7-
    public string sgtinToEpc(string sgtin, int gcpLength = null)
    {

    }
    //8-
    public string[] sgtinArrayToEpc(string gtin, string[] serials, int gcpLength = null)
    {

    }
    //9-
    public string sgtinBarcodeToEpc(string sgtinBarcode, int gcpLength = null)
    {

    }
    //10-
    public string gtinToEpc(string gtin, int gcpLength = null)
    {

    }
    //11-
    public string ssccToEpc(string sscc, int gcpLength = null)
    {

    }
    //12-
    public string ssccBarcodeToEpc(string ssccBarcode, int gcpLength = null)
    {

    }
    //13-
    public string glnToEpc(string gln, int gcpLength = null)
    {

    }
    //14-
    public string glnAndDocNumToBizTrx(string gln, string documentNumber)
    {

    }
    //15-
    public int getGcpLength(string gs1Element)
    {

    }
    //16-
    private string[] extractGTINFragments(string sgtin, int gcpLength)
    {

    }
    //17-
    private void reportGcpLengthError(string gs1Element, string elementType)
    {

    }
    //18-
    private void reportGenericError(string errorDescription)
    {

    }
    //19-
    public bool hasValidCheckDigit(string gln)
    {

    }


   
    //20-
    public string sgtinToEpc(string sgtin)
    {
        return "";
    }
    //21-
    public string glnToEpc(string sgtin)
    {
        return "";
    }
        //22-
    public string ssccToEpc(string sgtin)
    {
        return "";
    }
        //23-
    public string gtinToEpc(string sgtin)
    {
        return "";
    }       


    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        EPCConverter a = new EPCConverter();
        Console.WriteLine(a.gcpLengthMapping);
        Console.WriteLine();
    }

}


public class PrefixFormat
{
    string prefix;
    int gcpLength;
}
