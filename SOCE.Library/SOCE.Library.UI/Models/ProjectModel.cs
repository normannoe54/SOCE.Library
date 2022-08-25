//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows.Media;

//namespace SOCE.Library.UI
//{
//    public class ProjectUIModel : PropertyChangedBase
//    {
//        private string _projectName;
//        public string ProjectName
//        {
//            get { return _projectName; }
//            set
//            {
//                _projectName = value;
//                RaisePropertyChanged(nameof(ProjectName));
//            }
//        }

//        private double? _jobNum;
//        public double? JobNum
//        {
//            get { return _jobNum; }
//            set
//            {
//                _jobNum = value;
//                RaisePropertyChanged(nameof(JobNum));
//            }
//        }

//        public string JobNumStr
//        {
//            get
//            {
//                string jobnumstr = "";
//                if (JobNum!=null)
//                {
//                    jobnumstr = $"[{JobNum?.ToString()}]";
//                }
//                return jobnumstr;
//            }
//        }

//        private bool _isAdservice;
//        public bool IsAdservice
//        {
//            get { return _isAdservice; }
//            set
//            {
//                _isAdservice = value;
//                RaisePropertyChanged(nameof(IsAdservice));
//            }
//        }

//        private string _description;
//        public string Description
//        {
//            get { return _description; }
//            set
//            {
//                _description = value;
//                RaisePropertyChanged(nameof(Description));
//            }
//        }

//    }
//}
