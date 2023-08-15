using Xunit;
using Main.Services;
using System;
using System.Collections.Generic;

namespace Main.Tests;





public class UnitTest1 
{

    private EPCConverter epcConverter;

    public UnitTest1()
    {
        epcConverter = new EPCConverter();
    }
   

    [Theory]
    [MemberData(nameof(sgtinGs1ElementToEpcDataProvider))]
    public void testSgtinToEpc(string sgtin, string barcode, string expectedEpcUri)
    {
        string actualEpcUri = this.epcConverter.sgtinToEpc(sgtin);
        Assert.Equal(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider gtinGs1ElementToEpcDataProvider
     * @param string gtin
     * @param string barcode not used in this test
     * @param string expectedEpcUri
     */
    [Theory]
    [MemberData(nameof(gtinGs1ElementToEpcDataProvider))]
    public void testGtinToEpc(string gtin, string barcode, string expectedEpcUri)
    {
       string actualEpcUri = this.epcConverter.gtinToEpc(gtin);
        Assert.Equal(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider ssccGs1ElementToEpcDataProvider
     * @param string sscc
     * @param string barcode
     * @param string expectedEpcUri
     */
    [Theory]
    [MemberData(nameof(ssccGs1ElementToEpcDataProvider))]
    public void testSsccToEpc(string sscc, string barcode, string expectedEpcUri)
    {
       string actualEpcUri = this.epcConverter.ssccToEpc(sscc);
        Assert.Equal(expectedEpcUri,actualEpcUri);
    }

    /**
     * @dataProvider glnGs1ElementToEpcDataProvider
     * @param string gln
     * @param string extension
     * @param string barcode
     * @param string expectedEpcUri
     */
    [Theory]
    [MemberData(nameof(glnGs1ElementToEpcDataProvider))]
    public void testGlnToEpc(string gln, string extension, string barcode, string expectedEpcUri)
    {
       string actualEpcUri = this.epcConverter.glnToEpc(gln);
        Assert.Equal(expectedEpcUri,actualEpcUri);
    }


    

    public static IEnumerable<object[]> sgtinGs1ElementToEpcDataProvider =>
        new List<object[]>
        {
            new object[] { "06281080008450OUEY0LB818UM", "010628108000845021OUEY0LB818UM", "urn:epc:id:sgtin:6281080.000845.OUEY0LB818UM"},
            new object[] { "06281080008450OUEY0LB81XYZ", "010628108000845021OUEY0LB81XYZ", "urn:epc:id:sgtin:6281080.000845.OUEY0LB81XYZ"},
            new object[] { "06281080010088Ser1ex", "010628108001008821Ser1ex", "urn:epc:id:sgtin:6281080.001008.Ser1ex" },
            
        };

    public static IEnumerable<object[]> gtinGs1ElementToEpcDataProvider =>
        new List<object[]>
        {
            new object[] { "06281080008450", "0106281080008450", "urn:epc:id:sgtin:6281080.000845.*"},
            new object[] { "06281080008450", "0106281080008450", "urn:epc:id:sgtin:6281080.000845.*"},
            new object[] { "06281080010088", "0106281080010088", "urn:epc:id:sgtin:6281080.001008.*" },

        };

    
    public static IEnumerable<object[]> ssccGs1ElementToEpcDataProvider =>
        new List<object[]>
        {
            new object[] { "062810800000808289", "00062810800000808289", "urn:epc:id:sscc:6281080.0000080828"},
            new object[] { "806141411234567896", "00806141411234567896", "urn:epc:id:sscc:0614141.8123456789"},

        };



    public static IEnumerable<object[]> glnGs1ElementToEpcDataProvider =>
        new List<object[]>
        {
            new object[] { "0614141123452", "xxxxx", "xxxxx", "urn:epc:id:sgln:0614141.12345.0"},
            new object[] { "0857624006082", "xxxxx", "xxxxx", "urn:epc:id:sgln:0857624006.08.0"},

        };
    



}