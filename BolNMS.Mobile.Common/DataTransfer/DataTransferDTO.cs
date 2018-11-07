using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Common.DataTransfer
{
    public class DataTransferObject<T>
    {
        public bool IsSuccess { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }


    public class DataTransferList<T>
    {
        public bool IsSuccess { get; set; }

        public List<T> Data { get; set; }

        public string Message { get; set; }
    }
}
