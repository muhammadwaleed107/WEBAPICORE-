﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BolNMS.Mobile.Common.DataTransfer
{
    [DataContract]
    public class DataTransfer<T>
    {
        public DataTransfer()
        {
            this.IsSuccess = true;
        }
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string[] Errors { get; set; }

        [DataMember]
        public T Data { get; set; }

        public DataTransfer<T> GetDataTransferObj<T>(string[] Errors, T Data)
        {
            if (Data == null || Errors.Length > 0)
            {
                DataTransfer<T> dataTransfer = new DataTransfer<T>();
                dataTransfer.IsSuccess = false;
                dataTransfer.Errors = Errors;
                dataTransfer.Data = default(T);
                return dataTransfer;
            }
            else
            {
                DataTransfer<T> dataTransfer = new DataTransfer<T>();
                dataTransfer.IsSuccess = true;
                dataTransfer.Errors = null;
                dataTransfer.Data = Data;
                return dataTransfer;
            }
        }
    }
}
