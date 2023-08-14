using Xunit;
using Main.Services;
namespace Main.Tests;

public class UnitTest1
{


     private EPCConverter epcConverter;

    [SetUp]
    public void SetUp()
    {
        // common Arrange
        epcConverter = new EPCConverter();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
    {
        var main = new Class1();
        bool result = main.IsPrime(1);

        Assert.False(result, "${value} should not be prime");
    }


    public void testSgtinToEpc(String sgtin, String barcode, String expectedEpcUri)
    {
        String actualEpcUri = this.epcConverter.sgtinToEpc(sgtin);
        Assert.Equals(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider gtinGs1ElementToEpcDataProvider
     * @param String gtin
     * @param String barcode not used in this test
     * @param String expectedEpcUri
     */
    public void testGtinToEpc(String gtin, String barcode, String expectedEpcUri)
    {
       String actualEpcUri = this.epcConverter.gtinToEpc(gtin);
        Assert.Equals(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider ssccGs1ElementToEpcDataProvider
     * @param String sscc
     * @param String barcode
     * @param String expectedEpcUri
     */
    public void testSsccToEpc(String sscc, String barcode, String expectedEpcUri)
    {
       String actualEpcUri = this.epcConverter.ssccToEpc(sscc);
        Assert.Equals(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider glnGs1ElementToEpcDataProvider
     * @param String gln
     * @param String extension
     * @param String barcode
     * @param String expectedEpcUri
     */
    public void testGlnToEpc(String gln, String extension, String barcode, String expectedEpcUri)
    {
       String actualEpcUri = this.epcConverter.glnToEpc(gln);
        Assert.Equals(expectedEpcUri,actualEpcUri);
    }


    public static List<Dictionary<string, string>> sgtinGs1ElementToEpcDataProvider()
    {
        return new List<Dictionary<string, string>>
        {
            {
                "sgtin"         => "06281080008450OUEY0LB818UM",
                "elementString" => "010628108000845021OUEY0LB818UM",
                "epcUri"        => "urn:epc:id:sgtin:6281080.000845.OUEY0LB818UM"
            },
            {
                "sgtin"         => "06281080008450OUEY0LB81XYZ",
                "elementString" => "010628108000845021OUEY0LB81XYZ",
                "epcUri"        => "urn:epc:id:sgtin:6281080.000845.OUEY0LB81XYZ"
            },
            {
                "sgtin"         => "06281080010088Ser1ex",
                "elementString" => "010628108001008821Ser1ex",
                "epcUri"        => "urn:epc:id:sgtin:6281080.001008.Ser1ex"
            },

        };
    }

    public static List<Dictionary<string, string>> gtinGs1ElementToEpcDataProvider()
    {
        return new List<Dictionary<string, string>>
        {
            {
                "gtin"          => "06281080008450",
                "elementString" => "0106281080008450",
                "epcUri"        => "urn:epc:id:sgtin:6281080.000845.*"
            },
            {
                "gtin"          => "06281080008450",
                "elementString" => "0106281080008450",
                "epcUri"        => "urn:epc:id:sgtin:6281080.000845.*"
            },
            {
                "gtin"          => "06281080010088",
                "elementString" => "0106281080010088",
                "epcUri"        => "urn:epc:id:sgtin:6281080.001008.*"
            },

        };
    }


    public static List<Dictionary<string, string>> ssccGs1ElementToEpcDataProvider()
    {
        return new List<Dictionary<string, string>>
        {
            {
                "sscc"          => "062810800000808289",
                "elementString" => "00062810800000808289",
                "epcUri"        => "urn:epc:id:sscc:6281080.0000080828"
            },
            {
                "sscc"          => "806141411234567896",
                "elementString" => "00806141411234567896",
                "epcUri"        => "urn:epc:id:sscc:0614141.8123456789"
            },

        };
    }

    public static List<Dictionary<string, string>> glnGs1ElementToEpcDataProvider()
    {
        return new List<Dictionary<string, string>>
        {
            {
                "gln"           => "0614141123452",
                "glnExtension"  => "xxxxx",
                "elementString" => "xxxxx",
                "epcUri"        => "urn:epc:id:sgln:0614141.12345.0"
            },
            {
                "gln"           => "0857624006082",
                "glnExtension"  => "xxxxx",
                "elementString" => "xxxxx",
                "epcUri"        => "urn:epc:id:sgln:0857624006.08.0"
            },

        };
    }



}