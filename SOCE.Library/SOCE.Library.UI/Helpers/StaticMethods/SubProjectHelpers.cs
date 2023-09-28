using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public static class SubProjectHelpers
    {
        public static ObservableCollection<SubProjectModel> Renumber(this ObservableCollection<SubProjectModel> SubProjects, bool firstload)
        {
            bool toggle0 = false;
            SubProjectModel Lastitem = null;
            List<SubProjectModel> subs = new List<SubProjectModel>();
            for (int i = 0; i < SubProjects.Count; i++)
            {
                SubProjectModel sub = SubProjects[i];

                if (sub.Id != 0)
                {
                    int num = 0;
                    if (firstload)
                    {
                        if (sub.NumberOrder == 0 && !toggle0)
                        {
                            num = 0;
                            toggle0 = true;
                        }
                        else
                        {
                            num = sub.NumberOrder == 0 ? i : sub.NumberOrder;
                        }
                    }
                    else
                    {
                        num = i;
                    }

                    SQLAccess.UpdateNumberOrder(sub.Id, num);
                    sub.NumberOrder = num;
                    subs.Add(sub);
                }
                else
                {
                    Lastitem = sub;
                }
            }

            //SubProjects.ToList().Sort((x1, x2) =>  x1.NumberOrder - x2.NumberOrder );
            SubProjects = new ObservableCollection<SubProjectModel>(subs.OrderBy(x => x.NumberOrder).ToList());

            if (Lastitem != null)
            {
                SubProjects.Add(Lastitem);
            }

            return SubProjects;
        }
    }
}
