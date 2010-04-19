using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    public class ComparerContainer : IComparer
    {
        public List<IComparer> Comparers { get; private set; }

        public ComparerContainer()
        {
            this.Comparers = new List<IComparer>();
        }

        #region IComparer Members

        public ComparerResult Compare()
        {
            ComparerResult comparerResult = new ComparerResult();
            comparerResult.IsEqual = true;

            foreach (IComparer item in this.Comparers)
            {
                ComparerResult itemComparerResult = item.Compare();
                
                if (!itemComparerResult.IsEqual)
                    comparerResult.IsEqual = false;
                
                comparerResult.Message += itemComparerResult.Message + "\r\n";
            }

            return comparerResult;
        }

        #endregion
    }
}
