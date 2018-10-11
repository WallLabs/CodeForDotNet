﻿namespace CodeForDotNet.UI.Models
{
    /// <summary>
    /// Base class for all page UI models
    /// </summary>
    public abstract class PageUIModel<TApplicationUIModel> : UIModel
        where TApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected PageUIModel(TApplicationUIModel application)
            : base(application.UITaskFactory)
        {
            // Initialize members
            Application = application;
        }

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// Application model.
        /// </summary>
        public TApplicationUIModel Application { get; private set; }

        #endregion Properties
    }
}