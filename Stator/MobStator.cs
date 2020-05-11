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
 
namespace Stator
{
    public class MobStator<TEntity1, TEntity2> : MobStatorBase, IMobStator<TEntity1, TEntity2>
        where TEntity1 : class
        where TEntity2 : class
    {
        public MobStator(ConcurrentDictionary<Type, Func<object, object, object>> stators): base(stators){ }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2> mob)
        {
            if (Stators.Count != 2)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(2, Stators.Count);

            if (!Stators.TryGetValue(typeof(TEntity1), out var transiteEntity1) || transiteEntity1 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity1));

            if (!Stators.TryGetValue(typeof(TEntity2), out var transiteEntity2) || transiteEntity2 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity2));

            var result1 = (TransitionResult<TEntity1>)transiteEntity1.Invoke(mob.Item1, @event);
            var result2 = (TransitionResult<TEntity2>)transiteEntity2.Invoke(mob.Item2, @event);

            if (result1.Success && result2.Success)
                return new MobTransitionResult();

            var results = new Dictionary<Type, FailureTypes>
            {
                { typeof(TEntity1), result1.FailureType },
                { typeof(TEntity2), result2.FailureType }
            };

            return MobTransitionResult.MakeFailure(results);
        }
    }
    public class MobStator<TEntity1, TEntity2, TEntity3> : MobStatorBase, IMobStator<TEntity1, TEntity2, TEntity3>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
    {
        public MobStator(ConcurrentDictionary<Type, Func<object, object, object>> stators) : base(stators) { }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3> mob)
        {
            if (Stators.Count != 3)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(3, Stators.Count);

            if (!Stators.TryGetValue(typeof(TEntity1), out var transiteEntity1) || transiteEntity1 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity1));

            if (!Stators.TryGetValue(typeof(TEntity2), out var transiteEntity2) || transiteEntity2 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity2));

            if (!Stators.TryGetValue(typeof(TEntity3), out var transiteEntity3) || transiteEntity3 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity3));

            var result1 = (TransitionResult<TEntity1>)transiteEntity1.Invoke(mob.Item1, @event);
            var result2 = (TransitionResult<TEntity2>)transiteEntity2.Invoke(mob.Item2, @event);
            var result3 = (TransitionResult<TEntity3>)transiteEntity3.Invoke(mob.Item3, @event);

            if (result1.Success && result2.Success && result3.Success)
                return new MobTransitionResult();

            var results = new Dictionary<Type, FailureTypes>
            {
                { typeof(TEntity1), result1.FailureType },
                { typeof(TEntity2), result2.FailureType },
                { typeof(TEntity3), result3.FailureType }
            };

            return MobTransitionResult.MakeFailure(results);
        }
    }
    public class MobStator<TEntity1, TEntity2, TEntity3, TEntity4> : MobStatorBase, IMobStator<TEntity1, TEntity2, TEntity3, TEntity4>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        public MobStator(ConcurrentDictionary<Type, Func<object, object, object>> stators) : base(stators) { }
        public MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3, TEntity4> mob)
        {
            if (Stators.Count != 4)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(4, Stators.Count);

            if (!Stators.TryGetValue(typeof(TEntity1), out var transiteEntity1) || transiteEntity1 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity1));

            if (!Stators.TryGetValue(typeof(TEntity2), out var transiteEntity2) || transiteEntity2 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity2));

            if (!Stators.TryGetValue(typeof(TEntity3), out var transiteEntity3) || transiteEntity3 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity3));

            if (!Stators.TryGetValue(typeof(TEntity4), out var transiteEntity4) || transiteEntity4 == null)
                throw StatorConfigurationException.MobEntityStateMachineNotRegistered(typeof(TEntity4));

            var result1 = (TransitionResult<TEntity1>)transiteEntity1.Invoke(mob.Item1, @event);
            var result2 = (TransitionResult<TEntity2>)transiteEntity2.Invoke(mob.Item2, @event);
            var result3 = (TransitionResult<TEntity3>)transiteEntity3.Invoke(mob.Item3, @event);
            var result4 = (TransitionResult<TEntity4>)transiteEntity4.Invoke(mob.Item4, @event);

            if (result1.Success && result2.Success && result3.Success && result4.Success)
                return new MobTransitionResult();

            var results = new Dictionary<Type, FailureTypes>
            {
                { typeof(TEntity1), result1.FailureType },
                { typeof(TEntity2), result2.FailureType },
                { typeof(TEntity3), result3.FailureType },
                { typeof(TEntity4), result4.FailureType }
            };

            return MobTransitionResult.MakeFailure(results);
        }
    }
    public class MobStatorExpanded : MobStatorBase, IMobStator
    {
        public MobStatorExpanded(ConcurrentDictionary<Type, Func<object, object, object>> stators) : base(stators) { }
        public MobTransitionResult Go(IEvent @event, params object[] mob)
        {
            var entitiesCount = mob.Length;
            if (Stators.Count != entitiesCount)
                throw StatorConfigurationException.MobEntitiesAndStateMachinesCountNotEquals(entitiesCount, Stators.Count);

            var results = new Dictionary<Type, FailureTypes>();
            var transitions = new Action[entitiesCount];
            var failed = false;

            // TODO: Need compile delegates ***
            var resultType = typeof(TransitionResult<>).MakeGenericType();
            var successProperty = resultType.GetProperty("Success", typeof(bool));
            var failureTypeProperty = resultType.GetProperty("FailureType", typeof(FailureTypes));
            // TODO: Need compile delegates ***


            for (var i = 0; i < entitiesCount; i++)
            {
                var item = mob[i];
                var entityType = item.GetType();
                if (results.ContainsKey(entityType))
                    throw StatorConfigurationException.MobStatorCantPropessDuplicatedEntities(entityType);

                results[entityType] = default;
                if (!Stators.TryGetValue(entityType, out var transitionFunc) || transitionFunc == null)
                    throw StatorConfigurationException.MobEntityStateMachineNotRegistered(entityType);

                transitions[i] = () =>
                    {
                        var result = transitionFunc.Invoke(item, @event);
                        
                        if (!(bool)successProperty.GetValue(result)) failed = true;
                        results[entityType] = (FailureTypes)failureTypeProperty.GetValue(result);
                    };
            }

            foreach (var @delegate in transitions)
                @delegate.Invoke();

            if (!failed)
                return new MobTransitionResult();

            return MobTransitionResult.MakeFailure(results);
        }
    }
}

