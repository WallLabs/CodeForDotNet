using System;
using System.Collections.ObjectModel;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Summary description for IUndoable.
    /// </summary>
    public interface IUndoable
    {
        /// <summary>
        /// Gets a list of current Undo-able actions.
        /// </summary>
        Collection<string> GetUndoList();

        /// <summary>
        /// Gets a list of current Redo-able actions.
        /// </summary>
        Collection<string> GetRedoList();

        /// <summary>
        /// Undoes the specified number of actions.
        /// </summary>
        /// <param name="actions">Number of actions to undo.</param>
        void Undo(int actions);

        /// <summary>
        /// Redoes the specified number of actions.
        /// </summary>
        /// <param name="actions">Number of actions to redo.</param>
        void Redo(int actions);

        /// <summary>
        /// Prepares to start a new action, rejecting any previously incomplete actions beforehand.
        /// </summary>
        /// <remarks>
        /// To ensure all actions are atomic, you must always call BeginAction before making any changes. Otherwise changes from
        /// previously incomplete actions or direct data changes could be included.
        /// </remarks>
        void BeginAction();

        /// <summary>
        /// Cancels any changes since the last action was completed.
        /// </summary>
        /// <remarks>
        /// Equivalent to BeginAction(), but should still be called to ensure any partial changes are undone and the data is
        /// left in a consistent state.
        /// </remarks>
        void CancelAction();

        /// <summary>
        /// Completes the current action, making a record available for Undo/Redo as a unit.
        /// </summary>
        /// <param name="name">Short and descriptive action name, as seen in the Undo/Redo list by the user.</param>
        void EndAction(string name);

        /// <summary>
        /// Fired after an action has been made. Includes Undo or Redo.
        /// </summary>
        event EventHandler ActionCompleted;
    }
}
