using Stator.Enums;
using Stator.Exceptions;
using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stator
{
    public class MobStatorBase
    {
        protected bool _withRollbackOnFailure;
        internal ConcurrentDictionary<Type, EntityGetSetAccessor> Stators;
        public MobStatorBase(bool withRollbackOnFailure)
        {
            _withRollbackOnFailure = withRollbackOnFailure;
            Stators = new ConcurrentDictionary<Type, EntityGetSetAccessor>();
        }
        public MobStatorBase(MobStatorBase prototype)
        {
            _withRollbackOnFailure = prototype._withRollbackOnFailure;
            Stators = prototype.Stators;
        }

        internal void AddStatorNode<TEntity, TEntityState>(IStator<TEntity, TEntityState> stator) where TEntity : class
        {
            var nodeType = typeof(TEntity);
            if (Stators.Keys.Contains(nodeType)) throw StatorConfigurationException.MobStatorCantSetForDuplicatedEntities(nodeType);
            var accessor = new EntityGetSetAccessor
            {
                TransitionInvoker = (entity, @event) => stator.Go((TEntity)entity, (IEvent)@event),
                GetInvoker = (entity) => stator.GetGetter()((TEntity)entity),
                SetInvoker = (entity, state) => stator.GetSetter()((TEntity)entity, (TEntityState)state)
            };
            Stators[nodeType] = accessor;
        }

        protected void PrepareDelegates<T>(T entity, IEvent @event, List<Action> rollbackActions, List<Func<Tuple<Type, bool, FailureTypes>>> transitionActions) where T : class
        {
            var type = typeof(T);
            if (!Stators.TryGetValue(type, out var entityAccessor) || entityAccessor == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(type);

            var transitionInvoker = entityAccessor.TransitionInvoker;
            var setInvoker = entityAccessor.SetInvoker;
            var getInvoker = entityAccessor.GetInvoker;
            var oldValue = getInvoker(entity);
            Action rollbackAction = () => setInvoker(entity, oldValue);
            rollbackActions.Add(rollbackAction);
            transitionActions.Add(() =>
            {
                var result = (TransitionResult<T>)transitionInvoker(entity, @event);
                return Tuple.Create(type, result.Success, result.FailureType);
            });
        }

        protected MobTransitionResult ProcessResult(Tuple<Type, bool, FailureTypes>[] transitionResults)
        {
            var cap = transitionResults.Length;
            var toSuccessTypes = new List<Type>(cap);
            var failuresMap = new Dictionary<Type, FailureTypes>(cap);
            foreach (var (type, result, failureType) in transitionResults)
            {
                if (result) toSuccessTypes.Add(type);
                else failuresMap[type] = failureType;
            }
            if (toSuccessTypes.Count > 0) return new MobTransitionResult(toSuccessTypes.ToArray());

            return MobTransitionResult.MakeFailure(failuresMap);
        }
    }
}