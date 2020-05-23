using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Builders
{
    public class MobStatorBuilder<TEntity, TEntityState> : IMobStatorBuilder<TEntity>
        where TEntity : class
    {
        private readonly MobStatorBase _mobStator;
        internal MobStatorBuilder(MobStatorBase mobStator)
            => _mobStator = mobStator;

        public IMobStatorBuilder<TEntity, TEntity2> AddStateMachine<TEntity2, TEntityState2>(Stator<TEntity2, TEntityState2> stator) where TEntity2 : class
        {
            _mobStator.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity, TEntity2, TEntityState2>(_mobStator);
        }
    }

    public class MobStatorBuilder<TEntity1, TEntity2, TEntityState> : IMobStatorBuilder<TEntity1, TEntity2>
    where TEntity1 : class
    where TEntity2 : class
    {
        private readonly MobStatorBase _mobStator;
        internal MobStatorBuilder(MobStatorBase mobStator)
            => _mobStator = mobStator;

        public IMobStatorBuilder<TEntity1, TEntity2, TEntity3> AddStateMachine<TEntity3, TEntityState3>(Stator<TEntity3, TEntityState3> stator) where TEntity3 : class
        {
            _mobStator.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntityState3>(_mobStator);
        }

        public IMobStator<TEntity1, TEntity2> Build()
            => new MobStator<TEntity1, TEntity2>(_mobStator);
    }

    public class MobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntityState> : IMobStatorBuilder<TEntity1, TEntity2, TEntity3>
    where TEntity1 : class
    where TEntity2 : class
    where TEntity3 : class
    {
        private readonly MobStatorBase _mobStator;
        internal MobStatorBuilder(MobStatorBase mobStator)
            => _mobStator = mobStator;

        public IMobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4> AddStateMachine<TEntity4, TEntityState4>(Stator<TEntity4, TEntityState4> stator) where TEntity4 : class
        {
            _mobStator.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4, TEntityState4>(_mobStator);
        }

        public IMobStator<TEntity1, TEntity2, TEntity3> Build()
            => new MobStator<TEntity1, TEntity2, TEntity3>(_mobStator);

    }

    public class MobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4, TEntityState> : IMobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class

    {
        private readonly MobStatorBase _mobStator;
        internal MobStatorBuilder(MobStatorBase mobStator)
            => _mobStator = mobStator;

        public IMobStatorExpandedBuilder AddStateMachine<TEntity5, TEntityState5>(Stator<TEntity5, TEntityState5> stator) where TEntity5 : class
        {
            _mobStator.AddStatorNode(stator);
            return new MobStatorExpandedBuilder(_mobStator);
        }

        public IMobStator<TEntity1, TEntity2, TEntity3, TEntity4> Build()
            => new MobStator<TEntity1, TEntity2, TEntity3, TEntity4>(_mobStator);
    }

    public class MobStatorExpandedBuilder : IMobStatorExpandedBuilder

    {
        private readonly MobStatorBase _mobStator;
        internal MobStatorExpandedBuilder(MobStatorBase mobStator)
            => _mobStator = mobStator;

        public IMobStatorExpandedBuilder AddStateMachine<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator) where TEntity : class
        {
            _mobStator.AddStatorNode(stator);
            return this;
        }

        public IMobStator Build()
            => new MobStatorExpanded(_mobStator);
    }
}
