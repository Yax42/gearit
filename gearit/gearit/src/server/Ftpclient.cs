using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;

namespace gearit.src.server.ftpclient
{
    class Ftpclient
    {
        private FtpWebRequest _request;
        private String _server;
        private String _file;
        private FtpState state;

        public FtpState getState
        {
            get { return state; }
        }

        public Ftpclient(String server, String file)
        {
            _server = server;
            _file = file;
            state = new FtpState();
        }

        private bool connectToFtp()
        {
            _request = (FtpWebRequest)WebRequest.Create(_server + _file);
            return false;
        }

        private bool disconnectToFtp()
        {
            return false;
        }

        public void listOfFiles()
        {
        }

        public void uploadFile()
        {
            ManualResetEvent waitObject;

            Uri target = new Uri(_server + _file);
            string fileName = _file;
            //FtpState state = new FtpState();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(target);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential("login", "password");

            // Store the request in the object that we pass into the 
            // asynchronous operations.
            state.Request = request;
            state.FileName = fileName;

            // Get the event to wait on.
            waitObject = state.OperationComplete;

            // Asynchronously get the stream for the file contents.
            request.BeginGetRequestStream(
                new AsyncCallback(EndGetStreamCallback),
                state
            );

            // Block the current thread until all operations are complete.
            //waitObject.WaitOne();

            
        }

        public int isFinish()
        {
            if (state.OperationException != null && state.FinishStatus)
            {
                //throw state.OperationException;
                return (-1);
            }
            else if (!state.FinishStatus)
            {
                return (0);
            }
            else
            {
                return (1);
            }
        }

        public void downloadFile()
        {
        }

        private static void EndGetStreamCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;

            Stream requestStream = null;
            // End the asynchronous call to get the request stream. 
            try
            {
                requestStream = state.Request.EndGetRequestStream(ar);
                // Copy the file contents to the request stream. 
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                FileStream stream = File.OpenRead(state.FileName);
                state.TotalSize = stream.Length;
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    requestStream.Write(buffer, 0, readBytes);
                    count += readBytes;
                    state.CurrentSize = count;
                    //Console.WriteLine("Upload: {0} / {1}", state.CurrentSize, state.TotalSize);
                }
                while (readBytes != 0);
                Console.WriteLine("Writing {0} bytes to the stream.", count);
                // IMPORTANT: Close the request stream before sending the request.
                requestStream.Close();
                // Asynchronously get the response to the upload request.
                state.Request.BeginGetResponse(
                    new AsyncCallback(EndGetResponseCallback),
                    state
                );
            }
            // Return exceptions to the main application thread. 
            catch (Exception e)
            {
                Console.WriteLine("Could not get the request stream.");
                state.OperationException = e;
                //state.OperationComplete.Set();
                state.FinishStatus = true;
                return;
            }

        }

        // The EndGetResponseCallback method   
        // completes a call to BeginGetResponse. 
        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)state.Request.EndGetResponse(ar);
                response.Close();
                state.StatusDescription = response.StatusDescription;
                // Signal the main application thread that  
                // the operation is complete.
                //state.OperationComplete.Set();
            }
            // Return exceptions to the main application thread. 
            catch (Exception e)
            {
                Console.WriteLine("Error getting response.");
                state.OperationException = e;
                //state.OperationComplete.Set();
            }
            state.FinishStatus = true;
        }
    }
}
