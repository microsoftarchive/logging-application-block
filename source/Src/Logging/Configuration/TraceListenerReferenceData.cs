// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration
{
    /// <summary>
    /// Configuration for a reference to named <see cref="TraceListenerData"/>.
    /// </summary>
    [ViewModel(LoggingDesignTime.ViewModelTypeNames.TraceListenerReferenceViewModel)]
    [ResourceDisplayName(typeof(DesignResources), "TraceListenerReferenceDataDisplayName")]
    [ResourceDescription(typeof(DesignResources), "TraceListenerReferenceDataDescription")]
    public class TraceListenerReferenceData : NamedConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerReferenceData"/> class with default values.
        /// </summary>
        public TraceListenerReferenceData()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerReferenceData"/> class with a name.
        /// </summary>
        /// <param name="name">Name for trace listener.</param>
        public TraceListenerReferenceData(string name)
            : base(name)
        { }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        /// <remarks>
        /// Overriden in order to annotate with designtime attribute.
        /// </remarks>
        [Reference(typeof(TraceListenerDataCollection), typeof(TraceListenerData))]
        [EnvironmentalOverrides(true)]
        [ViewModel(CommonDesignTime.ViewModelTypeNames.CollectionEditorContainedElementReferenceProperty)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }
    }
}
