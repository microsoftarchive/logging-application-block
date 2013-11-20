// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration
{
    internal class CustomTraceListenerDataHelper
        : BasicCustomTraceListenerDataHelper
    {
        private static readonly ConfigurationProperty formatterProperty =
            new ConfigurationProperty(CustomTraceListenerData.formatterNameProperty,
                                        typeof(string),
                                        null,   // no reasonable default
                                        null,   // use default converter
                                        null,    // no validations
                                        ConfigurationPropertyOptions.None);

        internal CustomTraceListenerDataHelper(CustomTraceListenerData helpedCustomProviderData)
            : base(helpedCustomProviderData)
        {
            propertiesCollection.Add(formatterProperty);
        }

        protected override bool IsKnownPropertyName(string propertyName)
        {
            return base.IsKnownPropertyName(propertyName)
                || CustomTraceListenerData.formatterNameProperty.Equals(propertyName);
        }
    }
}
