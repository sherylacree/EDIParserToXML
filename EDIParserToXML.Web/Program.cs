using EDIParserToXML.Web;
using EDIParserToXML.Web.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;
using System.Security.Cryptography.X509Certificates;


namespace EDIParserToXML 
    {
    public class PurchaseOrder850
        {
        public string PONum;
        public DateTime PODate;
        public string PODateText;
        public string POType;
        public string VendorNumber;
        public string BuyerName;
        public string BuyerTelephone;
        public List<PurchaseOrder850LineItem> LineItems;
        }

    public class PurchaseOrder850LineItem {
        public string lineitem;
        public int quantity;
        public string uom;
        public decimal price;
        public string basisOfUnitPrice;
        public string catalogNumber;
        public string description;
        public string dateRequiredText;
        public DateTime dateRequired;
    }



    class Program {
        static void Main(string[] args) {
            string ediFilename = @"\\tql.com\shares\RedirectedFolders\Sheryl.Acree\My Documents\Udemy EDI Training Documents\Sample_850_01_Orig.edi.txt";
            //insert any file name above
            string ediFileContents = File.ReadAllText(ediFilename);

            string currentRef01 = " ";
            string currentPer01 = " ";
            //temporary variables for parsing
            Console.WriteLine(ediFileContents);

            string elementSeperator = ediFileContents.Substring(103, 1);
            //add the position # of the file's element seperator here, this one is an asterisk, for C# it will be position # minus 1            
            string lineSeperator = ediFileContents.Substring(105, 1);
            //add the position # of the file's line/segment seperator here, this one is a tilde,  for C# it will be position # minus 1  

            ediFileContents = ediFileContents.Replace("\r", "").Replace("\n", "");//replace the carriage return and line feeds with nothing


            Console.WriteLine("elementSeperator = " + elementSeperator);
            Console.WriteLine("lineSeperator = " + lineSeperator);

            PurchaseOrder850 po850 = new PurchaseOrder850();
            PurchaseOrder850LineItem lineItem = new PurchaseOrder850LineItem();
            po850.LineItems = new List<PurchaseOrder850LineItem>();


            string[] lines = ediFileContents.Split(char.Parse(lineSeperator));
            Console.WriteLine("Number of lines = " + lines.Length);


            foreach (string line in lines) {

                Console.WriteLine(line);
                string[] elements = line.Split(char.Parse(elementSeperator));
                int loopCounter = 0;
                string segment = "";
                string elNum = "";
                string elName = "";

                foreach (string el in elements) {
                    if (loopCounter == 0) {
                        segment = el;
                    } else {
                        elNum = loopCounter.ToString("D2");
                        elName = segment + elNum;
                        Console.WriteLine(elName + " = " + el);

                        switch (elName) {
                            case "BEG03":
                                po850.PONum = el;
                                break;
                            case "BEG05":
                                po850.PODateText = el;
                                po850.PODate = DateTime.ParseExact(
                                    el, "yyyyMMdd", CultureInfo.InvariantCulture);
                                break;
                            case "BEG02":
                                po850.POType = el;
                                break;
                            case "REF01":
                                currentRef01 = el;
                                break;
                            case "REF02":
                                if (currentRef01 == "VR") {
                                    po850.VendorNumber = el;
                                }
                                break;
                            case "PER01":
                                currentPer01 = el;
                                break;
                            case "PER02":
                                if (currentPer01 == "BD") {
                                    po850.BuyerName = el;
                                }
                                break;
                            case "PER04":
                                if (currentPer01 == "BD") {
                                    po850.BuyerTelephone = el;
                                }
                                break;
                            case "PO101":
                                lineItem.lineitem = el;
                                break;
                            case "PO102":
                                lineItem.quantity = Int32.Parse(el);
                                break;
                            case "PO103":
                                lineItem.uom = el;
                                break;
                            case "PO104":
                                lineItem.price = Decimal.Parse(el);
                                break;
                            case "PO105":
                                lineItem.basisOfUnitPrice = el;
                                break;
                            case "PO107":
                                lineItem.catalogNumber = el;
                                break;
                            case "PID05":
                                lineItem.description = el;
                                break;
                            case "DTM02":
                                lineItem.dateRequiredText = el;
                                lineItem.dateRequired = DateTime.ParseExact(el, "yyyyMMdd", CultureInfo.InvariantCulture);
                                po850.LineItems.Add(lineItem);
                                lineItem = new PurchaseOrder850LineItem();
                                break;
                        }

                    }
                    loopCounter++;
                }

                

          


                Console.WriteLine("*** PONum = " + po850.PONum + " PO Date = " + po850.PODateText + " PO Type = " + po850.POType);
                Console.WriteLine("*** Vendor = " + po850.VendorNumber + " Buyer Name : " + po850.BuyerName + " Buyer Telephone : " + po850.BuyerTelephone);
                foreach (PurchaseOrder850LineItem item in po850.LineItems) {
                    Console.WriteLine("***** " + item.lineitem + " " +
                        " qty = " + item.quantity + " " +
                        " uom = " + item.uom + " " +
                        " price = " + item.price + " " +
                        " basis = " + item.basisOfUnitPrice + " " +
                        " description = " + item.description + " " +
                        " reqDate = " + item.dateRequired
                        );
                }



            }

            string outputFilename = @"c:\EDIClass\EDITToXML\" + Path.GetFileName(ediFilename);
            //add ponumber from BEG03 to filename
            outputFilename = outputFilename.Replace(".edi", "_Serialized_" + po850.PONum + ".xml");

            XmlSerializer xs1 = new XmlSerializer(typeof(PurchaseOrder850));
            StreamWriter sw1 = new StreamWriter(outputFilename);
            xs1.Serialize(sw1, po850);
            sw1.Close();

            //Example of How to Deserialize back into an object
           /* XmlSerializer xs2 = new XmlSerializer(typeof(PurchaseOrder850));
            StreamReader sr2 = new StreamReader(@"c:DeserializeYourObject.xml");
            // deserialize and cast back to proper class
            PurchaseOrder850 po850Deserialized = (PurchaseOrder850)xs2.Deserialize(sr2);
            sr2.Close();
            Console.WriteLine("Deserialize2 completed");*/


            Console.WriteLine("\n\n Press enter to end;");//to keep the data on screen
            Console.ReadLine();
        }

    }

}

