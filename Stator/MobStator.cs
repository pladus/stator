using Stator.Enums;
using Stator.Exceptions;
using Stator.Interfaces;
using Stator.Models;
using System;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Stator
{
    public class MobStator<TEntity1, TEntity2> : MobStatorBase, IMobStator<TEntity1, TEntity2>
        where TEntity1 : class
        where TEntity2 : class
    {
        internal MobStator(MobStatorBase mobStator) : base(mobStator) { }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2> mob)
        {
            if (Stators.Count != 2)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(2, Stators.Count);

            var rollbackActions = new List<Action>();
            var transitionActions = new List<Func<Tuple<Type, bool, FailureTypes>>>();

            PrepareDelegates(mob.Item1, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item2, @event, rollbackActions, transitionActions);

            try
            {
                var results = transitionActions.Select(x => x?.Invoke()).ToArray();

                if (results.All(x => x.Item2))
                    return new MobTransitionResult();

                var resultsToReturn = results.ToDictionary(x => x.Item1, x => x.Item3);
                return MobTransitionResult.MakeFailure(resultsToReturn);
            }
            catch
            {
                if(_withRollbackOnFailure)
                    rollbackActions.ForEach(x => x?.Invoke());
                throw;
            }
        }
    }
    public class MobStator<TEntity1, TEntity2, TEntity3> : MobStatorBase, IMobStator<TEntity1, TEntity2, TEntity3>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
    {
        internal MobStator(MobStatorBase mobStator) : base(mobStator) { }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3> mob)
        {
            if (Stators.Count != 3)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(3, Stators.Count);

            var rollbackActions = new List<Action>();
            var transitionActions = new List<Func<Tuple<Type, bool, FailureTypes>>>();

            PrepareDelegates(mob.Item1, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item2, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item3, @event, rollbackActions, transitionActions);

            try
            {
                var results = transitionActions.Select(x => x?.Invoke());

                if (results.All(x => x.Item2))
                    return new MobTransitionResult();

                var resultsToReturn = results.ToDictionary(x => x.Item1, x => x.Item3);
                return MobTransitionResult.MakeFailure(resultsToReturn);
            }
            catch
            {
                if (_withRollbackOnFailure)
                    rollbackActions.ForEach(x => x?.Invoke());
                throw;
            }
        }
    }
    public class MobStator<TEntity1, TEntity2, TEntity3, TEntity4> : MobStatorBase, IMobStator<TEntity1, TEntity2, TEntity3, TEntity4>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        internal MobStator(MobStatorBase mobStator) : base(mobStator) { }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3, TEntity4> mob)
        {
            if (Stators.Count != 4)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(4, Stators.Count);
            var rollbackActions = new List<Action>();
            var transitionActions = new List<Func<Tuple<Type, bool, FailureTypes>>>();

            PrepareDelegates(mob.Item1, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item2, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item3, @event, rollbackActions, transitionActions);
            PrepareDelegates(mob.Item4, @event, rollbackActions, transitionActions);


            try
            {
                var results = transitionActions.Select(x => x?.Invoke()).ToArray();

                if (results.All(x => x.Item2))
                    return new MobTransitionResult();

                var resultsToReturn = results.ToDictionary(x => x.Item1, x => x.Item3);
                return MobTransitionResult.MakeFailure(resultsToReturn);
            }
            catch
            {
                if (_withRollbackOnFailure)
                    rollbackActions.ForEach(x => x?.Invoke());
                throw;
            }
        }
    }
    public class MobStatorExpanded : MobStatorBase, IMobStator
    {
        private Dictionary<Type, Action<object, IEvent, List<Action>, List<Func<Tuple<Type, bool, FailureTypes>>>>>
            _typePreparersMap;

        internal MobStatorExpanded(MobStatorBase mobStator) : base(mobStator) { }
        public MobTransitionResult Go(IEvent @event, params object[] mob)
        {
            if (_typePreparersMap == null) BuildMap(mob);

            var entitiesCount = mob.Length;
            if (Stators.Count != entitiesCount)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(entitiesCount, Stators.Count);

            var rollbackActions = new List<Action>();
            var transitionActions = new List<Func<Tuple<Type, bool, FailureTypes>>>();

            foreach (var item in mob)
            {
                _typePreparersMap[item.GetType()](item, @event, rollbackActions, transitionActions);
            }

            try
            {
                var results = transitionActions.Select(x => x?.Invoke()).ToArray();

                if (results.All(x => x.Item2))
                    return new MobTransitionResult();

                var resultsToReturn = results.ToDictionary(x => x.Item1, x => x.Item3);
                return MobTransitionResult.MakeFailure(resultsToReturn);
            }
            catch
            {
                if (_withRollbackOnFailure)
                    rollbackActions.ForEach(x => x?.Invoke());
                throw;
            }
        }

        private void BuildMap(object[] mobEntity)
        {
            var count = mobEntity.Length;
            _typePreparersMap = new Dictionary<Type, Action<object, IEvent, List<Action>, List<Func<Tuple<Type, bool, FailureTypes>>>>>();
            var touchedEntities = new Type[count];
            
            for (var i = 0; i< count; i++)
            {
                var entity = mobEntity[i];
                var entityType = entity.GetType();
                if (Array.IndexOf(touchedEntities, entityType) >= 0)
                    throw StatorConfigurationException.MobStatorCantPropessDuplicatedEntities(entityType);
               
                touchedEntities[i] = entityType;

                var constThis = Expression.Constant(this);
                var inEntityParam = Expression.Parameter(typeof(object), "entity");
                var entityParam = Expression.Convert(inEntityParam, entityType); var eventParam = Expression.Parameter(typeof(IEvent), "@event");
                var rollbackActionsParam = Expression.Parameter(typeof(List<Action>), "rollbackActions");
                var transitionInvokersParam = Expression.Parameter(typeof(List<Func<Tuple<Type, bool, FailureTypes>>>), "transitionInvokers");

                
                var callExp = Expression.Call(constThis, nameof(PrepareDelegates), new Type[] { entityType }, new Expression[] { entityParam, eventParam, rollbackActionsParam, transitionInvokersParam });

                var lambda = Expression.Lambda(callExp, new[] { inEntityParam, eventParam, rollbackActionsParam, transitionInvokersParam });
                var compiled = (Action<object, IEvent, List<Action>, List<Func<Tuple<Type, bool, FailureTypes>>>>)lambda.Compile();

                _typePreparersMap.Add(entityType, compiled);
            }
        }
    }
}

