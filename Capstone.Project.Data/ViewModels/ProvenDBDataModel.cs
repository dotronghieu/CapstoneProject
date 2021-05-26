using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class ProvenDBDataModel
    {
        public string _id { get; set; }
        public List<Version> versions { get; set; }
    }

    public class Document
    {
        public string transactionId { get; set; }
        public string prevOwner { get; set; }
        public string ownerID { get; set; }
        public string photoId { get; set; }
        public string photoHash { get; set; }
        public bool isTransaction { get; set; }
        public string amount { get; set; }
        public DateTime createDate { get; set; }
    }

    public class Version
    {
        public int minVersion { get; set; }
        public string maxVersion { get; set; }
        public string status { get; set; }
        public string started { get; set; }
        public string ended { get; set; }
        public Document document { get; set; }
    }

}
