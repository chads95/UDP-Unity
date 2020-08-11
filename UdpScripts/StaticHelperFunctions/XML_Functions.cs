using System;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Udp
{
    [XmlRootAttribute(ElementName = "DataCollection", IsNullable = true)]

    public class Pages
    {
        [XmlArray("Page")]
        public DataItem[] DataItems;
    }

    public class DataItem
    {
        [XmlElement]
        public string Label;
        public byte Item;
        public byte SubSystem;
        public string Type;
        public string Access;
    } 
    class XMLReader
    {
        public List<RC_Item> RC_Items = new List<RC_Item>();

        public List<rCHARM_MsgHandler> rCHARM_Msg;

        public XMLReader(List<rCHARM_MsgHandler> msg_handlers )
        {
            rCHARM_Msg = msg_handlers;
        } 

        public void ReadXML(string filename, cRC_UDP_IO cRC_UDP_IO)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(Pages));

            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);

            serializer.UnknownAttribute += new

            XmlAttributeEventHandler(serializer_UnknownAttribute);

            FileStream fs = new FileStream(filename, FileMode.Open);
            Pages po;


            po = (Pages)serializer.Deserialize(fs);

            DataItem[] items = po.DataItems;

            foreach(DataItem oi in items)
            {
                // create a new RC ITem
                // add it to list

                RC_Item rC_Item = new RC_Item();

                Debug.Log("\t" +
                    oi.Label + "\t" +
                    oi.SubSystem + "\t" +
                    oi.Item + "\t" +
                    oi.Type + "\t" +
                    oi.Access);

                rC_Item.sLabe = oi.Label;
                rC_Item.nSubsystem = oi.SubSystem;
                rC_Item.nItem = oi.Item;
                if(oi.Type == "Fixed")
                {
                    rC_Item.nType = RC_MsgDefinesCHARM.RC_DataTypes.DT_FIXED;
                }

                if(oi.Access == "RO")
                {
                    rC_Item.nAccess = RC_MsgDefns.AccessType.DIA_READ_ONLY;
                }

                RC_Items.Add(rC_Item);

                AddPage(cRC_UDP_IO);
            }
        }

        public void AddPage (cRC_UDP_IO cRC_UDP_IO)
        {

            cRC_UDP_IO.m_pageList.Add(RC_Items);
            cRC_UDP_IO.CreateMapList();
        }


        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogError("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute
   (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }
    }
    
}
