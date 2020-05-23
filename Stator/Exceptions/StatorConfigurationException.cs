using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Exceptions
{
    public class StatorConfigurationException : Exception
    {
        private static readonly string _mobEntityStateMachineNotRegisteredMessage =
            "Can't apply transition: state machine for Entity{0} not registered.";

        private static readonly string _mobEntitiesAndStateMachinesCountNotEqualsMessage =
            "Can't apply transition for {1} Entities with MobStator configured of {0} referenced state machines.";

        private static readonly string _mobStatorCantPropessDuplicatedEntities =
            "Can't apply transition: mob state machine can't applying transition for duplicated {0} entity.";

        private static readonly string _mobStatorCantSetForDuplicatedEntities =
           "Can't apply configuration: mob state machine can not be configured for duplicated {0} entity.";

        internal StatorConfigurationException(string message) : base(message)
        {
        }

        internal StatorConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal static StatorConfigurationException MobEntityStateMachineNotRegistered(Type entityType)
            => new StatorConfigurationException(string.Format(_mobEntityStateMachineNotRegisteredMessage, entityType.FullName));

        internal static StatorConfigurationException MobEntitiesAndStateMachinesCountNotEquals(int stateMachinesCount, int entitiesCount)
            => new StatorConfigurationException(string.Format(_mobEntitiesAndStateMachinesCountNotEqualsMessage, stateMachinesCount, entitiesCount));

        internal static Exception MobStatorCantPropessDuplicatedEntities(Type entityType)
            => new StatorConfigurationException(string.Format(_mobStatorCantPropessDuplicatedEntities, entityType.FullName));
        internal static Exception MobStatorCantSetForDuplicatedEntities(Type entityType)
            => new StatorConfigurationException(string.Format(_mobStatorCantPropessDuplicatedEntities, entityType.FullName));
    }
}
