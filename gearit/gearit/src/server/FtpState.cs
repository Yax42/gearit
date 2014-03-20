using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;

namespace gearit.src.server.ftpclient
{
    class FtpState
    {
        private ManualResetEvent wait;
        private FtpWebRequest request;
        private string fileName;
        private Exception operationException = null;
        string status;
        private bool finish;
        private long curSize;
        private long totalSize;

        public FtpState()
        {
            wait = new ManualResetEvent(false);
        }

        public ManualResetEvent OperationComplete
        {
            get { return wait; }
        }

        public FtpWebRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }
        public string StatusDescription
        {
            get { return status; }
            set { status = value; }
        }

        public bool FinishStatus
        {
            get { return finish; }
            set { finish = value; }
        }

        public long CurrentSize
        {
            get { return curSize; }
            set { curSize = value; }
        }

        public long TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }
    }
}
