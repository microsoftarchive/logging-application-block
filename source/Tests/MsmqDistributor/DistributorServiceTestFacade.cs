// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Tests
{
    public class DistributorServiceTestFacade : DistributorService
    {
        public const string MockServiceName = "Enterprise Library Logging Distributor Service";

        public DistributorServiceTestFacade()
        {
            this.ServiceName = MockServiceName;
            //this.EventLogger.EventSource = ServiceName;
        }

        private ServiceStatus status;

        public override ServiceStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        new public void OnContinue()
        {
            base.OnContinue();
        }

        public void Initialize()
        {
            base.InitializeComponent();
        }

        public void OnStart()
        {
            base.OnStart(null);
        }

        new public void OnStop()
        {
            base.OnStop();
        }

        new public void OnPause()
        {
            base.OnPause();
        }

        protected override MsmqListener CreateListener(DistributorService distributorService, int timerInterval, string msmqPath)
        {
            if (this.QueueListener != null)
            {
                return this.QueueListener;
            }

            return new MockMsmqListener(distributorService, timerInterval, msmqPath);
        }
    }
}
