using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Transferizer.Configuration
{
    public class TransferElement: ConfigurationElement
    {
        [ConfigurationProperty("from", IsRequired = true, IsKey = true)]
        public string From
        {
            get { return (string)this["from"]; }
        }

        [ConfigurationProperty("to", IsRequired = true, IsKey = false)]
        public string To
        {
            get { return (string)this["to"]; }
        }

        [ConfigurationProperty("include_origen_subfolders", IsRequired = true, IsKey = false)]
        public bool IncludeSubFolders
        {
            get { return (bool)this["include_origen_subfolders"]; }
        }

        [ConfigurationProperty("search_pattern", IsRequired = true, IsKey = false)]
        public string SearchPattern
        {
            get { return (string)this["search_pattern"]; }
        }
    }

    [ConfigurationCollection(typeof(TransferElement), AddItemName = "Transfer", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class TransferElementCollection : ConfigurationElementCollection
    {
        List<TransferElement> _elements  = new List<TransferElement>();
        protected override ConfigurationElement CreateNewElement()
        {
            TransferElement newElement = new TransferElement();
            _elements.Add(newElement);
            return newElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TransferElement)(element)).From;
        }

        public new IEnumerator<TransferElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
    public class TransferSection : ConfigurationSection
    {
        [ConfigurationProperty("Transfers")]
        public TransferElementCollection Transfers
        {
            get { return ((TransferElementCollection)(this["Transfers"])); }
            set { base["Transfers"] = value; }
        }
    }
}
