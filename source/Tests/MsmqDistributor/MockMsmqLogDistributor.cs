// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Tests
{
    internal class MockMsmqLogDistributor : MsmqLogDistributor
    {
        private bool stopRecv = false;
        private bool isCompleted = true;

        public bool ReceiveMsgCalled = false;

        public bool ExceptionOnGetIsCompleted = false;

        public override bool StopReceiving
        {
            get { return stopRecv; }
            set { stopRecv = value; }
        }

        public override bool IsCompleted
        {
            get
            {
                if (ExceptionOnGetIsCompleted)
                {
                    throw new Exception("simulated exception");
                }
                return isCompleted;
            }
        }

        public void SetIsCompleted(bool val)
        {
            isCompleted = val;
        }

        public MockMsmqLogDistributor(string msmqPath)
            : base(msmqPath, new DistributorEventLogger())
        {
        }

        public override void CheckForMessages()
        {
            ReceiveMsgCalled = true;
        }
    }
}
