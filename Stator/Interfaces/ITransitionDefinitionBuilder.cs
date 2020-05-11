using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface ITransitionDefinitionBuilder<TEntity, TEntityState> where TEntity : class
    {
        /// <summary>
        /// Set a condition which must be succesfully checked before the transition will be allowed
        /// </summary>
        /// <param name="predicate">Predicate</param>
        ITransitionDefinitionMatchBuilder<TEntity, TEntityState> Match(Func<TEntity, IEvent, bool> predicate);
        /// <summary>
        /// Set delegate that will be invoked before the transition occured
        /// </summary>
        /// <param name="action">Delegate to invoke</param>
        ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionBeforeTransition(Action<TEntity, IEvent> action);
        /// <summary>
        /// Set delegate that will be invoked after the transition occured
        /// </summary>
        /// <param name="action">Delegate to invoke</param>
        ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionAfterTransition(Action<TEntity, IEvent> action);
        /// <summary>
        /// Finish transition configuration and return to event configuration.
        /// </summary>
        IEventDefinitionBuilder<TEntity, TEntityState> ConfirmTransition();
    }
}
