using System;
using CodeForDotNet.ComponentModel;

namespace CodeForDotNet.WindowsUniversal.TestApp.Models;

    /// <summary>
    /// Model behind the "Control Test" view.
    /// </summary>
public partial class ControlTestUIModel : PropertyStore
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public ControlTestUIModel()
        {
            // Register properties
            RegisterProperty(TextBoxTextPropertyId, "TextBoxText", string.Empty, false);
            RegisterProperty(TextBoxTextChangedDatePropertyId, "TextBoxTextChangedDate", (DateTimeOffset?)null, false);
            RegisterProperty(DynamicTextBoxTextPropertyId, "DynamicTextBoxText", string.Empty, false);
            RegisterProperty(DynamicTextBoxTextChangedDatePropertyId, "DynamicTextBoxTextChangedDate", (DateTimeOffset?)null, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// ID of the <see cref="TextBoxText"/> property.
        /// </summary>
        public static readonly Guid TextBoxTextPropertyId = new("{E3965AB2-D624-4713-B556-8AA21802F707}");

        /// <summary>
        /// Text displayed and edited in a normal text box.
        /// </summary>
        public string TextBoxText
        {
        get => GetProperty<string>(TextBoxTextPropertyId);
            set
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Set value
                    SetProperty(TextBoxTextPropertyId, value);

                    // Update changed date
                    TextBoxTextChangedDate = DateTimeOffset.Now;
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// ID of the <see cref="TextBoxTextChangedDate"/> property.
        /// </summary>
        public static readonly Guid TextBoxTextChangedDatePropertyId = new("{14BF29E9-BCB6-447E-A5EF-B3C8A634667F}");

        /// <summary>
        /// Date the <see cref="TextBoxText"/> was last changed.
        /// </summary>
        public DateTimeOffset? TextBoxTextChangedDate
        {
        get => GetProperty<DateTimeOffset?>(TextBoxTextChangedDatePropertyId); private set => SetProperty(TextBoxTextChangedDatePropertyId, value);
        }

        /// <summary>
        /// ID of the <see cref="DynamicTextBoxText"/> property.
        /// </summary>
        public static readonly Guid DynamicTextBoxTextPropertyId = new("{B204D917-ACCC-4CAD-9B45-5C3E4FC2A2CE}");

        /// <summary>
        /// Text displayed and edited in a normal text box.
        /// </summary>
        public string DynamicTextBoxText
        {
        get => GetProperty<string>(DynamicTextBoxTextPropertyId);
            set
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Set value
                    SetProperty(DynamicTextBoxTextPropertyId, value);

                    // Update changed date
                    DynamicTextBoxTextChangedDate = DateTimeOffset.Now;
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// ID of the <see cref="DynamicTextBoxTextChangedDate"/> property.
        /// </summary>
        public static readonly Guid DynamicTextBoxTextChangedDatePropertyId = new("{86C79A70-5056-42AF-A4B3-2129FC72B85F}");

        /// <summary>
        /// Date the <see cref="DynamicTextBoxText"/> was last changed.
        /// </summary>
        public DateTimeOffset? DynamicTextBoxTextChangedDate
        {
        get => GetProperty<DateTimeOffset?>(DynamicTextBoxTextChangedDatePropertyId); private set => SetProperty(DynamicTextBoxTextChangedDatePropertyId, value);
        }

        #endregion
    }
}
