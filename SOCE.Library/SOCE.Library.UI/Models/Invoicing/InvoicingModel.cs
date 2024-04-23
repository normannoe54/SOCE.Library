using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class InvoicingModel : PropertyChangedBase
    {
        private int _id { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private int _invoiceId { get; set; }
        public int InvoiceId
        {
            get
            {
                return _invoiceId;
            }
            set
            {
                _invoiceId = value;
                RaisePropertyChanged(nameof(InvoiceId));
            }
        }

        private DateTime _date { get; set; }
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        private double _previousSpent { get; set; }
        public double PreviousSpent
        {
            get
            {
                return _previousSpent;
            }
            set
            {
                _previousSpent = value;
                RaisePropertyChanged(nameof(PreviousSpent));
            }
        }

        private double _amountDue { get; set; }
        public double AmountDue
        {
            get
            {
                return _amountDue;
            }
            set
            {
                _amountDue = value;
                RaisePropertyChanged(nameof(AmountDue));
            }
        }

        private double _toDate { get; set; }
        public double ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                RaisePropertyChanged(nameof(ToDate));
            }
        }

        private string _clientName { get; set; }
        public string ClientName
        {
            get
            {
                return _clientName;
            }
            set
            {
                _clientName = value;
                RaisePropertyChanged(nameof(ClientName));
            }
        }

        private string _clientCompany { get; set; }
        public string ClientCompany
        {
            get
            {
                return _clientCompany;
            }
            set
            {
                _clientCompany = value;
                RaisePropertyChanged(nameof(ClientCompany));
            }
        }

        private string _clientAddress { get; set; }
        public string ClientAddress
        {
            get
            {
                return _clientAddress;
            }
            set
            {
                _clientAddress = value;
                RaisePropertyChanged(nameof(ClientAddress));
            }
        }

        private string _clientCity { get; set; }
        public string ClientCity
        {
            get
            {
                return _clientCity;
            }
            set
            {
                _clientCity = value;
                RaisePropertyChanged(nameof(ClientCity));
            }
        }

        private int _employeeSignedId { get; set; }
        public int EmployeeSignedId
        {
            get
            {
                return _employeeSignedId;
            }
            set
            {
                _employeeSignedId = value;
                RaisePropertyChanged(nameof(EmployeeSignedId));
            }
        }

        private ObservableCollection<InvoicingRows> _rows { get; set; }
        public ObservableCollection<InvoicingRows> Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
                RaisePropertyChanged(nameof(Rows));
            }
        }

        public List<int> TimesheetIds { get; set; }

        public InvoicingModel()
        {

        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private List<T> ToListOf<T>(byte[] array, Func<byte[], int, T> bitConverter)
        {
            var size = Marshal.SizeOf(typeof(T));
            return Enumerable.Range(0, array.Length / size)
                             .Select(i => bitConverter(array, i * size))
                             .ToList();
        }

        public InvoicingModel(InvoicingModelDb iDb)
        {
            Id = iDb.Id;
            InvoiceId = iDb.InvoiceNumber;
            PreviousSpent = iDb.PreviousSpent;
            AmountDue = iDb.AmountDue;
            ToDate = PreviousSpent + AmountDue ;
            Date =  DateTime.ParseExact(iDb.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            ClientName = iDb.ClientName;
            ClientCompany = iDb.ClientCompany;
            ClientAddress = iDb.ClientAddress;
            ClientCity = iDb.ClientCity;
            EmployeeSignedId = iDb.EmployeeSignedId;

            try
            {

                //TimesheetIds = (List<int>)iDb.TimesheetIds;
                //byte[] bytes = ObjectToByteArray(iDb.TimesheetIds);
                //byte[] bytes = (byte[])iDb.TimesheetIds;
                //List<int> myAnythingList = (iDb.TimesheetIds as IEnumerable<int>).Cast<int>().ToList();
                //List<int> timeids = ToListOf<int>((byte[])iDb.TimesheetIds, BitConverter.ToInt32);
                //TimesheetIds = Enumerable.Range(0, bytes.Length / 4).Select(i => BitConverter.ToInt32(bytes, i * 4)).ToList();
                string input = iDb.TimesheetIds;
                List<int> TagIds = input.Split(',').Select(int.Parse).ToList();
                TimesheetIds = TagIds;
            }
            catch
            {

            }

        }
    }
}
