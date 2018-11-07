using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BolNMS.Mobile.Core.Cache
{
    public class Store<T>
    {

        public Store(int _expiryTimeInSeconds)
        {
            ExpiryTimeInSeconds = _expiryTimeInSeconds;
            LastUpdateDate = DateTime.MinValue;
        }

        public bool AlwaysCheckForUpdatedData = false;
        public DateTime MaxLastUpdateDate { get; set; }
        public DateTime MaxLastUpdateDateDuos { get; set; }
        private int ExpiryTimeInSeconds;
        private bool IsUpdating = false;
        private DateTime LastUpdateDate { get; set; }
        public T _data;
        public T Data
        {
            get
            {
                checkDataIsObselete();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        string lockVariable = string.Empty;
        private void checkDataIsObselete()
        {
            if (!IsUpdating || AlwaysCheckForUpdatedData)
            {
                lock (lockVariable)
                {
                    double seconds = DateTime.UtcNow.Subtract(LastUpdateDate).TotalSeconds;
                    if (seconds >= ExpiryTimeInSeconds || AlwaysCheckForUpdatedData)
                    {
                        if (_data == null)
                        {
                            updateData(null);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(updateData));
                        }
                    }
                    lockVariable = string.Empty;
                }
                //else if (seconds >= ExpiryTimeInSeconds) ThreadPool.QueueUserWorkItem(new WaitCallback(updateData));
            }
        }

        public void updateData(object data = null)
        {
            IsUpdating = true;
            try
            {
                if (OnUpdateDate != null)
                    _data = OnUpdateDate();
            }
            catch (Exception exp)
            {
            }
            finally
            {
                IsUpdating = false;
                LastUpdateDate = DateTime.UtcNow;
            }
        }

        public delegate T UpdateData();
        public event UpdateData OnUpdateDate;

    }
}