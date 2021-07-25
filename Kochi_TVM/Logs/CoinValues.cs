using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Kochi_TVM.Logs
{
    class CoinValues
    {
        private static ILog log = LogManager.GetLogger(typeof(CoinValues).Name);
        private static XmlDocument _doc;
        private static string folderName = AppDomain.CurrentDomain.BaseDirectory + "CoinValues";
        private static string fileName = "CoinValues.xml";
        private static object salesLimitLock = new object();
        private static object initialLock = new object();
        private static void CheckFolders()
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }
        public static void createFile()
        {
            lock (initialLock)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    XmlElement header = doc.CreateElement("CoinValues");

                    {
                        XmlAttribute Cassette1Value = doc.CreateAttribute("Coin1");
                        Cassette1Value.InnerText = "1";
                        header.Attributes.Append(Cassette1Value);
                    }

                    {
                        XmlAttribute Cassette1Value = doc.CreateAttribute("Coin2");
                        Cassette1Value.InnerText = "2";
                        header.Attributes.Append(Cassette1Value);
                    }

                    {
                        XmlAttribute Cassette1Value = doc.CreateAttribute("Coin3");
                        Cassette1Value.InnerText = "5";
                        header.Attributes.Append(Cassette1Value);
                    }

                    doc.AppendChild(header);

                    //create doc content into file
                    CheckFolders();
                    XmlWriter writer = XmlWriter.Create(folderName + "\\" + fileName);
                    doc.WriteTo(writer);
                    writer.Close();

                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> createFile() : " + ex.ToString());
                }
            }
        }

        private static bool loadFile()
        {
            lock (salesLimitLock)
            {
                try
                {
                    _doc = new XmlDocument();

                    if (File.Exists(folderName + "\\" + fileName) == false)
                        createFile();

                    _doc.Load(folderName + "\\" + fileName);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static void DeleteFile()
        {
            try
            {
                File.Delete(folderName + "\\" + fileName);
            }
            catch (Exception ex)
            {
                log.Error("Error CoinValues -> DeleteFile() : " + ex.ToString());
            }
        }

        private static void saveDocToFile()
        {
            try
            {
                File.WriteAllText(folderName + "\\" + fileName, _doc.OuterXml);
            }
            catch (Exception ex1)
            {
                log.Error("Error CoinValues -> saveDocToFile() 1 : " + ex1.ToString());
                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(1000);
                        File.WriteAllText(folderName + "\\" + fileName, _doc.OuterXml);
                        if (loadFile())
                            break;
                    }
                }
                catch (Exception ex2)
                {

                }
            }
        }
        public static void setCoin1(string val)
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    root.Attributes["Coin1"].Value = val.ToString();
                    checkAndSave(_doc, val.ToString(), "Coin1");
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> setCoin1() : " + ex.ToString());
                }
            }
        }
        public static string getCoin1()
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    if (!root.HasAttribute("Coin1"))
                    {
                        root.SetAttribute("Coin1", "1");
                        _doc.Save(fileName);
                        _doc = new XmlDocument();
                        _doc.Load(fileName);
                    }

                    string temp = root.GetAttribute("Coin1");
                    return temp;
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> getCoin1() : " + ex.ToString());
                    return "";
                }
            }
        }
        public static void setCoin2(string val)
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    root.Attributes["Coin2"].Value = val.ToString();
                    checkAndSave(_doc, val.ToString(), "Coin2");
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> setCoin2() : " + ex.ToString());
                }
            }
        }
        public static string getCoin2()
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    if (!root.HasAttribute("Coin2"))
                    {
                        root.SetAttribute("Coin2", "2");
                        _doc.Save(fileName);
                        _doc = new XmlDocument();
                        _doc.Load(fileName);
                    }

                    string temp = root.GetAttribute("Coin2");
                    return temp;
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> getCoin2() : " + ex.ToString());
                    return "";
                }
            }
        }
        public static void setCoin3(string val)
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    root.Attributes["Coin3"].Value = val.ToString();
                    checkAndSave(_doc, val.ToString(), "Coin3");
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> setCoin3() : " + ex.ToString());
                }
            }
        }
        public static string getCoin3()
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    if (!root.HasAttribute("Coin3"))
                    {
                        root.SetAttribute("Coin3", "5");
                        _doc.Save(fileName);
                        _doc = new XmlDocument();
                        _doc.Load(fileName);
                    }

                    string temp = root.GetAttribute("Coin3");
                    return temp;
                }
                catch (Exception ex)
                {
                    log.Error("Error CoinValues -> getCoin3() : " + ex.ToString());
                    return "";
                }
            }
        }
        private static void checkAndSave(XmlDocument _doc, string value, string valueName)
        {
            try
            {
                //_doc.Save(folderName + "\\" + fileName);
                saveDocToFile();

                if (!loadFile())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadFile();

                        XmlElement root = _doc.DocumentElement;

                        root.Attributes[valueName].Value = value;
                        //_doc.Save(folderName + "\\" + fileName);
                        saveDocToFile();
                        if (loadFile())
                            break;
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
